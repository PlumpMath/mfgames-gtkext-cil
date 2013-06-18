// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Diagnostics.Contracts;
using MfGames.HierarchicalPaths;

namespace MfGames.Commands
{
	/// <summary>
	/// Describes a reference to a command factory via a <see cref="HierarchicalPath"/>
	/// with an optional argument. This is an immutable class though the Data property
	/// may have inner properties changed (though this is highly discouraged).
	/// </summary>
	public class CommandFactoryReference
	{
		#region Properties

		/// <summary>
		/// Contains the optional data for the argument. In menu items, this typically
		/// won't have options (exception for maybe recent items), but in popups and
		/// other cases, it may have additional data to influence the generated
		/// command.
		/// </summary>
		public object Data { get; private set; }

		/// <summary>
		/// Contains the required 
		/// </summary>
		public HierarchicalPath Key { get; private set; }

		#endregion

		#region Constructors

		public CommandFactoryReference(
			HierarchicalPath key,
			object data = null)
		{
			// Make sure we have a sane state.
			Contract.Requires<ArgumentNullException>(key != null);

			// Store the values for later.
			Key = key;
			Data = data;
		}

		#endregion
	}
}
