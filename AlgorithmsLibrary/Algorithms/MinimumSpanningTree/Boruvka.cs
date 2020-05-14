using System;
using System.Collections.Generic;
using System.Text;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Boruvka : IAlgorithm
	{
		public void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public string Name => "Boruvka's algorithm";

		public string TimeComplexity => "m * log(n)";
		internal Graph<BoruvkaVertex> graph;
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree {get; private set;}
		public void Run(Graph<BoruvkaVertex> graph, BoruvkaVertex initialVertex)
		{
			graph.InitializeVertices(); // to get OutEdges sorted
			this.graph = graph;
			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(graph.Vertices.Values);
			List<int> ids = new List<int>(); //ID numbers of currents context components

			Initialize(minimumSpanningTree,ids); // each vertex is a context component

			while(minimumSpanningTree.ContextComponents.Count > 1) // graph is not continuous
			{
				for (int i = 0; i < ids.Count; i++)
				{
					var lightestEdge = FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[ids[i]]);
					minimumSpanningTree.Edges.Add(lightestEdge.Name, lightestEdge);
					minimumSpanningTree.NewEdges.Add(lightestEdge);
				}
				RemoveDoubleEdges(minimumSpanningTree);
				MergeContextComponents(minimumSpanningTree,ids);
			}

			MinimumSpanningTree = minimumSpanningTree;
		}
		void RemoveDoubleEdges(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree)
		{
			foreach(var edge in minimumSpanningTree.EdgesToRemove)
			{
				minimumSpanningTree.Edges.Remove(edge.Name);
				minimumSpanningTree.NewEdges.Remove(edge);
			}
			minimumSpanningTree.EdgesToRemove.Clear();
		}

		internal Edge<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree, ComponentTree<BoruvkaVertex> component)
		{
			var vertex = component.Vertices[0];
			Edge<BoruvkaVertex> lightestEdge = component.Vertices[0].OutEdges[0]; // some random edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				if (component.Vertices[i].OutEdges.Count > 0)
				{
					var edge = component.Vertices[i].OutEdges[0]; // OutEdges are sorted so the lightest edge should be on index 0
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
			if (!minimumSpanningTree.EdgesToRemove.Contains(lightestEdge))
			{
				var vertex2 = lightestEdge.To;
				minimumSpanningTree.EdgesToRemove.Add(graph.GetEdge(vertex2.Name + vertex.Name));
				//it means that between two components will be only one new edge
			}

			//vertex2.OutEdges.Remove(graph.GetEdge(vertex2.Name+vertex.Name));
			return lightestEdge;
		}

		internal void MergeContextComponents(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree,List<int> ids)
		{
			List<ComponentTree<BoruvkaVertex>> newComponents = new List<ComponentTree<BoruvkaVertex>>();
			for (int i = 0; i < minimumSpanningTree.NewEdges.Count; i++)
			{
				var newEdge = minimumSpanningTree.NewEdges[i];
				var newComponent = minimumSpanningTree.ContextComponents[newEdge.From.ComponentID];

				var toComponentID = newEdge.To.ComponentID;
				//adding vertices from component whose member is newEdge.To
				for (int j = 0; j < minimumSpanningTree.ContextComponents[toComponentID].Vertices.Count; j++)
				{
					var vertex = minimumSpanningTree.ContextComponents[toComponentID].Vertices[j];
					vertex.ComponentID = newComponent.ID;
					newComponent.Vertices.Add(vertex);
				}
				//adding edges
				for (int j = 0; j < minimumSpanningTree.ContextComponents[toComponentID].Edges.Count; j++)
				{
					var edge = minimumSpanningTree.ContextComponents[toComponentID].Edges[j];
					newComponent.Edges.Add(edge);
				}
				newComponent.Edges.Add(newEdge);
				minimumSpanningTree.ContextComponents.Remove(toComponentID);
				ids.Remove(toComponentID);
				//we have merged two components into one
			}
			minimumSpanningTree.NewEdges.Clear();
		}

		internal void Initialize(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree, List<int> ids)
		{
			for (int i = 0; i < minimumSpanningTree.Vertices.Count; i++)
			{
				var component = new ComponentTree<BoruvkaVertex>();
				component.ID = i;
				component.Vertices.Add(minimumSpanningTree.Vertices[i]);
				minimumSpanningTree.ContextComponents.Add(component.ID, component);
				minimumSpanningTree.Vertices[i].ComponentID = i;
				ids.Add(i);
			}
		}
		//TODO asi by stačilo používat tu MinimumSpanningTree a ne si to pořád předávat v parametrech
		public Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges)
		{
			Graph<BoruvkaVertex> graph = new Graph<BoruvkaVertex>();
			foreach (var vertex in vertices)
			{
				graph.AddVertex(new BoruvkaVertex(vertex.Name));
			}
			foreach (var edge in edges)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Length);
			}
			return graph;
		}
	}
}
