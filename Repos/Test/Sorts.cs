using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class Sorts
    {
        public static void InsertSort(int[] array)
        {
            int tmp, j;

            if (array == null || array.Length < 2)
                return;

            for (int i = 1; i < array.Length; i++)
            {
                tmp = array[i];
                j = i - 1;

                while (j >= 0 && array[j] > tmp)
                {
                    array[j + 1] = array[j];
                    j--;
                }

                array[j + 1] = tmp;
            }
        }

        static public void Merge(int[] array, int low, int middle, int high)
        {
            Queue<int> aq = new Queue<int>();
            Queue<int> bq = new Queue<int>();

            for (int i = low; i <= middle; i++)
                aq.Enqueue(array[i]);

            for (int i = middle; i <= high; i++)
                bq.Enqueue(array[i]);

            int j = low;
            while (aq.Count > 0 && bq.Count > 0)
            {
                if (aq.Peek() < bq.Peek())
                {
                    array[j++] = aq.Dequeue();
                }
                else
                {
                    array[j++] = bq.Dequeue();
                }
            }

            while (aq.Count > 0)
                array[j++] = aq.Dequeue();

            while (bq.Count > 0)
                array[j++] = bq.Dequeue();
        }

        public static void MergeSort(int[] array, int low, int high)
        {
            int mid;

            if (low < high)
            {
                mid = (low + high) / 2;

                MergeSort(array, low, mid);
                MergeSort(array, mid + 1, high);
                Merge(array, low, mid + 1, high);
            }
        }

        public static int BinarySearch(int[] arr, int left, int right, int x)
        {
            if (left > right)
                return -1;

            int mid = left + (right - left) / 2;

            if (arr[mid] == x)
                return mid;
            else if (x < arr[mid])
                return BinarySearch(arr, left, mid - 1, x);
            else
                return BinarySearch(arr, mid + 1, right, x);
        }

        public static void BubbleSort(int[] arr, int n)
        {
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int tmp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = tmp;
                    }
                }
        }



        // QuickSort
        public static void QuickSort(IComparable[] array, int left, int right)
        {
            int i = left;
            int j = right;
            IComparable pivot = array[(left + right) / 2];

            while (i < j)
            {
                while (array[i].CompareTo(pivot) < 0)
                    i++;

                while (array[j].CompareTo(pivot) > 0)
                    j--;

                if (i < j)
                {
                    IComparable tmp = array[j];
                    array[j] = array[i];
                    array[i] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(array, left, j);

            if (i < right)
                QuickSort(array, i, right);
        }

        public static void QuickSort(char[] word, int left, int right)
        {
            int i = left;
            int j = right;
            char pivot = word[(left + right) / 2];

            while (i < j)
            {
                while (word[i] < pivot)
                    i++;

                while (word[j] > pivot)
                    j--;

                if (i < j)
                {
                    char tmp = word[j];
                    word[j] = word[i];
                    word[i] = tmp;
                    i++;
                    j--;
                }
            }

            if (left < j)
                QuickSort(word, left, j);

            if (i < right)
                QuickSort(word, i, right);
        }

        public static void TestQuickSort()
        {
            string[] unsorted = new string[] { "x", "s", "t", "d", "p" };

            for (int i = 0; i < unsorted.Length; i++)
            {
                Console.Write(unsorted[i]);
                Console.Write(",");
            }

            Console.WriteLine(";");

            QuickSort(unsorted, 0, unsorted.Length - 1);


            for (int i = 0; i < unsorted.Length; i++)
            {
                //Debug.Assert(1 == 1);
                Console.Write(unsorted[i]);
                Console.Write(",");
            }
        }
    }

    public class Heap
    {
        private const int MAX_SIZE = 100000;
        private const int DEFAULT_SIZE = 1000;
        private int _size;
        private int[] _heap;
        private int _idx;

        public Heap() : this(DEFAULT_SIZE)
        {
        }

        public Heap(int heapSize)
        {
            if (heapSize > 0 && heapSize < MAX_SIZE)
            {
                _size = heapSize;
            }
            else
            {
                _size = DEFAULT_SIZE;
            }

            _heap = new int[_size + 1];
            _idx = 0;
        }

        public void MakeHeap(int[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Insert(values[i]);
            }
        }

        private void Insert(int k)
        {
            if (_idx >= _size)
            {
                throw new Exception("Heap overflow");
            }

            _idx++;
            _heap[_idx] = k;

            BubbleUp(_idx);
        }

        private void BubbleUp(int pos)
        {
            int parentPos = ParentPos(pos);

            if (parentPos > 0 && _heap[parentPos] < _heap[pos])
            {
                Swap(parentPos, pos);
                BubbleUp(parentPos);
            }
        }

        private int ParentPos(int pos)
        {
            if (pos == 1) return -1;

            return pos / 2;
        }

        private int LeftMostChild(int pos)
        {
            return pos * 2;
        }

        private void Swap(int pos1, int pos2)
        {
            int temp = _heap[pos1];
            _heap[pos1] = _heap[pos2];
            _heap[pos2] = temp;
        }
    }
}
