using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	interface IAlgorithm
	{ 
		// TODO singleton?
		public string Name { get; }

		// TODO TimeComplexity {get; }
		 public void Run (Graph graph, Vertex initialVertex) { }
	}
}
