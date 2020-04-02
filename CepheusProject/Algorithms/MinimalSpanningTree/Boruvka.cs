using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;

namespace Cepheus
{
	class Boruvka : IAlgorithm
	{
		public string Name => "Boruvka's algorithm";

		public string TimeComplexity => "m * log(n)";

		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
		{
			graph.InitializeVertices(); // to get OutEdges sorted

			TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimalSpanningTree.Vertices = new List<BoruvkaVertex>(graph.GetVertices().Values);

			Initialize(minimalSpanningTree); // each vertex is a context component

			while(minimalSpanningTree.ContextComponents.Count > 1) // graph is not continuous
			{
				for (int i = 0; i < minimalSpanningTree.ContextComponents.Count; i++)
				{
					var lightestEdge = FindLightestEdgeFromComponent(minimalSpanningTree, minimalSpanningTree.ContextComponents[i]);
					minimalSpanningTree.Edges.Add(lightestEdge.Name, lightestEdge);
					minimalSpanningTree.NewEdges.Add(lightestEdge);
				}
				UpdateContextComponents(minimalSpanningTree);
			}
		}

		EdgeWithLength<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree,  Tree<BoruvkaVertex> component)
		{
			EdgeWithLength<BoruvkaVertex> lightestEdge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[0].OutEdges[0]; // some random edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				var edge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[i].OutEdges[0]; // OutEdges are sorted so the lightest edge should be on index 0
				if (lightestEdge.Length > edge.Length && !minimalSpanningTree.Edges.ContainsKey(edge.Name))
					lightestEdge = edge;
			}
			return lightestEdge;
		}

		void UpdateContextComponents(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree)
		{
			//TODO bacha na obousměrný!
			for (int i = 0; i < minimalSpanningTree.NewEdges.Count; i++)
			{

			}
		}

		void Initialize(TreeWithContextComponents<BoruvkaVertex> tree)
		{
			for (int i = 0; i < tree.Vertices.Count; i++)
			{
				var component = new Tree<BoruvkaVertex>();
				component.Vertices.Add(tree.Vertices[i]);
				tree.ContextComponents.Add(component);
				tree.Vertices[i].ComponentID = i;
			}
		}
	}
}
