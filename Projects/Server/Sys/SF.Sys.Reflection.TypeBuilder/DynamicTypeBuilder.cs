#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using SF.Sys.ADT;
using SF.Sys.Collections.Generic;

namespace SF.Sys.Reflection
{
	public class DynamicTypeBuilder : IDynamicTypeBuilder
	{
		static AssemblyBuilder AssemblyBuilder { get; } =
			AssemblyBuilder.DefineDynamicAssembly(
				new AssemblyName("DynamicTypes"),
				AssemblyBuilderAccess.Run
			);
		static ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(new Guid().ToString("N"));
		static CustomAttributeBuilder CreateCustomAttributeBuilder(CustomAttributeExpression attr)
		{
			return new CustomAttributeBuilder(
					attr.Constructor,
					attr.Arguments.ToArray(),
					attr.InitProperties.ToArray(),
					attr.InitPropertyValues.ToArray()
					);
		}
		static PropertyBuilder BuildProperty(TypeBuilder typeBuilder, string name, Type type, PropertyAttributes Attributes)
		{
			var field = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(name, Attributes, type, null);

			var getSetAttr = MethodAttributes.Public |
							MethodAttributes.HideBySig |
							MethodAttributes.SpecialName |
							MethodAttributes.Virtual;

			var getter = typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);
			var getIL = getter.GetILGenerator();
			getIL.Emit(OpCodes.Ldarg_0);
			getIL.Emit(OpCodes.Ldfld, field);
			getIL.Emit(OpCodes.Ret);

			var setter = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });

			var setIL = setter.GetILGenerator();
			setIL.Emit(OpCodes.Ldarg_0);
			setIL.Emit(OpCodes.Ldarg_1);
			setIL.Emit(OpCodes.Stfld, field);
			setIL.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getter);
			propertyBuilder.SetSetMethod(setter);
			return propertyBuilder;
		}
		Type TypeResolve(TypeReference tr,TypeBuilder[] typeBuilders,Dictionary<TypeExpression,int> exprDict)
		{
			if(tr is SystemTypeReference str)
				return str.Type;
			if (tr is TypeExpressionReference ter)
				return typeBuilders[exprDict[ter.Type]];
			if (tr is GenericTypeReference gtr)
				return TypeResolve(gtr.GenericTypeDefine, typeBuilders, exprDict).MakeGenericType(
					gtr.GenericTypeArguments.Select(ta => TypeResolve(ta, typeBuilders, exprDict)).ToArray()
					);
			throw new NotSupportedException();
		}
		public Type[] Build(IEnumerable<TypeExpression> Expressions)
		{
			var exprs = Expressions.ToArray();
			var exprDict = exprs.Select((e, i) => (e, i)).ToDictionary(e => e.e, e => e.i);

			var idx = Enumerable.Range(0, exprs.Length);

			var typeSorted=Tree.AsEnumerable(
				Tree.Build(idx, e => exprs[e].BaseType as TypeExpressionReference == null?0:exprDict.Get((exprs[e].BaseType as TypeExpressionReference)?.Type,0)),
				n => n
				);
			var typeBuilders = new TypeBuilder[exprs.Length];

			foreach (var t in typeSorted)
			{
				var e = exprs[t.Value];
				var tb = ModuleBuilder.DefineType(
					e.Name,
					e.Attributes,
					TypeResolve(e.BaseType ,typeBuilders,exprDict),
					e.ImplementInterfaces
						.Select(i => i is SystemTypeReference ti ? ti.Type : throw new NotSupportedException()).ToArray()
					);

				foreach (var a in e.CustomAttributes)
					tb.SetCustomAttribute(CreateCustomAttributeBuilder(a));
				typeBuilders[t.Value] = tb;
			}

			for(var i=0;i<typeBuilders.Length;i++)
			{
				foreach(var p in exprs[i].Properties)
				{
					var pb = BuildProperty(
						typeBuilders[i],
						p.Name,
						TypeResolve(p.PropertyType, typeBuilders, exprDict),
						p.Attributes
						);
					foreach (var a in p.CustomAttributes)
						pb.SetCustomAttribute(CreateCustomAttributeBuilder(a));
				}
			}
			return typeBuilders.Select(tb => tb.CreateTypeInfo().AsType()).ToArray();
		}
	}
}
