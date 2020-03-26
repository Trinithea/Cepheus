using System;
using System.Collections.Generic;
using System.Text;

namespace Cepheus
{
	class Floyd_Warshall : IAlgorithm
	{
		public string Name => "Floyd-Warshall's algorithm";
		public string TimeComplexity => "O(n^3)";
		int?[,,] distanceMatrix = null;
		Dictionary<string, FloydWarschallVertex> vertices;

		public int? GetDistance(Graph<FloydWarschallVertex> graph,string from, string to)
		{
			if(distanceMatrix == null) // TODO what if there are only some changes
				Run(graph, graph.GetVertex(from));

			FloydWarschallVertex vertexFrom = vertices[from];
			FloydWarschallVertex vertexTo = vertices[to];

			return (distanceMatrix[vertices.Count-1, vertexFrom.ID, vertexTo.ID]);
		}


		// initialVertex is not necessary but it's here for same type of argumetnts of method IAlgorithm.Run()
		public void Run(Graph<FloydWarschallVertex> graph, FloydWarschallVertex initialVertex)
		{
			vertices = graph.GetVertices();
			int countOfVertices = vertices.Count;

			FloydWarschallVertex[] verticesArray = GetVerticesInArrayWithConcreteId();

			distanceMatrix = GetDistancesWithZeroInnerVertices(graph, verticesArray);

			for (int k = 0; k < countOfVertices - 1; ++k)
				for (int i = 0; i < countOfVertices; ++i)
					for (int j = 0; j < countOfVertices; ++j)
					{
						distanceMatrix[k + 1, i, j] = GetMinimum(distanceMatrix[k, i, j], distanceMatrix[k, i, k+1] + distanceMatrix[k, k + 1, j]);
					}
		}
		FloydWarschallVertex[] GetVerticesInArrayWithConcreteId()
		{
			FloydWarschallVertex[] verticesArray = new FloydWarschallVertex[vertices.Count];
			int index = 0;

			foreach(FloydWarschallVertex vertex in vertices.Values)
			{
				verticesArray[index] = vertex;
				vertex.ID = index;
				index++;
			}

			return verticesArray;
		}

		// TODO override operator +

		public int? GetMinimum(int? num1, int? num2)
		{
			if (num1 == null && num2 == null)
				return null;
			else if (num1 == null)
				return num2; // if num2 is also null, it's ok to return null
			else if (num2 == null)
				return num1;
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
						matrixOfDistances[0,i, j] = null;
					else
						matrixOfDistances[0,i, j] = ((EdgeWithLength<FloydWarschallVertex>)edge).Length; // TODO better
				}
			}
			return matrixOfDistances;
		} 
	}
}
