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

namespace CepheusProjectWpf.UIWindows
{
	/// <summary>
	/// Interaction logic for ErrorNonIntegerLengthWindow.xaml
	/// </summary>
	public partial class ErrorNonIntegerLengthWindow : Window
	{
		public ErrorNonIntegerLengthWindow()
		{
			InitializeComponent();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
