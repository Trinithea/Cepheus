﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CepheusProjectWpf.GraphShapes;

namespace CepheusProjectWpf.Import_Export
{
	public class Export
	{
        /// <summary>
        /// Saves the text to a file.
        /// </summary>
        /// <param name="text"></param>
        public static void Save(string text)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, text);
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.CantWrite, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Describes the graph in a format from which the application is able to import it.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static string Print(Dictionary<EllipseVertex,string> vertices, Dictionary<ArrowEdge,string> edges)
        {
            var text = new StringBuilder();
            var rootDescriptor = new RootDescriptor();
            

            GetVertexDescriptor(rootDescriptor);
            text.Append("Vertices:\n");
            foreach (var vertex in vertices.Keys)
                text.Append(rootDescriptor.Serialize(vertex));

            GetEdgeDescriptor(rootDescriptor);
            text.Append("Edges:\n");
            foreach (var edge in edges.Keys)
                text.Append(rootDescriptor.Serialize(edge));

            text.Remove(text.Length - 1, 1); //last new line
            return text.ToString();
        }
        static void GetVertexDescriptor(RootDescriptor rootDescriptor)
        {
            rootDescriptor.Descriptor = rootDescriptor.DescriptorsByType[typeof(EllipseVertex)];
        }
        static void GetEdgeDescriptor(RootDescriptor rootDescriptor)
        {
            rootDescriptor.Descriptor = rootDescriptor.DescriptorsByType[typeof(ArrowEdge)];
        }

        
    }
}
