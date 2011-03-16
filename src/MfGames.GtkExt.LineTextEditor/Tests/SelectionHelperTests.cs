using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Renderers;

using NUnit.Framework;

namespace MfGames.GtkExt.LineTextEditor.Tests
{
	/// <summary>
	/// Tests the various functionality and markup of the selection helper.
	/// </summary>
	[TestFixture]
	public class SelectionHelperTests
	{
		#region Simple Tests

		/// <summary>
		/// Tests how the selection helper handles <see langword="null"/>.
		/// </summary>
		[Test]
		public void HandleNull()
		{
			// Setup
			const string markup = null;
			var characters = new CharacterRange(1, 2);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.IsNull(output);
		}

		/// <summary>
		/// Tests how the selection helper handles blank strings.
		/// </summary>
		[Test]
		public void HandleBlank()
		{
			// Setup
			string markup = string.Empty;
			var characters = new CharacterRange(1, 2);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(string.Empty, output);
		}

		#endregion

		#region Plain Markup

		[Test]
		public void PlainMiddleOfString()
		{
			// Setup
			const string markup = "this is a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>is a</span> string", output);
		}

		[Test]
		public void PlainBeginningOfString()
		{
			// Setup
			const string markup = "this is a string";
			var characters = new CharacterRange(0, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("<span background='#CCCCFF'>this is a</span> string", output);
		}

		[Test]
		public void PlainEndOfString()
		{
			// Setup
			const string markup = "this is a string";
			var characters = new CharacterRange(5, markup.Length);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>is a string</span>", output);
		}

		[Test]
		public void PlainEntity1()
		{
			// Setup
			const string markup = "this &#0069;s a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>&#0069;s a</span> string", output);
		}

		[Test]
		public void PlainEntity2()
		{
			// Setup
			const string markup = "this is &#0061; string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>is &#0061;</span> string", output);
		}

		[Test]
		public void PlainEntity3()
		{
			// Setup
			const string markup = "this i&#0073; a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>i&#0073; a</span> string", output);
		}

		[Test]
		public void PlainEntity4()
		{
			// Setup
			const string markup = "this i&amp; a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>i&amp; a</span> string", output);
		}

		#endregion

		#region Markup Tests

		[Test]
		public void MarkupContaining()
		{
			// Setup
			const string markup = "this i<span>s</span> a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual("this <span background='#CCCCFF'>i<span>s</span> a</span> string", output);
		}

		[Test]
		public void MarkupLeading()
		{
			// Setup
			const string markup = "thi<span>s is</span> a string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"thi<span>s </span><span background='#CCCCFF'><span>is</span> a</span> string", output);
		}

		[Test]
		public void MarkupTrailing()
		{
			// Setup
			const string markup = "this i<span>s a str</span>ing";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span background='#CCCCFF'>i<span>s a</span></span><span> str</span>ing", output);
		}

		[Test]
		public void MarkupOuter()
		{
			// Setup
			const string markup = "thi<span>s is a str</span>ing";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"thi<span>s </span><span background='#CCCCFF'><span>is a</span></span><span> str</span>ing", output);
		}

		[Test]
		public void MarkupExact()
		{
			// Setup
			const string markup = "this <span>is a</span> string";
			var characters = new CharacterRange(5, 9);

			// Operation
			string output = SelectionHelper.GetSelectionMarkup(markup, characters);

			// Verification
			Assert.AreEqual(
				"this <span></span><span background='#CCCCFF'><span>is a</span></span> string", output);
		}

		#endregion
	}
}