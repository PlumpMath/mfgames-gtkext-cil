// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using MfGames.Commands.TextEditing;

namespace MfGames.GtkExt.TextEditor.Models.Buffers
{
	public abstract class TextEditingOperation:
		ITextEditingCommand<OperationContext>
	{
		#region Properties

		public bool CanUndo
		{
			get { return true; }
		}

		public bool IsTransient
		{
			get { return false; }
		}

		public bool UpdateTextPosition { get; set; }
		public bool UpdateTextSelection { get; set; }
		protected TextPosition InitialPosition { get; set; }

		#endregion

		#region Methods

		public abstract void Do(OperationContext state);

		public abstract void Redo(OperationContext state);

		public abstract void Undo(OperationContext state);

		#endregion
	}
}
