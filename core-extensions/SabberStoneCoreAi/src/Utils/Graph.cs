using System;
using System.Collections.Generic;
using System.Text;

namespace SabberStoneCoreAi.Utils
{
	delegate void TreeVisitor<T>(TreeNode<T> nodeData);

	class TreeNode<T>
	{
		public T data;
		public LinkedList<TreeNode<T>> children { get; protected set; }
		public TreeNode<T> parent { get; protected set; }

		public TreeNode(T data)
		{
			this.data = data;
			children = new LinkedList<TreeNode<T>>();
			parent = null;
		}

		public TreeNode<T> AddChild(T data)
		{
			TreeNode<T> child = new TreeNode<T>(data);
			children.AddFirst(child);

			child.parent = this;
			return child;
		}

		public TreeNode<T> GetChild(int i)
		{
			foreach (TreeNode<T> n in children)
				if (--i == 0)
					return n;
			return null;
		}

		public void Traverse(TreeNode<T> node, TreeVisitor<T> visitor)
		{
			visitor(node);
			foreach (TreeNode<T> kid in node.children)
				Traverse(kid, visitor);
		}
	}
}
