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
		public abstract void Delete();
		public abstract void SetStroke(SolidColorBrush color);
		public void SetDefaultLook() => SetStroke(MainWindow.DefaultColor);
		public void SetMarkedLook() => SetStroke(MainWindow.HiglightColor);
		public void Unmark()
		{
			SetDefaultLook();
			isMarked = false;
		}
		public bool isMarked = false;

	
	}
}
