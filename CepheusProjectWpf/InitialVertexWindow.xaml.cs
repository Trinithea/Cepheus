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

namespace CepheusProjectWpf
{
	/// <summary>
	/// Interaction logic for InitialVertexWindow.xaml
	/// </summary>
	public partial class InitialVertexWindow : Window
	{
		public InitialVertexWindow()
		{
			InitializeComponent();
		}
		bool IsNameUnique(string name) //TODO bleh time
		{
			List<string> usedNames = new List<string>();
			foreach(var vertexName in MainWindow.Vertices.Values)
			{
				if (usedNames.Contains(name))
					return false;
				usedNames.Add(vertexName);
			}
			return true;
		}
		private void btnConfirm_Click(object sender, RoutedEventArgs e)
		{
			if (txtInitialVertex.Text != "")
				this.Close();
			else
			{
				lblErrorMessage.Content = "Name can't be empty.";
				lblErrorMessage.Visibility = Visibility.Visible;
			}

			if (IsNameUnique(txtInitialVertex.Text))
			{
				if (MainWindow.Vertices.ContainsValue(txtInitialVertex.Text))
					this.Close();
				else
				{
					lblErrorMessage.Content = "This name of vertex doesn't exist.";
					lblErrorMessage.Visibility = Visibility.Visible;
				}
			}
			else
			{
				lblErrorMessage.Content = "Name of initial vertex must be unique.";
				lblErrorMessage.Visibility = Visibility.Visible;
			}
				
			
		}
	}
}
