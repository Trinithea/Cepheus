using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;
namespace Cepheus
{
	interface IAlgorithm
	{ 
		string Name { get; }
		string TimeComplexity {get; }
		Graph CreateGraph(List<MainWindow.EllipseVertex> vertices, List<MainWindow.ArrowEdge> edges);
		void Accept(Visitor visitor);
	}
}
