// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System.ComponentModel;

namespace GtkExtDemo
{
	public enum ExampleEnumeration
	{
		[Description("Short Name")]
		ShortName,

		[Description("Long Name Too")]
		LongName,
	}
}
