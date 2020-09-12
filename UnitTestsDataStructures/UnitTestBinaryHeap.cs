using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CepheusProjectWpf.DataStructures;

namespace UnitTestsDataStructures
{
	[TestClass]
	public class UnitTestBinaryHeap
	{
		[TestMethod]
		public void OneElement()
		{
			var heap = new BinaryHeap<int,string>(4);
			heap.Insert(2, "ha");
			Assert.AreEqual("ha", heap.Min());
			heap.ExtractMin();

		}
		[TestMethod]
		public void MoreElements()
		{
			var heap = new BinaryHeap<int, string>(7);
			heap.Insert(2, "ha");
			heap.Insert(4, "haha");
			heap.Insert(1, "h");
			heap.Insert(2, "ha");
			heap.Insert(5, "hahah");
			heap.Insert(7, "hahahah");
			heap.Insert(1, "h");
			Assert.AreEqual("h", heap.Min());
			heap.ExtractMin();
			Assert.AreEqual("ha", heap.Min());
			heap.ExtractMin();
			Assert.AreEqual("haha", heap.Min());
			heap.ExtractMin();
			Assert.AreEqual("hahah", heap.Min());
			heap.ExtractMin();
			Assert.AreEqual("hahahah", heap.Min());
			heap.ExtractMin();
		}
	}
}
