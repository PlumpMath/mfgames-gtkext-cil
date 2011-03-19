using C5;

using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;
using MfGames.GtkExt.TextEditor.Models.Styles;

namespace GtkExtDemo.TextEditor
{
	/// <summary>
	/// Implements a demo editable buffer that is intended to be styled and
	/// formatted.
	/// </summary>
	public class DemoEditableLineBuffer : MemoryLineBuffer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DemoEditableLineBuffer"/> class.
		/// </summary>
		public DemoEditableLineBuffer()
		{
			// Set up the line styles.
			styles = new HashDictionary<int, DemoLineStyleType>();

			// Create the initial lines. There is already one in the buffer before
			// this insert operates.
			InsertLines(0, 4);

			// Set the text on the lines with the prefix so they can be styled
			// as part of the set operation.
			SetText(0, "H: Heading Line");
			SetText(1, "D: Regular Text");
			SetText(2, "H:");
			SetText(3, "D: Regular Text");
			SetText(4, "D: Regular Text");
		}

		#endregion

		#region Operations

		/// <summary>
		/// Performs the given operation, raising any events for changing.
		/// </summary>
		/// <param name="operation">The operation.</param>
		public override void Do(ILineBufferOperation operation)
		{
			// Figure out any changes to the operations before passing them
			// to the memory buffer.
			if (operation.OperationType == LineBufferOperationType.SetText)
			{
				var setText = (SetTextOperation) operation;

				if (setText.Text.Length >= 2 &&
					setText.Text.Substring(1, 1) == ":")
				{
					switch (setText.Text[0])
					{
						case 'D':
							styles.Remove(setText.LineIndex);
							break;

						case 'H':
							styles[setText.LineIndex] = DemoLineStyleType.Heading;
							break;
					}
				}
			}

			base.Do(operation);
		}

		#endregion

		#region Buffers

		private HashDictionary<int, DemoLineStyleType> styles;

		/// <summary>
		/// Gets the name of the line style based on the settings.
		/// </summary>
		/// <param name="lineIndex">The line index in the buffer or
		/// Int32.MaxValue for the last line.</param>
		/// <returns></returns>
		public override string GetLineStyleName(int lineIndex)
		{
			// See if we have the line in the styles.
			if (styles.Contains(lineIndex))
			{
				return styles[lineIndex].ToString();
			}

			return DemoLineStyleType.Default.ToString();
		}

		#endregion
	}
}