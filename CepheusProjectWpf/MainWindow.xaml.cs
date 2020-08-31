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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cepheus;

namespace CepheusProjectWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			SetAvailbaleAlgorithms();
			txtConsole.Text = "Welcome to Cepheus. Feel free to create any graphs you want and experiment with the prepared algorithms. If you're using this app for the first time, there's a tutorial made right for you in the upper right corner.";
			txtConsole.Text += "\nIf you have troubles with deleting vertices or edges, try to press Tab so names and lengths will lost focus."; //TODO write this properly!!
		}
		public static Dictionary<EllipseVertex, string> Vertices = new Dictionary< EllipseVertex, string>();
		public static Dictionary<ArrowEdge,string> Edges = new Dictionary< ArrowEdge, string>();
		public static int? initialVertex = null;
		public static int? sinkVertex = null;
		static int idCounter=0;
		public static string DefaultColor = "Aqua";
		public static string HiglightColor = "Orange";
		public static List<GraphShape> Marked = new List<GraphShape>();
		public static bool AttemptToRun = false;
		public static bool isFlowAlgorithm = false;
		List<Algorithm> flowAlgorithms = new List<Algorithm>();
		public static int sourceSinkCounter = 0;
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, graphCanvas,txtConsole);
				vertex.KeepVertexInCanvas(Canvas.GetLeft(vertex.MainEllipse), Canvas.GetTop(vertex.MainEllipse));
			}
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			List<ArrowEdge> edgesToRemove = new List<ArrowEdge>();
			List<EllipseVertex> verticesToRemove = new List<EllipseVertex>();
			if (e.Key == Key.Delete)
			{
				foreach (var shape in Marked)
					shape.Delete(); //TODO - create MyShape with abstract method Delete 
				Marked.Clear();
			}
			/*
			foreach (var edge in edgesToRemove)
				edge.Delete();
			foreach (var vertex in verticesToRemove)
				vertex.Delete();*/
		}
		void ClearCanvas()
		{
			graphCanvas.Children.Clear();
			Vertices.Clear();
			Edges.Clear();
		}

		public abstract class GraphShape : Shape
		{
			public abstract void Delete();
			public abstract void SetStroke(string color);
			public void SetDefaultLook() => SetStroke(DefaultColor);
			public void SetMarkedLook() => SetStroke(HiglightColor);
			public void Unmark()
			{
				SetDefaultLook();
				isMarked = false;
			}
			public bool isMarked = false;
		}
		public class EllipseVertex : GraphShape
		{
			public int UniqueId;
			private bool isDraggingVertex = false;
			bool wasMoving = false;
			protected override Geometry DefiningGeometry { get; }
			public Ellipse MainEllipse { get; private set; }
			public List<ArrowEdge> OutEdges = new List<ArrowEdge>();
			public List<ArrowEdge> InEdges = new List<ArrowEdge>();
			Canvas GraphCanvas;
			public TextBox txtName;
			TextBox outputConsole;
			public new string Name => txtName.Text;
			public EllipseVertex(Point mousePos, Canvas graphCanvas, TextBox console)
			{
				GraphCanvas = graphCanvas;
				CreateVertexEllipse(mousePos);
				outputConsole = console;
			}
			private Ellipse CreateVertexEllipse(Point mousePos)
			{
				Ellipse newVertex = new Ellipse();

				SolidColorBrush mySolidColorBrush = new SolidColorBrush();

				mySolidColorBrush.Color = Colors.White;
				newVertex.Fill = mySolidColorBrush;
				newVertex.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"]; ;
				newVertex.StrokeThickness = 4;

				newVertex.Width = 20;
				newVertex.Height = 20;
				Canvas.SetLeft(newVertex, mousePos.X - newVertex.Width / 2);
				Canvas.SetTop(newVertex, mousePos.Y - newVertex.Height / 2);
				Canvas.SetZIndex(newVertex, 2);
				newVertex.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
				newVertex.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
				newVertex.MouseMove += Ellipse_MouseMove;
				newVertex.MouseEnter += Ellipse_MouseEnter;
				newVertex.MouseLeave += Ellipse_MouseLeave;
				newVertex.MouseRightButtonDown += Ellipse_MouseRightButtonDown;

				GraphCanvas.Children.Add(newVertex);
				

				MainEllipse = newVertex;

				txtName = new TextBox();
				UniqueId = idCounter;
				SetNameTextBox(Canvas.GetLeft(newVertex),Canvas.GetTop(newVertex));
				Vertices.Add(this, Name);
				idCounter++;
				return newVertex;
			}

			void SetNameTextBox(double left, double top)
			{
				txtName.Background = Brushes.Transparent;
				txtName.BorderBrush = Brushes.Transparent;
				txtName.Foreground = Brushes.White;
				txtName.Height = 23;
				txtName.Text = "Vertex #"+UniqueId;
				Canvas.SetLeft(txtName, left);
				if (top - txtName.Height < 0)
					Canvas.SetTop(txtName, top + MainEllipse.Height ) ;
				else
					Canvas.SetTop(txtName, top - txtName.Height);
				GraphCanvas.Children.Add(txtName);
			}
			void SetNameCoordinates(double left, double top)
			{
				if (left + txtName.ActualWidth > GraphCanvas.ActualWidth)
					Canvas.SetLeft(txtName, left - (left + txtName.ActualWidth - GraphCanvas.ActualWidth));
				else
					Canvas.SetLeft(txtName, left);

				if (top - txtName.Height < 0)
					Canvas.SetTop(txtName, top + MainEllipse.Height );
				else
					Canvas.SetTop(txtName, top - txtName.Height);
			}
			#region MouseActions
			public override void SetStroke(string color)
			{
				MainEllipse.Stroke = (SolidColorBrush)Application.Current.Resources[color];
				if(color == DefaultColor)
					txtName.Foreground = Brushes.White;
				else
					txtName.Foreground = (SolidColorBrush)Application.Current.Resources[color];
			}
			private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
			{
				if (!isMarked)
				{
					SetDefaultLook();
				}
					
			}

			private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
			{
				SetMarkedLook();
			}

			private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
			{
				ArrowEdge arrow = new ArrowEdge(GraphCanvas, this,outputConsole);
				
				OutEdges.Add(arrow);
			}
			private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				isDraggingVertex = true;
				var draggableControl = (Shape)sender;
				var clickPosition = e.GetPosition(GraphCanvas);
				draggableControl.CaptureMouse();
			}
			private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
			{
				
				var draggable = (Shape)sender;
				draggable.ReleaseMouseCapture();
				if (isMarked || wasMoving)
				{
					Unmark();
					Marked.Remove(this); //TODO nic moc...
				}
				else 
				{
					SetMarkedLook();
					isMarked = true;
					Marked.Add(this);
					if (AttemptToRun)
					{
						if(!isFlowAlgorithm && Marked.Count>1)
						{
							Marked[0].Unmark();
							Marked.RemoveAt(0);
						}
						if (isFlowAlgorithm && sourceSinkCounter == 1)
							sinkVertex = UniqueId;
						else
						{
							initialVertex = UniqueId;
							if (isFlowAlgorithm)
								outputConsole.Text += "\nSelect the sink vertex and press Done again.";
						}
							
					}
				}
				wasMoving = false;
				isDraggingVertex = false;
			}
			private void Ellipse_MouseMove(object sender, MouseEventArgs e)
			{
				var draggableControl = (Shape)sender;
				var mousePos = e.GetPosition(GraphCanvas);
				
				if (isDraggingVertex && draggableControl != null)
				{
					double left = mousePos.X - (((Shape)sender).Width / 2);
					double top = mousePos.Y - (((Shape)sender).Height / 2);
					KeepVertexInCanvas(left, top);
					MoveWithOutEdges(Canvas.GetLeft(MainEllipse) + MainEllipse.Width / 2, Canvas.GetTop(MainEllipse) + MainEllipse.Height / 2);
					MoveWithInEdges();
					SetNameCoordinates(Canvas.GetLeft(MainEllipse), Canvas.GetTop(MainEllipse));
					wasMoving = true;
				}
			}
			#endregion
			void MoveWithOutEdges(double x, double y)
			{
				foreach (ArrowEdge edge in OutEdges)
				{
					edge.MainLine.X1 = x;
					edge.MainLine.Y1 = y;
					edge.SetEndCoordinatesToCenter(edge.ToVertex);
				}
			}
			public override void Delete()
			{
				Vertices.Remove(this);
				GraphCanvas.Children.Remove(MainEllipse);
				GraphCanvas.Children.Remove(txtName);
				List<ArrowEdge> edgesToRemove = new List<ArrowEdge>();
				foreach (ArrowEdge edge in OutEdges)
					edgesToRemove.Add(edge);
				foreach (ArrowEdge edge in InEdges)
					edgesToRemove.Add(edge);
				foreach (var edge in edgesToRemove)
					edge.Delete();
				
			}
			void MoveWithInEdges()
			{
				foreach (ArrowEdge edge in InEdges)
				{
					edge.SetEndCoordinatesToCenter(edge.ToVertex);
				}
			}
			public void KeepVertexInCanvas(double left, double top)
			{
				double newLeft = left + MainEllipse.Width;
				double newTop = top + MainEllipse.Height;
				bool mustChange = false;
				if (newLeft < 0)
				{
					newLeft = 0;
					mustChange = true;
				}
				else if (newLeft > GraphCanvas.ActualWidth)
				{
					newLeft = GraphCanvas.ActualWidth - MainEllipse.Width;
					mustChange = true;
				}
				if (newTop < 0)
				{
					newTop = 0;
					mustChange = true;
				}
				else if (newTop > GraphCanvas.ActualHeight)
				{
					newTop = GraphCanvas.ActualHeight - MainEllipse.Height;
					mustChange = true;
				}

				if (mustChange)
				{
					Canvas.SetLeft(MainEllipse, newLeft);
					Canvas.SetTop(MainEllipse, newTop);
				}
				else
				{
					Canvas.SetLeft(MainEllipse, left);
					Canvas.SetTop(MainEllipse, top);
				}
			}

		}
		public class ArrowEdge : GraphShape
		{
			protected override Geometry DefiningGeometry { get; }
			public Line MainLine { get; private set; }
			public Line LeftLine { get; private set; }
			public Line RightLine { get; private set; }
			bool isDraggingEdge = false;
			public EllipseVertex FromVertex { get; private set; }
			public EllipseVertex ToVertex { get; private set; }
			private Canvas GraphCanvas { get; }
			public TextBox txtLength;
			public new string Name => FromVertex.UniqueId + "->" + ToVertex.UniqueId;
			Line[] Arrow;
			public int Length => Convert.ToInt32(txtLength.Text);
			TextBox outputConsole;
			public ArrowEdge(Canvas graphCanvas, EllipseVertex currentVertex, TextBox console)
			{
				GraphCanvas = graphCanvas;
				CreateEdgeArrow(Canvas.GetLeft(currentVertex.MainEllipse) + currentVertex.MainEllipse.Width / 2, Canvas.GetTop(currentVertex.MainEllipse) + currentVertex.MainEllipse.Height / 2);
				FromVertex = currentVertex;
				outputConsole = console;
			}
			public override void SetStroke(string color)
			{
				for (int i = 0; i < Arrow.Length; i++)
					Arrow[i].Stroke = (SolidColorBrush)Application.Current.Resources[color];
				if(txtLength != null)
				{
					if (color == DefaultColor)
						txtLength.Foreground = Brushes.White;
					else
						txtLength.Foreground = (SolidColorBrush)Application.Current.Resources[color];
				}
			}
			private void SetThickness(int thickness)
			{
				for(int i=0; i < Arrow.Length; i++)
				{
					Arrow[i].StrokeEndLineCap = PenLineCap.Round;
					Arrow[i].StrokeThickness = thickness;
				}
			}
			private void CreateEdgeArrow(double X, double Y)
			{
				MainLine = new Line();
				GraphCanvas.Children.Add(MainLine);
				MainLine.X1 = X;
				MainLine.Y1 = Y;
				MainLine.X2 = X;
				MainLine.Y2 = Y;
				Canvas.SetZIndex(MainLine, 1);
				MainLine.MouseLeftButtonDown += MainLine_MouseLeftButtonDown;
				MainLine.MouseRightButtonUp += MainLine_MouseRightButtonUp;
				MainLine.MouseMove += MainLine_MouseMove;
				MainLine.MouseEnter += MainLine_MouseEnter;
				MainLine.MouseLeave += MainLine_MouseLeave;
				MainLine.CaptureMouse();
				LeftLine = new Line();
				LeftLine.X1 = MainLine.X2;
				LeftLine.Y1 = MainLine.Y2;
				RightLine = new Line();
				RightLine.X1 = MainLine.X2;
				RightLine.Y1 = MainLine.Y2;
				Arrow = new Line[3] { MainLine, LeftLine, RightLine };
				SetEnd(MainLine.X1, MainLine.Y1);
				SetDefaultLook();
				SetThickness(2);
				isDraggingEdge = true;
				SetLengthTextBox();

				GraphCanvas.Children.Add(LeftLine);
				GraphCanvas.Children.Add(RightLine);
			}
			void SetLengthTextBox()
			{
				txtLength = new TextBox();
				txtLength.Background = Brushes.Transparent;
				txtLength.BorderBrush = Brushes.Transparent;
				txtLength.Foreground = Brushes.White;
				txtLength.Height = 23;
				txtLength.Text = "1";
				SetTxtLengthCoordinates();
				txtLength.TextChanged += TxtLength_TextChanged;
				GraphCanvas.Children.Add(txtLength);
			}

			private void TxtLength_TextChanged(object sender, TextChangedEventArgs e)
			{
				if (System.Text.RegularExpressions.Regex.IsMatch(txtLength.Text, "[^0-9]")) //TODO nebo ^-?[0-9] pro záporné hodnoty
				{
					outputConsole.Text += "\n\nOnly integer length of an edge is acceptable...";

					txtLength.Text = "1";
				}
			}

			void SetTxtLengthCoordinates()
			{
				if(txtLength != null)
				{
					double centerX = (MainLine.X1 + MainLine.X2) / 2;
					double centerY = (MainLine.Y1 + MainLine.Y2 )/ 2;
					Canvas.SetLeft(txtLength, centerX);
					if (centerY - txtLength.Height < 0)
						Canvas.SetTop(txtLength, centerY + txtLength.Height);
					else
						Canvas.SetTop(txtLength, centerY - txtLength.Height);
				}
				
			}
			#region MouseActions
			private void MainLine_MouseLeave(object sender, MouseEventArgs e)
			{
				if(!isMarked && Arrow != null)
				{
					SetDefaultLook();
				}
			}

			private void MainLine_MouseEnter(object sender, MouseEventArgs e)
			{
				if(Arrow != null && AttemptToRun ==false)
				{
					SetMarkedLook();
				}
			}

			private void MainLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				if (!AttemptToRun)
				{
					if (isMarked)
					{
						isMarked = false;
						Marked.Remove(this);
						SetDefaultLook();
					}
					else
					{
						isMarked = true;
						Marked.Add(this);
						SetMarkedLook();
					}
				}
			}
			private void MainLine_MouseMove(object sender, MouseEventArgs e)
			{
				var mousePos = e.GetPosition(GraphCanvas);
				MainLineMouseMove(mousePos);
			}
			private void MainLine_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
			{
				var mousePos = e.GetPosition(GraphCanvas);
				var touchedVertex = CheckIntersection(mousePos);
				if (touchedVertex != null)
				{
					MainLine.ReleaseMouseCapture();
					UndragMainLine(mousePos);
					ToVertex = touchedVertex;
					ToVertex.InEdges.Add(this);
					SetEndCoordinatesToCenter(touchedVertex);
					Edges.Add(this, Name);
				}
				else
				{
					Delete();
				}

			}
			#endregion
			public EllipseVertex CheckIntersection(Point mousePosition)
			{
				foreach (EllipseVertex vertex in Vertices.Keys)
				{
					var left = Canvas.GetLeft(vertex.MainEllipse);
					var top = Canvas.GetTop(vertex.MainEllipse);
					if (vertex != FromVertex)
						if (mousePosition.X > left && mousePosition.X < (left + vertex.MainEllipse.Width))
							if (mousePosition.Y > top && mousePosition.Y < (top + vertex.MainEllipse.Height))
								return vertex;
				}
				return null;
			}
			public void SetEnd(double X2, double Y2)
			{
				MainLine.X2 = X2;
				MainLine.Y2 = Y2;
				//vzorec je odsud: https://docs.telerik.com/devtools/wpf/controls/dragdropmanager/how-to/howto-create-custom-drag-arrow
				double theta = Math.Atan2(MainLine.Y1 - MainLine.Y2, MainLine.X1 - MainLine.X2);
				double sint = Math.Round(Math.Sin(theta), 2);
				double cost = Math.Round(Math.Cos(theta), 2);
				var arrowWidth = 5;
				var arrowHeight = 5;
				Point leftPoint = new Point(MainLine.X2 + ((arrowWidth * cost) - (arrowHeight * sint)), MainLine.Y2 + ((arrowWidth * sint) + (arrowHeight * cost)));
				Point rightPoint = new Point(MainLine.X2 + ((arrowWidth * cost) + (arrowHeight * sint)), MainLine.Y2 - ((arrowHeight * cost) - (arrowWidth * sint)));

				LeftLine.X1 = MainLine.X2;
				LeftLine.Y1 = MainLine.Y2;
				RightLine.X1 = MainLine.X2;
				RightLine.Y1 = MainLine.Y2;
				LeftLine.X2 = leftPoint.X;
				LeftLine.Y2 = leftPoint.Y;
				RightLine.X2 = rightPoint.X;
				RightLine.Y2 = rightPoint.Y;

				SetTxtLengthCoordinates();

			}
			public void SetEndCoordinatesToCenter(EllipseVertex vertex)
			{
				var s1 = Canvas.GetLeft(vertex.MainEllipse) + vertex.MainEllipse.Width / 2;
				var s2 = Canvas.GetTop(vertex.MainEllipse) + vertex.MainEllipse.Height / 2;
				var x = MainLine.X1;
				var y = MainLine.Y1;

				var r = vertex.MainEllipse.Width / 2;

				//directional vector
				var u1 = s1 - x;
				var u2 = s2 - y;

				//normal vector
				var n1 = -u2;
				var n2 = u1;
				//const d in equation of line: n1*x + n2*y + d = 0 
				var d = -(n1 * x + n2 * y);

				if (n2 == 0)
				{
					var x1 = -d / n1;
					//coeficients in the quadratic equation
					var a = 1;
					var b = -2 * s2;
					var c = Math.Pow(d / n1, 2) + 2 * (d / n1) * s1 + Math.Pow(s1, 2) + Math.Pow(s2, 2) - Math.Pow(r, 2);
					var det = Math.Pow(b, 2) - 4 * a * c;
					var y1 = (-b + Math.Sqrt(det)) / (2 * a);
					var y2 = (-b - Math.Sqrt(det)) / (2 * a);
					if (Math.Abs(y - y1) > Math.Abs(y - y2)) //closer intersection
						SetEnd(x1, y2);
					else
						SetEnd(x1, y1);
				}
				else
				{
					//coeficients in the quadratic equation
					var a = 1 + Math.Pow(n1 / n2, 2);
					var b = -2 * s1 + 2 * n1 * d / Math.Pow(n2, 2) + 2 * n1 * s2 / n2;
					var c = Math.Pow(s1, 2) + Math.Pow(d / n2, 2) + 2 * d * s2 / n2 + Math.Pow(s2, 2) - Math.Pow(r, 2);
					var det = Math.Pow(b, 2) - 4 * a * c;
					var x1 = (-b + Math.Sqrt(det)) / (2 * a);
					var x2 = (-b - Math.Sqrt(det)) / (2 * a);
					double y1;
					if (Math.Abs(x - x1) > Math.Abs(x - x2)) //closer intersection
					{
						y1 = (-d - n1 * x2) / n2;
						SetEnd(x2, y1);
					}
					else
					{
						y1 = (-d - n1 * x1) / n2;
						SetEnd(x1, y1);
					}
				}

			}
			void UndragMainLine(Point mousePos)
			{
				if (isDraggingEdge)
				{
					isDraggingEdge = false;
					var draggable = MainLine;
					draggable.ReleaseMouseCapture();
					SetEnd(mousePos.X, mousePos.Y);

				}
			}
			void MainLineMouseMove(Point mousePos)
			{
				if (isDraggingEdge)
				{
					SetEnd(mousePos.X, mousePos.Y);
					KeepEdgeInCanvas();
					var touchedVertex = CheckIntersection(mousePos);
					if (touchedVertex != null)
						touchedVertex.MainEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					else
						SetVerticesToGreen();

				}
			}
			void SetVerticesToGreen()
			{
				foreach (EllipseVertex vertex in Vertices.Keys)
				{
					//TODO zachovat barvu u omarkovanej (až nebude vector jen ellipsa :/ ), ale možná je to takhle lepší..
					vertex.MainEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
				}
			}
			private void KeepEdgeInCanvas()
			{
				double newLeft = MainLine.X2;
				double newTop = MainLine.Y2;

				if (newLeft < 0)
					newLeft = 0;
				else if (newLeft > GraphCanvas.ActualWidth)
					newLeft = GraphCanvas.ActualWidth;

				if (newTop < 0)
					newTop = 0;
				else if (newTop > GraphCanvas.ActualHeight)
					newTop = GraphCanvas.ActualHeight;

				SetEnd(newLeft, newTop);
			}
			public override void Delete()
			{
				GraphCanvas.Children.Remove(MainLine);
				GraphCanvas.Children.Remove(RightLine);
				GraphCanvas.Children.Remove(LeftLine);
				GraphCanvas.Children.Remove(txtLength);
				Edges.Remove(this);
				FromVertex.OutEdges.Remove(this);
				if(ToVertex != null)
					ToVertex.InEdges.Remove(this);
				
			}

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
			List<Algorithm> algorithms = new List<Algorithm>() { new BFS(), new DFS(), new Dinic(), new FordFulkerson(), new Goldberg(), new Boruvka(), new Jarnik(), new Kruskal(), new BellmanFord(), new Dijkstra(), new FloydWarshall(), new Relaxation() };
			foreach (var algorithm in algorithms)
			{
				//availbaleAlgorithms.Add(algorithm.Name, algorithm);
				cmbAlgorithms.Items.Add(algorithm);
				if (algorithm is Dinic || algorithm is FordFulkerson || algorithm is Goldberg)
					flowAlgorithms.Add(algorithm);
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

			//EnableEverything();
			
		}
		void DisableEverything()
		{
			graphCanvas.IsEnabled = false;
			imgClear.IsEnabled = false;
			btnClear.IsEnabled = false;
		}
		public void EnableEverything() //TODO spouštět v jinym vlákně
		{
			graphCanvas.IsEnabled = true;
			imgClear.IsEnabled = true;
			btnClear.IsEnabled = true;
		}
		private void imgStepByStep_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StepByStep();

			
		}
		void StepByStep()
		{
			
		}
		async Task Run()
		{
			btnOkRun.Visibility = Visibility.Hidden;
			lblInfo.Visibility = Visibility.Hidden;
			txtConsole.Text += "\n\nSelected algorithm is running.";
			((Algorithm)cmbAlgorithms.SelectedItem).SetOutputConsole(txtConsole);
			StartCreating(); //tady se disabluje
			await StartRunning(); //tady se spustí někdy metoda async void Run()
			EnableEverything();
		}
		void LightenGrid(Grid uc)
		{
			uc.Background = new SolidColorBrush(Color.FromRgb(44, 47, 68));
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
		private void gridRun_MouseUp(object sender, MouseButtonEventArgs e)
		{
			lblInfo.Visibility = Visibility.Hidden;
			if(cmbAlgorithms.SelectedItem != null)
			{
				LightenGrid(gridRun);
				txtConsole.Text += "\n\n"+((Algorithm)cmbAlgorithms.SelectedItem).Name + " attempts to run...";
				if (flowAlgorithms.Contains(((Algorithm)cmbAlgorithms.SelectedItem)))
					isFlowAlgorithm = true;
				if (isFlowAlgorithm)
					txtConsole.Text = txtConsole.Text + "\n\nSelect the source vertex. Then press green Done button.";
				else
					txtConsole.Text = txtConsole.Text + "\n\nSelect the initial vertex. Then press green Done button.";
				lblInfo.Visibility = Visibility.Visible;
				btnOkRun.Visibility = Visibility.Visible;
				AttemptToRun = true;
				
				UnmarkEverything();
			}
			else
			{
				txtConsole.Text += "\n\nYou have to choose an algorithm in the upper left corner corner.";
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

		private void gridStepByStep_MouseEnter(object sender, MouseEventArgs e)
		{
			LightenGrid(gridStepByStep);
		}

		private void gridStepByStep_MouseLeave(object sender, MouseEventArgs e)
		{
			DarkenGrid(gridStepByStep);
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
				TxbTimComplexity.Text = "Time complexity: "+ ((Algorithm)cmbAlgorithms.SelectedItem).TimeComplexity;
			}
			else
			{
				TxbTimComplexity.Visibility = Visibility.Hidden;
				text = "No algorithm is selected.";
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

		private async void btnOkRun_Click(object sender, RoutedEventArgs e)
		{
			if(Marked.Count >= 1) //initial vertex is selected or sink & source
			{
				if (isFlowAlgorithm && sourceSinkCounter < 1)
					sourceSinkCounter++;
				else
				{
					await Run();
					DarkenGrid(gridRun);
					txtConsole.Text += "\n\n" + ((Algorithm)cmbAlgorithms.SelectedItem).Name + " has finished.";
					AttemptToRun = false;
					foreach (var marked in Marked) // can be one or two marked
						marked.Unmark();
					Marked.Clear();
					sourceSinkCounter = 0;
					isFlowAlgorithm = false;
				}
			}		
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
					return;
				
				dialog.PrintVisual(graphCanvas, "Graph canvas");
				ChangeCanvasLook((SolidColorBrush)Application.Current.Resources["Dark"], Brushes.Black, Brushes.White);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Some mistake occured...", MessageBoxButton.OK, MessageBoxImage.Error);
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
	}
}
