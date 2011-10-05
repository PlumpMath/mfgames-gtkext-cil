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

using System;

using Gtk;

using MfGames.HierarchicalPaths;

#endregion

namespace MfGames.GtkExt.Configurators
{
    /// <summary>
    /// Describes the signature for configurators that provide Gtk# panel
    /// for user configuration. Configurator panels are organized using 
    /// hierarchial path to create the nested tree.
    /// 
    /// When a configurator is first shown, the container panel will call
    /// CreateConfiguratorWidget(). When the container is closed, the Dispose()
    /// method of this interface will be called.
    /// </summary>
    public interface IGtkConfigurator : IDisposable, IHierarchicalPathContainer
    {
        /// <summary>
        /// Creates a widget to be placed at the configurator tree as given
        /// by the path. The created widget should be destroyed as part of the
        /// Dispose() method.
        /// </summary>
        /// <returns></returns>
        Widget CreateConfiguratorWidget();
    }
}