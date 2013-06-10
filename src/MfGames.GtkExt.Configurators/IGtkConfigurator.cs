// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using Gtk;
using MfGames.HierarchicalPaths;

namespace MfGames.GtkExt.Configurators
{
	/// <summary>
	/// Describes the signature for configurators that provide Gtk# panel
	/// for user configuration. Configurator panels are organized using 
	/// hierarchial path to create the nested tree.
	/// 
	/// When a configurator is first shown, the container panel will call
	/// CreateConfiguratorWidget(). When the container is closed, the Dispose()
	/// method of this interface will be called.
	/// </summary>
	public interface IGtkConfigurator: IDisposable,
		IHierarchicalPathContainer
	{
		#region Properties

		/// <summary>
		/// Indicates the apply mode (instant or explicit) that the configurator
		/// operates under. A single configurators panel cannot have differing
		/// types of apply modes and will throw an exception.
		/// </summary>
		ApplyMode ApplyMode { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Applies the configurator, in case of explict apply configurators.
		/// 
		/// This will only be called for explict apply configurators.
		/// </summary>
		void ApplyConfigurator();

		/// <summary>
		/// Reverts or restores the state of the system at the point that
		/// InitializeConfigurator() was called.
		/// 
		/// This will only be called for explict apply configurators.
		/// </summary>
		void CancelConfigurator();

		/// <summary>
		/// Creates a widget to be placed at the configurator tree as given
		/// by the path. For a given instance, this must always return the
		/// same widget. The created widget should be destroyed as part of the
		/// Dispose() method of this class.
		/// </summary>
		/// <returns></returns>
		Widget CreateConfiguratorWidget();

		/// <summary>
		/// Initializes the configurator. For Explict Apply configurators, this
		/// should mark the state of the system to properly roll back.
		/// 
		/// This will be called for all configurators.
		/// </summary>
		void InitializeConfigurator();

		#endregion
	}
}
