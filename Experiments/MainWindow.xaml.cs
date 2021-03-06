﻿using System;
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
		static List<EllipseVertex> vertices = new List<EllipseVertex>();
		static List<ArrowEdge> edges = new List<ArrowEdge>();
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (/*!isDraggingVertex &&*/ Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				var mousePos = e.GetPosition(graphCanvas);
				EllipseVertex vertex = new EllipseVertex(mousePos, graphCanvas);
				vertex.KeepVertexInCanvas(Canvas.GetLeft(vertex.MainEllipse),Canvas.GetTop(vertex.MainEllipse));
			}
		}
		
		void ClearCanvas()
		{
			graphCanvas.Children.Clear();
			vertices.Clear();
			edges.Clear();
		}
		class EllipseVertex : Shape
		{
			private bool isDraggingVertex = false;
			protected override Geometry DefiningGeometry { get; }
			public Ellipse MainEllipse { get; private set; }
			public List<ArrowEdge> OutEdges = new List<ArrowEdge>();
			public List<ArrowEdge> InEdges = new List<ArrowEdge>();
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
				vertices.Add(this);
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
				ArrowEdge arrow = new ArrowEdge(GraphCanvas, this, vertices);
				edges.Add(arrow);
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
					KeepVertexInCanvas(left, top);
					MoveWithOutEdges(Canvas.GetLeft(MainEllipse) + MainEllipse.Width/2,Canvas.GetTop(MainEllipse)+MainEllipse.Height/2);
					MoveWithInEdges();
				}
			}
			void MoveWithOutEdges(double x, double y)
			{
				foreach(ArrowEdge edge in OutEdges)
				{
					edge.MainLine.X1 = x;
					edge.MainLine.Y1 = y;
					edge.SetEndCoordinatesToCenter(edge.ToVertex);
				}
			}
			public void Delete()
			{
				vertices.Remove(this);
				GraphCanvas.Children.Remove(MainEllipse);
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
		class ArrowEdge : Shape
		{
			protected override Geometry DefiningGeometry { get; }

			public Line MainLine { get; private set; }
			public Line LeftLine { get; private set; }
			public Line RightLine { get; private set; }
			bool isDraggingEdge=false;
			public EllipseVertex FromVertex { get; private set; }
			public EllipseVertex ToVertex { get; private set; }
			private Canvas GraphCanvas { get; }
			List<EllipseVertex> Vertices; //TODO asi trochu zbytečný
			public bool isMarked = false;

			public ArrowEdge(Canvas graphCanvas, EllipseVertex currentVertex, List<EllipseVertex> vertices)
			{
				GraphCanvas = graphCanvas;
				CreateEdgeArrow(Canvas.GetLeft(currentVertex.MainEllipse)+currentVertex.MainEllipse.Width/2,Canvas.GetTop(currentVertex.MainEllipse)+currentVertex.MainEllipse.Height/2);
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

			public EllipseVertex CheckIntersection(Point mousePosition)
			{
				foreach(EllipseVertex vertex in Vertices) 
				{
					var left = Canvas.GetLeft(vertex.MainEllipse) ;
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
				/**/
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
				RightLine.Y2 = rightPoint.Y;/**/

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
				}
				else
				{
					GraphCanvas.Children.Remove(MainLine);
					GraphCanvas.Children.Remove(LeftLine);
					GraphCanvas.Children.Remove(RightLine);
					MainWindow.edges.Remove(this);
					FromVertex.OutEdges.Remove(this);
				}
					
			}
			public void SetEndCoordinatesToCenter(EllipseVertex vertex)
			{
				var s1 = Canvas.GetLeft(vertex.MainEllipse) + vertex.MainEllipse.Width/2;
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
					if (touchedVertex != null)
						touchedVertex.MainEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Orange"];
					else
						SetVerticesToGreen();

				}
			}
			void SetVerticesToGreen()
			{
				foreach(EllipseVertex vertex in vertices)
				{
					//TODO zachovat barvu u omarkovanej (až nebude vector jen ellipsa :/ ), ale možná je to takhle lepší..
					vertex.MainEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
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
			public void Delete()
			{
				GraphCanvas.Children.Remove(MainLine);
				GraphCanvas.Children.Remove(RightLine);
				GraphCanvas.Children.Remove(LeftLine);
				edges.Remove(this);
				FromVertex.OutEdges.Remove(this);
				ToVertex.InEdges.Remove(this);
			}
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
				foreach (EllipseVertex vertex in vertices)
					if (vertex.isMarked)
						verticesToRemove.Add(vertex);
			}

			foreach (var edge in edgesToRemove)
				edge.Delete();
			foreach (var vertex in verticesToRemove)
				vertex.Delete();
		}

		
	}

}
