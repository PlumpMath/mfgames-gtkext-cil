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
using System.Diagnostics;

using MfGames.GtkExt.TextEditor.Interfaces;
using MfGames.GtkExt.TextEditor.Models;

#endregion

namespace MfGames.GtkExt.TextEditor.Renderers
{
	/// <summary>
	/// Wraps around a <see cref="EditorViewRenderer"/> and allows for extending classes
	/// to override various methods and properties. This implementation simply
	/// calls the underlying <see cref="EditorViewRenderer"/> for everything.
	/// </summary>
	public abstract class EditorViewRendererDecorator : EditorViewRenderer
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorViewRendererDecorator"/> class.
		/// </summary>
		/// <param name="displayContext">The display context.</param>
		/// <param name="editorViewRenderer">The text renderer.</param>
		protected EditorViewRendererDecorator(
			IDisplayContext displayContext,
			EditorViewRenderer editorViewRenderer)
			: base(displayContext)
		{
			EditorViewRenderer = editorViewRenderer;

			if (editorViewRenderer == null)
			{
				throw new ArgumentNullException("editorViewRenderer");
			}
		}

		#endregion

		#region Renderer

		private EditorViewRenderer editorViewRenderer;

		/// <summary>
		/// Gets or sets the underlying Renderer.
		/// </summary>
		/// <value>The text renderer.</value>
		public EditorViewRenderer EditorViewRenderer
		{
			[DebuggerStepThrough]
			get { return editorViewRenderer; }

			private set
			{
				// Disconnect any events if we previously had a renderer.
				if (editorViewRenderer != null)
				{
					editorViewRenderer.LineChanged -= OnLineChanged;
					editorViewRenderer.LinesDeleted -= OnLinesDeleted;
					editorViewRenderer.LinesInserted -= OnLinesInserted;
				}

				// Set the value for the underlying data.
				editorViewRenderer = value;

				// Connect the events if we have a new value.
				if (editorViewRenderer != null)
				{
					editorViewRenderer.LineChanged += OnLineChanged;
					editorViewRenderer.LinesDeleted += OnLinesDeleted;
					editorViewRenderer.LinesInserted += OnLinesInserted;
				}
			}
		}

		#endregion

		#region Buffer

		/// <summary>
		/// Gets the line buffer associated with this renderer.
		/// </summary>
		/// <value>The line buffer.</value>
		public override LineBuffer LineBuffer
		{
			[DebuggerStepThrough]
			get { return EditorViewRenderer.LineBuffer; }
		}

		/// <summary>
		/// Sets the line buffer since the renderer.
		/// </summary>
		/// <param name="value">The value.</param>
		public override void SetLineBuffer(LineBuffer value)
		{
			EditorViewRenderer.SetLineBuffer(value);
		}

		#endregion

		#region Selection

		/// <summary>
		/// Gets or sets the selection renderer.
		/// </summary>
		/// <value>The selection renderer.</value>
		public override SelectionRenderer SelectionRenderer
		{
			[DebuggerStepThrough]
			get { return EditorViewRenderer.SelectionRenderer; }
			[DebuggerStepThrough]
			set { editorViewRenderer.SelectionRenderer = value; }
		}

		#endregion
	}
}