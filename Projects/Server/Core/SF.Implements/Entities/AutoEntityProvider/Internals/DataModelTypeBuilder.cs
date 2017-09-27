using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals
{
	public interface IDataModelAttributeGenerator
	{
		CustomAttributeBuilder Generate(IAttribute Attr);
	}
	public class BaseDataModel
	{

	}
	public class DataModelTypeBuilder
	{
		IMetadataCollection metas { get; }
		
		static AssemblyBuilder AssemblyBuilder { get; }= 
			AssemblyBuilder.DefineDynamicAssembly(
				new AssemblyName("SFAutoEntityProviderDynamicClass"), 
				AssemblyBuilderAccess.Run
				);

		Dictionary<string, TypeBuilder> TypeBuilders = new Dictionary<string, TypeBuilder>();
		ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(new Guid().ToString("N"));
		NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver { get; }

		public DataModelTypeBuilder(
			IMetadataCollection metas,
			NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver
			)
		{
			this.metas = metas;
			this.DataModelAttributeGeneratorResolver = DataModelAttributeGeneratorResolver;
		}
		void EnsureIndex(PropertyBuilder propBuilder)
		{
			var idxs = propBuilder.GetCustomAttributes<IndexAttribute>();
			if (idxs.Any(a => a.Name == null))
				return;
			propBuilder.SetCustomAttribute(
				new CustomAttributeBuilder(
					typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()),
					Array.Empty<object>()
					)
				);
		}

		PropertyBuilder BuildProperty(TypeBuilder typeBuilder, string name, Type type)
		{
			var field = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);

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
		PropertyBuilder DefineProperty(TypeBuilder typeBuilder, string Name,IEnumerable<IAttribute> Attributes, Type type)
		{
			var pb = BuildProperty(typeBuilder, Name, type);

			if(Attributes!=null)
				foreach (var a in Attributes)
				{
					var g = DataModelAttributeGeneratorResolver(a.Name);
					if (g == null)
						throw new ArgumentException($"��֧����������{a.Name}�����������ԣ�ʵ������:{typeBuilder.Name} ����{Name}");
					pb.SetCustomAttribute(g.Generate(a));
				}
			return pb;
		}
		PropertyBuilder DefineProperty(TypeBuilder typeBuilder, IProperty prop, Type type) =>
			DefineProperty(typeBuilder, prop.Name, prop.Attributes, type);

		void EnsureForginKeyField(TypeBuilder typeBuilder, IEntityType curEntity, IEntityType foreignEntity, string name)
		{
			var fepk = foreignEntity.Properties.Where(p => p.Attributes.Any(a => a.Name == typeof(KeyAttribute).FullName)).ToArray();
			if (fepk.Length ==0)
				throw new InvalidOperationException($"����ʵ��{curEntity.FullName}���������{name}ʱ,���ʵ����{foreignEntity.FullName}�Ҳ�������");
			else if (fepk.Length > 1)
				throw new InvalidOperationException($"����ʵ��{curEntity.FullName}���������{name}ʱ,��֧�����ʵ��{foreignEntity.FullName}���ж������{fepk.Select(p=>p.Name).Join(",")}");
			var propType = fepk[0].Type;
			if(!(propType is IValueType))
				throw new InvalidOperationException($"ʵ��{foreignEntity.FullName}����{fepk[0].Name}�����������ͣ���������{fepk[0].Type.Name}");
			var propSysType = ((IValueType)propType).SysType;

			PropertyBuilder pb;
			var prop = curEntity.Properties.FirstOrDefault(p => p.Name == name);
			if (prop == null)
				pb = DefineProperty(typeBuilder, name, null, propSysType);
			else
			{
				if (prop.Mode != PropertyMode.Value)
					throw new InvalidOperationException($"ʵ��{curEntity.FullName}������{prop.Name}����Ϊ{prop.Type.Name}�Ĺ�ϵ�ֶΣ���������Ϊ{propSysType}��{foreignEntity.FullName}���ⲿ��ϵ���");
				else if (((IValueType)prop.Type).SysType != propSysType)
					throw new InvalidOperationException($"ʵ��{curEntity.FullName}�����{fepk[0].Name}����Ϊ{((IValueType)prop.Type).SysType},�����ʵ��{foreignEntity.FullName}����{fepk[0].Name}������{propSysType}��һ��");
				pb = (PropertyBuilder)typeBuilder.GetProperty(name);
			}

			//ȷ������ֶ�������
			if (pb != null)
				EnsureIndex(pb);
		}
		void EnsureForginRelationField(TypeBuilder typeBuilder, IEntityType curEntity, IEntityType foreignEntity, string name)
		{
			var prop = curEntity.Properties.SingleOrDefault(p => p.Name == name);
			if(prop==null)
				DefineProperty(typeBuilder, name, null, TypeBuilders[foreignEntity.FullName]);
			else if(prop.Mode!=PropertyMode.SingleRelation)
				throw new InvalidOperationException($"ʵ��{curEntity.FullName}������{prop.Name}����Ϊ{prop.Type.Name}�������ֶΣ�����{foreignEntity.FullName}�ⲿ��ϵ");
			else if(prop.Type!=foreignEntity)
				throw new InvalidOperationException($"ʵ��{curEntity.FullName}������{prop.Name}�Ĺ�ϵΪ{prop.Type.Name}������{foreignEntity.FullName}�ⲿ��ϵ");

			var idFieldName = prop?.Attributes
				.FirstOrDefault(a => a.Name == typeof(ForeignKeyAttribute).FullName)
				.Values.Get("Name") as string  ??
				(name + "Id");

			EnsureForginKeyField(typeBuilder, curEntity, foreignEntity, idFieldName);
		}
		void BuildValueProperty(TypeBuilder typeBuilder,IEntityType type, IProperty prop)
		{
			var pb=DefineProperty(
				typeBuilder, 
				prop, 
				((IValueType)prop.Type).SysType
				);

			//��鵱ǰ�����ʹ���ֶ���Ϊ������ֶΣ���ǰ�ֶ���Ҫ��������
			var foreignProp = type.Properties.Where(p =>
				   p.Mode == PropertyMode.SingleRelation &&
				   (p.Name + "Id" == prop.Name ||
				   p.Attributes.Any(a => a.Name == typeof(ForeignKeyAttribute).FullName && a?.Values.Get("Name") as string == prop.Name)
				   )).SingleOrDefault();
			if (foreignProp != null)
				EnsureForginKeyField(typeBuilder, type, (IEntityType)foreignProp.Type, prop.Name);

		}
		void BuildSingleRelationProperty(TypeBuilder typeBuilder, IEntityType type, IProperty prop)
		{
			var pd=DefineProperty(
				typeBuilder, 
				prop, 
				TypeBuilders[((IEntityType)prop.Type).FullName]
				);

			//ȷ���ⲿ��ϵ�ֶ�
			EnsureForginRelationField(typeBuilder, type, (IEntityType)prop.Type, prop.Name);

		}
		void BuildMultipleRelationProperty(TypeBuilder typeBuilder, IEntityType type, IProperty prop)
		{
			var pd=DefineProperty(
				typeBuilder, 
				prop, 
				typeof(ICollection<>).MakeGenericType(TypeBuilders[((IEntityType)prop.Type).FullName])
				);

			//ȷ������ֶ�
			var foreignType = (IEntityType)prop.Type;
			var ForeignRelationPropName = pd.GetCustomAttribute<InversePropertyAttribute>()?.Property ?? type.Name;
			EnsureForginRelationField(typeBuilder, foreignType, type, ForeignRelationPropName);

		}

		Type BuildType(TypeBuilder typeBuilder,IEntityType type)
		{
			var keys = type.Properties.Where(p => p.Attributes.Any(a => a.Name == typeof(KeyAttribute).FullName)).ToArray();
			if (keys.Length == 0)
				throw new ArgumentException($"ʵ��{type.FullName}δ��������");
			else
			{
				var nks = keys.Where(k => !(k.Type is IValueType)).ToArray();
				if(nks.Length>0)
					throw new ArgumentException($"ʵ��{type.FullName}�������ֶβ�Ϊԭ������{nks.Select(n=>n.Name+":"+n.Type.Name).Join(",")}");
				if (keys.Length == 1)
				{
					typeBuilder.AddInterfaceImplementation(
						typeof(IEntityWithId<>).MakeGenericType(((IValueType)keys[0].Type).SysType)
					);
				}
			}
			foreach (var a in type.Attributes)
			{
				var g = DataModelAttributeGeneratorResolver(a.Name);
				if (g == null)
					throw new ArgumentException($"��֧����������{a.Name}�����������ԣ�ʵ������:{typeBuilder.Name}");
				typeBuilder.SetCustomAttribute(g.Generate(a));
			}

			foreach (var prop in type.Properties)
				switch (prop.Mode)
				{
					case PropertyMode.Value:
						BuildValueProperty(typeBuilder, type,prop);
						break;
					case PropertyMode.MultipleRelation:
						BuildValueProperty(typeBuilder, type, prop);
						break;
					case PropertyMode.SingleRelation:
						BuildValueProperty(typeBuilder, type, prop);
						break;
					default:
						throw new NotSupportedException();
				}
			return typeBuilder.CreateTypeInfo().AsType();
		}
		public Type[] Build()
		{
			foreach (var et in metas.EntityTypes)
				TypeBuilders.Add(et.Key, ModuleBuilder.DefineType(et.Key, TypeAttributes.Public, typeof(BaseDataModel)));

			return metas.EntityTypes.Values.Select(et => BuildType(TypeBuilders[et.FullName], et)).ToArray();
			
		}
	}
}
