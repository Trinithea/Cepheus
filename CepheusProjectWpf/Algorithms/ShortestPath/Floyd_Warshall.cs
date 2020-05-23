using System;
using System.Collections.Generic;
using System.Text;
using CepheusProjectWpf;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	public class Floyd_Warshall : Algorithm<FloydWarshallVertex>
	{
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public override void Accept(VisitorRunner visitor)
		{
			visitor.Visit(this);
		}
		public override string Name => "Floyd-Warshall's algorithm";
		public override string TimeComplexity => "O(n^3)";
		int?[,] distanceMatrix = null;
		Dictionary<int, FloydWarshallVertex> vertices;


		//TODO why is this here?
		public int? GetDistance(Graph<FloydWarshallVertex> graph,int fromId, int toId)
		{
			if(distanceMatrix == null) // TODO what if there are only some changes
				Run();

			FloydWarshallVertex vertexFrom = vertices[fromId];
			FloydWarshallVertex vertexTo = vertices[toId];

			return (distanceMatrix[vertexFrom.ID, vertexTo.ID]);
		}

		public void Run()
		{
			vertices = graph.Vertices;
			int countOfVertices = vertices.Count;

			FloydWarshallVertex[] verticesArray = GetVerticesInArrayWithConcreteId();

			distanceMatrix = GetDistancesWithZeroInnerVertices( verticesArray);

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

		int?[,] GetDistancesWithZeroInnerVertices( FloydWarshallVertex[] vertices)
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
