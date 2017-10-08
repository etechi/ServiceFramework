using SF.Common.Documents.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Entities;
namespace SF.Common.Documents
{
	public static class DocumentsSetupExtension
	{
		public static async Task<TEditable> DocumentEnsure<TInternal, TEditable>(
			this IDocumentManager<TInternal,TEditable> Manager, 
			string ident, 
			string name, 
			string content, 
			string image = null, 
			string summary = null, 
			long? author = null
			)
			where TInternal:DocumentInternal
			where TEditable:DocumentEditable, new()
		{
			return await Manager.DocumentEnsure( null, ident, 0, name, content, image, summary, author);
		}
		public static async Task<TEditable> DocumentEnsure<TInternal, TEditable>(
				this IDocumentManager<TInternal, TEditable> Manager,
				long? cid, 
				int order, 
				string name, 
				string content, 
				string image = null, 
				string summary = null,
				long? author = null
			)
				where TInternal : DocumentInternal
			where TEditable : DocumentEditable, new()
		{
			return await Manager.DocumentEnsure( cid, null, order, name, content, image, summary, author);
		}
		public static async Task<TEditable> DocumentEnsure<TInternal, TEditable>(
				this IDocumentManager<TInternal, TEditable> Manager,
				long? cid, 
				string ident, 
				int order, 
				string name, 
				string content, 
				string image = null, 
				string summary = null,
				long? author = null
			)
			where TInternal : DocumentInternal
			where TEditable : DocumentEditable,new()
		{
			return await Manager.EnsureEntity(
				await Manager.QuerySingleEntityIdent(new DocumentQueryArguments { Name = name }),
				s =>
				{
					s.Name = name;
					s.Title = name;
					s.Ident = ident;
					s.ContainerId= cid;
					s.Content = content;
					s.Image = image;
					s.LogicState= EntityLogicState.Enabled;
					s.Description = summary;
					s.ItemOrder = order;
					s.PublishDate = DateTime.Now;
				});
		}

		public static async Task<TInternal> CategoryEnsure<TInternal>(
			this IDocumentCategoryManager<TInternal> Manager, 
			long? pid, 
			int order, 
			string name, 
			string title=null,
			string image = null, 
			string summary = null, 
			long? author = null
			)
			where TInternal : CategoryInternal,new()
		{
			return await Manager.EnsureEntity(
				await Manager.QuerySingleEntityIdent(new DocumentCategoryQueryArgument { Name = name }),
				(TInternal s) =>
				{
					s.Name = name;
					s.Title = title??name;
					s.ContainerId = pid;
					s.Image = image;
					s.LogicState = EntityLogicState.Enabled;
					s.Description = summary;
					s.ItemOrder = order;
				}
				);

		}

		public class Item
		{
			public long Id;
			public string Name;
			public bool IsFile;
			public Item[] Children;
		}
		public static async Task<Item[]> DocEnsureFromFiles<TDocInternal, TDocEditable, TCategoryInternal>(
			this IDocumentManager<TDocInternal,TDocEditable> DocManager,
			IDocumentCategoryManager<TCategoryInternal> CatManager,
			long? catId,
			string folder,
			Dictionary<string, string> idents=null
			)
			where TDocInternal:DocumentInternal
			where TDocEditable:DocumentEditable,new()
			where TCategoryInternal:CategoryInternal,new()
		{
			var items = new List<Item>();
			var files = System.IO.Directory.GetFiles(folder)
			   .Where(f => f.EndsWith(".htm"))
			   .Select(f =>
			   {
				   var name = System.IO.Path.GetFileNameWithoutExtension(f);
				   var i = name.IndexOf('-');
				   int order = 0;
				   if (i != -1 && int.TryParse(name.Substring(0, i), out order))
					   name = name.Substring(i + 1);
				   return new
				   {
					   order = order,
					   name = name,
					   file = f
				   };
			   })
				.OrderBy(p => p.order)
				.Select((p, i) => new { index = i, name = p.name, file = p.file });

			foreach (var f in files)
			{
				var html = System.IO.File.ReadAllText(f.file);
				var re = await DocManager.DocumentEnsure(
					catId,
					idents?.Get(f.name),
					f.index,
					f.name,
					html
					);
				items.Add(new Item { Id = re.Id, Name = re.Name, IsFile = true });
			}

			var dirs = System.IO.Directory.GetDirectories(folder).Select(f =>
			{
				var name = System.IO.Path.GetFileNameWithoutExtension(f);
				var i = name.IndexOf('-');
				int order = 0;
				if (i != -1 && int.TryParse(name.Substring(0, i), out order))
					name = name.Substring(i + 1);
				return new
				{
					order = order,
					name = name,
					file = f
				};
			})
			 .OrderBy(p => p.order)
			 .Select((p, i) => new { index = i, name = p.name, file = p.file });

			foreach (var d in dirs)
			{
				var pcat = await CatManager.CategoryEnsure(catId, d.index, d.name);
				var chds = await DocEnsureFromFiles(DocManager,CatManager, pcat.Id, d.file, idents);
				items.Add(new Item { Id = pcat.Id, Name = pcat.Name, Children = chds });
			}
			return items.ToArray();
		}

	}

}

