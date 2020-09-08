﻿using CepheusProjectWpf.GraphShapes;
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
		static Canvas canvas;
		static TextBox outputConsole;
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
					ReadFile(openFileDialog.FileName);
				}
				catch (FormatException)
				{
					MessageBox.Show("File has an incorrect format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch
				{
					MessageBox.Show("There is some problem with reading this file. Please, try it again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
				MessageBox.Show("Can't open an OpenFileDialog", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		static void ReadFile(string url)
		{
			int idCounter = 0;
			StreamReader reader = new StreamReader(url);
			if (!reader.EndOfStream) // file is not empty
			{
				if (reader.ReadLine() == "Vertices:")
				{
					string line = reader.ReadLine();
					while (line != "Edges:")
					{
						try
						{
							ConvertLineToVertex(line);
							idCounter++;
						}
						catch
						{
							throw new FormatException();
						}
						line = reader.ReadLine();
					}
					if(!reader.EndOfStream)
						line = reader.ReadLine();
					while (!reader.EndOfStream)
					{
						try
						{
							ConvertLineToEdge(line);
						}
						catch
						{
							throw new FormatException();
						}
						line = reader.ReadLine();
					}
					ConvertLineToEdge(line);
				}
				else
				{
					throw new FormatException();
				}
			}
			else
				MessageBox.Show("The file is empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			MainWindow.IdCounter = idCounter;
		}

		static void ConvertLineToVertex(string line)
		{
			string[] vertexInfo = line.Split(';');
			double left = Convert.ToDouble(vertexInfo[0]);
			double top = Convert.ToDouble(vertexInfo[1]);
			int uniqueId = Convert.ToInt32(vertexInfo[2]);
			string name = vertexInfo[3];

			var newVertex = new EllipseVertex(new System.Windows.Point(left, top), uniqueId, name, canvas, outputConsole); //creates new vertex right on canvas in MainWindow
		}
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
		}

	}
}
