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
        public void Visit(BellmanFord algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(Dijkstra algorithm)
        {
            algorithm.CreateGraph();
        }
        public void Visit(FloydWarshall algorithm)
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
        public async Task Visit(BFS algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(DFS algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Dinic algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(FordFulkerson algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Goldberg algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Boruvka algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Jarnik algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Kruskal algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(BellmanFord algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Dijkstra algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(FloydWarshall algorithm)
        {
            await algorithm.Run();
        }
        public async Task Visit(Relaxation algorithm)
        {
            await algorithm.Run();
        }
    }
}
