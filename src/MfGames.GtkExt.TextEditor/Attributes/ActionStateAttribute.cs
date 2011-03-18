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

using MfGames.GtkExt.TextEditor.Interfaces;

#endregion

namespace MfGames.GtkExt.TextEditor.Attributes
{
	/// <summary>
	/// Attribute that indicates the types of objects that are used to maintain
	/// state for this method. All other states are requested to be removed
	/// before the method is called.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionStateAttribute : Attribute
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionStateAttribute"/> class.
		/// </summary>
		/// <param name="stateType">Type of state object to leave in the action states.</param>
		public ActionStateAttribute(Type stateType)
		{
			// Save the state to be retrieved later.
			StateType = stateType;

			// Make sure the state extends the proper class.
			if (!typeof(IActionState).IsAssignableFrom(StateType))
			{
				throw new Exception(
					"Can only assign an IActionState type of ActionState attributes");
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type object that represents an action state.
		/// </summary>
		/// <value>The type of the state.</value>
		public Type StateType { get; private set; }

		#endregion
	}
}