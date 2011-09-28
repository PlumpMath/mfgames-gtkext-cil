#region Copyright and License

// Copyright (c) 2005-2011, Moonfire Games
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

using Gtk;

#endregion

namespace GtkExtDemo.Actions
{
    /// <summary>
    /// Defines a user action that switches notebook pages.
    /// </summary>
    public class SwitchPageAction : Action
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchPageAction"/>
        /// class and configures it to switch to the given page.
        /// </summary>
        /// <param name="notebook">The notebook.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="label">The label.</param>
        public SwitchPageAction(
            Notebook notebook,
            int pageNumber,
            string label)
            : base("SwitchPage" + pageNumber, label)
        {
            this.notebook = notebook;
            this.pageNumber = pageNumber;
        }

        #endregion

        #region Action

        private readonly Notebook notebook;
        private readonly int pageNumber;

        /// <summary>
        /// Switched the notebook page when activated.
        /// </summary>
        protected override void OnActivated()
        {
            notebook.Page = pageNumber;
        }

        #endregion
    }
}