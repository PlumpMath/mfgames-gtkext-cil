// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

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
