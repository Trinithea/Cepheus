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

		private bool isDraggingVertex = false;
		private Point clickPosition;
		List<Ellipse> vertices = new List<Ellipse>();
		private void graphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!isDraggingVertex && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				
				KeepVertexInCanvas(CreateVertexEllipse(e));
			}
		}
		private Ellipse CreateVertexEllipse(MouseButtonEventArgs e)
		{
			Point p = e.GetPosition(graphCanvas);
			Ellipse myEllipse = new Ellipse();

			SolidColorBrush mySolidColorBrush = new SolidColorBrush();

			mySolidColorBrush.Color = Colors.White;
			myEllipse.Fill = mySolidColorBrush;
			myEllipse.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"]; ;
			myEllipse.StrokeThickness = 4;

			myEllipse.Width = 20;
			myEllipse.Height = 20;
			Canvas.SetLeft(myEllipse, p.X - myEllipse.Width / 2);
			Canvas.SetTop(myEllipse, p.Y - myEllipse.Height / 2);
			Canvas.SetZIndex(myEllipse, 2);
			myEllipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
			myEllipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
			myEllipse.MouseMove += Ellipse_MouseMove;
			myEllipse.MouseEnter += Ellipse_MouseEnter;
			myEllipse.MouseLeave += Ellipse_MouseLeave;
			myEllipse.MouseRightButtonDown += Ellipse_MouseRightButtonDown;

			graphCanvas.Children.Add(myEllipse);
			vertices.Add(myEllipse);
			return myEllipse;
		}

		private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
		{
			((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"];
		}

		private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
		{
			((Ellipse)sender).Stroke = (SolidColorBrush)Application.Current.Resources["Orange"]; 
		}

		private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			clickPosition = e.GetPosition(graphCanvas);

			Arrow arrow = new Arrow(graphCanvas, (Ellipse)sender, vertices);
		}
		private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isDraggingVertex = true;
			var draggableControl = (Shape)sender;
			clickPosition = e.GetPosition(graphCanvas);
			draggableControl.CaptureMouse();
		}
		private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			isDraggingVertex = false;
			var draggable = (Shape)sender;
			draggable.ReleaseMouseCapture();
		}
		private void Ellipse_MouseMove(object sender, MouseEventArgs e)
		{
			var draggableControl = (Shape)sender;
			var mousePos = e.GetPosition(graphCanvas);

			if (isDraggingVertex && draggableControl != null)
			{
				double left = mousePos.X - (((Shape)sender).Width / 2);
				double top = mousePos.Y - (((Shape)sender).Height / 2);
				Canvas.SetLeft(((Shape)sender), left);
				Canvas.SetTop(((Shape)sender), top);
				KeepVertexInCanvas((Ellipse)draggableControl);
			}
		}
		private void KeepVertexInCanvas(Ellipse vertex)
		{
			double newLeft = Canvas.GetLeft(vertex) + vertex.Width;
			double newTop = Canvas.GetTop(vertex) + vertex.Height;
			bool mustChange = false;
			if (newLeft < 0)
			{
				newLeft = 0;
				mustChange = true;
			}
			else if (newLeft > graphCanvas.Width)
			{
				newLeft = graphCanvas.Width - vertex.Width;
				mustChange = true;
			}
			if (newTop < 0)
			{
				newTop = 0;
				mustChange = true;
			}
			else if (newTop > graphCanvas.Height)
			{
				newTop = graphCanvas.Height - vertex.Height;
				mustChange = true;
			}

			if (mustChange)
			{
				Canvas.SetLeft(vertex, newLeft);
				Canvas.SetTop(vertex, newTop);
			}
		}
		class Arrow : Shape
		{
			protected override Geometry DefiningGeometry { get; }

			Line MainLine;
			Line LeftLine;
			Line RightLine;
			bool isDraggingEdge=false;
			Ellipse FromVertex;
			Ellipse ToVertex;
			private Canvas GraphCanvas { get; }
			List<Ellipse> Vertices;

			public Arrow(Canvas graphCanvas, Ellipse currentVertex, List<Ellipse> vertices)
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
				MainLine.MouseRightButtonUp += Line_MouseRightButtonUp;
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

			//TODO ještě nefunkční teda
			private void MainLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				MainLine.CaptureMouse();
				isDraggingEdge = true;
				var mousePos = e.GetPosition(GraphCanvas);
				if(Math.Abs(mousePos.X - MainLine.X1) > Math.Abs(mousePos.X - MainLine.X2))
					MainLineMouseMove(new Point(MainLine.X2, MainLine.Y2));
				else
					MainLineMouseMove(new Point(MainLine.X1, MainLine.Y1));
			}

			//TODO - linear time is bleh
			public bool CheckIntersection(Point mousePosition)
			{
				foreach(Ellipse vertex in Vertices) 
				{
					var left = Canvas.GetLeft(vertex) ;
					var top = Canvas.GetTop(vertex);
					if(vertex != FromVertex)
						if (mousePosition.X > left && mousePosition.X < (left+vertex.Width))
							if(mousePosition.Y > top && mousePosition.Y < (top + vertex.Height))
							{
								MainLine.ReleaseMouseCapture();
								UndragMainLine(mousePosition);
								ToVertex = vertex;
								return true;
							}
				}
				return false;
			}

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
			private void Line_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
			{
				var mousePos = e.GetPosition(GraphCanvas);
				UndragMainLine(mousePos);
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
					CheckIntersection(mousePos);
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
	}

}
