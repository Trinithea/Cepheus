using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

namespace Cepheus
{
    public class Visitor { }
    public class VisitorGraphCreator : Visitor
    {
        public void Visit(BFS algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(DFS algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Dinic algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(FordFulkerson algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Goldberg algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Boruvka algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Jarnik algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Kruskal algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Bellman_Ford algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Dijkstra algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Floyd_Warshall algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Relaxation algorithm)
        {
            algorithm.CreateGraph();
        }
    }
    public class VisitorRunner : Visitor
    {
        public void Visit(BFS algorithm)
        {
            algorithm.Run();
        }
        public void Visit(DFS algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Dinic algorithm)
        {
            algorithm.Run();
        }
        public void Visit(FordFulkerson algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Goldberg algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Boruvka algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Jarnik algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Kruskal algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Bellman_Ford algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Dijkstra algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Floyd_Warshall algorithm)
        {
            algorithm.Run();
        }
        public void Visit(Relaxation algorithm)
        {
            algorithm.Run();
        }
    }
    public class VisitorStepper : Visitor
    {

    }
}
