#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

#region Namespaces

using System.Collections.Generic;

using Gtk;

using MfGames.Extensions.System.Collections.Generic;
using MfGames.HierarchicalPaths;

#endregion

namespace MfGames.GtkExt.Extensions.System.Collections.Generic
{
	/// <summary>
	/// Defines the extension methods for IList generic classes.
	/// </summary>
	public static class SystemCollectionsGenericIListExtensions
	{
		#region ToTreeStore

		/// <summary>
		/// Converts the given list of path containers into a Gtk.TreeStore.
		/// The resulting store will have three fields: the part of the path
		/// that represents the item, the full path, and the item. If the level
		/// doesn't exist (such as the created "/a" when "/a/b" is the only item),
		/// then the third column will be the default value.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="list">The list.</param>
		/// <returns></returns>
		public static TreeStore ToTreeStore<TItem>(
			this IList<TItem> list) where TItem: IHierarchicalPathContainer
		{
			return ToTreeStore<TItem>(list, false);
		}

		/// <summary>
		/// Converts the given list of path containers into a Gtk.TreeStore.
		/// The resulting store will have three fields: the part of the path
		/// that represents the item, the full path, and the item. If the level
		/// doesn't exist (such as the created "/a" when "/a/b" is the only item),
		/// then the third column will be the default value.
		/// </summary>
		/// <typeparam name="TItem">The type of the item.</typeparam>
		/// <param name="list">The list.</param>
		/// <param name="reorderInPlace">if set to <c>true</c> [reorder in place].</param>
		/// <returns></returns>
		public static TreeStore ToTreeStore<TItem>(
			this IList<TItem> list,
			bool reorderInPlace) where TItem: IHierarchicalPathContainer
		{
			// Create a new tree store to populate.
			var store = new TreeStore(
				typeof(string), typeof(HierarchicalPath), typeof(TItem));

			// If we are not reordering the list in place, then create a new one
			// that we can reorder.
			if (!reorderInPlace)
			{
				var newList = new List<TItem>(list);
				list = newList;
			}

			// Order the list so everything is grouped together.
			list.OrderByHierarchicalPath();

			// Go through the ordered list and append all the tree elements.
			// Because of the ordering, we can do this as a single pass through
			// the list.
			HierarchicalPath lastPath = null;
			var iterPath = new List<TreeIter>();

			foreach (TItem item in list)
			{
				// Pull out the path we'll be appending.
				HierarchicalPath path = item.HierarchicalPath;

				// Roll up the list to a common root.
				while (lastPath != null && lastPath.Count > 0 && !path.StartsWith(lastPath))
				{
					// We don't have a common root, so move up a level.
					iterPath.RemoveLast();
					lastPath = lastPath.Parent;
				}

				// Add any parent items to the list that we don't already
				// have. We do this by adding placeholders up to the current
				// TreeIter list until we get to the parent.
				TreeIter treeIter;

				while (iterPath.Count < path.Count - 1)
				{
					// Pull out the path we'll be inserting.
					string parentName = path[iterPath.Count];

					lastPath = path.Splice(iterPath.Count, 1);

					// We have to add a placeholder item for this inserted 
					// parent item.
					if (iterPath.Count == 0)
					{
						// We are at the top, so insert it there.
						treeIter = store.AppendValues(parentName, lastPath, default(TItem));
					}
					else
					{
						// Append it to the parent item above it.
						treeIter = store.AppendValues(
							iterPath[iterPath.Count - 2], parentName, lastPath, default(TItem));
					}

					// Append the newly inserted iterator into the list.
					iterPath.Add(treeIter);
				}

				// Once we get through this, we insert the current item.
				lastPath = item.HierarchicalPath;
				string name = lastPath.Last;

				if (iterPath.Count == 0)
				{
					// We are at the top, so insert it there.
					treeIter = store.AppendValues(name, lastPath, item);
				}
				else
				{
					// Append it to the parent item above it.
					treeIter = store.AppendValues(
						iterPath[iterPath.Count - 1], name, lastPath, item);
				}

				// Append the newly inserted iterator into the list.
				iterPath.Add(treeIter);
			}

			// Return the resulting store.
			return store;
		}

		#endregion
	}
}