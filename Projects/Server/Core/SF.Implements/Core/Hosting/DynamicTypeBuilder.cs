using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Core.Drawing;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Linq.TypeExpressions;
using System.Reflection;

namespace SF.Core.Hosting
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

			var typeSorted=ADT.Tree.AsEnumerable(
				ADT.Tree.Build(idx, e => exprs[e].BaseType as TypeExpressionReference == null?0:exprDict.Get((exprs[e].BaseType as TypeExpressionReference)?.Type,0)),
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
