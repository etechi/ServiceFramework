using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.TableExports
{
    public class Column
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
    }
    public interface ITableExporter : IDisposable
    {
        void AddRow(object[] Cells);
        string ContentType { get; }
        string FileExtension { get; }
    }
    public interface ITableExporterFactory
    {
        ITableExporter Create(
            System.IO.Stream Stream,
            string Title,
            Column[] Columns
            );
    }
}
