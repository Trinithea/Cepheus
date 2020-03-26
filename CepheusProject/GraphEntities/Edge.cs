using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus
{
	class Edge<T> where T: Vertex
	{
		public T From { get; set; }
		public T To { get; set; }
		public string Name { get; set; }
		
	}
	class EdgeWithLength<T> : Edge<T> where T : Vertex
	{
		public int Length { get; set; }
		
		//TODO
		/*{ get => Length; set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException(); // TODO something more user friendly
				else
					Length = value;
			} }*/
	}
}
