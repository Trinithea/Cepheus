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
	public class EllipseVertex : GraphShape
	{
		/// <summary>
		/// Unique integer ID
		/// </summary>
		public int UniqueId;
		/// <summary>
		/// Determines if the vertex is dragged.
		/// </summary>
		private bool isDraggingVertex = false;
		/// <summary>
		/// Determines if the vertex has moved.
		/// </summary>
		bool wasMoving = false;
		protected override Geometry DefiningGeometry { get; }
		/// <summary>
		/// The main circle showing the vertex.
		/// </summary>
		public Ellipse MainEllipse { get; private set; }
		/// <summary>
		/// Edges that come out of the vertex.
		/// </summary>
		public List<ArrowEdge> OutEdges = new List<ArrowEdge>();
		/// <summary>
		/// Edges that enter the vertex.
		/// </summary>
		public List<ArrowEdge> InEdges = new List<ArrowEdge>();
		/// <summary>
		/// Canvas on which the vertices are drawn.
		/// </summary>
		public Canvas GraphCanvas;
		/// <summary>
		/// A text box with the name of the vertex appearing above the vertex.
		/// </summary>
		public TextBox txtName;
		/// <summary>
		/// Dump console.
		/// </summary>
		TextBox outputConsole;
		/// <summary>
		/// X coordinate of the position of the main ellipse on the canvas.
		/// </summary>
		double left => Canvas.GetLeft(MainEllipse);
		/// <summary>
		/// Y coordinate of the position of the main ellipse on the canvas.
		/// </summary>
		double top => Canvas.GetTop(MainEllipse);
		/// <summary>
		/// X coordinate of the position of the center of the main ellipse on the canvas.
		/// </summary>
		public double Left => left + MainEllipse.Width/2;
		/// <summary>
		/// Y coordinate of the position of the center of the main ellipse on the canvas.
		/// </summary>
		public double Top => top + MainEllipse.Height/2; 
		/// <summary>
		/// Vertex name.
		/// </summary>
		public new string Name => txtName.Text;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mousePos">Coordinates where the user clicked on the canvas.</param>
		/// <param name="uniqueId">Unique vertex ID.</param>
		/// <param name="name">Vertex name.</param>
		/// <param name="graphCanvas">Canvas on which the vertices are drawn.</param>
		/// <param name="console">Dump console.</param>
		public EllipseVertex(Point mousePos, int uniqueId, string name, Canvas graphCanvas, TextBox console)
		{
			GraphCanvas = graphCanvas;
			CreateVertexEllipse(mousePos, uniqueId,name);
			outputConsole = console;
			
		}
		/// <summary>
		/// Creates the entire vertex, draws the main ellipse on the canvas, assigns the appropriate method to the vertex, and adds it to the graph.
		/// </summary>
		/// <param name="mousePos"></param>
		/// <param name="uniqueId"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private Ellipse CreateVertexEllipse(Point mousePos, int uniqueId, string name)
		{
			Ellipse newVertex = new Ellipse();

			SolidColorBrush mySolidColorBrush = new SolidColorBrush();

			mySolidColorBrush.Color = Colors.White;
			newVertex.Fill = mySolidColorBrush;
			newVertex.Stroke = (SolidColorBrush)Application.Current.Resources["Aqua"]; ;
			newVertex.StrokeThickness = 4;

			newVertex.Width = 20;
			newVertex.Height = 20;
			Canvas.SetLeft(newVertex, mousePos.X - newVertex.Width / 2); //so that the center of the ellipse is where the user clicked
			Canvas.SetTop(newVertex, mousePos.Y - newVertex.Height / 2);
			Canvas.SetZIndex(newVertex, 3);
			newVertex.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
			newVertex.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
			newVertex.MouseMove += Ellipse_MouseMove;
			newVertex.MouseEnter += Ellipse_MouseEnter;
			newVertex.MouseLeave += Ellipse_MouseLeave;
			newVertex.MouseRightButtonDown += Ellipse_MouseRightButtonDown;

			GraphCanvas.Children.Add(newVertex);
			MainEllipse = newVertex;
			txtName = new TextBox();
			txtName.KeyUp += TxtName_KeyUp;

			UniqueId = uniqueId;
			SetNameTextBox(Canvas.GetLeft(newVertex), Canvas.GetTop(newVertex), name);
			
			return newVertex;
		}
		/// <summary>
		/// Checks if the user has not used a semicolon in the name, which could cause a problem when importing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtName_KeyUp(object sender, KeyEventArgs e)
		{
			if (txtName.Text.Contains(';'))
			{
				txtName.Text.Replace(";", "");
				outputConsole.Text += "\n" + CepheusProjectWpf.Properties.Resources.CantContainSemicolon;
			}
				
		}
		/// <summary>
		/// Sets the vertex name to the default value and the correct position of the text box with the name above the vertex to fit in the canvas.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="name"></param>
		void SetNameTextBox(double left, double top, string name)
		{
			txtName.Background = Brushes.Transparent;
			txtName.BorderBrush = Brushes.Transparent;
			txtName.Foreground = Brushes.White;
			txtName.Height = 23;
			if (name == null)
				txtName.Text = CepheusProjectWpf.Properties.Resources.VertexHash + UniqueId;
			else
				txtName.Text = name;
			Canvas.SetLeft(txtName, left);
			if (top - txtName.Height < 0)
				Canvas.SetTop(txtName, top + MainEllipse.Height);
			else
				Canvas.SetTop(txtName, top - txtName.Height);
			GraphCanvas.Children.Add(txtName);
		}
		/// <summary>
		/// Sets the correct position of the text box with the name above the vertex to fit in the canvas.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		void SetNameCoordinates(double left, double top)
		{
			if (left + txtName.ActualWidth > GraphCanvas.ActualWidth)
				Canvas.SetLeft(txtName, left - (left + txtName.ActualWidth - GraphCanvas.ActualWidth));
			else
				Canvas.SetLeft(txtName, left);

			if (top - txtName.Height < 0)
				Canvas.SetTop(txtName, top + MainEllipse.Height);
			else
				Canvas.SetTop(txtName, top - txtName.Height);
		}
		#region MouseActions
		/// <summary>
		/// Sets the appearance of the vertex (and possibly its textbox) to the color in the argument.
		/// </summary>
		/// <param name="color"></param>
		public override void SetStroke(SolidColorBrush color)
		{
			MainEllipse.Stroke = color;
			if (color == MainWindow.DefaultColor)
				txtName.Foreground = Brushes.White;
			else
				txtName.Foreground = color;
		}
		/// <summary>
		/// Deselects the vertex when the mouse cursor leaves it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!isMarked)
			{
				SetDefaultLook();
			}
		}
		/// <summary>
		/// Marks a vertex when the mouse cursor hovers over it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
		{
			SetMarkedLook();
		}
		/// <summary>
		/// If the user right-clicks on a vertex, the formation of an edge starting from that vertex begins.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArrowEdge arrow = new ArrowEdge(GraphCanvas, this, outputConsole);

			OutEdges.Add(arrow);
		}
		/// <summary>
		/// If the user presses the left mouse button on the vertex, he wants to either mark it or move it. The appropriate indicators are set accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			isDraggingVertex = true;
			var draggableControl = (Shape)sender;
			var clickPosition = e.GetPosition(GraphCanvas);
			draggableControl.CaptureMouse();
		}
		/// <summary>
		/// If the user releases the left mouse button on the vertex, it depends on whether he moved the vertex or just wanted to mark it. This method solves both cases.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{

			var draggable = (Shape)sender;
			draggable.ReleaseMouseCapture();
			if (isMarked || wasMoving)
			{
				Unmark();
				MainWindow.Marked.Remove(this); 
			}
			else
			{
				SetMarkedLook();
				isMarked = true;
				MainWindow.Marked.Add(this);
				if (MainWindow.AttemptToRun)
				{
					if (!MainWindow.isFlowAlgorithm && MainWindow.Marked.Count > 1)
					{
						MainWindow.Marked[0].Unmark();
						MainWindow.Marked.RemoveAt(0);
					}
					if (MainWindow.isFlowAlgorithm && MainWindow.sourceSinkCounter == 1)
					{
						MainWindow.sinkVertex = UniqueId;
						MainWindow.sourceSinkCounter++;
					}
						
					else
					{
						MainWindow.initialVertex = UniqueId;

					}

				}
			}
			wasMoving = false;
			isDraggingVertex = false;
		}
		/// <summary>
		/// The method solves the correct movement with the vertex (eg to move all edges that enter and leave the vertex).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// It moves all the edges that come from the vertex to the position passed in the arguments (X,Y coorinate).
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void MoveWithOutEdges(double x, double y)
		{
			foreach (ArrowEdge edge in OutEdges)
			{
				edge.MainLine.X1 = x;
				edge.MainLine.Y1 = y;
				edge.SetEndCoordinatesToCenter(edge.ToVertex);
			}
		}
		/// <summary>
		/// Deletes the vertex and all edges that come out of or enter it.
		/// </summary>
		public override void Delete()
		{
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
		/// <summary>
		/// It moves with all the edges that enter the vertex.
		/// </summary>
		void MoveWithInEdges()
		{
			foreach (ArrowEdge edge in InEdges)
			{
				edge.SetEndCoordinatesToCenter(edge.ToVertex);
			}
		}
		/// <summary>
		/// It maintains the vertex only in the canvas areas, so that the user cannot pull it to other parts of the application.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
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
		/// <summary>
		/// Draws a copy of the vertex shifted by leftDifference to the left and topDifference up on the canvas passed in the argument.
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="leftDifference"></param>
		/// <param name="topDifference"></param>
		/// <returns></returns>
		public EllipseVertex DrawThisOnCanvasAndReturnCopy(Canvas canvas, double leftDifference, double topDifference)
		{
			var copy = new EllipseVertex(new Point(Left - leftDifference, Top - topDifference), UniqueId, Name, canvas, outputConsole);
			return copy;
		}
	}
}
