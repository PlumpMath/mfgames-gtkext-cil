// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;

namespace MfGames.GtkExt.Actions.Layouts
{
	/// <summary>
	/// A list of <see cref="LayoutGroup"/> objects.
	/// </summary>
	public class LayoutGroupCollection: List<LayoutGroup>
	{
		#region Properties

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

		#endregion
	}
}
