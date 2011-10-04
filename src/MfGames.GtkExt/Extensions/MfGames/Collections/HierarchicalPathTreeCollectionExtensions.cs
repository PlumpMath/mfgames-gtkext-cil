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

using Gtk;

using MfGames.Collections;

#endregion

namespace MfGames.GtkExt.Extensions.MfGames.Collections
{
	/// <summary>
	/// Extends MfGames.Collections.HierarchicalPathTreeCollection to convert to
	/// Gtk# objects.
	/// </summary>
	public static class HierarchicalPathTreeCollectionExtensions
	{
		/// <summary>
		/// Appends all of the child nodes into the tree.
		/// </summary>
		/// <typeparam name="TValue">The type represented by the tree.</typeparam>
		/// <param name="store">The TreeStore to append to.</param>
		/// <param name="iter">The TreeIter to append the nodes underneath.</param>
		/// <param name="tree">The tree level to append the children from.</param>
		private static void AppendChildrenToTreeStore<TValue>(
			TreeStore store,
			TreeIter iter,
			HierarchicalPathTreeCollection<TValue> tree)
		{
			// Go through all the child nodes under the three.
			foreach (HierarchicalPathTreeCollection<TValue> child in tree.DirectNodes)
			{
				AppendToTreeStore(store, iter, child, child.Path.Last);
			}
		}

		/// <summary>
		/// Appends the given tree to the root of the tree store.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="store"></param>
		/// <param name="tree"></param>
		/// <param name="nodeName"></param>
		private static void AppendToTreeStore<TValue>(
			TreeStore store,
			HierarchicalPathTreeCollection<TValue> tree,
			string nodeName)
		{
			// Insert the first node into the tree.
			TreeIter iter = store.AppendValues(nodeName, tree.Path.Last, tree);

			// Go through all the child nodes under the three.
			AppendChildrenToTreeStore(store, iter, tree);
		}

		/// <summary>
		/// Appends to the tree store underneath the given iterator.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="store"></param>
		/// <param name="iter"></param>
		/// <param name="tree"></param>
		/// <param name="nodeName"></param>
		private static void AppendToTreeStore<TValue>(
			TreeStore store,
			TreeIter iter,
			HierarchicalPathTreeCollection<TValue> tree,
			string nodeName)
		{
			// Insert the node into the tree.
			TreeIter childIter = store.AppendValues(iter, nodeName, tree.Path.Last, tree);

			// Insert the children into the tree.
			AppendChildrenToTreeStore(store, childIter, tree);
		}

		/// <summary>
		/// Converts a HierarchicalPathTreeCollection into a Gtk.TreeStore.
		/// </summary>
		public static TreeStore ToTreeStore<TValue>(
			this HierarchicalPathTreeCollection<TValue> tree,
			bool includeRootNode = false,
			string rootName = "<Root>")
		{
			// Create a new tree store to populate.
			var store = new TreeStore(
				typeof(string), typeof(HierarchicalPath), typeof(TValue));

			// If we are including the parent, then start with that.
			if (includeRootNode)
			{
				AppendToTreeStore(store, tree, rootName);
			}
			else
			{
				// Populate the child nodes.
				foreach (HierarchicalPathTreeCollection<TValue> node in tree.DirectNodes)
				{
					AppendToTreeStore(store, node, node.Path.Last);
				}
			}

			// Return the resulting store.
			return store;
		}
	}
}