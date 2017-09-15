using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.ADT
{
	public static class Tree
	{
		public static IEnumerable<T> AsEnumerable<T>(
			T Node,
			Func<T,IEnumerable<T>> GetChildren
			)
		{
			yield return Node;
			var cs = GetChildren(Node);
			if (cs != null)
				foreach (var n in cs)
					foreach (var c in AsEnumerable(n, GetChildren))
						yield return c;
		}
		public static IEnumerable<T> AsEnumerable<T>(
			IEnumerable<T> Nodes,
			Func<T, IEnumerable<T>> GetChildren
			)
			=> Nodes.SelectMany(n => AsEnumerable(n, GetChildren));
		public static IEnumerable<T> AsLastEnumerable<T>(
			T Node,
			Func<T, IEnumerable<T>> GetChildren
			)
		{
			var cs = GetChildren(Node);
			if (cs != null)
				foreach (var n in cs)
					foreach (var c in AsEnumerable(n, GetChildren))
						yield return c;
			yield return Node;
		}
		public static IEnumerable<T> AsLastEnumerable<T>(
			IEnumerable<T> Nodes,
			Func<T, IEnumerable<T>> GetChildren
			)
			=> Nodes.SelectMany(n => AsLastEnumerable(n, GetChildren));

		public static IEnumerable<T> Build<I, T, K>(
			this IEnumerable<I> nodes,
			Func<I, T> NewTreeItem,
			Func<I, K> GetId,
			Func<I, K> GetParentId,
			Action<T, T> AddChildItem
			)
			where K : IEquatable<K>
		{
			var dic = nodes.ToDictionary(GetId, i => (TreeItem:NewTreeItem(i), Item:i));
			var roots = new List<T>();
			foreach (var (ti, ii) in dic.Values)
			{
				var pi = GetParentId(ii);
				if (EqualityComparer<K>.Default.Equals(pi, default(K)))
					roots.Add(ti);
				else
				{
					if (!dic.TryGetValue(pi, out var pnt))
						throw new InvalidOperationException($"在节点列表中找不到节点{GetId(ii)}的父节点{pi}");
					var (pti, pii) = pnt;
					AddChildItem(pti, ti);
				}
			}
			return roots;
		}
		public static IEnumerable<I> Build<I, K>(
		   this IEnumerable<I> nodes,
		   Func<I, K> GetId,
		   Func<I, K> GetParentId,
		   Action<I, I> AddChildItem
		   )
		   where K : IEquatable<K>
			=> Build(nodes, i => i, GetId, GetParentId, AddChildItem);



		public static IEnumerable<T> BuildByPath<I, T>(
			this IEnumerable<I> nodes,
			Func<I, T> NewTreeItem,
			Func<I, string[]> GetPath,
			Action<T, T> AddChildItem
			)
		{
			var dic = nodes.Select(i =>
			{
				var path = GetPath(i);
				return new
				{
					item = i,
					parent = path.Take(path.Length - 1).Join("\n"),
					id = path.Join("\n")
				};
			}).ToDictionary(p => p.item);
			return nodes.Build<I, T, string>(
				i => NewTreeItem(i),
				i => dic[i].id,
				i => dic[i].parent,
				AddChildItem
				);
		}

		public static IEnumerable<T> BuildByParentPath<I, T>(
			IEnumerable<I> nodes,
			Func<I[], T> NewTreeItem,
			Func<I, string[]> GetPath,
			Action<T, T> AddChildItem
			)
		{
			var dic = nodes.Select(i =>
			{
				var path = GetPath(i);
				return new
				{
					item = i,
					id = path.Join("\n")
				};
			})
			.GroupBy(p => p.id)
			.ToDictionary(g => g.Key, g => g.ToArray());

			return dic.Build(
				p => NewTreeItem(p.Value.Select(ii => ii.item).ToArray()),
				p => p.Key,
				p => p.Key.LastSplit2('\n').Item1,
				AddChildItem
				);

		}
		public static async Task ValidateTreeParent<TKey>(
			string Title,
			TKey Id,
			TKey NewParentId,
			Func<TKey, Task<TKey>> GetParentId
			) where TKey : IEquatable<TKey>
		{
			//检查父节点不能是自身，或自身的子节点
			if (Id.Equals(default(TKey)) || NewParentId.Equals(default(TKey)))
				return;
			if (Id.Equals(NewParentId))
				throw new PublicArgumentException($"父{Title}不能是当前{Title}自己");

			for (; ; )
			{
				var npid = await GetParentId(NewParentId);
				if (npid.Equals(default(TKey)))
					break;
				if (npid.Equals(Id))
					throw new PublicArgumentException($"父{Title}不能是当{Title}的子{Title}");
				NewParentId = npid;
			}
		}
	}
}

