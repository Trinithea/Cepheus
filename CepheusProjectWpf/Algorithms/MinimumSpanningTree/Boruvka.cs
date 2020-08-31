using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cepheus.DataStructures;
using CepheusProjectWpf;

namespace Cepheus
{
	public class Boruvka : Algorithm<BoruvkaVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => "Boruvka's algorithm";

		public override string TimeComplexity => "m * log(n)";
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree {get; private set;}

		public override string Description => "Description is not implemented.";//TODO

		public async Task Run()
		{
			Graph.InitializeVertices(); // to get OutEdges sorted
			PrintVerticesInitialized(Graph);
			outputConsole.Text += "\nOutEdges are sorted from lightest to heaviest for each vertex."; //TODO asi každej neví, co je outedges...


			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(Graph.Vertices.Values);
			List<int> ids = new List<int>(); //ID numbers of currents context components

			Initialize(minimumSpanningTree,ids); // each vertex is a context component
			outputConsole.Text += "\nContext components are inicialized...";

			while(minimumSpanningTree.ContextComponents.Count > 1) // graph is not continuous
			{
				for (int i = 0; i < ids.Count; i++)
				{
					var lightestEdge = FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[ids[i]]);
					if (lightestEdge != null)
					{
						minimumSpanningTree.Edges.Add(lightestEdge.Name, lightestEdge);
						PrintEdgeAddedToMinimumSpanningTree(lightestEdge.From, lightestEdge.To);

						ColorEdge(lightestEdge);
						minimumSpanningTree.NewEdges.Add(lightestEdge);
						await Task.Delay(delay);
					}
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
				if(edge != null) //TODO tahle blbost je tu jen kvůli těm dublovanejm hranám
				{
					minimumSpanningTree.Edges.Remove(edge.Name);
					minimumSpanningTree.NewEdges.Remove(edge);
				}
				
			}
			minimumSpanningTree.EdgesToRemove.Clear();
		}

		internal Edge<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree, ComponentTree<BoruvkaVertex> component)
		{
			var vertex = component.Vertices[0];
			if (vertex.OutEdges.Count == 0)
				return null;
			else
			{
				Edge<BoruvkaVertex> lightestEdge = vertex.OutEdges[0]; // some random edge
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
				//TODO if not oriented a vůbec nechápu, jestli se to tu už řeší nebo ne
				if (!minimumSpanningTree.EdgesToRemove.Contains(lightestEdge))
				{
					var vertex2 = lightestEdge.To;
					minimumSpanningTree.EdgesToRemove.Add(Graph.GetEdge(vertex2.UniqueId + "->" + vertex.UniqueId));
					//it means that between two components will be only one new edge
				}

				//vertex2.OutEdges.Remove(graph.GetEdge(vertex2.Name+vertex.Name));
				return lightestEdge;
			}
			
		}

		internal void MergeContextComponents(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree,List<int> ids)
		{
			List<ComponentTree<BoruvkaVertex>> newComponents = new List<ComponentTree<BoruvkaVertex>>();
			outputConsole.Text += "\n\nContext components are merging...";
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
					PrintVertex(vertex);
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
				PrintVertex(minimumSpanningTree.Vertices[i]);
				ColorVertex(minimumSpanningTree.Vertices[i]);
				//TODO await Task.Delay(delay - 500); //tady při tom to přeskočí dál!!
			}
		}
		//TODO asi by stačilo používat tu MinimumSpanningTree a ne si to pořád předávat v parametrech
		
	}
}
