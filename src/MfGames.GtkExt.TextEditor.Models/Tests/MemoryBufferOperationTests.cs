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

using System;

using MfGames.GtkExt.TextEditor.Models.Buffers;

using NUnit.Framework;

#endregion

namespace MfGames.GtkExt.TextEditor.Models.Tests
{
	/// <summary>
	/// Tests the various functionality of operations on the memory buffer.
	/// </summary>
	[TestFixture]
	public class MemoryBufferOperationTests
	{
		#region Setup

		private MemoryLineBuffer buffer;

		/// <summary>
		/// Sets up the test and creates the initial memory buffer.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			buffer = new MemoryLineBuffer();
		}

		#endregion

		#region Tests

		/// <summary>
		/// Creates the buffer and verifies the setup works.
		/// </summary>
		[Test]
		public void CreateBuffer()
		{
			// This test does nothing, just verifies that the setup works.
		}

		/// <summary>
		/// Inserts the one at the beginning of the buffer.
		/// </summary>
		[Test]
		public void InsertOneLine()
		{
			// Setup

			// Operation
			LineBufferOperationResults results = buffer.InsertLines(0, 1);

			// Validation
			Assert.AreEqual(2, buffer.LineCount);
			Assert.AreEqual(1, results.BufferPosition.LineIndex);
			Assert.AreEqual(0, results.BufferPosition.CharacterIndex);
		}

		/// <summary>
		/// Sets the text into an empty line.
		/// </summary>
		[Test]
		public void SetTextIntoEmptyLine()
		{
			// Setup
			const string input = "Test";

			// Operation
			LineBufferOperationResults results = buffer.SetText(0, input);

			// Verification
			Assert.AreEqual(input, buffer.GetLineText(0, LineContexts.None));
			Assert.AreEqual(input.Length, buffer.GetLineLength(0, LineContexts.None));
			Assert.AreEqual(0, results.BufferPosition.LineIndex);
			Assert.AreEqual(input.Length, results.BufferPosition.CharacterIndex);
		}

		/// <summary>
		/// Inserts the text into an empty line.
		/// </summary>
		[Test]
		public void InsertTextIntoEmptyLine()
		{
			// Setup
			const string input = "Test";

			// Operation
			LineBufferOperationResults results = buffer.InsertText(new BufferPosition(0, 0), input);

			// Verification
			Assert.AreEqual(input, buffer.GetLineText(0, LineContexts.None));
			Assert.AreEqual(input.Length, buffer.GetLineLength(0, LineContexts.None));
			Assert.AreEqual(0, results.BufferPosition.LineIndex);
			Assert.AreEqual(input.Length, results.BufferPosition.CharacterIndex);
		}

		/// <summary>
		/// Inserts the text into an empty line.
		/// </summary>
		[Test]
		public void InsertTextIntoBeginningOfLine()
		{
			// Setup
			const string input = "Test ";

			buffer.SetText(0, "Original");

			// Operation
			LineBufferOperationResults results = buffer.InsertText(new BufferPosition(0, 0), input);

			// Verification
			Assert.AreEqual("Test Original", buffer.GetLineText(0, LineContexts.None));
			Assert.AreEqual("Test Original".Length, buffer.GetLineLength(0, LineContexts.None));
			Assert.AreEqual(0, results.BufferPosition.LineIndex);
			Assert.AreEqual(input.Length, results.BufferPosition.CharacterIndex);
		}

		/// <summary>
		/// Inserts the text into an empty line.
		/// </summary>
		[Test]
		public void InsertTextIntoEndOfLine()
		{
			// Setup
			const string input = " Test";

			buffer.SetText(0, "Original");

			// Operation
			LineBufferOperationResults results =
				buffer.InsertText(new BufferPosition(0, "Original".Length), input);

			// Verification
			Assert.AreEqual("Original Test", buffer.GetLineText(0, LineContexts.None));
			Assert.AreEqual("Original Test".Length, buffer.GetLineLength(0, LineContexts.None));
			Assert.AreEqual(0, results.BufferPosition.LineIndex);
			Assert.AreEqual("Original Test".Length, results.BufferPosition.CharacterIndex);
		}

		/// <summary>
		/// Inserts the text into an empty line.
		/// </summary>
		[Test]
		public void InsertTextIntoMaxEndOfLine()
		{
			// Setup
			const string input = " Test";

			buffer.SetText(0, "Original");

			// Operation
			LineBufferOperationResults results =
				buffer.InsertText(new BufferPosition(0, Int32.MaxValue), input);

			// Verification
			Assert.AreEqual("Original Test", buffer.GetLineText(0, LineContexts.None));
			Assert.AreEqual("Original Test".Length, buffer.GetLineLength(0, LineContexts.None));
			Assert.AreEqual(0, results.BufferPosition.LineIndex);
			Assert.AreEqual("Original Test".Length, results.BufferPosition.CharacterIndex);
		}

		#endregion

	}
}