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
		double left => Canvas.GetLeft(MainEllipse);
		double top => Canvas.GetTop(MainEllipse);
		public double Left => left + MainEllipse.Width/2; 
		public double Top => top + MainEllipse.Height/2; 
		public new string Name => txtName.Text;

		

		public EllipseVertex(Point mousePos, int uniqueId, string name, Canvas graphCanvas, TextBox console)
		{
			GraphCanvas = graphCanvas;
			CreateVertexEllipse(mousePos, uniqueId,name);
			outputConsole = console;
			
		}
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

			UniqueId = uniqueId;
			SetNameTextBox(Canvas.GetLeft(newVertex), Canvas.GetTop(newVertex), name);
			MainWindow.Vertices.Add(this, Name);
			MainWindow.VerticesById.Add(UniqueId, this);
			MainWindow.IdCounter++;
			return newVertex;
		}

		void SetNameTextBox(double left, double top, string name)
		{
			txtName.Background = Brushes.Transparent;
			txtName.BorderBrush = Brushes.Transparent;
			txtName.Foreground = Brushes.White;
			txtName.Height = 23;
			if (name == null)
				txtName.Text = "Vertex #" + UniqueId;
			else
				txtName.Text = name;
			Canvas.SetLeft(txtName, left);
			if (top - txtName.Height < 0)
				Canvas.SetTop(txtName, top + MainEllipse.Height);
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
				Canvas.SetTop(txtName, top + MainEllipse.Height);
			else
				Canvas.SetTop(txtName, top - txtName.Height);
		}
		#region MouseActions
		public override void SetStroke(SolidColorBrush color)
		{
			MainEllipse.Stroke = color;
			if (color == MainWindow.DefaultColor)
				txtName.Foreground = Brushes.White;
			else
				txtName.Foreground = color;
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
			ArrowEdge arrow = new ArrowEdge(GraphCanvas, this, outputConsole);

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
				MainWindow.Marked.Remove(this); //TODO nic moc...
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
						MainWindow.sinkVertex = UniqueId;
					else
					{
						MainWindow.initialVertex = UniqueId;

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
			MainWindow.Vertices.Remove(this);
			MainWindow.VerticesById.Remove(UniqueId);
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
}
