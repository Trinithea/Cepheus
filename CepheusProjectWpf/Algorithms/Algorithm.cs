using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;
namespace Cepheus
{
	public abstract class Algorithm 
	{
		public abstract string Name { get; }
		public abstract string TimeComplexity { get; }
		public abstract void Accept(Visitor visitor);
	}
	public abstract class Algorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{ 
		public Graph CreateGraph(Dictionary<MainWindow.EllipseVertex, string> vertices, Dictionary<MainWindow.ArrowEdge, string> edges)
		{
			Graph<TVertex> graph = new Graph<TVertex>();
			foreach (var vertexName in vertices.Values)
			{
				graph.AddVertex(vertexName);
			}
			foreach (var edge in edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Name, edge.Length);
			}
			return graph;
		}
	}
	public abstract class FlowAlgorithm<TVertex> : Algorithm where TVertex : VertexBase<TVertex>, new()
	{
		public FlowNetwork<TVertex> CreateGraph(Dictionary<MainWindow.EllipseVertex, string> vertices, Dictionary<MainWindow.ArrowEdge, string> edges)
		{
			FlowNetwork<TVertex> graph = new FlowNetwork<TVertex>();
			foreach (var vertexName in vertices.Values)
			{
				graph.AddVertex(vertexName);
			}
			foreach (var edge in edges.Keys)
			{
				graph.AddEdge(graph.GetVertex(edge.FromVertex.Name), graph.GetVertex(edge.ToVertex.Name), edge.Name, edge.Length);
			}
			return graph;
		}
	}
}
