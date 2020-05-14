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
	/// Interaction logic for Properties.xaml
	/// </summary>
	public partial class PropertiesUserControl : UserControl
	{
		public PropertiesUserControl()
		{
			InitializeComponent();
		}
		List<string> Names = new List<string>(); //List should be good enough because I don't expect milions of vertices in canvas

		void CheckIfNameDoesntAlreadyExist(string name)
		{
			if (Names.Contains(name))
				MessageBox.Show("This name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else
				Names.Add(name);
		}
		public string GetName() => txtName.Text;
		private void btnConfirm_Click(object sender, RoutedEventArgs e)
		{
			CheckIfNameDoesntAlreadyExist(txtName.Text);

		}
	}
}
