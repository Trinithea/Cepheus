using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	interface IAlgorithm
	{ 
		// TODO singleton?
		public string Name { get; }
		public string TimeComplexity {get; }
		public void Run (Graph graph, Vertex initialVertex) { }
	}
}
