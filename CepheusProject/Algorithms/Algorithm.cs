using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	public interface IAlgorithm
	{
		public static string Name { get; }

		// TODO TimeComplexity {get; }
		// TODO public static void Run(Graph graph, Vertex initialVertex) 
	}
}
