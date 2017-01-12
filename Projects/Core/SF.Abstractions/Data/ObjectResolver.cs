using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Reflection;
using System.Reflection;
namespace SF.Data
{
    public interface IDataObject
    {
        string Ident { get; }
        string Name { get; }
    }
	public interface IDataObjectResolver
	{
        Task<IDataObject[]> Resolve(string Type,string[][] Keys);
    }
    public interface IDataObjectLoader
    {
        Task<IDataObject[]> Load(string Type,string[][] Keys);
    }
   
   
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class DataObjectLoaderAttribute : Attribute
    {
        public string Type { get; }
        public Type AliasType { get; }
        public DataObjectLoaderAttribute(string Type,Type AliasType=null)
        {
            this.Type = Type;
            this.AliasType = AliasType;
        }
    }
}
