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
			TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimalSpanningTree.Vertices = new List<BoruvkaVertex>(graph.GetVertices().Values);

			Initialize(minimalSpanningTree); // each vertex is a context component

			while(minimalSpanningTree.ContextComponents.Count > 1) // graph is not continuous
			{
				for (int i = 0; i < minimalSpanningTree.ContextComponents.Count; i++)
				{
					//TODO bacha na obousměrný!
					var ligtestEdge = FindLightestEdgeFromComponent(minimalSpanningTree, minimalSpanningTree.ContextComponents[i]);
					minimalSpanningTree.Edges.Add(ligtestEdge.Name, ligtestEdge);
				}
				UpdateContextComponents(minimalSpanningTree);
			}
		}

		//TODO this is very uneffective
		EdgeWithLength<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree,  Tree<BoruvkaVertex> component)
		{
			EdgeWithLength<BoruvkaVertex> lightestEdge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[0].OutEdges[0]; // some random edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				foreach(EdgeWithLength<BoruvkaVertex> edge in component.Vertices[i].OutEdges)
				{
					if (lightestEdge.Length > edge.Length && !minimalSpanningTree.Edges.ContainsKey(edge.Name))
						lightestEdge = edge;
				}
			}
			return lightestEdge;
		}
		void UpdateContextComponents(TreeWithContextComponents<BoruvkaVertex> tree)
		{
			
		}
		
		void Initialize(TreeWithContextComponents<BoruvkaVertex> tree)
		{
			for (int i = 0; i < tree.Vertices.Count; i++)
			{
				var component = new Tree<BoruvkaVertex>();
				component.Vertices.Add(tree.Vertices[i]);
				tree.ContextComponents.Add(component);
			}
		}
	}
}
