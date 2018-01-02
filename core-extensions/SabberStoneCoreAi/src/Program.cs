using System;
using System.Linq;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Agents;

namespace SabberStoneCoreAi
{
	internal class Program
	{
		private static readonly Random Rnd = new Random();

		private static void Main(string[] args)
		{
			Agent agent1 = new MyopicAgent();
			Agent agent2 = new ScoreAgent();

			PitAgents(agent1, agent2, 10);

			Console.WriteLine("Stopping...");
			Console.ReadLine();
		}

		public static void PitAgents(Agent Player1, Agent Player2, int NoOfGames = 100)
		{

			for (int i = 0; i < NoOfGames; ++i)
			{
				FullGame(Player1, Player2);
			}


			int Player1Wins = Player1.WinHistory.Count(x => x == PlayState.WON);
			int Player2Wins = Player2.WinHistory.Count(x => x == PlayState.WON);

			Console.WriteLine($"Player1 - Player2");
			Console.WriteLine($"{Player1Wins}   -   {Player2Wins}");

		}

		/// <summary>
		/// Play a full game between 2 agents
		/// </summary>
		/// <param name="Player1"> Player 1 (instance of an agent)</param>
		/// <param name="Player2"> Player 2 (instance of an agent)</param>
		public static void FullGame(Agent Player1, Agent Player2)
		{

			Player1.PreGame();
			Player2.PreGame();

			var game = new Game(
				new GameConfig()
				{
					StartPlayer = 1,

					Player1Name = Player1.Name + '1',
					Player1HeroClass = Player1.AgentClass,
					Player1Deck = Player1.Deck,

					Player2Name = Player2.Name + '2',
					Player2HeroClass = Player2.AgentClass,
					Player2Deck = Player2.Deck,

					FillDecks = false,
					Shuffle = true,
					SkipMulligan = false
				});
			game.StartGame();

			Player1.StartGame(game.Player1);
			Player2.StartGame(game.Player2);

			game.Process(Player1.GetMulliganChoices());
			game.Process(Player2.GetMulliganChoices());

			game.MainReady();

			int LastTurn;

			while (game.State != State.COMPLETE)
			{
				LastTurn = game.Turn;
				Console.WriteLine("");
				Console.WriteLine($"Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState} - " +
								  $"ROUND {(game.Turn + 1) / 2} - {game.CurrentPlayer.Name}");
				Console.WriteLine($"Hero[P1]: {game.Player1.Hero.Health} / Hero[P2]: {game.Player2.Hero.Health}");
				Console.WriteLine("");
				Console.WriteLine(game.FullPrint());

				while(LastTurn == game.Turn && game.State == State.RUNNING)
				{
					LastTurn = game.Turn;

					Agent CurrentAgent = game.CurrentPlayer == game.Player1 ? Player1 : Player2;

					PlayerTask move = CurrentAgent.GetMove();
					game.Process(move);

					Console.WriteLine(move.FullPrint());

				}
				//Console.ReadKey();
			}
			Console.WriteLine($"Game: {game.State}, Player1: {game.Player1.PlayState} / Player2: {game.Player2.PlayState}");

			Player1.EndGame();
			Player2.EndGame();
		}
	}
}
