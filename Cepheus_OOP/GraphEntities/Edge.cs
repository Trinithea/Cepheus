using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus_OOP
{
	class Edge
	{
		public Vertex From { get;  }
		public Vertex To { get; }
		public string Name { get; }
		public Edge(string name, Vertex from, Vertex to)
		{
			From = from;
			To = to;
			Name = name;
			From.OutEdges.Add(this);
			To.InEdges.Add(this);
			Graph.Edges.Add(name,this);
		}
	}
}
