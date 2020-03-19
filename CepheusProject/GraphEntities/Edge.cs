using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	class Edge<T> where T: Vertex
	{
		public T From { get;  }
		public T To { get; }
		public string Name { get; }
		public Edge(string name, T from, T to)
		{
			From = from;
			To = to;
			Name = name;
		}
	}
}
