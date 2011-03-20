#region Copyright and License

// Copyright (c) 2009-2011, Moonfire Games
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

using Gdk;

using Gtk;

using MfGames.GtkExt.TextEditor.Editing;
using MfGames.GtkExt.TextEditor.Editing.Actions;
using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Renderers;

using NUnit.Framework;

using Key=Gdk.Key;

#endregion

namespace MfGames.GtkExt.TextEditor.Tests
{
	/// <summary>
	/// Tests the various functionality of text actions.
	/// </summary>
	[TestFixture]
	public class TextActionTests
	{
		#region Setup

		private MemoryLineBuffer buffer;
		private EditorView editor;
		private EditorViewController controller;
		private EditorViewRenderer renderer;

		/// <summary>
		/// Configures the entire fixture to ensure Gtk# is initialized.
		/// </summary>
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			// Set up Gtk
			Application.Init();
		}

		/// <summary>
		/// Sets up the individual tests with a clean slate.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			// Set up an editor without a cached renderer and a memory buffer
			// we can easily verify.
			editor = new EditorView();
			controller = editor.Controller;
			buffer = new MemoryLineBuffer();
			renderer = new Renderers.LineBufferRenderer(editor, buffer);
			
			editor.SetRenderer(renderer);
		}

		#endregion

		#region Tests

		#region Simple Tests

		/// <summary>
		/// Verifies that the unit tests can start without errors.
		/// </summary>
		[Test]
		public void VerifySetup()
		{
		}

		#endregion

		#region Text Input

		/// <summary>
		/// Inserts a short amount of text using the HandleKeyPress.
		/// </summary>
		[Test]
		public void InsertTextUsingHandleKeyPress()
		{
			// Setup

			// Operation
			controller.HandleKeyPress(Key.T, ModifierType.None, 'T');
			controller.HandleKeyPress(Key.h, ModifierType.None, 'e');
			controller.HandleKeyPress(Key.i, ModifierType.None, 's');
			controller.HandleKeyPress(Key.s, ModifierType.None, 't');

			// Verification
			Assert.AreEqual(1, buffer.LineCount);
			Assert.AreEqual("Test", buffer.GetLineText(0));
		}

		/// <summary>
		/// Inserts the text using the TextAction single character entry.
		/// </summary>
		[Test]
		public void InsertTextUsingTextActionChar()
		{
			// Setup

			// Operation
			TextActions.InsertText(controller, 'T');
			TextActions.InsertText(controller, 'e');
			TextActions.InsertText(controller, 's');
			TextActions.InsertText(controller, 't');

			// Verification
			Assert.AreEqual(1, buffer.LineCount);
			Assert.AreEqual("Test", buffer.GetLineText(0));
		}

		/// <summary>
		/// Inserts the text using the TextAction single character entry.
		/// </summary>
		[Test]
		public void InsertTextUsingTextActionString()
		{
			// Setup

			// Operation
			TextActions.InsertText(controller, "Test");

			// Verification
			Assert.AreEqual(1, buffer.LineCount);
			Assert.AreEqual("Test", buffer.GetLineText(0));
		}

		/// <summary>
		/// Inserts the multiple paragraphs into the buffer.
		/// </summary>
		[Test]
		public void InsertMultipleParagraphs()
		{
			// Setup

			// Operation
			TextActions.InsertText(controller, "One");
			TextActions.InsertParagraph(controller);
			TextActions.InsertText(controller, "Two");
			TextActions.InsertParagraph(controller);
			TextActions.InsertText(controller, "Three");

			// Verification
			Assert.AreEqual(3, buffer.LineCount);
			Assert.AreEqual("One", buffer.GetLineText(0));
			Assert.AreEqual("Two", buffer.GetLineText(1));
			Assert.AreEqual("Three", buffer.GetLineText(2));
		}

		/// <summary>
		/// Verifies that the pattern insert method works correctly.
		/// </summary>
		[Test]
		public void TestInsertPatternIntoBuffer()
		{
			// Setup

			// Operation
			InsertPatternIntoBuffer(3);

			// Verification
			Assert.AreEqual(3, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line 2", buffer.GetLineText(1));
			Assert.AreEqual("Line 3", buffer.GetLineText(2));
		}

		#endregion

		#region Paste Actions

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleInMiddle()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert ";
			editor.Caret.Position = new BufferPosition(1, 5);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(3, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line Insert 2", buffer.GetLineText(1));
			Assert.AreEqual("Line 3", buffer.GetLineText(2));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleAtBeginning()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert ";
			editor.Caret.Position = new BufferPosition(1, 0);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(3, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Insert Line 2", buffer.GetLineText(1));
			Assert.AreEqual("Line 3", buffer.GetLineText(2));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleAtEnd()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = " Inserted";
			editor.Caret.Position = new BufferPosition(1, 0).ToEndOfLine(renderer);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(3, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line 2 Inserted", buffer.GetLineText(1));
			Assert.AreEqual("Line 3", buffer.GetLineText(2));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleEolInMiddle()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\n";
			editor.Caret.Position = new BufferPosition(1, 5);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line Insert", buffer.GetLineText(1));
			Assert.AreEqual("2", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleEolAtBeginning()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\n";
			editor.Caret.Position = new BufferPosition(1, 0);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Insert", buffer.GetLineText(1));
			Assert.AreEqual("Line 2", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteSingleEolAtEnd()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = " Inserted\n";
			editor.Caret.Position = new BufferPosition(1, 0).ToEndOfLine(renderer);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line 2 Inserted", buffer.GetLineText(1));
			Assert.AreEqual("", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleInMiddle()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\nNew ";
			editor.Caret.Position = new BufferPosition(1, 5);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line Insert", buffer.GetLineText(1));
			Assert.AreEqual("New 2", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleAtBeginning()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\nNew ";
			editor.Caret.Position = new BufferPosition(1, 0);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Insert", buffer.GetLineText(1));
			Assert.AreEqual("New Line 2", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleAtEnd()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = " Inserted\nNew";
			editor.Caret.Position = new BufferPosition(1, 0).ToEndOfLine(renderer);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(4, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line 2 Inserted", buffer.GetLineText(1));
			Assert.AreEqual("New", buffer.GetLineText(2));
			Assert.AreEqual("Line 3", buffer.GetLineText(3));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleEolInMiddle()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\nNew\n";
			editor.Caret.Position = new BufferPosition(1, 5);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(5, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line Insert", buffer.GetLineText(1));
			Assert.AreEqual("New", buffer.GetLineText(2));
			Assert.AreEqual("2", buffer.GetLineText(3));
			Assert.AreEqual("Line 3", buffer.GetLineText(4));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleEolAtBeginning()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = "Insert\nNew\n";
			editor.Caret.Position = new BufferPosition(1, 0);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(5, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Insert", buffer.GetLineText(1));
			Assert.AreEqual("New", buffer.GetLineText(2));
			Assert.AreEqual("Line 2", buffer.GetLineText(3));
			Assert.AreEqual("Line 3", buffer.GetLineText(4));
		}

		/// <summary>
		/// Pastes a single line text into the middle of a string.
		/// </summary>
		[Test]
		public void PasteMultipleEolAtEnd()
		{
			// Setup
			InsertPatternIntoBuffer(3);
			editor.Clipboard.Text = " Inserted\nNew\n";
			editor.Caret.Position = new BufferPosition(1, 0).ToEndOfLine(renderer);

			// Operation
			TextActions.Paste(controller);

			// Verification
			Assert.AreEqual(5, buffer.LineCount);
			Assert.AreEqual("Line 1", buffer.GetLineText(0));
			Assert.AreEqual("Line 2 Inserted", buffer.GetLineText(1));
			Assert.AreEqual("New", buffer.GetLineText(2));
			Assert.AreEqual("", buffer.GetLineText(3));
			Assert.AreEqual("Line 3", buffer.GetLineText(4));
		}

		#endregion

		#endregion

		#region Utility

		/// <summary>
		/// Inserts patterned text into the buffer.
		/// </summary>
		private void InsertPatternIntoBuffer(int lines)
		{
			for (int i = 0; i < lines; i++)
			{
				if (i > 0)
				{
					TextActions.InsertParagraph(controller);
				}

				TextActions.InsertText(controller, "Line " + (i + 1));
			}
		}

		#endregion

	}
}