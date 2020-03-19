using System;
using System.Collections.Generic;
using System.Text;

namespace CepheusProject.DataStructures
{
	class MinimumBinaryHeap
	{
		public List<Node> heap = new List<Node>();
		public Node Root { get; set; }
		int CountOFNodes = -1; // because firts node will be on index 0
		public void Insert(Node newNode)
		{
			CountOFNodes++;

		}
	}
	class Node
	{
		public Node LeftSon { get; set; }
		public Node RightSon { get; set; }
		public int Key { get; set; }
	}
}
