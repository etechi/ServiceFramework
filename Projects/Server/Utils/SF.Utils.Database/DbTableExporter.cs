using SF.Sys.IO;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Utils.Database
{
	public static class DBTableExporter
	{
		public static Task SqlExport(
			this DbConnection Conn, 
			string FilePath,
			string Sql
			)
		{
			return FS.UseTextWriter(FilePath, async w =>
			{
				if (Conn.State != ConnectionState.Open)
					await Conn.OpenAsync();
				using (var cmd = Conn.CreateCommand())
				{
					Console.Write($"{FilePath} < {Sql} ... ");
					cmd.CommandText = Sql;
					using (var r = await cmd.ExecuteReaderAsync())
					{
						await CSVFile.Write(
							w,
							Enumerable.Range(0, r.FieldCount).Select(i => r.GetName(i))
							);
						while (await r.ReadAsync())
						{
							await CSVFile.Write(
								w,
								Enumerable.Range(0, r.FieldCount)
									.Select(i =>
										r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i))
										)
								);
						}
					}
					Console.WriteLine($"Done!");
				}
			});
		}

	}
}
