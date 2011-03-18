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

using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Renderers;

using NUnit.Framework;

#endregion

namespace MfGames.GtkExt.TextEditor.Tests
{
	/// <summary>
	/// Tests the various functionality and markup of the selection helper.
	/// </summary>
	[TestFixture]
	public class SelectionRendererTests
	{
		#region Simple Tests

		/// <summary>
		/// Tests how the selection helper handles blank strings.
		/// </summary>
		[Test]
		public void HandleBlank()
		{
			// Setup
			string markup = string.Empty;
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(1, 2);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(string.Empty, output);
		}

		/// <summary>
		/// Tests how the selection helper handles <see langword="null"/>.
		/// </summary>
		[Test]
		public void HandleNull()
		{
			// Setup
			const string markup = null;
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(1, 2);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.IsNull(output);
		}

		#endregion

		#region Plain Markup

		[Test]
		public void PlainBeginningOfString()
		{
			// Setup
			const string markup = "this is a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(0, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("<span background='#CCCCFF'>this is a</span> string", output);
		}

		[Test]
		public void PlainBeginningOfString2()
		{
			// Setup
			const string markup = "this is a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(1, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("t<span background='#CCCCFF'>his is a</span> string", output);
		}

		[Test]
		public void PlainEndOfString()
		{
			// Setup
			const string markup = "this is a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, markup.Length);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>is a string</span>", output);
		}

		[Test]
		public void PlainEntity1()
		{
			// Setup
			const string markup = "this &#0069;s a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>&#0069;s a</span> string", output);
		}

		[Test]
		public void PlainEntity2()
		{
			// Setup
			const string markup = "this is &#0061; string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>is &#0061;</span> string", output);
		}

		[Test]
		public void PlainEntity3()
		{
			// Setup
			const string markup = "this i&#0073; a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>i&#0073; a</span> string", output);
		}

		[Test]
		public void PlainEntity4()
		{
			// Setup
			const string markup = "this i&amp; a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>i&amp; a</span> string", output);
		}

		[Test]
		public void PlainMiddleOfString()
		{
			// Setup
			const string markup = "this is a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>is a</span> string", output);
		}

		#endregion

		#region Markup Tests

		[Test]
		public void MarkupContaining()
		{
			// Setup
			const string markup = "this i<span>s</span> a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>i<span>s</span> a</span> string", output);
		}

		[Test]
		public void MarkupExact()
		{
			// Setup
			const string markup = "this <span>is a</span> string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'><span>is a</span></span> string", output);
		}

		[Test]
		public void MarkupLeading()
		{
			// Setup
			const string markup = "thi<span>s is</span> a string";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"thi<span>s </span><span background='#CCCCFF'><span>is</span> a</span> string",
				output);
		}

		[Test]
		public void MarkupOuter()
		{
			// Setup
			const string markup = "thi<span>s is a str</span>ing";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"thi<span>s </span><span background='#CCCCFF'><span>is a</span></span><span> str</span>ing",
				output);
		}

		[Test]
		public void MarkupTrailing()
		{
			// Setup
			const string markup = "this i<span>s a str</span>ing";
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>i<span>s a</span></span><span> str</span>ing",
				output);
		}

		#endregion
	}
}