using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Cepheus;

namespace CepheusProjectWpf.DataStructures
{
	public class BinaryHeap<TKey,TValue> where TKey:IComparable
	{
		/// <summary>
		/// Heap is a ValuTuple array indexed from 1 to Count (<=).
		/// </summary>
		public ValueTuple<TKey,TValue>[] Heap { get; private set; }
		private int currentIndex;
		public int Count => currentIndex;
		public BinaryHeap(int elementCount)
		{
			Heap = new ValueTuple<TKey, TValue>[elementCount+1];
			currentIndex = 0;
		}
		/// <summary>
		/// Inserts value to heap.
		/// </summary>
		public void Insert(TKey key, TValue value) 
		{
			int index = HeapContains(value);
			if (index != -1)
			{
				Heap[index].Item1 = key;
				BubbleUp(index);
			}
			else
			{
				currentIndex++;
				Heap[currentIndex].Item1 = key;
				Heap[currentIndex].Item2 = value;
				BubbleUp(currentIndex);
			}
		} 
		private void BubbleUp(int indexWithChangedKey)
		{
			while (indexWithChangedKey > 1)
			{
				int fatherIndex = indexWithChangedKey / 2;
				if (Heap[fatherIndex].Item1.CompareTo(Heap[indexWithChangedKey].Item1)<=0)
					break;
				var tmp = Heap[fatherIndex];
				Heap[fatherIndex] = Heap[indexWithChangedKey];
				Heap[indexWithChangedKey] = tmp;
				indexWithChangedKey = fatherIndex;
			}
		}

		private int HeapContains(TValue value)
		{
			for (int i = 1; i <= currentIndex; i++)
			{
				if (Heap[i].Item2.Equals(value))
					return i;
			}
			return -1;
		}
		/// <summary>
		/// Finds value with the lowest key.
		/// </summary>
		/// <returns></returns>
		public TValue Min() => Heap[1].Item2;
		/// <summary>
		/// Returns and removes value with the lowest key.
		/// </summary>
		/// <returns></returns>
		public TValue ExtractMin() 
		{
			var minimum = Heap[1];
			Heap[1] = Heap[currentIndex];
			Heap[currentIndex] = default;
			currentIndex--;
			BubbleDown(1);
			return minimum.Item2;
		} 
		private void BubbleDown(int indexWithChangedKey)
		{
			while(2*indexWithChangedKey <= currentIndex) //vertex with indexWithChangedKey has sons
			{
				int sonIndex = 2 * indexWithChangedKey;
				if (sonIndex + 1 <= currentIndex && Heap[sonIndex + 1].Item1.CompareTo(Heap[sonIndex].Item1) < 0)
					sonIndex++;
				if (Heap[indexWithChangedKey].Item1.CompareTo(Heap[sonIndex].Item1) < 0)
					break;
				var tmp = Heap[indexWithChangedKey];
				Heap[indexWithChangedKey] = Heap[sonIndex];
				Heap[sonIndex] = tmp;
				indexWithChangedKey = sonIndex;
			}
		}
		public bool ContainsValue(TValue value)
		{
			for (int i = 1; i <= currentIndex; i++)
			{
				if (value.Equals(Heap[i].Item2))
					return true;
			}
			return false;
		}
	}
}
