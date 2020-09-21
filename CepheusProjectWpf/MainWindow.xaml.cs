using Cepheus;
using CepheusProjectWpf.GraphShapes;
using CepheusProjectWpf.Import_Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

			// so that the cursor is after the last statement for each statement
			txtConsole.TextChanged += (o, e) =>
			{
				txtConsole.Focus();
				txtConsole.CaretIndex = txtConsole.Text.Length;
				txtConsole.ScrollToEnd();
			};

			// prints welcome and basic information to the console
			txtConsole.Text = CepheusProjectWpf.Properties.Resources.Welcome;
			txtConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.TroubleDel;


			DefaultColor = (SolidColorBrush)Application.Current.Resources["Aqua"];
			HiglightColor = (SolidColorBrush)Application.Current.Resources["Orange"];
		}

		/// <summary>
		/// Terminates the application so that the calculation within the algorithm also ends.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Application.Current.Shutdown();
		}
		/// <summary>
		/// All user-created vertices in the graph. Value is the name of the vertex.
		/// </summary>
		public static Dictionary<EllipseVertex, string> Vertices = new Dictionary<EllipseVertex, string>();
		/// <summary>
		/// All user-created edges in the graph. Value is the name of the edge.
		/// </summary>
		public static Dictionary<ArrowEdge, string> Edges = new Dictionary<ArrowEdge, string>();
		/// <summary>
		/// All graph vertices searchable by their unique ID.
		/// </summary>
		public static Dictionary<int, EllipseVertex> VerticesById = new Dictionary<int, EllipseVertex>();
		/// <summary>
		/// Initial vertex when running the algorithm.
		/// </summary>
		public static int? initialVertex = null;
		/// <summary>
		/// Sink vertex when running the flow algorithm.
		/// </summary>
		public static int? sinkVertex = null;
		/// <summary>
		/// ID generator, after adding a vertex, its value will increase.
		/// </summary>
		public static int IdCounter = 0;
		/// <summary>
		/// Default color of vertices and edges.
		/// </summary>
		public static SolidColorBrush DefaultColor { get; private set; }
		/// <summary>
		/// Highlight color of vertices and edges
		/// </summary>
		public static SolidColorBrush HiglightColor { get; private set; }
		/// <summary>
		/// List of all marked vertices and edges.
		/// </summary>
		public static List<GraphShape> Marked = new List<GraphShape>();
		/// <summary>
		/// Specifies whether the user clicks Run and the algorithm should run.
		/// </summary>
		public static bool AttemptToRun = false;
		/// <summary>
		/// Specifies whether the user-run algorithm is a maximum flow search algorithm.
		/// </summary>
		public static bool isFlowAlgorithm = false;
		/// <summary>
		/// It counts how many vertices the user has already marked so that the algorithm can start executing.
		/// </summary>
		public static int sourceSinkCounter = 0;
		/// <summary>
		/// If the user holds the left Ctrl and left-clicks on the canvas, it creates a vertex.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, IdCounter, null, graphCanvas, txtConsole);
				Vertices.Add(vertex, vertex.Name);
				VerticesById.Add(vertex.UniqueId, vertex);
				IdCounter++;
				vertex.KeepVertexInCanvas(Canvas.GetLeft(vertex.MainEllipse), Canvas.GetTop(vertex.MainEllipse));
			}
		}
		/// <summary>
		/// If the user presses the Delete key, all marked vertices and edges are deleted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				foreach (var shape in Marked)
				{
					shape.Delete();
					if (shape is EllipseVertex)
					{
						Vertices.Remove((EllipseVertex)shape);
						VerticesById.Remove(((EllipseVertex)shape).UniqueId);
					}
					else if (shape is ArrowEdge)
						Edges.Remove((ArrowEdge)shape);

				}

				Marked.Clear();
			}
		}
		/// <summary>
		/// Deletes the entire graph from the canvas and memory.
		/// </summary>
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
		/// <summary>
		/// Sets instances of the algorithms available in the application to the menu of algorithms that the user sees.
		/// </summary>
		void SetAvailbaleAlgorithms()
		{
			cmbAlgorithms.Items.Clear();
			List<Algorithm> algorithms = new List<Algorithm>() { new BFS(), new DFS(), new Dijkstra(), new Relaxation(), new BellmanFord(), new FloydWarshall(), new Jarnik(), new Boruvka(), new Kruskal(), new FordFulkerson(), new Dinic(), new Goldberg() };
			foreach (var algorithm in algorithms)
			{
				cmbAlgorithms.Items.Add(algorithm);
			}

		}
		/// <summary>
		/// Creates a graph suitable for running the algorithm from a graph drawn by the user using the Visitor Design Pattern.
		/// </summary>
		void StartCreating()
		{
			var visitor = new VisitorGraphCreator();
			DisableEverything();
			var algorithm = (Algorithm)cmbAlgorithms.SelectedItem;
			algorithm.Accept(visitor); //Create graph
		}
		/// <summary>
		/// Runs the algorithm selected by the user using the Visitor Design Pattern.
		/// </summary>
		/// <returns></returns>
		async Task StartRunning()
		{
			var visitor = new VisitorRunner();
			var algorithm = (Algorithm)cmbAlgorithms.SelectedItem;
			await algorithm.Accept(visitor); //Run

		}
		/// <summary>
		/// Prevents the user from editing the graph in any way.
		/// </summary>
		void DisableEverything()
		{
			graphCanvas.IsEnabled = false;
			imgClear.IsEnabled = false;
			btnClear.IsEnabled = false;
			SetNames();
		}

		/// <summary>
		/// Allows the user to edit the graph again.
		/// </summary>
		public void EnableEverything()
		{
			graphCanvas.IsEnabled = true;
			imgClear.IsEnabled = true;
			btnClear.IsEnabled = true;
		}

		/// <summary>
		/// The main method in which a graph is created for an algorithm and the selected algorithm runs.
		/// </summary>
		/// <returns></returns>
		async Task Run()
		{
			btnOkRun.Visibility = Visibility.Hidden;
			lblInfo.Visibility = Visibility.Hidden;
			txtConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.SelectedAlgoRunning;
			((Algorithm)cmbAlgorithms.SelectedItem).SetOutputConsole(txtConsole);
			StartCreating(); 
			await StartRunning(); 
		}
		/// <summary>
		/// Lightens the grid passed in the argument.
		/// </summary>
		/// <param name="uc"></param>
		void LightenGrid(Grid uc)
		{
			uc.Background = new SolidColorBrush(Color.FromRgb(44, 47, 68));
		}
		/// <summary>
		/// Saves the current name of each vertex in the Vertices dictionary.
		/// </summary>
		void SetNames()
		{
			foreach (var vertex in VerticesById.Values)
				Vertices[vertex] = vertex.Name;
		}
		/// <summary>
		/// Darkens the grid passed in the argument.
		/// </summary>
		/// <param name="uc"></param>
		void DarkenGrid(Grid uc)
		{
			uc.Background = new SolidColorBrush(Color.FromRgb(18, 19, 27));
		}
		/// <summary>
		/// Everything marked is unmarked.
		/// </summary>
		void UnmarkEverything()
		{
			foreach (GraphShape shape in Marked)
			{
				shape.Unmark();
			}
			Marked.Clear();
		}
		/// <summary>
		/// Checks if all edge lengths are non-negative.
		/// </summary>
		/// <returns></returns>
		bool CheckLengthsIfNonNegative()
		{
			try
			{
				foreach (var edge in Edges.Keys)
					if (edge.Length < 0)
						return false;
				return true;
			}
			catch (OverflowException)
			{
				MessageBox.Show(Properties.Resources.DistanceLongerMaxInt, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
		/// <summary>
		/// The user wants to run the algorithm. This method addresses whether an algorithm has been selected at all and, if so, 
		/// whether it has any special requirements to start at all. If it does not or the requirements are met, then the algorithm starts.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void gridRun_MouseUp(object sender, MouseButtonEventArgs e)
		{
			sourceSinkCounter = 0;
			lblInfo.Visibility = Visibility.Hidden;
			if (cmbAlgorithms.SelectedItem != null)
			{
				LightenGrid(gridRun);
				txtConsole.Text += "\n\n" + ((Algorithm)cmbAlgorithms.SelectedItem).Name + CepheusProjectWpf.Properties.Resources.AttemptToRun;
				if (((Algorithm)cmbAlgorithms.SelectedItem).NeedsOnlyNonNegativeEdgeLenghts)
				{
					if (!CheckLengthsIfNonNegative())
					{
						txtConsole.Text += "\n" + ((Algorithm)cmbAlgorithms.SelectedItem).Name + CepheusProjectWpf.Properties.Resources.DijkstraPosLengths;
						return;
					}
				}
				if (((Algorithm)cmbAlgorithms.SelectedItem).IsFlowAlgorithm)
					isFlowAlgorithm = true;
				if (isFlowAlgorithm)
					txtConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.SelectSource;
				else
				{
					if (!((Algorithm)cmbAlgorithms.SelectedItem).DontNeedInitialVertex)
						txtConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.SelectInit;
				}
				UnmarkEverything();
				if (!((Algorithm)cmbAlgorithms.SelectedItem).DontNeedInitialVertex)
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
				txtConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.ChooseAlgo;
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
		/// <summary>
		/// Sets the original appearance, name of the vertices and edges before running the algorithm.
		/// </summary>
		private void SetDefaultLookToEverything()
		{
			foreach (var vertex in Vertices)
			{
				vertex.Key.txtName.Text = vertex.Value;
				vertex.Key.SetDefaultLook();
			}
			foreach (var edge in Edges.Keys)
				edge.SetDefaultLook();
		}
		/// <summary>
		/// Darkens the image passed in the argument.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DarkenImage(object sender, MouseEventArgs e)
		{
			((Image)sender).Opacity = 0.5;
		}
		/// <summary>
		/// Lightens the image passed in the argument.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LightenImage(object sender, MouseEventArgs e)
		{
			((Image)sender).Opacity = 1;
		}
		/// <summary>
		/// Displays a description of the algorithm and its time complexity. If no algorithm is selected, it will write it instead.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imgInfo_MouseEnter(object sender, MouseEventArgs e)
		{
			DarkenImage(sender, e);
			gridInfo.Visibility = Visibility.Visible;
			string text;
			if (cmbAlgorithms.SelectedItem != null)
			{
				text = ((Algorithm)cmbAlgorithms.SelectedItem).Description;
				TxbTimComplexity.Visibility = Visibility.Visible;
				TxbTimComplexity.Text = CepheusProjectWpf.Properties.Resources.TimeComplexity + ((Algorithm)cmbAlgorithms.SelectedItem).TimeComplexity;
			}
			else
			{
				TxbTimComplexity.Visibility = Visibility.Hidden;
				text = CepheusProjectWpf.Properties.Resources.NonAlgoSelected;
			}

			TxbInfo.Text = text;
		}

		/// <summary>
		/// The algorithm description disappears.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imgInfo_MouseLeave(object sender, MouseEventArgs e)
		{
			LightenImage(sender, e);
			gridInfo.Visibility = Visibility.Hidden;
		}

		private void ImgHelp_MouseUp(object sender, MouseButtonEventArgs e)
		{

		}
		/// <summary>
		/// Sets the edges to their original length before running the algorithm (get rid of flow in "Flow/Length").
		/// </summary>
		void SetBackLengthOfEdges() 
		{
			foreach (var edge in Edges.Keys)
			{
				var value = edge.txtLength.Text.Split('/');
				edge.txtLength.Text = value[1];
			}
		}
		/// <summary>
		/// If the algorithm is to run but has not yet been run, it checks whether all the initial conditions have already been met and runs it. 
		/// If the algorithm has already run, it returns the graph to its original state before the algorithm ran.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btnOkRun_Click(object sender, RoutedEventArgs e)
		{
			if (AttemptToStop)
			{
				btnOkRun.Visibility = Visibility.Hidden;
				SetDefaultLookToEverything();
				EnableEverything();
				AttemptToStop = false;
				if (isFlowAlgorithm)
				{
					SetBackLengthOfEdges();
					isFlowAlgorithm = false;
				}
			}
			if (Marked.Count >= 1) //initial vertex is selected or sink & source
			{
				if (isFlowAlgorithm && sourceSinkCounter < 1)
				{
					sourceSinkCounter++;
					txtConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.SelectSink;
				}
				else if ((isFlowAlgorithm && sourceSinkCounter == 2) || isFlowAlgorithm == false)
				{
					await Execute();
				}

			}
		}
		/// <summary>
		/// Determines whether the algorithm has already run, but the user is not yet in edit mode.
		/// </summary>
		bool AttemptToStop = false;
		/// <summary>
		/// Starts the algorithm. Once it's done, it prepares everything for the user to continue editing.
		/// </summary>
		/// <returns></returns>
		private async Task Execute()
		{
			try
			{
				await Run();
			}
			catch (OverflowException)
			{
				MessageBox.Show(Properties.Resources.DistanceLongerMaxInt, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch
			{
				MessageBox.Show(Properties.Resources.CantFinish, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			DarkenGrid(gridRun);
			txtConsole.Text += "\n\n" + ((Algorithm)cmbAlgorithms.SelectedItem).Name + CepheusProjectWpf.Properties.Resources.HasFinished;
			AttemptToRun = false;
			AttemptToStop = true;
			btnOkRun.Visibility = Visibility.Visible;
			txtConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.Continue;
			UnmarkEverything();
			sourceSinkCounter = 0;
		}
		private void ImgHelp_MouseEnter(object sender, MouseEventArgs e)
		{
			gridTutorial.Visibility = Visibility.Visible;
		}
		private void ImgHelp_MouseLeave(object sender, MouseEventArgs e)
		{
			gridTutorial.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Prints the contents of a canvas whose background is changed to white for printing so that black is not wasted when printed. 
		/// The vertex name, if white, changes to black.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Changes the colors of vertices, edges and their textboxes so that they are visible on a white background.
		/// </summary>
		/// <param name="canvasBackground"></param>
		/// <param name="dangerousColor"></param>
		/// <param name="newColor"></param>
		private void ChangeCanvasLook(Brush canvasBackground, Brush dangerousColor, Brush newColor)
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
		/// <summary>
		/// All marked shapes change color to a new highlight color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// All unmarked shapes change color to a new default color.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Saves the user-created graph to a text file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imgSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Export.Save(Export.Print(Vertices, Edges));
		}
		/// <summary>
		/// Reads a graph from a file and draws it on the canvas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imgOpen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.Upload(graphCanvas, txtConsole);
		}
		/// <summary>
		/// Sets the application language to Czech.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Default application language (English)
		/// </summary>
		System.Globalization.CultureInfo defaultCultureInfo;
		/// <summary>
		/// Specifies whether the application language has changed.
		/// </summary>
		bool changedCulture = false;
		/// <summary>
		/// Changes the application language to the default English.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imgEnglish_MouseUp(object sender, MouseButtonEventArgs e)
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = defaultCultureInfo;
			UpdateControls();
		}
		/// <summary>
		/// Adjusts the string properties of controls according to the selected application language.
		/// </summary>
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
			Import.ReadFile(new StringReader(Properties.Resources._5_5_Prasatko), graphCanvas, txtConsole);
		}

		private void img14_2_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFile(new StringReader(Properties.Resources._14_2_Toky_v_sitich), graphCanvas, txtConsole);
		}

		private void img7_2_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFile(new StringReader(Properties.Resources._7_2_Minimalni_kostry), graphCanvas, txtConsole);
		}

		private void img6_1_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFile(new StringReader(Properties.Resources._6_1_Stredovy_graf), graphCanvas, txtConsole);
		}

		private void img5_13_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFile(new StringReader(Properties.Resources._5_13_Grafove_komponenty), graphCanvas, txtConsole);
		}

		private void img5_10_MouseUp(object sender, MouseButtonEventArgs e)
		{
			ClearCanvas();
			Import.ReadFile(new StringReader(Properties.Resources._5_10_Mosty_a_artikulace), graphCanvas, txtConsole);
		}
	}
}
