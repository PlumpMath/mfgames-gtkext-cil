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

namespace MfGames.GtkExt.Configurators
{
	/// <summary>
	/// Describes the types of apply modes that both regular Gtk dialog
	/// boxes and configurators use.
	/// </summary>
	public enum ApplyMode
	{
		/// <summary>
		/// Indicates the configurator applies all changes immediately. This
		/// means the window/panel will only have a "close" button. If there
		/// is a section that needs (effectively) explicit apply, that group
		/// should have a seperate and dedicated apply button.
		/// </summary>
		Instant,

		/// <summary>
		/// Indicates the configurator is an "explicit" apply which means
		/// the configurator has an "apply", "cancel", and "ok" button. When
		/// a configuration uses this mode, the configurator panel will call
		/// Apply or Cancel on the IGtkConfigurator interface.
		/// </summary>
		Explicit,
	}
}