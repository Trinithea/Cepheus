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
		}
		public static Dictionary<EllipseVertex, string> Vertices = new Dictionary< EllipseVertex, string>();
		public static Dictionary<ArrowEdge,string> Edges = new Dictionary< ArrowEdge, string>();
		public static int? initialVertex = null;
		public static int? sinkVertex = null;
		static int idCounter=0;
		string SelectedAlgorithm => treeViewAlgorithms.SelectedItem.ToString();
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, graphCanvas);
				vertex.KeepVertexInCanvas(Canvas.GetLeft(vertex.MainEllipse), Canvas.GetTop(vertex.MainEllipse));
			}
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			List<ArrowEdge> edgesToRemove = new List<ArrowEdge>();
			List<EllipseVertex> verticesToRemove = new List<EllipseVertex>();
			if (e.Key == Key.Delete)
			{
				foreach (ArrowEdge edge in Edges.Keys)
					if (edge.isMarked)
						edgesToRemove.Add(edge);
				foreach (EllipseVertex vertex in Vertices.Keys)
					if (vertex.isMarked)
						verticesToRemove.Add(vertex);
			}

			foreach (var edge in edgesToRemove)
				edge.Delete();
			foreach (var vertex in verticesToRemove)
				vertex.Delete();
		}
		void ClearCanvas()
		{
			graphCanvas.Children.Clear();
			Vertices.Clear();
			Edges.Clear();
		}
		public class EllipseVertex : Shape
		{
			public int UniqueId;
			private bool isDraggingVertex = false;
			bool wasMoving = false;
			protected override Geometry DefiningGeometry { get; }
			public Ellipse MainEllipse { get; private set; }
			public List<ArrowEdge> OutEdges = new List<ArrowEdge>();
			public List<ArrowEdge> InEdges = new List<ArrowEdge>();
			Canvas GraphCanvas;
			TextBox txtName;
			public bool isMarked { get; private set; }
			public new string Name => txtName.Text;
			public EllipseVertex(Point mousePos, Canvas graphCanvas)
			{
				GraphCanvas = graphCanvas;
				CreateVertexEllipse(mousePos);
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
				SetNameTextBox(Canvas.GetLeft(newVertex),Canvas.GetTop(newVertex));
				Vertices.Add(this, Name);
				UniqueId = idCounter;
				idCounter++;
				return newVertex;
			}
			void SetNameTextBox(double left, double top)
			{
				txtName.Background = Brushes.Transparent;
				txtName.BorderBrush = Brushes.Transparent;
				txtName.Foreground = Brushes.White;
				txtName.Height = 23;
				txtName.Text = "Name";
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
			private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
			{
				if (!isMarked)
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
			}

			private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
			{
				((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
			}

			private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
			{
				ArrowEdge arrow = new ArrowEdge(GraphCanvas, this);
				
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
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
					isMarked = false;
				}
				else 
				{
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					isMarked = true;
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
			public void Delete()
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
		public class ArrowEdge : Shape
		{
			protected override Geometry DefiningGeometry { get; }

			public Line MainLine { get; private set; }
			public Line LeftLine { get; private set; }
			public Line RightLine { get; private set; }
			bool isDraggingEdge = false;
			public EllipseVertex FromVertex { get; private set; }
			public EllipseVertex ToVertex { get; private set; }
			private Canvas GraphCanvas { get; }
			TextBox txtLength;
			public bool isMarked = false;
			public new string Name => FromVertex.UniqueId + "->" + ToVertex.UniqueId;
			Line[] Arrow;
			public int Length => Convert.ToInt32(txtLength.Text);
			public ArrowEdge(Canvas graphCanvas, EllipseVertex currentVertex)
			{
				GraphCanvas = graphCanvas;
				CreateEdgeArrow(Canvas.GetLeft(currentVertex.MainEllipse) + currentVertex.MainEllipse.Width / 2, Canvas.GetTop(currentVertex.MainEllipse) + currentVertex.MainEllipse.Height / 2);
				FromVertex = currentVertex;
			}
			private void SetStroke(string color)
			{
				for (int i = 0; i < Arrow.Length; i++)
					Arrow[i].Stroke = (SolidColorBrush)Application.Current.Resources[color];
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
				SetStroke("Aqua");
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
				if (System.Text.RegularExpressions.Regex.IsMatch(txtLength.Text, "[^0-9]"))
				{
					var errorWindow = new UIWindows.ErrorNonIntegerLengthWindow();
					errorWindow.ShowDialog();
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
					SetStroke("Aqua");
					txtLength.Foreground = Brushes.White;
				}
					
			}

			private void MainLine_MouseEnter(object sender, MouseEventArgs e)
			{
				if(Arrow != null)
				{
					SetStroke("Orange");
					txtLength.Foreground = (SolidColorBrush)Application.Current.Resources["Orange"];
				}
					

			}

			private void MainLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{

				if (isMarked)
				{
					isMarked = false;
					MainLine.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
					LeftLine.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
					RightLine.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
				}
				else
				{
					isMarked = true;
					MainLine.Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					LeftLine.Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					RightLine.Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
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
			public void Delete()
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
		Dictionary<string, Algorithm> availbaleAlgorithms = new Dictionary<string, Algorithm>();
		void SetAvailbaleAlgorithms()
		{
			List<Algorithm> algorithms = new List<Algorithm>() { new BFS(), new DFS(), new Dinic(), new FordFulkerson(), new Goldberg(), new Boruvka(), new Jarnik(), new Kruskal(), new Bellman_Ford(), new Dijkstra(), new Floyd_Warshall(), new Relaxation() };
			foreach (var algorithm in algorithms)
				availbaleAlgorithms.Add(algorithm.Name, algorithm);
		}
		
		void StartProcessing()
		{
			var visitor = new VisitorGraphCreator();
			graphCanvas.IsEnabled = false;
			var algorithm = availbaleAlgorithms[SelectedAlgorithm];
			algorithm.Accept(visitor); //Create graph

		}
		bool InitialVertexMustBeUnique() 
		{
			var warningWindow = new UIWindows.WarningUniqueNameOfInitialVertex();
			warningWindow.ShowDialog();
			return warningWindow.correct;
		}
		int? GetInitialVertex() // jen u některejch!
		{
			var initialVertexWindow = new InitialVertexWindow();
			initialVertexWindow.ShowDialog();
			if (initialVertexWindow.correct)
				return initialVertexWindow.initialVertexId;
			else
				return null;
		}
		private void imgStepByStep_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StepByStep();

			
		}
		void StepByStep()
		{
			
		}
		void Run()
		{
			if (InitialVertexMustBeUnique())
			{
				var nameOfInitialVertex = GetInitialVertex();//TODO sourceand sink
				if (nameOfInitialVertex != null)
				{
					initialVertex = nameOfInitialVertex;
					StartProcessing();
				}
			}
		}
		private void btnStepByStep_Click(object sender, RoutedEventArgs e)
		{

		}

		private void imgRun_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Run();
		}

		private void btnRun_Click(object sender, RoutedEventArgs e)
		{
			Run();
		}
	}
}
