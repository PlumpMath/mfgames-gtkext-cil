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

using MfGames.GtkExt.TextEditor.Models;
using MfGames.GtkExt.TextEditor.Models.Buffers;
using MfGames.GtkExt.TextEditor.Renderers;

using NUnit.Framework;

using Gdk;

#endregion

namespace MfGames.GtkExt.TextEditor.Tests
{
	/// <summary>
	/// Tests the various functionality of TextActions.
	/// </summary>
	[TestFixture]
	public class TextActionTests
	{
		#region Setup
		
		private EditorView editor;
		private MemoryLineBuffer buffer;
		
		[SetUp]
		public void Setup()
		{
			editor = new EditorView();
			buffer = new MemoryLineBuffer();
			editor.SetLineBuffer(buffer);
		}
		
		#endregion
		
		#region Tests
		
		[Test]
		public void VerifySetup()
		{
		}
		
		[Test]
		public void InsertText()
		{
			// Setup
			
			// Operation
			editor.Controller.HandleKeyPress(Key.T, ModifierType.None, (uint) 'T');
			editor.Controller.HandleKeyPress(Key.h, ModifierType.None, (uint) 'h');
			editor.Controller.HandleKeyPress(Key.i, ModifierType.None, (uint) 'i');
			editor.Controller.HandleKeyPress(Key.s, ModifierType.None, (uint) 's');
			
			// Verification
			Assert.AreEqual(1, buffer.LineCount);
			Assert.AreEqual("This", buffer.GetLineText(0, LineContexts.None));
		}
		
		#endregion
	}
}