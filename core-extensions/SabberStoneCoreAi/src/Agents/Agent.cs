using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Model;
using SabberStoneCoreAi.Meta;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;
using System.Diagnostics;
using SabberStoneCore.Tasks;

namespace SabberStoneCoreAi.Agents
{

    abstract class Agent
    {
		/// <summary>
		/// Internal variable that signifies is the agent is currently playing a game
		/// </summary>
		private bool _InGame = false;

		/// <summary>
		/// The agent's name. Should be set in the constructor
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// The Deck that should be used in the game. Only change it in CreateDeck()
		/// </summary>
		public List<Card> Deck { get; protected set; }

		/// <summary>
		/// Get the Hero Class for the next game. Only change it during SelectClass()
		/// </summary>
		public CardClass AgentClass { get; protected set; }

		/// <summary>
		/// The game as observed by our agent.
		/// TODO: Should only be a partial view of the game. (There should be nothing from the opponents deck/hand in here)
		/// </summary>
		protected Game _Observation;

		/// <summary>
		/// Our Game's controller
		/// </summary>
		protected Controller _BoundController;

		/// <summary>
		/// History of play states at the time of EndGame()
		/// </summary>
		public List<PlayState> WinHistory = new List<PlayState>();

		public Agent()
		{
			Console.WriteLine("Initializing Agent");
		}

		/// <summary>
		/// PreGame should be called once before any game and will make the agent initialize itself
		/// </summary>
		public void PreGame()
		{
			Debug.Assert(!_InGame);

			SelectClass();
			CreateDeck();
		}

		/// <summary>
		/// Should be called when the Game starts (before Mulligan!)
		/// </summary>
		/// <param name="c">Provide the Game's Controller</param>
		public void StartGame(Controller c)
		{
			Debug.Assert(!_InGame);

			/* initialize things when game starts*/
			_InGame = true;
			_BoundController = c;
		}

		/// <summary>
		/// Should be called when a game ends for clean up operations (and for reusing the agent)
		/// </summary>
		public void EndGame()
		{
			Debug.Assert(_InGame);

			WinHistory.Add(_BoundController.PlayState);

			/* clean up stuff */
			_InGame = false;
			_BoundController = null;

			Deck.Clear();
		}

		/// <summary>
		/// Returns the play state (win/lose/concede etc!)
		/// </summary>
		/// <returns></returns>
		public PlayState GetPlayState()
		{
			return _BoundController.PlayState;
		}

		/// <summary>
		/// Should be called once per game during Mulligan to get the PlayerTask
		/// </summary>
		/// <returns>Returns the Player Task that should be played during Mulligan</returns>
		abstract public PlayerTask GetMulliganChoices();

		/// <summary>
		/// Get Player Tasks that should be performed during the main course of the Game
		/// </summary>
		/// <returns>returns a PlayerTask that should be played</returns>
		abstract public PlayerTask GetMove();

		/// <summary>
		/// Called during PreGame() and should initialize the AgentClass variable
		/// </summary>
		abstract protected void SelectClass();

		/// <summary>
		/// Called during PreGame() and should initialize the Deck variable for the next game
		/// </summary>
		abstract protected void CreateDeck();

	}
}
