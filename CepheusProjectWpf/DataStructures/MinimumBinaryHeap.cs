using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Cepheus;

namespace CepheusProjectWpf.DataStructures
{
	public class MinimumBinaryHeap<TKey,TValue> where TKey:IComparable
	{
		/// <summary>
		/// Heap is a ValueTuple array indexed from 1 to Count (<=). First item is a Key and second item is a Value.
		/// </summary>
		public ValueTuple<TKey,TValue>[] Heap { get; private set; }
		/// <summary>
		/// The number of elements that the heap contains.
		/// </summary>
		private int currentIndex;
		/// <summary>
		/// Return the number of elements that the heap contains.
		/// </summary>
		public int Count => currentIndex;
		public MinimumBinaryHeap(int elementCount)
		{
			Heap = new ValueTuple<TKey, TValue>[elementCount+1]; // I store in the heap from index 1, so that it corresponds to the description of the binary heap in Průvodce labyrintem algoritmů
			currentIndex = 0;
		}
		/// <summary>
		/// Inserts value to heap.
		/// </summary>
		public void Insert(TKey key, TValue value) 
		{
			int index = Contains(value);
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
		/// <summary>
		/// Arranges the heap from the bottom so that it is still properly sorted.
		/// </summary>
		/// <param name="indexWithChangedKey"></param>
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
		/// <summary>
		/// Indicates whether the heap contains a specific value. If so, it returns its position in the heap, if not, it returns -1.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Contains(TValue value)
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
		/// <summary>
		/// Arranges the heap from above so that it is still properly sorted.
		/// </summary>
		/// <param name="indexWithChangedKey"></param>
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
		
	}
}
