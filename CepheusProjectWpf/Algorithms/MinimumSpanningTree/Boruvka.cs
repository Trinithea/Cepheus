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
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => true;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.BoruvkaAlgo;

		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.BoruvkaTime;
		/// <summary>
		/// Gradually formed minimum spanning tree.
		/// </summary>
		public TreeWithContextComponents<BoruvkaVertex> MinimumSpanningTree {get; private set;}

		public override string Description => CepheusProjectWpf.Properties.Resources.BoruvkaDesc;
		/// <summary>
		/// The main method of Boruvka's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.ComponentId;
			Graph.InitializeVertices(); // to get OutEdges from each vertex sorted from lightest to heaviest
			PrintVerticesInitialized(Graph);
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.OutEdgesSorted; 


			TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree = new TreeWithContextComponents<BoruvkaVertex>();
			minimumSpanningTree.Vertices = new List<BoruvkaVertex>(Graph.Vertices.Values); // every vertex is in the minimum spanning tree
			List<int> ids = new List<int>(); //ID numbers of currents context components

			Initialize(minimumSpanningTree,ids); // each vertex is a context component
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.ContextComponentInicialized;

			while(minimumSpanningTree.ContextComponents.Count > 1) // graph is not continuous
			{
				for (int i = 0; i < ids.Count; i++)
				{
					var lightestEdge = FindLightestEdgeFromComponent(minimumSpanningTree, minimumSpanningTree.ContextComponents[ids[i]]);
					if (lightestEdge != null) // lightestEdge can be null if no edge leads from the component
					{
						minimumSpanningTree.Edges.Add(lightestEdge.Name, lightestEdge);
						PrintEdgeAddedToMinimumSpanningTree(lightestEdge.From, lightestEdge.To);

						ColorEdge(lightestEdge);
						minimumSpanningTree.NewEdges.Add(lightestEdge); // the context components are then merged via newEdges
						await Task.Delay(delay);
					}
				}
				RemoveDoubleEdges(minimumSpanningTree);
				MergeContextComponents(minimumSpanningTree,ids);
			}
			MinimumSpanningTree = minimumSpanningTree;
		}
		/// <summary>
		/// Removes edges which were to be removed in method FindLightestEdgeFromComponent(...)
		/// </summary>
		/// <param name="minimumSpanningTree"></param>
		void RemoveDoubleEdges(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree)
		{
			foreach(var edge in minimumSpanningTree.EdgesToRemove)
			{
				if(edge != null) // opposite edge of lightestEdge may not exist
				{
					minimumSpanningTree.Edges.Remove(edge.Name);
					minimumSpanningTree.NewEdges.Remove(edge);
				}
			}
			minimumSpanningTree.EdgesToRemove.Clear();
		}
		/// <summary>
		/// Finds the lightest edge leading from the component in the argument. It marks it for deletion, even the opposite edge.
		/// </summary>
		/// <param name="minimumSpanningTree"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		internal Edge<BoruvkaVertex> FindLightestEdgeFromComponent(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree, ComponentTree<BoruvkaVertex> component)
		{
			BoruvkaVertex vertex = null;
			Edge<BoruvkaVertex> lightestEdge = null;

			// Finding the lightest edge
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				if (component.Vertices[i].OutEdges.Count > 0)
				{
					var edge = component.Vertices[i].OutEdges[0]; // OutEdges are sorted so the lightest edge should be on index 0
					if (lightestEdge==null || (lightestEdge.Length > edge.Length && edge.From.ComponentID != edge.To.ComponentID)) 
					{
							lightestEdge = edge;
							vertex = component.Vertices[i];
					}
				}
			}

			if (lightestEdge != null)
			{
				//remove lightest edge from OutEdges because we will never add it again
				vertex.OutEdges.Remove(lightestEdge);
				//also remove the same edge in opposite direction if we have not-oriented graph
				if (!minimumSpanningTree.EdgesToRemove.Contains(lightestEdge))
				{
					var vertex2 = lightestEdge.To;
					minimumSpanningTree.EdgesToRemove.Add(Graph.GetEdge(vertex2.UniqueId + "->" + vertex.UniqueId));
					//it means that between two components will be only one new edge
				}
			}
			return lightestEdge;
		}


		/// <summary>
		/// Merge context components over newly added edges into the spanning tree (NewEdges). List of 'ids' will be shortened accordingly. The component ID of the vertices changes.
		/// </summary>
		/// <param name="minimumSpanningTree"></param>
		/// <param name="ids"></param>
		internal void MergeContextComponents(TreeWithContextComponents<BoruvkaVertex> minimumSpanningTree,List<int> ids)
		{
			List<ComponentTree<BoruvkaVertex>> newComponents = new List<ComponentTree<BoruvkaVertex>>();
			outputConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.ContextComponentsMerging;
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
					vertex.UpdateVertexInfo();
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
		/// <summary>
		/// Each vertex is in a different component and the component is added to the minimum spanning tree.
		/// </summary>
		/// <param name="minimumSpanningTree"></param>
		/// <param name="ids"></param>
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
				minimumSpanningTree.Vertices[i].UpdateVertexInfo();
				ColorVertex(minimumSpanningTree.Vertices[i]);
			}
		}
		
	}
}
