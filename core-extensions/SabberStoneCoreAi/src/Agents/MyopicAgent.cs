using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.Nodes;
using SabberStoneCoreAi.Score;
using SabberStoneCoreAi.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.Agents
{
	using GameNode = TreeNode<NodeData>;

	class NodeData
	{
		public PlayerTask LastMove;
		public Game State;
		public int Evaluation;

		public NodeData(PlayerTask m, Game g)
		{
			LastMove = m;
			State = g;
			Evaluation = Int32.MinValue;
		}
	}

	class MyopicAgent : Agent
	{


		IScore EvaluationRule;

		public MyopicAgent() : base()
		{
			Name = @"Myopic Agent";

			EvaluationRule = new RampScore();
		}

		private int EvaluateGame(Game game)
		{
			EvaluationRule.Controller = game.ControllerById(EntityID);
			return EvaluationRule.Rate();
		}

		/// <summary>
		/// Recursively expand and build a game tree in a DFS manner
		/// Ends recursion on opponents turn
		/// </summary>
		/// <param name="ParentNode">the node currently to be expanded</param>
		private void ExpandNode(GameNode ParentNode, int depth=0)
		{
			
			//if game has ended do not try to expand
			if (ParentNode.data.State.State != State.RUNNING)
				return;

			// do not expand the opponent's round
			if (ParentNode.data.State.CurrentPlayer.PlayerId != _BoundController.PlayerId)
				return;

			List<PlayerTask> options = ParentNode.data.State.ControllerById(EntityID).Options();

			foreach (PlayerTask task in options)
			{
				// first create this node
				Game GameCopy = ParentNode.data.State.Clone();
				GameCopy.Process(task);
				NodeData NewNode = new NodeData(task, GameCopy);
				NewNode.Evaluation = EvaluateGame(GameCopy);

				if (NewNode.Evaluation < ParentNode.data.Evaluation)
					continue; //don't bother if we are making our situation worse

				GameNode node = ParentNode.AddChild(NewNode);

				//then expand it
				ExpandNode(node, depth+1);
			}

		}

		/// <summary>
		/// Given the root of a game tree returns the list of all leaf nodes
		/// </summary>
		/// <param name="root">Root node of the tree</param>
		/// <returns></returns>
		private List<GameNode> GetLeafNodes(GameNode root)
		{
			List<GameNode> LeafNodes = new List<GameNode>();

			// function that saves the leaf nodes to our list
			void SaveLeafNode(GameNode node)
			{
				if (node.children.Count == 0)
					LeafNodes.Add(node);
			}

			//traverse the tree
			root.Traverse(root, SaveLeafNode);

			return LeafNodes;
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

			GameNode root = new GameNode(new NodeData(null, game));
			root.data.Evaluation = EvaluateGame(root.data.State);

			ExpandNode(root);

			List<GameNode> LeafNodes = GetLeafNodes(root);

			GameNode BestMoveEnding = LeafNodes.OrderByDescending(o => o.data.Evaluation).First();
			GameNode BestMove = BestMoveEnding;

			while (BestMove.parent.parent != null)
				BestMove = BestMove.parent;



			Debug.Assert(BestMove.data.LastMove != null);
			return BestMove.data.LastMove;
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
