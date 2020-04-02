using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Jarnik : IAlgorithm
	{
		public string Name => "Jarnik's algorithm";

		public string TimeComplexity => "O(m * log(n))";

		public void Run(Graph<JarnikVertex> graph, Vertex initialVertex)
		{
			throw new NotImplementedException();
		}
	}
}
