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
	/// Interaction logic for WarningUniqueNameOfInitialVertex.xaml
	/// </summary>
	public partial class WarningUniqueNameOfInitialVertex : Window
	{
		
		public WarningUniqueNameOfInitialVertex()
		{
			InitializeComponent();
		}
		public bool correct = false;
		private void btnCorrect_Click(object sender, RoutedEventArgs e)
		{
			correct = true;
			this.Close();
		}

		private void btn_Click(object sender, RoutedEventArgs e)
		{
			correct = false;
			this.Close();
		}
	}
}
