using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class Main
    {
        static PriorityQueue<Integer> minHeap = new PriorityQueue<>();
        static PriorityQueue<Integer> maxHeap = new PriorityQueue<>(new Comparator<Integer>());
        
        public int compare(Integer a, Integer b)
        {
            return b.compareTo(a);
        }

    public static void main(String[] args)
    {
        System.out.println("Hello, world!");
        int[] arr = { 4, 3, 1, 8, 4, 7, 6 };
        for (int i: arr)
        {
            findmedian(i);
        }
    }

    public static void findmedian(int x)
    {
        if (maxHeap.isEmpty() || x < maxHeap.peek())
        {
            maxHeap.add(x);
        }
        else
        {
            minHeap.add(x);
        }
        rebalance();
        printmedian();
    }

    public static void rebalance()
    {
        if (Math.abs(minHeap.size() - maxHeap.size()) > 1){
            PriorityQueue<Integer> biggerHeap = (minHeap.size() > maxHeap.size()) ? minHeap : maxHeap;
            PriorityQueue<Integer> smallerHeap = (minHeap.size() > maxHeap.size()) ? maxHeap : minHeap;
            int element = biggerHeap.remove();
            smallerHeap.add(element);
        }
    }

    public static void printmedian()
    {
        PriorityQueue<Integer> biggerHeap = (minHeap.size() > maxHeap.size()) ? minHeap : maxHeap;
        PriorityQueue<Integer> smallerHeap = (minHeap.size() > maxHeap.size()) ? maxHeap : minHeap;
        if (biggerHeap.size() == smallerHeap.size())
        {
            float median = (float)(biggerHeap.peek() + smallerHeap.peek()) / 2;
            System.out.println("ans:" + median);
        }
        else
        {
            System.out.println("ans:" + biggerHeap.peek());
        }
    }
}
}
