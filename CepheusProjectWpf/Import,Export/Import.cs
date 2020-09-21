using CepheusProjectWpf.GraphShapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CepheusProjectWpf.Import_Export
{
	class Import
	{
		/// <summary>
		/// Canvas on which the imported graph will be drawn.
		/// </summary>
		static Canvas canvas;
		/// <summary>
		/// Dump console.
		/// </summary>
		static TextBox outputConsole;
		/// <summary>
		/// It loads the chart from a file of the correct format, otherwise it displays an Error MessageBox.
		/// </summary>
		/// <param name="graphCanvas"></param>
		/// <param name="console"></param>
		public static void Upload(Canvas graphCanvas, TextBox console)
		{
			canvas = graphCanvas;
			outputConsole = console;

			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.Filter = "Text files (*.txt)|*.txt";
			if (openFileDialog.ShowDialog() == true)
			{
				try
				{
					ReadFile(new StreamReader(openFileDialog.FileName), graphCanvas, console);
				}
				catch (FormatException)
				{
					MessageBox.Show(CepheusProjectWpf.Properties.Resources.IncorrectFormat, CepheusProjectWpf.Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch
				{
					MessageBox.Show(CepheusProjectWpf.Properties.Resources.ProblemReading, CepheusProjectWpf.Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		/// <summary>
		/// Retrieves a graph from text via StringReader.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="graphCanvas"></param>
		/// <param name="console"></param>
		public static void ReadFile(TextReader reader, Canvas graphCanvas, TextBox console)
		{
			canvas = graphCanvas;
			outputConsole = console;

				var line = reader.ReadLine();
				if (line == "Vertices:" )
				{
					while ((line = reader.ReadLine()) != "Edges:")
					{
						try
						{
							ConvertLineToVertex(line);
						}
						catch
						{
							throw new FormatException();
						}
					}
						
					while ((line = reader.ReadLine()) != null)
					{
						try
						{
							ConvertLineToEdge(line);
						}
						catch
						{
							throw new FormatException();
						}
					}
				}
				else
				{
					throw new FormatException();
				}
			
		}

		/// <summary>
		/// Creates a new vertex from the information on the line.
		/// </summary>
		/// <param name="line"></param>
		static void ConvertLineToVertex(string line)
		{
			string[] vertexInfo = line.Split(';');
			double left = Convert.ToDouble(vertexInfo[0]);
			double top = Convert.ToDouble(vertexInfo[1]);
			int uniqueId = Convert.ToInt32(vertexInfo[2]);
			string name = vertexInfo[3];

			var newVertex = new EllipseVertex(new System.Windows.Point(left, top), uniqueId, name, canvas, outputConsole); //creates new vertex right on canvas in MainWindow
			MainWindow.Vertices.Add(newVertex, newVertex.Name);
			MainWindow.VerticesById.Add(newVertex.UniqueId, newVertex);
			MainWindow.IdCounter++;
		}
		/// <summary>
		/// Creates a new edge from the information on the line.
		/// </summary>
		/// <param name="line"></param>
		static void ConvertLineToEdge(string line)
		{
			string[] edgeInfo = line.Split(';');
			double X1 = Convert.ToDouble(edgeInfo[0]);
			double Y1 = Convert.ToDouble(edgeInfo[1]);
			double X2 = Convert.ToDouble(edgeInfo[2]);
			double Y2 = Convert.ToDouble(edgeInfo[3]);
			string length = edgeInfo[4];
			int fromVertexId = Convert.ToInt32(edgeInfo[5]);
			int toVertexId = Convert.ToInt32(edgeInfo[6]);

			var newEdge = new ArrowEdge(canvas, MainWindow.VerticesById[fromVertexId], MainWindow.VerticesById[toVertexId], X1, Y1, X2, Y2, length, outputConsole); //creates new edge on canvas
			MainWindow.Edges.Add(newEdge, newEdge.Name);
		}

	}
}
