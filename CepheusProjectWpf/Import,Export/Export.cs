using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf.GraphShapes;

namespace CepheusProjectWpf.Import_Export
{
	public class Export
	{
        public static void Save(string text)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, text);
        }
        public static string Print(Dictionary<EllipseVertex,string> vertices, Dictionary<ArrowEdge,string> edges)
        {
            var text = new StringBuilder();
            text.Append("Vertices:\n");
            foreach (var vertex in vertices.Keys)
                text.Append(vertex.Description);
            text.Append("Edges:\n");
            foreach (var edge in edges.Keys)
                text.Append(edge.Description);
            text.Remove(text.Length - 1, 1); //last new line
            return text.ToString();
        }
	}
}
