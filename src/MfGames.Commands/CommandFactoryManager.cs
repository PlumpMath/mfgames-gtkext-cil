// Copyright 2011-2013 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-gtkext-cil/license

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MfGames.HierarchicalPaths;

namespace MfGames.Commands
{
	/// <summary>
	/// Encapsulates the functionality of rendering <see cref="CommandFactoryReference"/>
	/// objects into a user-formatted display. This manager allows for commands to
	/// be referenced via a <see cref="HierarchicalPaths"/> within menus and
	/// popups while still expanding into a (potentially translated) description with
	/// toolkit-specific icons and elements.
	/// </summary>
	public class CommandFactoryManager<TStatus>
	{
		#region Properties

		public ICommandController<TStatus> CommandController { get; private set; }

		#endregion

		#region Methods

		public TStatus Do(
			object context,
			CommandFactoryReference commandFactoryReference)
		{
			// Establish our contracts.
			Contract.Requires<ArgumentNullException>(commandFactoryReference != null);

			// Grab the factory for the key pass the request to the factory.
			ICommandFactory<TStatus> factory = GetFactory(commandFactoryReference.Key);
			TStatus results = factory.Do(context, commandFactoryReference, this);
			return results;
		}

		/// <summary>
		/// Retrives a short, descriptive name of the command for the user. This should
		/// be translated appropriately before returning.
		/// </summary>
		/// <param name="commandFactoryReference">A <see cref="CommandFactoryReference"/> representing the command and an optional parameter.</param>
		/// <returns>A string suitable for display to the user.</returns>
		public string GetTitle(CommandFactoryReference commandFactoryReference)
		{
			// Establish our contracts.
			Contract.Requires<ArgumentNullException>(commandFactoryReference != null);

			// Grab the factory for the key and return the title.
			ICommandFactory<TStatus> factory = GetFactory(commandFactoryReference.Key);
			string results = factory.GetTitle(commandFactoryReference);
			return results;
		}

		/// <summary>
		/// Registers all of the known paths in a command view with the manager.
		/// </summary>
		/// <param name="commandFactory"></param>
		public void Register(ICommandFactory<TStatus> commandFactory)
		{
			// Ensure our code contract.
			Contract.Requires<ArgumentNullException>(commandFactory != null);

			// Go through the the keys inside the view.
			foreach (HierarchicalPath key in commandFactory.Keys)
			{
				Register(key, commandFactory);
			}
		}

		/// <summary>
		/// Registers a command view for a given <c>ICommand.Key</c> reference.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="commandFactory"></param>
		public void Register(
			HierarchicalPath key,
			ICommandFactory<TStatus> commandFactory)
		{
			// Ensure our code contracts.
			Contract.Requires<ArgumentNullException>(commandFactory != null);

			// Register the factory for the given key.
			factories[key] = commandFactory;
		}

		/// <summary>
		/// Retrieves the factory associated with a given key, or throws an exception
		/// if one cannot be found.
		/// </summary>
		private ICommandFactory<TStatus> GetFactory(HierarchicalPath key)
		{
			// Establish our contracts.
			Contract.Requires<ArgumentNullException>(key != null);

			// If we have the command factory registered, then grab it and return it.
			ICommandFactory<TStatus> commandFactory;

			if (factories.TryGetValue(key, out commandFactory))
			{
				return commandFactory;
			}

			// If we get this far, we don't have the registered factory.
			throw new KeyNotFoundException(
				"Cannot find a command factory registered under " + key);
		}

		#endregion

		#region Constructors

		public CommandFactoryManager(ICommandController<TStatus> commandController)
		{
			// Establish our code contracts.
			Contract.Requires<ArgumentNullException>(commandController != null);

			// Save the member variables for later.
			CommandController = commandController;

			// Set up the internal collections.
			factories = new Dictionary<HierarchicalPath, ICommandFactory<TStatus>>();
		}

		#endregion

		#region Fields

		private readonly Dictionary<HierarchicalPath, ICommandFactory<TStatus>>
			factories;

		#endregion
	}
}
