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
	public class SystemAttributeBuilder
	{
		public ConstructorInfo Constructor { get; }
		public object[] Arguments { get; }
		public PropertyInfo[] InitProperties { get; }
		public object[] InitPropertyValues { get; }
		public SystemAttributeBuilder(
			ConstructorInfo Constructor,
			object[] Arguments =null,
			PropertyInfo[] InitProperties=null,
			object[] InitPropertyValues=null
		)
		{
			this.Constructor = Constructor;
			this.Arguments = Arguments ?? Array.Empty<object>();
			this.InitProperties = InitProperties ?? Array.Empty<PropertyInfo>();
			this.InitPropertyValues = InitPropertyValues ?? Array.Empty<object>();
		}
	}
	public interface IDataModelAttributeGenerator
	{
		SystemAttributeBuilder Generate(IAttribute Attr);
	}
	public class BaseDataModel
	{

	}
	public interface IDataModelTypeCollection: IReadOnlyDictionary<string, Type>
	{

	}
	public class DataModelTypeCollection: Dictionary<string,Type>, IDataModelTypeCollection
	{

	}
	public class DataModelTypeBuilder
	{
		IMetadataCollection metas { get; }
		
		static AssemblyBuilder AssemblyBuilder { get; }= 
			AssemblyBuilder.DefineDynamicAssembly(
				new AssemblyName("SFAutoEntityProviderDataModelClasses"), 
				AssemblyBuilderAccess.Run
				);
		static ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(new Guid().ToString("N"));

		Dictionary<string, Type> TypeBuilders = new Dictionary<string, Type>();
		Dictionary<MemberInfo, List<SystemAttributeBuilder>> MemberAttributes { get; } = new Dictionary<MemberInfo, List<SystemAttributeBuilder>>();
		NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver { get; }

		public DataModelTypeBuilder(
			IMetadataCollection metas,
			NamedServiceResolver<IDataModelAttributeGenerator> DataModelAttributeGeneratorResolver
			)
		{
			this.metas = metas;
			this.DataModelAttributeGeneratorResolver = DataModelAttributeGeneratorResolver;
		}
		void EnsureSingleFieldIndex(IEntityType type, IProperty prop, PropertyBuilder propBuilder)
		{
			if (MemberAttributes.TryGetValue(propBuilder, out var attrs))
			{
				//�Ѿ�������
				if (attrs.Any(a =>
					 a.Constructor.ReflectedType == typeof(IndexAttribute) &&
					 (a.Arguments?.Length ?? 0) == 0 &&
					 (a.InitProperties?.Length ?? 0) == 0 &&
					 (a.InitPropertyValues?.Length ?? 0) == 0
					))
					return;

				//��ǰ�ֶ�������
				if (prop.Attributes.Any(a =>
						 a.Name == typeof(KeyAttribute).FullName &&
						 type.Properties.Count(p => p.Attributes.Any(aa => aa.Name == typeof(KeyAttribute).FullName)) == 1
						))
					return;
			}

			propBuilder.SetCustomAttribute(
				AddAttribute(
					propBuilder,
					new SystemAttributeBuilder(typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()))
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
		CustomAttributeBuilder AddAttribute(MemberInfo member,SystemAttributeBuilder attr)
		{
			if (!MemberAttributes.TryGetValue(member, out var attrs))
				MemberAttributes.Add(member, attrs = new List<SystemAttributeBuilder>());
			attrs.Add(attr);
			return new CustomAttributeBuilder(
				attr.Constructor,
				attr.Arguments,
				attr.InitProperties,
				attr.InitPropertyValues
				);
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
					var aa = g.Generate(a);
					if(aa!=null)
						pb.SetCustomAttribute(AddAttribute(pb,aa));
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
				EnsureSingleFieldIndex(curEntity,prop, pb);
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

			//�����ʵ���ʶ����Ҫ��������
			if (prop.Attributes.Any(a=>a.Name==typeof(EntityIdentAttribute).FullName))
				EnsureSingleFieldIndex(type,prop,pb);
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

		Type BuildType(TypeBuilder typeBuilder,IEntityType type,string TablePrefix)
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

			foreach (var a in type.Attributes.Where(a=>a.Name!=typeof(TableAttribute).FullName))
			{
				var g = DataModelAttributeGeneratorResolver(a.Name);
				if (g == null)
					throw new ArgumentException($"��֧����������{a.Name}�����������ԣ�ʵ������:{typeBuilder.Name}");
				var aa = g.Generate(a);
				if (aa != null)
					typeBuilder.SetCustomAttribute(AddAttribute(typeBuilder, aa));
				
			}
			var tableName = type.Name;
			var tableAttr = type.Attributes.FirstOrDefault(a => a.Name == typeof(TableAttribute).FullName);
			if (tableAttr != null)
				tableName = tableAttr.Values?.Get("Name") as string ?? tableName;

			typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(
				typeof(TableAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { (TablePrefix??"")+( type.Namespace ??"")+tableName }
				));


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
		static volatile int TypeIdSeed = 1;
		static int NextTypeId()
		{
			return System.Threading.Interlocked.Increment(ref TypeIdSeed);
		}
		public IDataModelTypeCollection Build(string Prefix)
		{
			//throw new ArgumentException(metas.ToString());
			foreach (var et in metas.EntityTypes)
			{
				TypeBuilders.Add(
					et.Key, 
					ModuleBuilder.DefineType(
						et.Key+"_"+ NextTypeId(), 
						TypeAttributes.Public, 
						typeof(BaseDataModel)
						)
					);
			}

			var re = new DataModelTypeCollection();
			foreach (var et in metas.EntityTypes.Values)
			{
				var mt = BuildType((TypeBuilder)TypeBuilders[et.FullName], et, Prefix);
				re.Add(et.FullName, mt);
			}
			return re;
		}
	}
}
