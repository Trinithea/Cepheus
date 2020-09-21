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
		/// <summary>
		/// Edge arrow main line.
		/// </summary>
		public Line MainLine { get; private set; }
		/// <summary>
		/// The left part of the end of the arrow.
		/// </summary>
		public Line LeftLine { get; private set; }
		/// <summary>
		/// The right part of the end of the arrow
		/// </summary>
		public Line RightLine { get; private set; }
		/// <summary>
		/// Determines if an edge is just forming and the user pulls it to the end vertex.
		/// </summary>
		bool isDraggingEdge = false;
		/// <summary>
		/// The vertex from which the edge rises.
		/// </summary>
		public EllipseVertex FromVertex { get; private set; }
		/// <summary>
		/// The vertex into which the edge enters.
		/// </summary>
		public EllipseVertex ToVertex { get; private set; }
		/// <summary>
		/// The canvas on which the edge is drawn.
		/// </summary>
		private Canvas GraphCanvas { get; }
		/// <summary>
		/// A text box in which is the length of the edge that the user can edit. By default, its value is 1.
		/// </summary>
		public TextBox txtLength;
		/// <summary>
		/// A unique edge name consisting of a unique ID of fromVertex, an arrow and a unique ID of toVertex.
		/// </summary>
		public new string Name => FromVertex.UniqueId + "->" + ToVertex.UniqueId;
		/// <summary>
		/// The three main lines that make up an arrow (edge).
		/// </summary>
		Line[] Arrow;
		/// <summary>
		/// Edge length.
		/// </summary>
		public int Length => Convert.ToInt32(txtLength.Text);
		/// <summary>
		/// Dump console.
		/// </summary>
		TextBox outputConsole;
		/// <summary>
		/// It forms the edge from which it is drawn. The first pair of coordinates is the center of this vertex.
		/// </summary>
		/// <param name="graphCanvas"></param>
		/// <param name="currentVertex"></param>
		/// <param name="console"></param>
		public ArrowEdge(Canvas graphCanvas, EllipseVertex currentVertex, TextBox console)
		{
			GraphCanvas = graphCanvas;
			double X = Canvas.GetLeft(currentVertex.MainEllipse) + currentVertex.MainEllipse.Width / 2;
			double Y = Canvas.GetTop(currentVertex.MainEllipse) + currentVertex.MainEllipse.Height / 2;
			CreateEdgeArrow(X,Y,X,Y,"1",false);
			FromVertex = currentVertex;
			outputConsole = console;
		}
		/// <summary>
		/// Forms an edge from top to top, but not by dragging. Coordinates in the argument: X1, Y1 are the beginning of the arrow and X2, Y2 is the end of the arrow.
		/// </summary>
		/// <param name="graphCanvas"></param>
		/// <param name="fromVertex"></param>
		/// <param name="toVertex"></param>
		/// <param name="X1"></param>
		/// <param name="Y1"></param>
		/// <param name="X2"></param>
		/// <param name="Y2"></param>
		/// <param name="length"></param>
		/// <param name="console"></param>
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
		/// <summary>
		/// Sets the color of the arrow.
		/// </summary>
		/// <param name="color"></param>
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
		/// <summary>
		/// Sets the thickness of the arrow.
		/// </summary>
		/// <param name="thickness"></param>
		private void SetThickness(int thickness)
		{
			for (int i = 0; i < Arrow.Length; i++)
			{
				Arrow[i].StrokeEndLineCap = PenLineCap.Round;
				Arrow[i].StrokeThickness = thickness;
			}
		}
		/// <summary>
		/// Creates and draws an arrow (edge) on the canvas, assigns it to the appropriate methods, and adds it to the graph.
		/// </summary>
		/// <param name="X1"></param>
		/// <param name="Y1"></param>
		/// <param name="X2"></param>
		/// <param name="Y2"></param>
		/// <param name="length"></param>
		/// <param name="fromFile"></param>
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
		/// <summary>
		/// Sets the text box with the edge length to the correct text size and color. The content is the length passed in the argument.
		/// </summary>
		/// <param name="length"></param>
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
		/// <summary>
		/// Resolves incorrect format of user-specified length (only integer values are recognized).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtLength_KeyUp(object sender, KeyEventArgs e)
		{
			if (txtLength.Text != "")
			{
				var length = new StringBuilder(txtLength.Text);
				bool wrong = false;
				if (length[0] != '-')
				{
					if (System.Text.RegularExpressions.Regex.IsMatch(txtLength.Text, "[^0-9]")) 
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

		/// <summary>
		/// Sets where to display the text box with the length so that it is above the center of the edge.
		/// </summary>
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
		/// <summary>
		/// Deselects an edge when the mouse cursor leaves it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainLine_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!isMarked && Arrow != null)
			{
				SetDefaultLook();
			}
		}
		/// <summary>
		/// Marks an edge when the mouse cursor hovers over it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainLine_MouseEnter(object sender, MouseEventArgs e)
		{
			if (Arrow != null && MainWindow.AttemptToRun == false)
			{
				SetMarkedLook();
			}
		}
		/// <summary>
		/// If the user clicks on the edge, he wants to either mark or unmark it. This situation is solved by this method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Solves edge movement.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainLine_MouseMove(object sender, MouseEventArgs e)
		{
			var mousePos = e.GetPosition(GraphCanvas);
			MainLineMouseMove(mousePos);
		}
		/// <summary>
		/// User dropped edge. The method solves whether he let it go where a vertex lies, and therefore the edge will enter it or not, and then erases the edge.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Determines if the end of the edge is at any vertex.
		/// </summary>
		/// <param name="mousePosition"></param>
		/// <returns></returns>
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
		/// <summary>
		/// Creates the end of the arrow so that LeftLine and RightLine form the correct angle. X2,Y2 are the coordinates of the end of the arrow.
		/// </summary>
		/// <param name="X2"></param>
		/// <param name="Y2"></param>
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
		/// <summary>
		/// Sets the end of the edge to point to the center of the ToVertex.
		/// </summary>
		/// <param name="vertex"></param>
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
		/// <summary>
		/// Performs the necessary actions when the user has already released the edge.
		/// </summary>
		/// <param name="mousePos"></param>
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
		/// <summary>
		/// Moves with the edge as the user moves with it. Checks that the user does not pull the edge out of the canvas.
		/// </summary>
		/// <param name="mousePos"></param>
		void MainLineMouseMove(Point mousePos)
		{
			if (isDraggingEdge)
			{
				SetEnd(mousePos.X, mousePos.Y);
				KeepEdgeInCanvas();
				var touchedVertex = CheckIntersection(mousePos);
				if (touchedVertex != null)
					touchedVertex.SetMarkedLook();

			}
		}
		/// <summary>
		/// Holds the edge inside the canvas.
		/// </summary>
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
		/// <summary>
		/// Deletes an edge from both the canvas and the graph.
		/// </summary>
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
		/// <summary>
		/// Redraws a copy of this edge on the canvas passed in the argument, at the coordinates shifted by leftDifference and topDifference.
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="fromVertex"></param>
		/// <param name="toVertex"></param>
		/// <param name="leftDifference"></param>
		/// <param name="topDifference"></param>
		/// <returns></returns>
		public ArrowEdge DrawThisOnCanvasAndReturnCopy(Canvas canvas,EllipseVertex fromVertex, EllipseVertex toVertex, double leftDifference, double topDifference)
		{
			var copy = new ArrowEdge(canvas, FromVertex, ToVertex, MainLine.X1 - leftDifference, MainLine.Y1- topDifference, MainLine.X2- leftDifference, MainLine.Y2- topDifference, txtLength.Text, outputConsole);
			return copy;
		}
	}
}
