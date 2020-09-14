using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
	public class DFS : Algorithm<DfsVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => false;
		public override string Name => CepheusProjectWpf.Properties.Resources.DFSAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.DFSTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.DFSDesc;
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}

		static int Time = 0;
		/// <summary>
		/// The main method of Depth-first search algorithm. This is where the recursion starts.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			PrintVertexInfo();
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			Time = 0;
			
			await SearchDeeply(initialVertex); 
		}
		/// <summary>
		/// The recursion method where the whole graph is gradually traversed.
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		private async Task SearchDeeply(DfsVertex vertex)
		{
			vertex.State = States.Open;
			Time++;
			vertex.InTime = Time;
			vertex.UpdateVertexInfo();
			PrintOpenedVertex(vertex);
			ColorVertex(vertex);
			foreach (Edge<DfsVertex> edge in vertex.OutEdges)
			{
				if (edge.To.State == States.Unvisited)
				{
					ColorEdge(edge);
					await Task.Delay(delay-250);
					ColorVertex(edge.To);
					edge.To.UpdateVertexInfo();
					await Task.Delay(delay);
					await SearchDeeply(edge.To);
				}
				UncolorEdge(edge);
				await Task.Delay(delay);
			}
			vertex.State = States.Closed;
			Time++;
			vertex.OutTime = Time;
			vertex.UpdateVertexInfo();
			PrintClosedVertex(vertex);
			UncolorVertex(vertex);
			await Task.Delay(delay);
		}
		/// <summary>
		/// Prints "In brackets is the state of vertex, entry time and departure time."
		/// </summary>
		void PrintVertexInfo()
		{
			outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.StateInOutTime;
		}
		void PrintOpenedVertex(DfsVertex vertex)
		{
			outputConsole.Text += "\n"+CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.OpenTime + vertex.InTime;
		}
		void PrintClosedVertex(DfsVertex vertex)
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.NLVertexSpace + vertex.Name + CepheusProjectWpf.Properties.Resources.CloseTime+ vertex.OutTime;
		}
		
	}
}
