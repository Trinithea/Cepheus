using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	public class Floyd_Warshall : Algorithm<FloydWarshallVertex>
	{
		public override void Accept(Visitor visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Floyd-Warshall's algorithm";
		public override string TimeComplexity => "O(n^3)";
		int?[,] distanceMatrix = null;
		Dictionary<string, FloydWarshallVertex> vertices;

		public int? GetDistance(Graph<FloydWarshallVertex> graph,string from, string to)
		{
			if(distanceMatrix == null) // TODO what if there are only some changes
				Run(graph, graph.GetVertex(from));

			FloydWarshallVertex vertexFrom = vertices[from];
			FloydWarshallVertex vertexTo = vertices[to];

			return (distanceMatrix[vertexFrom.ID, vertexTo.ID]);
		}


		// initialVertex is not necessary but it's here for same type of argumetnts of method IAlgorithm.Run()
		public void Run(Graph<FloydWarshallVertex> graph, FloydWarshallVertex initialVertex)
		{
			vertices = graph.Vertices;
			int countOfVertices = vertices.Count;

			FloydWarshallVertex[] verticesArray = GetVerticesInArrayWithConcreteId();

			distanceMatrix = GetDistancesWithZeroInnerVertices(graph, verticesArray);

			for (int k = 0; k < countOfVertices; k++)
			{
				for (int i = 0; i < countOfVertices; i++)
					for (int j = 0; j < countOfVertices; j++)
					{
						distanceMatrix[i, j] = GetMinimum(distanceMatrix[i, j], distanceMatrix[i, k] + distanceMatrix[k, j]);
					}

			}
				
		}

		FloydWarshallVertex[] GetVerticesInArrayWithConcreteId()
		{
			FloydWarshallVertex[] verticesArray = new FloydWarshallVertex[vertices.Count];
			int index = 0;

			foreach(FloydWarshallVertex vertex in vertices.Values)
			{
				verticesArray[index] = vertex;
				vertex.ID = index;
				index++;
			}

			return verticesArray;
		}

		internal int? GetMinimum(int? num1, int? num2)
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

		int?[,] GetDistancesWithZeroInnerVertices(Graph<FloydWarshallVertex> graph, FloydWarshallVertex[] vertices)
		{
			
			int?[,] matrixOfDistances = new int?[vertices.Length,vertices.Length];

			for (int i = 0; i <vertices.Length; i++)
			{
				for (int j = 0; j < vertices.Length; j++)
				{
					var edge = graph.GetEdge(vertices[i], vertices[j]);
					if (edge == null)
						matrixOfDistances[i, j] = null;
					else
						matrixOfDistances[i, j] = edge.Length;
				}
			}
			return matrixOfDistances;
		}
		
	}
}
