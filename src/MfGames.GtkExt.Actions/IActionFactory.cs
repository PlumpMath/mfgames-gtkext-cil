// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.Collections.Generic;
using Gtk;

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Identifies a class that can configure one or more multiple user actions
	/// in a single call.
	/// </summary>
	public interface IActionFactory
	{
		#region Methods

		/// <summary>
		/// Creates all the <see cref="Action"/> objects associated with the extending
		/// class.
		/// </summary>
		/// <returns></returns>
		ICollection<Action> CreateActions();

		#endregion
	}
}
