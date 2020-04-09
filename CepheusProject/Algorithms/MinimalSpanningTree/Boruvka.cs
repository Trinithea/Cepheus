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
		public TreeWithContextComponents<BoruvkaVertex> MinimalSpanningTree {get; private set;}
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
				MergeContextComponents(minimalSpanningTree);
			}

			MinimalSpanningTree = minimalSpanningTree;
		}

		internal EdgeWithLength<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree, ComponentTree<BoruvkaVertex> component)
		{
			var vertex = component.Vertices[0];
			EdgeWithLength<BoruvkaVertex> lightestEdge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[0].OutEdges[0]; // some random edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				if (component.Vertices[i].OutEdges.Count > 0)
				{
					var edge = (EdgeWithLength<BoruvkaVertex>)component.Vertices[i].OutEdges[0]; // OutEdges are sorted so the lightest edge should be on index 0
					if (lightestEdge.Length > edge.Length && edge.From.ComponentID != edge.To.ComponentID) //TODO asi nepotřebuju: && !minimalSpanningTree.Edges.ContainsKey(edge.Name)
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
			//it means that between two components is only one new edge
			return lightestEdge;
		}

		internal void MergeContextComponents(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree)
		{
			List<ComponentTree<BoruvkaVertex>> newComponents = new List<ComponentTree<BoruvkaVertex>>();
			for (int i = 0; i < minimalSpanningTree.NewEdges.Count; i++)
			{
				var newEdge = minimalSpanningTree.NewEdges[i];
				var newComponent = minimalSpanningTree.ContextComponents[newEdge.From.ComponentID];

				var toComponentID = newEdge.To.ComponentID;
				//adding vertices from component whose member is newEdge.To
				for (int j = 0; j < minimalSpanningTree.ContextComponents[toComponentID].Vertices.Count; j++)
				{
					var vertex = minimalSpanningTree.ContextComponents[toComponentID].Vertices[j];
					vertex.ComponentID = newComponent.ID;
					newComponent.Vertices.Add(vertex);
				}
				//adding edges
				for (int j = 0; j < minimalSpanningTree.ContextComponents[toComponentID].Edges.Count; j++)
				{
					var edge = minimalSpanningTree.ContextComponents[toComponentID].Edges[j];
					newComponent.Edges.Add(edge);
				}

				minimalSpanningTree.ContextComponents.Remove(toComponentID);
				//we have merged two components into one
			}
			minimalSpanningTree.NewEdges.Clear();
		}

		internal void Initialize(TreeWithContextComponents<BoruvkaVertex> minimalSpanningTree)
		{
			for (int i = 0; i < minimalSpanningTree.Vertices.Count; i++)
			{
				var component = new ComponentTree<BoruvkaVertex>();
				component.ID = i;
				component.Vertices.Add(minimalSpanningTree.Vertices[i]);
				minimalSpanningTree.ContextComponents.Add(component.ID, component);
				minimalSpanningTree.Vertices[i].ComponentID = i;
			}
		}
	}
}
