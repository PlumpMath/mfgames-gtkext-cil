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

#endregion

namespace MfGames.GtkExt.Extensions.Gtk
{
	/// <summary>
	/// Defines extensions on Gtk.Window.
	/// </summary>
	public static class GtkWindowExtensions
	{
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
			WindowStateSettings.Instance.RestoreState(
				window,
				windowName,
				attachToWindow);
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
			WindowStateSettings.Instance.SaveState(
				window,
				windowName);
		}
	}
}