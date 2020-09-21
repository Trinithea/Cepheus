using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CepheusProjectWpf.GraphShapes
{
	public abstract class GraphShape : Shape
	{
		/// <summary>
		/// Deletes an edge or vertex from both the canvas and the graph.
		/// </summary>
		public abstract void Delete();
		/// <summary>
		/// Sets the color of the vertex or edge.
		/// </summary>
		/// <param name="color"></param>
		public abstract void SetStroke(SolidColorBrush color);
		/// <summary>
		/// Sets the shape color to default.
		/// </summary>
		public void SetDefaultLook() => SetStroke(MainWindow.DefaultColor);
		/// <summary>
		/// Sets the shape color to highlight.
		/// </summary>
		public void SetMarkedLook() => SetStroke(MainWindow.HiglightColor);
		/// <summary>
		/// Deselects the shape.
		/// </summary>
		public void Unmark()
		{
			SetDefaultLook();
			isMarked = false;
		}
		/// <summary>
		/// Specifies whether the shape is selected.
		/// </summary>
		public bool isMarked = false;

	
	}
}
