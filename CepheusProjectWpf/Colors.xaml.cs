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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CepheusProjectWpf
{
	/// <summary>
	/// Interaction logic for Colors.xaml
	/// </summary>
	public partial class ColorsUserControl : UserControl
	{
		public SolidColorBrush SelectedColor { get; private set; }
		public ColorsUserControl()
		{
			InitializeComponent();
			foreach(var rectangle in mainGrid.Children)
			{
				((Rectangle)rectangle).MouseEnter += rectangle_MouseEnter;
				((Rectangle)rectangle).MouseLeave += rectangle_MouseLeave;
				((Rectangle)rectangle).MouseUp += rectangle_MouseUp;
			}
		}
		private void rectangle_MouseEnter(object sender, MouseEventArgs e)
		{
			SetStroke((Rectangle)sender, Brushes.White);
		}
		private void rectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			SetStroke((Rectangle)sender, Brushes.Transparent);
		}
		private void rectangle_MouseUp(object sender, MouseEventArgs e)
		{
			SelectedColor = (SolidColorBrush)((Rectangle)sender).Fill;
		}

		private void SetStroke(Rectangle rectangle, SolidColorBrush color)
		{
			rectangle.Stroke = color;
		}
		
	}
}
