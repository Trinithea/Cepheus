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

		static int Time = 0;
		public async Task Run()
		{
			PrintVertexInfo();
			Graph.InitializeVertices();
			PrintVerticesInitialized(Graph);

			Time = 0;
			
			await Recursion(initialVertex); 
		}
		async Task Recursion(DfsVertex vertex)
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
					await Recursion(edge.To);
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
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
	}
}
