// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using Gtk;

namespace MfGames.GtkExt.Extensions.Gtk
{
	/// <summary>
	/// Defines extensions on Gtk.Window.
	/// </summary>
	public static class GtkWindowExtensions
	{
		#region Methods

		/// <summary>
		/// Restores the window state from the installed WindowStateSettings.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="windowName">Name of the window.</param>
		/// <param name="attachToWindow">if set to <c>true</c> [attach to window].</param>
		public static void RestoreState(
			this Window window,
			string windowName,
			bool attachToWindow = false)
		{
			WindowStateSettings.Instance.RestoreState(window, windowName, attachToWindow);
		}

		/// <summary>
		/// Saves the state of the window using the installed WindowStateSettings.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="windowName">Name of the window.</param>
		public static void SaveState(
			this Window window,
			string windowName)
		{
			WindowStateSettings.Instance.SaveState(window, windowName);
		}

		#endregion
	}
}
