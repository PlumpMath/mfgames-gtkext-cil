// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.GtkExt.TextEditor.Models.Buffers;

namespace MfGames.GtkExt.TextEditor.Models
{
	public class OperationContext
	{
		public OperationContext(LineBuffer lineBuffer)
		{
			LineBuffer = lineBuffer;
		}

		public LineBufferOperationResults? Results { get; set; }
		public LineBuffer LineBuffer { get; private set; }
	}
}