using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Floyd_Warshall : IAlgorithm
	{
		public string Name => "Floyd-Warshall's algorithm";
		public string TimeComplexity => "O(n^3)";

		// initialVertex is not necessary but it's here for same type of argumetnts of method IAlgorithm.Run()
		public void Run(Graph<FloydWarschallVertex> graph, FloydWarschallVertex initialVertex)
		{
			FloydWarschallVertex[] vertices = graph.GetVertices();
			int countOfVertices = vertices.Length;
			int?[,,] distanceMatrix = GetDistancesWithZeroInnerVertices(graph, vertices);

			for (int k = 0; k < countOfVertices - 1; k++)
				for (int i = 1; i < countOfVertices; i++)
					for (int j = 1; j < countOfVertices; j++)
					{
						distanceMatrix[k+1,i,j] = GetMinimum(distanceMatrix[k,i,j],(distanceMatrix[])
					}


		}
		int? GetMinimum(int? num1, int? num2)
		{
			if (num1 == null)
				return num2; // if num2 is also null, it's ok to return null
			else if (num2 == null)
				return null;
			else if (num1 <= num2)
				return num1;
			else
				return num2;
		}

		int?[,,] GetDistancesWithZeroInnerVertices(Graph<FloydWarschallVertex> graph, FloydWarschallVertex[] vertices)
		{
			
			int?[,,] matrixOfDistances = new int?[vertices.Length, vertices.Length,vertices.Length];

			for (int i = 0; i <vertices.Length; i++)
			{
				for (int j = 0; j < vertices.Length; j++)
				{
					var edge = graph.GetEdge(vertices[i], vertices[j]);
					if (edge == null)
						matrixOfDistances[i, j,0] = null;
					else
						matrixOfDistances[i, j,0] = ((EdgeWithLength<FloydWarschallVertex>)edge).Length; // TODO better
				}
			}
			return matrixOfDistances;
		} 
	}
}
