using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cepheus;
using CepheusProjectWpf;

namespace ViewModel
{
    public class Visitor
    {
        public Graph Visit(BFS algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(DFS algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Dinic algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(FordFulkerson algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Goldberg algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Boruvka algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Jarnik algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Kruskal algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Bellman_Ford algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Dijkstra algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Floyd_Warshall algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }
        public Graph Visit(Relaxation algorithm)
        {
            return algorithm.CreateGraph(MainWindow.Vertices, MainWindow.Edges);
        }

    }
}
