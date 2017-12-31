using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.Agents
{
	class RandomAgent : Agent
	{

		public RandomAgent() : base()
		{
			Name = @"Random Agent";
		}

		override public PlayerTask GetMulliganChoices()
		{
			Debug.Assert(_BoundController.Game.Step == Step.BEGIN_MULLIGAN);

			List<PlayerTask> options = new List<PlayerTask>();

			IEnumerable<IEnumerable<int>> choices = Util.GetPowerSet(_BoundController.Choice.Choices);
			choices.ToList().ForEach(p => options.Add(ChooseTask.Mulligan(this._BoundController, p.ToList())));

			return options[Util.Random.Next(options.Count())];
		}

		public override PlayerTask GetMove()
		{

			List<PlayerTask> options = _BoundController.Options();

			return options[Util.Random.Next(options.Count())];

		}

		protected override void SelectClass()
		{
			AgentClass = CardClass.WARRIOR;
		}

		protected override void CreateDeck()
		{
			Deck = Decks.AggroPirateWarrior;
		}

	}
}
