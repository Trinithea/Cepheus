using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestCepheusAlgorithms")]
namespace Cepheus
{
	public class FloydWarshall : Algorithm<FloydWarshallVertex>
	{
		public override bool IsFlowAlgorithm => false;
		public override bool NeedsOnlyNonNegativeEdgeLenghts => false;
		public override bool DontNeedInitialVertex => true;
		public override void Accept(VisitorGraphCreator visitor)
		{
			visitor.Visit(this);
		}
		public async override Task Accept(VisitorRunner visitor)
		{
			await visitor.Visit(this);
		}
		public override string Name => CepheusProjectWpf.Properties.Resources.FWAlgo;
		public override string TimeComplexity => CepheusProjectWpf.Properties.Resources.FWTime;

		public override string Description => CepheusProjectWpf.Properties.Resources.FWDesc;

		int?[,] distanceMatrix = null;
		Dictionary<int, FloydWarshallVertex> vertices; //abbreviation for Graph.Vertices
		/// <summary>
		/// The main method of Floyd-Warshall's algorithm. This is where the whole calculation takes place.
		/// </summary>
		/// <returns></returns>
		public async Task Run()
		{
			vertices = Graph.Vertices;
			int countOfVertices = vertices.Count;

			FloydWarshallVertex[] verticesArray = GetVerticesInArrayWithConcreteId();
			PrintVerticesInitialized(Graph);

			distanceMatrix = GetDistancesWithZeroInnerVertices( verticesArray);
			Print2DMatrix(distanceMatrix);

			for (int k = 0; k < countOfVertices; k++)
			{
				await Task.Delay(delay);
				outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.ShortesPathContain + k;
				for (int i = 0; i < countOfVertices; i++)
					for (int j = 0; j < countOfVertices; j++)
					{
						distanceMatrix[i, j] = GetMinimum(distanceMatrix[i, j], distanceMatrix[i, k] + distanceMatrix[k, j]);
					}
				Print2DMatrix(distanceMatrix);
				outputConsole.Text += "\n";
			}
				
		}
		/// <summary>
		/// Each vertex is assigned an ID - a position in the returned array.
		/// </summary>
		/// <returns></returns>
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
		/// <summary>
		/// Proper comparison with a null value. Returns null if both arguments are null.
		/// </summary>
		/// <param name="num1"></param>
		/// <param name="num2"></param>
		/// <returns></returns>
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
		/// <summary>
		/// Sets the initial state of the distance matrix. Only the vertices have a zero distance with themselves, with the others they have null.
		/// </summary>
		/// <param name="vertices"></param>
		/// <returns></returns>
		int?[,] GetDistancesWithZeroInnerVertices( FloydWarshallVertex[] vertices)
		{
			
			int?[,] matrixOfDistances = new int?[vertices.Length,vertices.Length];

			for (int i = 0; i <vertices.Length; i++)
			{
				for (int j = 0; j < vertices.Length; j++)
				{
					if (i == j)
						matrixOfDistances[i, j] = 0;
					else
					{
						var edge = Graph.GetEdge(vertices[i], vertices[j]);
						if (edge == null)
							matrixOfDistances[i, j] = null;
						else
							matrixOfDistances[i, j] = edge.Length;
					}
					
				}
			}
			return matrixOfDistances;
		}

		void Print2DMatrix(int?[,] matrix) 
		{
			outputConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.MatrixDist;
			outputConsole.Text += "\n   | ";
			for (int i = 0; i < vertices.Count; i++)
				outputConsole.Text += i + "  ";
			outputConsole.Text += "\n---|";
			for (int i = 0; i < vertices.Count; i++)
				outputConsole.Text += "---";
			for (int i = 0; i < vertices.Count; i++)
			{
				outputConsole.Text += "\n "+i+" | ";
				for (int j = 0; j < vertices.Count; j++)
				{
					if (matrix[i, j] == null)
						outputConsole.Text += "∞  ";
					else
						outputConsole.Text += matrix[i, j] + "  ";
				}
			}
			outputConsole.Text += "\n";
		}
		
	}
}
