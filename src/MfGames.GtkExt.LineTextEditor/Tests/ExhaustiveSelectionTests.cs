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

using MfGames.GtkExt.LineTextEditor.Buffers;
using MfGames.GtkExt.LineTextEditor.Renderers;

using NUnit.Framework;

#endregion

namespace MfGames.GtkExt.LineTextEditor.Tests
{
	/// <summary>
	/// Performs a series of exhaustive tests on the selection using data generated
	/// by the CreateExhaustiveSelectionTests project.
	/// </summary>
	public class ExhaustiveSelectionTests
	{
		/// <summary>
		/// Performs one of the exhaustive tests by taking the input and applying
		/// the selection to characters 3-4.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="endIndex">The end index.</param>
		/// <param name="input">The input.</param>
		/// <param name="desiredOutput">The output.</param>
		protected void TestExhaustive(
			int startIndex,
			int endIndex,
			string input,
			string desiredOutput)
		{
			// Setup
			var selectionRenderer = new SelectionRenderer();
			var characters = new CharacterRange(startIndex, endIndex);

			// Operation
			string output = selectionRenderer.GetSelectionMarkup(
				input, characters, "s", String.Empty);

			// Verification
			Assert.AreEqual(desiredOutput, output);
		}
	}
}