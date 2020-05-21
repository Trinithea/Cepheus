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
		public bool correct = false;
		public InitialVertexWindow()
		{
			InitializeComponent();
		}
		public int? initialVertexId = null;
		bool IsNameUnique(string name) //TODO bleh time
		{
			int i = 0;
			foreach(var vertex in MainWindow.Vertices.Keys)
			{
				if (i==0 && vertex.Name == name)
				{
					i++;
					initialVertexId = vertex.UniqueId;
				}
				else if (i==1 && vertex.Name == name)
				{
					return false;
				}
			}
			if (i == 0)
				return false;
			else
				return true;
		}
		private void btnConfirm_Click(object sender, RoutedEventArgs e)
		{
			if (txtInitialVertex.Text != "")
			{
				if (IsNameUnique(txtInitialVertex.Text))
				{
					correct = true;
					this.Close();
				}
				else if (initialVertexId == null)
				{
					lblErrorMessage.Content = "This name of vertex doesn't exist.";
					lblErrorMessage.Visibility = Visibility.Visible;
				}
				else
				{
					lblErrorMessage.Content = "Name of initial vertex must be unique.";
					lblErrorMessage.Visibility = Visibility.Visible;
				}
			}
			else
			{
				lblErrorMessage.Content = "Name can't be empty.";
				lblErrorMessage.Visibility = Visibility.Visible;
			}
			
		}
	}
}
