#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
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

using System;
using System.Collections.Generic;

#endregion

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// A list of <see cref="LayoutGroup"/> objects.
	/// </summary>
	public class LayoutGroupCollection : List<LayoutGroup>
	{
		/// <summary>
		/// Gets the <see cref="MfGames.GtkExt.Actions.Layouts.LayoutGroup"/> with the
		/// specified group name.
		/// </summary>
		/// <value></value>
		public LayoutGroup this[string groupName]
		{
			get
			{
				foreach (LayoutGroup group in this)
				{
					if (group.Id == groupName)
					{
						return group;
					}
				}

				throw new Exception("Cannot find group " + groupName);
			}
		}
	}
}