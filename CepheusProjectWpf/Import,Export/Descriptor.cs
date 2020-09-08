using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CepheusProjectWpf.GraphShapes;

namespace CepheusProjectWpf.Import_Export
{
	public class RootDescriptor 
	{
		public Dictionary<Type, Descriptor> DescriptorsByType;
		public RootDescriptor()
		{
			vertexDescriptor = new VertexDescriptor();
			edgeDescriptor = new EdgeDescriptor();
			DescriptorsByType = new Dictionary<Type, Descriptor>();
			DescriptorsByType.Add(typeof(EllipseVertex), vertexDescriptor);
			DescriptorsByType.Add(typeof(ArrowEdge), edgeDescriptor);
		}
		static VertexDescriptor vertexDescriptor;
		static EdgeDescriptor edgeDescriptor;
		public Descriptor Descriptor { get; set; }
		public string Serialize<T>(T instance) where T:GraphShape
		{
			return ((Descriptor<T>)Descriptor).GetDescription(Descriptor, instance) + "\n";
		}
	}
	public abstract class Descriptor
	{
		public abstract string GetDescription(Descriptor descriptor, GraphShape shape);
	}
	public abstract class Descriptor<T> :Descriptor where T:GraphShape
	{
		public List<Func<T, string>> Properties { get; protected set; }

		public override string GetDescription(Descriptor descriptor, GraphShape shape)
		{
			StringBuilder description = new StringBuilder();
			foreach(var property in Properties)
			{
				description.Append(property.Invoke((T)shape));
				description.Append(";");
			}
			description.Remove(description.Length - 1, 1);
			return description.ToString();
		}
	}
	class VertexDescriptor : Descriptor<EllipseVertex>
	{
		public VertexDescriptor()
		{
			Properties = new List<Func<EllipseVertex, string>>();
			Properties.Add(GetLeftCoordinate);
			Properties.Add(GetTopCoordinate);
			Properties.Add(GetUniqueID);
			Properties.Add(GetName);
		}
		string GetLeftCoordinate(EllipseVertex vertex) => vertex.Left.ToString();
		string GetTopCoordinate(EllipseVertex vertex) => vertex.Top.ToString();
		string GetUniqueID(EllipseVertex vertex) => vertex.UniqueId.ToString();
		string GetName(EllipseVertex vertex) => vertex.Name;
	}
	class EdgeDescriptor : Descriptor<ArrowEdge>
	{
		public EdgeDescriptor()
		{
			Properties = new List<Func<ArrowEdge, string>>();
			Properties.Add(GetLeft1Coordinate);
			Properties.Add(GetTop1Coordinate);
			Properties.Add(GetLeft2Coordinate);
			Properties.Add(GetTop2Coordinate);
			Properties.Add(GetLength);
			Properties.Add(GetFromUId);
			Properties.Add(GetToUId);
		}
		string GetLeft1Coordinate(ArrowEdge edge) => edge.MainLine.X1.ToString();
		string GetTop1Coordinate(ArrowEdge edge) => edge.MainLine.Y1.ToString();
		string GetLeft2Coordinate(ArrowEdge edge) => edge.MainLine.X2.ToString();
		string GetTop2Coordinate(ArrowEdge edge) => edge.MainLine.Y2.ToString();
		string GetLength(ArrowEdge edge) => edge.Length.ToString();
		string GetFromUId(ArrowEdge edge) => edge.FromVertex.UniqueId.ToString();
		string GetToUId(ArrowEdge edge) => edge.ToVertex.UniqueId.ToString();

	}
}
