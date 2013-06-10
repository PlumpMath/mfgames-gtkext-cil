// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

namespace MfGames.GtkExt.Actions
{
	/// <summary>
	/// Defines the additional elements of an action that can be configured
	/// and arranged.
	/// </summary>
	public interface IConfigurableAction
	{
		#region Properties

		/// <summary>
		/// Gets the name of the action group to associate with this action.
		/// </summary>
		/// <value>
		/// The name of the group.
		/// </value>
		string GroupName { get; }

		#endregion
	}
}
