using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CepheusProjectWpf.GraphShapes;

namespace CepheusProjectWpf.UIWindows__remove__
{
	public partial class NetOfReservesWindow : Window
	{
		public Canvas NetCanvas;
		public static Dictionary<int, EllipseVertex> VerticesById = new Dictionary<int, EllipseVertex>();
		public NetOfReservesWindow()
		{
			InitializeComponent();
			NetCanvas = netCanvas;
		}
	}
}
