// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license
namespace MfGames.Commands
{
	/// <summary>
	/// Encapsulates the functionality for executing commands within a system. The
	/// controller handles undo and redo functionality, performing the actions, and
	/// keeping track of the last state of the executing command.
	/// </summary>
	/// <typeparam name="TState">
	/// The type of state object needed to be updated with the execution of commands.
	/// </typeparam>
	public class CommandController<TState>
	{
		public void Do(ICommand<TState> command)
		{
		}

		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public bool CanUndo
		{
			get { return false; }
		}

		public bool CanRedo
		{
			get { return false; }
		}

		public TState State { get; private set; }
}