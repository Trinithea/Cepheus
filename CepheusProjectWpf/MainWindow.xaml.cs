using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cepheus;
using CepheusProjectWpf.GraphShapes;
using CepheusProjectWpf.Import_Export;

namespace CepheusProjectWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");
			InitializeComponent();
			UpdateControls();
			txtConsole.Text = CepheusProjectWpf.Properties.Resources.Welcome;
			txtConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.TroubleDel;
			DefaultColor = (SolidColorBrush)Application.Current.Resources["Aqua"];
			HiglightColor = (SolidColorBrush)Application.Current.Resources["Orange"];
			ellipseHighlightColor = imgHighlightColor;
			ellipseDefaultColor = imgDefaultColor;
		}
		public static Dictionary<EllipseVertex, string> Vertices = new Dictionary< EllipseVertex, string>(); 
		public static Dictionary<ArrowEdge,string> Edges = new Dictionary< ArrowEdge, string>();
		public static Dictionary<int, EllipseVertex> VerticesById = new Dictionary<int, EllipseVertex>();
		public static int? initialVertex = null;
		public static int? sinkVertex = null;
		public static int IdCounter=0;
		private static Ellipse ellipseHighlightColor;
		private static Ellipse ellipseDefaultColor;
		
		public static SolidColorBrush DefaultColor { get; private set; }
		
		public static SolidColorBrush HiglightColor { get; private set; }

		public static List<GraphShape> Marked = new List<GraphShape>();
		public static bool AttemptToRun = false;
		public static bool isFlowAlgorithm = false;
		List<Algorithm> flowAlgorithms = new List<Algorithm>();
		List<Algorithm> onlyPositiveEdgesAlgorithms = new List<Algorithm>();
		List<Algorithm> dontNeedInitialVertexAlgorithms = new List<Algorithm>();
		public static int sourceSinkCounter = 0;
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, IdCounter, null, graphCanvas,txtConsole);
				Vertices.Add(vertex, vertex.Name);
				VerticesById.Add(vertex.UniqueId, vertex);
				IdCounter++;
				vertex.KeepVertexInCanvas(Canvas.GetLeft(vertex.MainEllipse), Canvas.GetTop(vertex.MainEllipse));
			}
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				foreach (var shape in Marked)
				{
					shape.Delete();
					if(shape is EllipseVertex)
					{
						Vertices.Remove((EllipseVertex)shape);
						VerticesById.Remove(((EllipseVertex)shape).UniqueId);
					}
					else if(shape is ArrowEdge)
						Edges.Remove((ArrowEdge)shape);
					
				}
					
				Marked.Clear();
			}
		}
		void ClearCanvas()
		{
			graphCanvas.Children.Clear();
			Vertices.Clear();
			VerticesById.Clear();
			Edges.Clear();
			IdCounter = 0;
		}

		private void imgClear_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
		}

		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			ClearCanvas();
		}
		//Dictionary<string, Algorithm> availbaleAlgorithms = new Dictionary<string, Algorithm>();
		void SetAvailbaleAlgorithms()
		{
			cmbAlgorithms.Items.Clear();
			List<Algorithm> algorithms = new List<Algorithm>() { new BFS(), new DFS(), new Dijkstra(), new Relaxation(), new BellmanFord(), new FloydWarshall(), new Jarnik(), new Boruvka(), new Kruskal(), new FordFulkerson(), new Dinic(),  new Goldberg()  };
			foreach (var algorithm in algorithms)
			{
				cmbAlgorithms.Items.Add(algorithm);
				if (algorithm is Dinic || algorithm is FordFulkerson || algorithm is Goldberg)
					flowAlgorithms.Add(algorithm);
				if (algorithm is Dijkstra)
					onlyPositiveEdgesAlgorithms.Add(algorithm);
				if (algorithm is Kruskal || algorithm is Boruvka || algorithm is FloydWarshall)
					dontNeedInitialVertexAlgorithms.Add(algorithm);
			}
			
		}
		
		void StartCreating()
		{
			var visitor = new VisitorGraphCreator();
			DisableEverything();
			var algorithm = (Algorithm)cmbAlgorithms.SelectedItem;
			algorithm.Accept(visitor); //Create graph
		}
		async Task StartRunning()
		{
			var visitor = new VisitorRunner();
			var algorithm = (Algorithm)cmbAlgorithms.SelectedItem;
			await algorithm.Accept(visitor); //Run

			
		}
		void DisableEverything()
		{
			graphCanvas.IsEnabled = false;
			imgClear.IsEnabled = false;
			btnClear.IsEnabled = false;
			SetNames();
		}
		public void EnableEverything() //TODO spouštět v jinym vlákně
		{
			graphCanvas.IsEnabled = true;
			imgClear.IsEnabled = true;
			btnClear.IsEnabled = true;
		}

		async Task Run()
		{
			btnOkRun.Visibility = Visibility.Hidden;
			lblInfo.Visibility = Visibility.Hidden;
			txtConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.SelectedAlgoRunning;
			((Algorithm)cmbAlgorithms.SelectedItem).SetOutputConsole(txtConsole);
			StartCreating(); //tady se disabluje
			await StartRunning(); //tady se spustí někdy metoda async void Run()
			
		}
		void LightenGrid(Grid uc)
		{
			uc.Background = new SolidColorBrush(Color.FromRgb(44, 47, 68));
		}
		void SetNames()
		{
			foreach (var vertex in VerticesById.Values)
				Vertices[vertex] = vertex.Name;
		}

		void DarkenGrid(Grid uc)
		{
			uc.Background = new SolidColorBrush(Color.FromRgb(18, 19, 27));
		}
		
		void UnmarkEverything()
		{
			foreach (GraphShape shape in Marked)
			{
				shape.Unmark();
			}
			Marked.Clear();
		}
		bool CheckLengthsIfPositive()
		{
			foreach (var edge in Edges.Keys)
				if (edge.Length < 0)
					return false;
			return true;
		}
		private async void gridRun_MouseUp(object sender, MouseButtonEventArgs e)
		{
			lblInfo.Visibility = Visibility.Hidden;
			if(cmbAlgorithms.SelectedItem != null)
			{
				LightenGrid(gridRun);
				txtConsole.Text += "\n\n"+((Algorithm)cmbAlgorithms.SelectedItem).Name + CepheusProjectWpf.Properties.Resources.AttemptToRun;
				if(onlyPositiveEdgesAlgorithms.Contains(cmbAlgorithms.SelectedItem))
				{
					if (!CheckLengthsIfPositive())
					{
						txtConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.DijkstraPosLengths;
						return;
					}
				}
				if (flowAlgorithms.Contains(((Algorithm)cmbAlgorithms.SelectedItem)))
					isFlowAlgorithm = true;
				if (isFlowAlgorithm)
					txtConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.SelectSource;
				else
				{
					if(!dontNeedInitialVertexAlgorithms.Contains(cmbAlgorithms.SelectedItem))
						txtConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.SelectInit;
				}
				UnmarkEverything();
				if (!dontNeedInitialVertexAlgorithms.Contains(cmbAlgorithms.SelectedItem))
				{
					lblInfo.Visibility = Visibility.Visible;
					btnOkRun.Visibility = Visibility.Visible;
					AttemptToRun = true;
				}
				else
					await Execute();
			}
			else
			{
				txtConsole.Text += "\n\n"+ CepheusProjectWpf.Properties.Resources.ChooseAlgo;
				lblInfo.Visibility = Visibility.Visible;
			}
		}

		private void gridRun_MouseEnter(object sender, MouseEventArgs e)
		{
			LightenGrid(gridRun);
		}

		private void gridRun_MouseLeave(object sender, MouseEventArgs e)
		{
			DarkenGrid(gridRun);
		}

		

		private void DarkenImage(object sender, MouseEventArgs e)
		{
			((Image)sender).Opacity =0.5;
		}

		private void LightenImage(object sender, MouseEventArgs e)
		{
			((Image)sender).Opacity =1;
		}

		private void imgInfo_MouseEnter(object sender, MouseEventArgs e)
		{
			DarkenImage(sender,e);
			gridInfo.Visibility = Visibility.Visible;
			string text;
			if (cmbAlgorithms.SelectedItem != null)
			{
				text = ((Algorithm)cmbAlgorithms.SelectedItem).Description;
				TxbTimComplexity.Visibility = Visibility.Visible;
				TxbTimComplexity.Text = CepheusProjectWpf.Properties.Resources.TimeComplexity+ ((Algorithm)cmbAlgorithms.SelectedItem).TimeComplexity;
			}
			else
			{
				TxbTimComplexity.Visibility = Visibility.Hidden;
				text = CepheusProjectWpf.Properties.Resources.NonAlgoSelected;
			}

			TxbInfo.Text = text;
		}
		

		private void imgInfo_MouseLeave(object sender, MouseEventArgs e)
		{
			LightenImage(sender, e);
			gridInfo.Visibility = Visibility.Hidden;
		}

		private void ImgHelp_MouseUp(object sender, MouseButtonEventArgs e)
		{
			
		}
		void SetBackLengthOfEdges() // get rid of flow in "Flow/Length"
		{
			foreach(var edge in Edges.Keys)
			{
				var value = edge.txtLength.Text.Split('/');
				edge.txtLength.Text = value[1];
			}
		}
		void SetBackNamesOfVertices()
		{
			foreach(var vertex in Vertices)
			{
				vertex.Key.txtName.Text = vertex.Value; 
			}
		}
		private async void btnOkRun_Click(object sender, RoutedEventArgs e)
		{
			if(Marked.Count >= 1) //initial vertex is selected or sink & source
			{
				if (isFlowAlgorithm && sourceSinkCounter < 1)
				{
					sourceSinkCounter++;
					if (isFlowAlgorithm)
						txtConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.SelectSink;
				}
				else
				{
					await Execute();
				}
			}		
			else if (isFlowAlgorithm && AttemptToRun == false)
			{
				SetBackLengthOfEdges();
				if (cmbAlgorithms.SelectedItem is Goldberg)
					SetBackNamesOfVertices();
				EnableEverything();
				isFlowAlgorithm = false;
				btnOkRun.Visibility = Visibility.Hidden;
				
			}
		}
		private async Task Execute()
		{
			await Run();
			DarkenGrid(gridRun);
			txtConsole.Text += "\n\n" + ((Algorithm)cmbAlgorithms.SelectedItem).Name + CepheusProjectWpf.Properties.Resources.HasFinished;
			AttemptToRun = false;
			if (isFlowAlgorithm)
			{
				btnOkRun.Visibility = Visibility.Visible;
				txtConsole.Text += "\n"+ CepheusProjectWpf.Properties.Resources.Continue;
			}
			else
				EnableEverything();
			foreach (var marked in Marked) // can be one or two marked
				marked.Unmark();
			
			Marked.Clear();
			sourceSinkCounter = 0;
			
		}
		private void ImgHelp_MouseEnter(object sender, MouseEventArgs e)
		{
			imgTutorial.Visibility = Visibility.Visible;
		}
		private void ImgHelp_MouseLeave(object sender, MouseEventArgs e)
		{
			imgTutorial.Visibility = Visibility.Hidden;
		}
		private void imgPrint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try
			{
				PrintDialog dialog = new PrintDialog();
				var brush = graphCanvas.Background;
				ChangeCanvasLook(Brushes.White, Brushes.White, Brushes.Black);
				if (dialog.ShowDialog() != true)
				{
					ChangeCanvasLook((SolidColorBrush)Application.Current.Resources["Dark"], Brushes.Black, Brushes.White);
					return;
				}
				dialog.PrintVisual(graphCanvas, "Graph canvas");
				ChangeCanvasLook((SolidColorBrush)Application.Current.Resources["Dark"], Brushes.Black, Brushes.White);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, CepheusProjectWpf.Properties.Resources.Mistake, MessageBoxButton.OK, MessageBoxImage.Error);
				ChangeCanvasLook((SolidColorBrush)Application.Current.Resources["Dark"], Brushes.Black, Brushes.White);
			}
		}
		private void ChangeCanvasLook(Brush canvasBackground, Brush dangerousColor,Brush newColor)
		{
			graphCanvas.Background = canvasBackground;
			foreach (var vertex in Vertices)
			{
				if (vertex.Key.txtName.Foreground == dangerousColor)
					vertex.Key.txtName.Foreground = newColor;
			}
			foreach (var edge in Edges)
			{
				if (edge.Key.txtLength.Foreground == dangerousColor)
					edge.Key.txtLength.Foreground = newColor;
			}
		}

		private void imgHighlightColor_MouseEnter(object sender, MouseEventArgs e)
		{
			menuDefaultColors.Visibility = Visibility.Hidden;
			menuHiglightColors.Visibility = Visibility.Visible;
		}

		private void imgDefaultColor_MouseEnter(object sender, MouseEventArgs e)
		{
			menuHiglightColors.Visibility = Visibility.Hidden;
			menuDefaultColors.Visibility = Visibility.Visible;
		}

		private void menuHiglightColors_MouseUp(object sender, MouseButtonEventArgs e)
		{
			HiglightColor = menuHiglightColors.SelectedColor;
			imgHighlightColor.Fill = HiglightColor;
			foreach (var graphShape in Marked)
				graphShape.SetMarkedLook();
		}

		private void menuHiglightColors_MouseLeave(object sender, MouseEventArgs e)
		{
			menuHiglightColors.Visibility = Visibility.Hidden;
		}

		private void menuDefaultColors_MouseLeave(object sender, MouseEventArgs e)
		{
			menuDefaultColors.Visibility = Visibility.Hidden;
		}

		private void menuDefaultColors_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DefaultColor = menuDefaultColors.SelectedColor;
			imgDefaultColor.Fill = DefaultColor;
			foreach (var vertex in Vertices.Keys)
				if (vertex.MainEllipse.Stroke != HiglightColor)
					vertex.SetDefaultLook();
			foreach (var edge in Edges.Keys)
				if (edge.MainLine.Stroke != HiglightColor)
					edge.SetDefaultLook();
		}

		private void imgSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Export.Save(Export.Print(Vertices, Edges));
		}

		private void imgOpen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.Upload(graphCanvas, txtConsole);
		}

		private void imgCzech_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!changedCulture)
			{
				defaultCultureInfo = Thread.CurrentThread.CurrentUICulture;
				changedCulture = true;
			}
			System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");
			UpdateControls();
		}
		System.Globalization.CultureInfo defaultCultureInfo;
		bool changedCulture = false;
		private void imgEnglish_MouseUp(object sender, MouseButtonEventArgs e)
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = defaultCultureInfo;
			UpdateControls();
		}

		void UpdateControls()
		{
			SetAvailbaleAlgorithms(); 
			btnClear.Content = Properties.Resources.ClearCanvas_Content;
			btnOkRun.Content = Properties.Resources.Done;
			lblRun.Content = Properties.Resources.Run;
			imgDefaultColor.ToolTip = Properties.Resources.DefaultColor;
			imgHighlightColor.ToolTip = Properties.Resources.HighlightColor;
			imgCursor.ToolTip = Properties.Resources.Cursor;
			imgCan.ToolTip = Properties.Resources.FillColor;
			imgZoomIn.ToolTip = Properties.Resources.ZoomIn;
			imgZoomOut.ToolTip = Properties.Resources.ZoomOut;
			imgFamousGraphs.ToolTip = Properties.Resources.CommonGraphs;
			imgSave.ToolTip = Properties.Resources.Save;
			imgOpen.ToolTip = Properties.Resources.Import;
			imgPrint.ToolTip = Properties.Resources.Print;
			ImgHelp.ToolTip = Properties.Resources.Tutorial;
			imgAboutCepheus.ToolTip = Properties.Resources.About;
			imgCzech.ToolTip = Properties.Resources.Czech;
			imgEnglish.ToolTip = Properties.Resources.English;
			imgTutorial.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.imgTutorial.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			lblAbout.Text = Properties.Resources.AboutLong;
		}

		private void imgCzech_MouseEnter(object sender, MouseEventArgs e)
		{
			DarkenImage(sender, e);
		}

		private void imgCzech_MouseLeave(object sender, MouseEventArgs e)
		{
			LightenImage(sender, e);
		}

		private void imgEnglish_MouseEnter(object sender, MouseEventArgs e)
		{
			DarkenImage(sender, e);
		}

		private void imgEnglish_MouseLeave(object sender, MouseEventArgs e)
		{
			LightenImage(sender, e);
		}

		private void imgAboutCepheus_MouseEnter(object sender, MouseEventArgs e)
		{
			gridAbout.Visibility = Visibility.Visible;
		}

		private void imgAboutCepheus_MouseLeave(object sender, MouseEventArgs e)
		{
			gridAbout.Visibility = Visibility.Hidden;
		}

		private void imgFamousGraphs_MouseEnter(object sender, MouseEventArgs e)
		{
			DarkenImage(sender, e);
			gridCommonGraphs.Visibility = Visibility.Visible;
		}

		private void gridCommonGraphs_MouseLeave(object sender, MouseEventArgs e)
		{
			gridCommonGraphs.Visibility = Visibility.Hidden;
		}

		private void img5_5_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._5_5_Prasatko, graphCanvas,txtConsole);
		}

		private void img14_2_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._14_2_Toky_v_sitich, graphCanvas, txtConsole);
		}

		private void img7_2_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._7_2_Minimalni_kostry, graphCanvas, txtConsole);
		}

		private void img6_1_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._6_1_Stredovy_graf, graphCanvas, txtConsole);
		}

		private void img5_13_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._5_13_Grafove_komponenty, graphCanvas, txtConsole);
		}

		private void img5_10_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFileFromText(Properties.Resources._5_10_Mosty_a_artikulace, graphCanvas, txtConsole);
		}
	}
}
