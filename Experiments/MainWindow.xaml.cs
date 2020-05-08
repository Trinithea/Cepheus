using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Experiments
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		static List<Ellipse> vertices = new List<Ellipse>();
		static List<ArrowEdge> edges = new List<ArrowEdge>();
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, graphCanvas);
				vertex.KeepVertexInCanvas();
			}
		}
		

		
		class EllipseVertex : Shape
		{
			private bool isDraggingVertex = false;
			protected override Geometry DefiningGeometry { get; }
			Ellipse MainEllipse;
			List<ArrowEdge> outEdges = new List<ArrowEdge>();
			List<ArrowEdge> inEdges = new List<ArrowEdge>();
			Canvas GraphCanvas;
			public bool isMarked { get; private set; }
			public EllipseVertex(Point mousePos, Canvas graphCanvas)
			{
				GraphCanvas = graphCanvas;
				CreateVertexEllipse(mousePos);
			}
			private Ellipse CreateVertexEllipse(Point mousePos)
			{
				Ellipse myEllipse = new Ellipse();

				SolidColorBrush mySolidColorBrush = new SolidColorBrush();

				mySolidColorBrush.Color = Colors.White;
				myEllipse.Fill = mySolidColorBrush;
				myEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"]; ;
				myEllipse.StrokeThickness = 4;

				myEllipse.Width = 20;
				myEllipse.Height = 20;
				Canvas.SetLeft(myEllipse, mousePos.X - myEllipse.Width / 2);
				Canvas.SetTop(myEllipse, mousePos.Y - myEllipse.Height / 2);
				Canvas.SetZIndex(myEllipse, 2);
				myEllipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
				myEllipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
				myEllipse.MouseMove += Ellipse_MouseMove;
				myEllipse.MouseEnter += Ellipse_MouseEnter;
				myEllipse.MouseLeave += Ellipse_MouseLeave;
				myEllipse.MouseRightButtonDown += Ellipse_MouseRightButtonDown;

				GraphCanvas.Children.Add(myEllipse);
				vertices.Add(myEllipse);
				MainEllipse = myEllipse;
				return myEllipse;
			}
			private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
			{
				if(!isMarked)
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
			}

			private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
			{
				((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
			}

			private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
			{
				var clickPosition = e.GetPosition(GraphCanvas);

				ArrowEdge arrow = new ArrowEdge(GraphCanvas, (Ellipse)sender, vertices);
				edges.Add(arrow);
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
				isDraggingVertex = false;
				var draggable = (Shape)sender;
				draggable.ReleaseMouseCapture();
				if (isMarked)
				{
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
					isMarked = false;
				}
				else
				{
					((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					isMarked = true;
				}
			}
			private void Ellipse_MouseMove(object sender, MouseEventArgs e)
			{
				var draggableControl = (Shape)sender;
				var mousePos = e.GetPosition(GraphCanvas);

				if (isDraggingVertex && draggableControl != null)
				{
					double left = mousePos.X - (((Shape)sender).Width / 2);
					double top = mousePos.Y - (((Shape)sender).Height / 2);
					Canvas.SetLeft(((Shape)sender), left);
					Canvas.SetTop(((Shape)sender), top);
					KeepVertexInCanvas();
				}
			}
			public void KeepVertexInCanvas()
			{
				double newLeft = Canvas.GetLeft(MainEllipse) + MainEllipse.Width;
				double newTop = Canvas.GetTop(MainEllipse) + MainEllipse.Height;
				bool mustChange = false;
				if (newLeft < 0)
				{
					newLeft = 0;
					mustChange = true;
				}
				else if (newLeft > GraphCanvas.Width)
				{
					newLeft = GraphCanvas.Width - MainEllipse.Width;
					mustChange = true;
				}
				if (newTop < 0)
				{
					newTop = 0;
					mustChange = true;
				}
				else if (newTop > GraphCanvas.Height)
				{
					newTop = GraphCanvas.Height - MainEllipse.Height;
					mustChange = true;
				}

				if (mustChange)
				{
					Canvas.SetLeft(MainEllipse, newLeft);
					Canvas.SetTop(MainEllipse, newTop);
				}
			}
		}
		class ArrowEdge : Shape
		{
			protected override Geometry DefiningGeometry { get; }

			public Line MainLine { get; private set; }
			public Line LeftLine { get; private set; }
			public Line RightLine { get; private set; }
			bool isDraggingEdge=false;
			Ellipse FromVertex;
			Ellipse ToVertex;
			private Canvas GraphCanvas { get; }
			List<Ellipse> Vertices;
			public bool isMarked = false;

			public ArrowEdge(Canvas graphCanvas, Ellipse currentVertex, List<Ellipse> vertices)
			{
				GraphCanvas = graphCanvas;
				CreateEdgeArrow(Canvas.GetLeft(currentVertex)+currentVertex.Width/2,Canvas.GetTop(currentVertex)+currentVertex.Height/2);
				FromVertex = currentVertex;
				Vertices = vertices;
			}
			private void SetStrokeAndThickness(Shape shape)
			{
				shape.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
				shape.StrokeEndLineCap = PenLineCap.Round;
				shape.StrokeThickness = 2;
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
				MainLine.MouseMove += Line_MouseMove;
				MainLine.CaptureMouse();
				LeftLine = new Line();
				LeftLine.X1 = MainLine.X2;
				LeftLine.Y1 = MainLine.Y2;
				RightLine = new Line();
				RightLine.X1 = MainLine.X2;
				RightLine.Y1 = MainLine.Y2;

				SetEnd(MainLine.X1, MainLine.Y1);
				SetStrokeAndThickness(MainLine);
				SetStrokeAndThickness(LeftLine);
				SetStrokeAndThickness(RightLine);
				isDraggingEdge = true;

				GraphCanvas.Children.Add(LeftLine);
				GraphCanvas.Children.Add(RightLine);
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

			public Ellipse CheckIntersection(Point mousePosition)
			{
				foreach(Ellipse vertex in Vertices) 
				{
					var left = Canvas.GetLeft(vertex) ;
					var top = Canvas.GetTop(vertex);
					if (vertex != FromVertex)
						if (mousePosition.X > left && mousePosition.X < (left + vertex.Width))
							if (mousePosition.Y > top && mousePosition.Y < (top + vertex.Height))
								return vertex;
				}
				return null;
			}

			//vzorec je odsud: https://docs.telerik.com/devtools/wpf/controls/dragdropmanager/how-to/howto-create-custom-drag-arrow
			public void SetEnd(double X2, double Y2)
			{
				MainLine.X2 = X2;
				MainLine.Y2 = Y2;
				/**/
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
				RightLine.Y2 = rightPoint.Y;/**/

			}
			private void MainLine_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
			{
				var mousePosition = e.GetPosition(GraphCanvas);
				var intersectVertex = CheckIntersection(mousePosition);
				if (intersectVertex != null )
				{
					isDraggingEdge = false;
					var draggable = MainLine;
					draggable.ReleaseMouseCapture();
					SetEndCoordinatesToCenter(intersectVertex);
				}
					//UndragMainLine(mousePosition);
				else
				{
					GraphCanvas.Children.Remove(MainLine);
					GraphCanvas.Children.Remove(LeftLine);
					GraphCanvas.Children.Remove(RightLine);
					MainWindow.edges.Remove(this);
				}
					
			}
			void SetEndCoordinatesToCenter(Ellipse vertex)
			{
				var s1 = Canvas.GetLeft(vertex) + vertex.Width/2;
				var s2 = Canvas.GetTop(vertex) + vertex.Height / 2;
				var x1 = MainLine.X1;
				var y1 = MainLine.Y1;
				var length = Math.Sqrt(Math.Pow(x1-s1,2) + Math.Pow(y1-s2, 2)) - vertex.Width / 2;
				var a = vertex.Width - length + Math.Pow(y1, 2) - Math.Pow(s2, 2) + Math.Pow(x1, 2) - Math.Pow(s1, 2);
				var b = -2 * s1 + 2 * x1;
				var c = -2 * s2 + 2 * y1;
				var f = 1 + 1 / b;
				var e = -2 * a * c / Math.Pow(b, 2)+2*s2*c/b;
				var g = Math.Pow(s1, 2) + Math.Pow(a / b, 2) - 2 * s2 * a / b - vertex.Width / 2;
				var x= (-e+Math.Sqrt(Math.Pow(e,2)-4*f*g))/ 2;
				var y = (a - c * x) / b;
				SetEnd(x, y);
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

			private void Line_MouseMove(object sender, MouseEventArgs e)
			{
				var mousePos = e.GetPosition(GraphCanvas);
				MainLineMouseMove(mousePos);
			}
			void MainLineMouseMove (Point mousePos)
			{
				if (isDraggingEdge)
				{
					SetEnd(mousePos.X, mousePos.Y);
					KeepEdgeInCanvas();
					var touchedVertex = CheckIntersection(mousePos);
					if(touchedVertex != null)
					{
						MainLine.ReleaseMouseCapture();
						UndragMainLine(mousePos);
						ToVertex = touchedVertex;
					}
				}
			}
			private void KeepEdgeInCanvas()
			{
				double newLeft =MainLine.X2;
				double newTop = MainLine.Y2;

				if (newLeft < 0)
					newLeft = 0;
				else if (newLeft > GraphCanvas.Width)
					newLeft = GraphCanvas.Width;

				if (newTop < 0)
					newTop = 0;
				else if (newTop > GraphCanvas.Height)
					newTop = GraphCanvas.Height;

				SetEnd(newLeft, newTop);
			}
		}
		void DeleteEdge(ArrowEdge edge)
		{
			graphCanvas.Children.Remove(edge.MainLine);
			graphCanvas.Children.Remove(edge.RightLine);
			graphCanvas.Children.Remove(edge.LeftLine);
			edges.Remove(edge);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			List<ArrowEdge> edgesToRemove = new List<ArrowEdge>();
			List<EllipseVertex> verticesToRemove = new List<EllipseVertex>();
			if (e.Key == Key.Delete)
			{
				foreach (ArrowEdge edge in edges)
					if (edge.isMarked)
						edgesToRemove.Add(edge);
			}

			foreach (var edge in edgesToRemove)
				DeleteEdge(edge);
		}

		
	}

}
