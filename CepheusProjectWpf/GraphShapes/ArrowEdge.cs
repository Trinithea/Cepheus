using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CepheusProjectWpf.GraphShapes
{
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
		public double Left => Canvas.GetLeft(MainLine);
		public double Top => Canvas.GetTop(MainLine);
		

		TextBox outputConsole;
		public ArrowEdge(Canvas graphCanvas, EllipseVertex currentVertex, TextBox console)
		{
			GraphCanvas = graphCanvas;
			double X = Canvas.GetLeft(currentVertex.MainEllipse) + currentVertex.MainEllipse.Width / 2;
			double Y = Canvas.GetTop(currentVertex.MainEllipse) + currentVertex.MainEllipse.Height / 2;
			CreateEdgeArrow(X,Y,X,Y,"1",false);
			FromVertex = currentVertex;
			outputConsole = console;
		}
		public ArrowEdge(Canvas graphCanvas, EllipseVertex fromVertex, EllipseVertex toVertex, double X1, double Y1, double X2, double Y2, string length, TextBox console)
		{
			GraphCanvas = graphCanvas;
			CreateEdgeArrow(X1, Y1, X2, Y2,length,true);
			outputConsole = console;
			FromVertex = fromVertex;
			FromVertex.OutEdges.Add(this);
			ToVertex = toVertex;
			ToVertex.InEdges.Add(this);
		}
		public override void SetStroke(SolidColorBrush color)
		{
			for (int i = 0; i < Arrow.Length; i++)
				Arrow[i].Stroke = color;
			if (txtLength != null)
			{
				if (color == MainWindow.DefaultColor)
					txtLength.Foreground = Brushes.White;
				else
					txtLength.Foreground = color;
			}
		}
		private void SetThickness(int thickness)
		{
			for (int i = 0; i < Arrow.Length; i++)
			{
				Arrow[i].StrokeEndLineCap = PenLineCap.Round;
				Arrow[i].StrokeThickness = thickness;
			}
		}
		private void CreateEdgeArrow(double X1, double Y1, double X2, double Y2, string length, bool fromFile)
		{
			MainLine = new Line();
			GraphCanvas.Children.Add(MainLine);
			MainLine.X1 = X1;
			MainLine.Y1 = Y1;
			MainLine.X2 = X2;
			MainLine.Y2 = Y2;
			Canvas.SetZIndex(MainLine, 1);
			MainLine.MouseLeftButtonDown += MainLine_MouseLeftButtonDown;
			MainLine.MouseRightButtonUp += MainLine_MouseRightButtonUp;
			MainLine.MouseMove += MainLine_MouseMove;
			MainLine.MouseEnter += MainLine_MouseEnter;
			MainLine.MouseLeave += MainLine_MouseLeave;
			if(!fromFile)
				MainLine.CaptureMouse();
			LeftLine = new Line();
			LeftLine.X1 = MainLine.X2;
			LeftLine.Y1 = MainLine.Y2;
			RightLine = new Line();
			RightLine.X1 = MainLine.X2;
			RightLine.Y1 = MainLine.Y2;
			Arrow = new Line[3] { MainLine, LeftLine, RightLine };
			SetEnd(MainLine.X2, MainLine.Y2);
			SetDefaultLook();
			SetThickness(2);
			if(!fromFile)
				isDraggingEdge = true;
			SetLengthTextBox(length);

			GraphCanvas.Children.Add(LeftLine);
			GraphCanvas.Children.Add(RightLine);
		}
		void SetLengthTextBox(string length)
		{
			txtLength = new TextBox();
			txtLength.Background = Brushes.Transparent;
			txtLength.BorderBrush = Brushes.Transparent;
			txtLength.Foreground = Brushes.White;
			txtLength.Height = 23;
			txtLength.Text = length;
			SetTxtLengthCoordinates();
			txtLength.KeyUp += TxtLength_KeyUp;
			GraphCanvas.Children.Add(txtLength);
		}
		private void TxtLength_KeyUp(object sender, KeyEventArgs e)
		{
			if (txtLength.Text != "")
			{
				var length = new StringBuilder(txtLength.Text);
				bool wrong = false;
				if (length[0] != '-')
				{
					if (System.Text.RegularExpressions.Regex.IsMatch(txtLength.Text, "[^0-9]")) //TODO nebo ^-?[0-9] pro záporné hodnoty
						wrong = true;
				}
				else
				{
					length.Remove(0, 1);
					if (System.Text.RegularExpressions.Regex.IsMatch(length.ToString(), "[^0-9]"))
						wrong = true;
				}
				if (wrong)
				{
					txtLength.Text = "1";
					outputConsole.Text += "\n\n" + CepheusProjectWpf.Properties.Resources.OnlyInteger;
				}

			}

		}


		void SetTxtLengthCoordinates()
		{
			if (txtLength != null)
			{
				double centerX = (MainLine.X1 + MainLine.X2) / 2;
				double centerY = (MainLine.Y1 + MainLine.Y2) / 2;
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
			if (!isMarked && Arrow != null)
			{
				SetDefaultLook();
			}
		}

		private void MainLine_MouseEnter(object sender, MouseEventArgs e)
		{
			if (Arrow != null && MainWindow.AttemptToRun == false)
			{
				SetMarkedLook();
			}
		}

		private void MainLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!MainWindow.AttemptToRun)
			{
				if (isMarked)
				{
					isMarked = false;
					MainWindow.Marked.Remove(this);
					SetDefaultLook();
				}
				else
				{
					isMarked = true;
					MainWindow.Marked.Add(this);
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
				MainWindow.Edges.Add(this, Name);
			}
			else
			{
				Delete();
			}

		}
		#endregion
		public EllipseVertex CheckIntersection(Point mousePosition)
		{
			foreach (EllipseVertex vertex in MainWindow.Vertices.Keys)
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
					touchedVertex.SetMarkedLook();
				else
					SetVerticesToDefault();

			}
		}
		void SetVerticesToDefault()
		{
			foreach (EllipseVertex vertex in MainWindow.Vertices.Keys)
			{
				//TODO zachovat barvu u omarkovanej (až nebude vector jen ellipsa :/ ), ale možná je to takhle lepší..
				vertex.SetDefaultLook();
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
			
			FromVertex.OutEdges.Remove(this);
			if (ToVertex != null)
				ToVertex.InEdges.Remove(this);

		}
		public ArrowEdge DrawThisOnCanvasAndReturnCopy(Canvas canvas,EllipseVertex fromVertex, EllipseVertex toVertex, double leftDifference, double topDifference)
		{
			var copy = new ArrowEdge(canvas, FromVertex, ToVertex, MainLine.X1 - leftDifference, MainLine.Y1- topDifference, MainLine.X2- leftDifference, MainLine.Y2- topDifference, txtLength.Text, outputConsole);
			return copy;
		}
	}
}
