using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.Score;
using System.Diagnostics;
using System.Linq;
using SabberStoneCoreAi.Nodes;

namespace SabberStoneCoreAi.Agents
{
    class ScoreAgent : Agent
    {


        int CurrentTurn;
        List<PlayerTask> FoundSolution;
        IScore EvaluationRule;


        public ScoreAgent() : base()
		{
            Name = @"Score Agent";
            EvaluationRule = new RampScore();
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
            Game game = _BoundController.Game.Clone();

			if (CurrentTurn == game.Turn)
			{
				//we probably have a solution calculated already!
				PlayerTask move = FoundSolution[0];
				FoundSolution.RemoveAt(0);
				return move;
			}
			//next turn! Find a solution again!
			CurrentTurn = game.Turn;


            List<OptionNode> solutions = OptionNode.GetSolutions(game, EntityID, EvaluationRule, 10, 500);
            var solution = new List<PlayerTask>();
            solutions.OrderByDescending(p => p.Score).First().PlayerTasks(ref solution);

			FoundSolution = solution;

			return GetMove();
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
