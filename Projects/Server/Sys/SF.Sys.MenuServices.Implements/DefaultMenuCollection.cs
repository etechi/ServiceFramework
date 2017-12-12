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
using System.Linq;
using System.Collections.Generic;

namespace SF.Sys.MenuServices
{
	public class DefaultMenuCollection : IDefaultMenuCollection
	{
		public string[] MenuIdents => Menus.Keys.ToArray();

		Dictionary<string, List<MenuItem>> Menus { get; } = new Dictionary<string, List<MenuItem>>();
		public void AddMenuItems(
			string MenuIdent,
			string Path,
			MenuItem[] Items
			)
		{
			var rootItems = Path
				.Split('/')
				.Reverse()
				.Aggregate(
					Items.ToList(), 
					(i, path) => new List<MenuItem>{new MenuItem
					{
						Name = path,
						Title = path,
						Children = i
					} });

			lock (Menus)
			{
				Menus.TryGetValue(MenuIdent, out var items);
				Menus[MenuIdent] = ADT.Tree.Merge(
					items,
					rootItems,
					i => i.Name,
					i => i?.Children,
					(x, y, cs) =>
					{
						if (x == null) return y;
						if (y == null) return x;
						var ycs = y.Children;
						y.Children = null;
						var xOrder = x.ItemOrder;
						Poco.Update(x, y);
						if (xOrder < x.ItemOrder)
							x.ItemOrder = xOrder;
						y.Children = ycs;
						x.Children = cs.ToList();//.OrderBy(m=>m.ItemOrder).ToList();
						return x;
					}).ToList();

				//	Menus[MenuIdent] = items = new List<MenuItem>();

				//if(!Path.IsNullOrEmpty())
				//	foreach(var f in Path.Split('/'))
				//	{
				//		var idx = items.IndexOf(i => i.Name == f);
				//		if (idx == -1)
				//		{
				//			idx = items.Count;
				//			items.Add(new MenuItem
				//			{
				//				Name = f,
				//				Title = f
				//			});
				//		}
				//		var ci = items[idx];
				//		if (ci.Children == null)
				//			ci.Children = new List<MenuItem>();
				//		items = (List< MenuItem > )ci.Children;
				//	}
				
				//MergeItem(items, Item);
			}

		}

		//void MergeItem(List<MenuItem> items,MenuItem Item)
		//{
		//	var cidx = items.IndexOf(i => i.Name == Item.Name);
		//	if (cidx == -1)
		//	{
		//		items.Add(Item);
		//		return;
		//	}
		//	var oitem = items[cidx];
		//	oitem.Update(Item, Item.UpdatedTime);
		//	oitem.ServiceId = Item.ServiceId;
		//	oitem.Action = Item.Action;
		//	oitem.ActionArgument = Item.ActionArgument;
		//	oitem.FontIcon = Item.FontIcon;

		//	if (Item.Children == null)
		//		return;
		//	if (oitem.Children == null)
		//		oitem.Children = new List<MenuItem>();
		//	foreach (var ci in Item.Children)
		//		MergeItem((List<MenuItem>)oitem.Children, ci);
		//}

		public MenuItem[] GetMenuItems(string MenuIdent)
		{
			lock (Menus)
				if (Menus.TryGetValue(MenuIdent, out var items))
					return Poco.Clone(items).ToArray();
				else
					return Array.Empty<MenuItem>();

		}
	}

}
