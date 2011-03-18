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

using MfGames.GtkExt.TextEditor.Buffers;

using NUnit.Framework;

#endregion

namespace MfGames.GtkExt.TextEditor.Tests
{
	/// <summary>
	/// Defines tests that explore the functionality of the 
	/// <c>EnglishWordSplitter</c> class.
	/// </summary>
	[TestFixture]
	public class EnglishWordSplitterTests
	{
		#region Previous

		[Test]
		public void PreviousAtEndOfLine()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One. Two.";
			const int index = 9;

			// Test
			int boundary = splitter.GetPreviousWordBoundary(text, index);

			// Assertion
			Assert.AreEqual(".", text.Substring(boundary));
			Assert.AreEqual(8, boundary);
		}

		[Test]
		public void PreviousOneWord()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "word";
			const int index = 1;

			// Test
			int boundary = splitter.GetPreviousWordBoundary(text, index);

			// Assertion
			Assert.AreEqual(0, boundary);
		}

		[Test]
		public void PreviousPuncutationWithoutSpace()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One.Two.";
			const int index = 5;

			// Test
			int boundary = splitter.GetPreviousWordBoundary(text, index);

			// Assertion
			Assert.AreEqual("Two.", text.Substring(boundary));
			Assert.AreEqual(4, boundary);
		}

		[Test]
		public void PreviousPuncutationWithoutSpaceInMiddleWithEndingSpace()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One.Two ";
			const int index = 8;

			// Test
			int boundary = splitter.GetPreviousWordBoundary(text, index);

			// Assertion
			Assert.AreEqual("Two ", text.Substring(boundary));
			Assert.AreEqual(4, boundary);
		}

		[Test]
		public void PreviousPuncutationWithSpace()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One. Two.";
			const int index = 6;

			// Test
			int boundary = splitter.GetPreviousWordBoundary(text, index);

			// Assertion
			Assert.AreEqual("Two.", text.Substring(boundary));
			Assert.AreEqual(5, boundary);
		}

		#endregion

		#region Next

		[Test]
		public void NextOneWord()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "word";
			const int index = 1;

			// Test
			int boundary = splitter.GetNextWordBoundary(text, index);

			// Assertion
			Assert.AreEqual(4, boundary);
		}

		[Test]
		public void NextPuncutationWithoutSpace()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One.Two.";
			const int index = 3;

			// Test
			int boundary = splitter.GetNextWordBoundary(text, index);

			// Assertion
			Assert.AreEqual("Two.", text.Substring(boundary));
			Assert.AreEqual(4, boundary);
		}

		[Test]
		public void NextPuncutationWithSpace()
		{
			// Setup
			EnglishWordSplitter splitter = new EnglishWordSplitter();
			const string text = "One. Two.";
			const int index = 3;

			// Test
			int boundary = splitter.GetNextWordBoundary(text, index);

			// Assertion
			Assert.AreEqual("Two.", text.Substring(boundary));
			Assert.AreEqual(5, boundary);
		}

		#endregion
	}
}