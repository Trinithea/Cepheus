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
		internal Graph<BoruvkaVertex> graph;
		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
		{
			graph.InitializeVertices(); // to get OutEdges sorted
			this.graph = graph;
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

		internal EdgeWithLength<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree,  Tree<BoruvkaVertex> component)
		{
			var vertex = component.Vertices[0];
			EdgeWithLength<BoruvkaVertex> lightestEdge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[0].OutEdges[0]; // some random edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				if (component.Vertices[i].OutEdges.Count > 0)
				{
					var edge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[i].OutEdges[0]; // OutEdges are sorted so the lightest edge should be on index 0
					if (lightestEdge.Length > edge.Length && !minimalSpanningTree.Edges.ContainsKey(edge.Name))
					{
						lightestEdge = edge;
						vertex = component.Vertices[i];
					}
				}	
			}

			//remove lightest edge from OutEdges because we will never add it again
			vertex.OutEdges.Remove(lightestEdge);
			//also remove the same edge in opposite direction if we have not-oriented graph
			//TODO if not oriented
			var vertex2 = lightestEdge.To;
			vertex2.OutEdges.Remove(graph.GetEdge(vertex2.Name+vertex.Name));
			return lightestEdge;
		}

		void UpdateContextComponents(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree)
		{
			//TODO bacha na obousměrný!
			for (int i = 0; i < minimalSpanningTree.NewEdges.Count; i++)
			{

			}
		}

		internal void Initialize(TreeWithContextComponents<BoruvkaVertex> tree)
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
