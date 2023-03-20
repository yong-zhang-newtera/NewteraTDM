using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.List
{
    public class Node
    {
        public int val;
        public Node Next;
    }
    class List
    {
        Node Head;

        // reverse a linked list without recursion
        public static Node reverse(Node node)
        {
            if (node == null)
                return null;

            Node current = node;
            Node prev = null;
            Node next = null;

            while (current != null)
            {
                next = current.Next;
                current.Next = prev;
                prev = current;
                current = next;
            }

            return prev;
        }

        public void PrintList()
        {
            Node node = this.Head;
            while (node != null)
            {
                Console.Write(node.val + " ");
                node = node.Next;
            }
            Console.WriteLine();
        }

        public Node Reverse(Node head, int k)
        {
            if (head == null)
                return null;

            Node current = head;
            Node next = null;
            Node prev = null;
            int i = 0;
            while (current != null && i < k)
            {
                next = current.Next;
                current.Next = prev;
                prev = current;
                current = next;
                i++;
            }

            if (next != null)
            {
                Node reverseHead = Reverse(next, k);

                head.Next = reverseHead;
            }

            return prev;
        }

        private Node GetMiddle(Node head)
        {
            if (head == null)
                return head;

            Node slow = head;
            Node fast = head.Next;

            while (fast != null)
            {
                fast = fast.Next;
                if (fast != null)
                {
                    slow = slow.Next;
                    fast = fast.Next;
                }
            }

            return slow;
        }


    }

    public class FibonacciUtil
    {
        public IEnumerable<int> Fibonacci(int x)
        {
            int prev = 0;
            int next = 1;
            for (int i = 0; i < x; i++)
            {
                int sum = prev + next;
                prev = next;
                next = sum;
                yield return sum;
            }
        }

        public void Main()
        {
            IEnumerable<int> fb = Fibonacci(20);

            foreach (int num in fb)
            {
                Console.WriteLine(num);
            }

            for (int i = 0; i < fb.Count(); i++)
            {
                int fnum = fb.ElementAt(i);
            }
        }

        public class SinglyLinkedListBackward
        {
            public static void main(String[] args)
            {
                // TODO Auto-generated method stub
                LinkedNode root = new LinkedNode("A");
                root.next = new LinkedNode("B");
                root.next.next = new LinkedNode("C");

                //recursiveReverse(root);
                //iterativeReverse(root);
                iterativeReverseWithOutMemory(root);

            }

            /**
             * reverse linked list using iterative approach with O(n2) runtime
             * reverse the LinkedList and iterate
             * @param root
             */
            private static void iterativeReverseWithOutMemory(LinkedNode root)
            {
                if (root == null) return;
                LinkedNode previousNode = null;
                LinkedNode currentNode = root;
                LinkedNode nextNode = null;

                while (currentNode.next != null)
                {

                    nextNode = currentNode.next;
                    currentNode.next = previousNode;
                    previousNode = currentNode;
                    currentNode = nextNode;

                }

                currentNode.next = previousNode;

                while (currentNode != null)
                {
                    Console.WriteLine(currentNode.value + " -> ");
                    currentNode = currentNode.next;
                }
            }

            /**
             * reverse linked list using iterative approach
             * push node to stack, and pop it
             * consume O(n) memeory
             * @param root
             */
            private static void iterativeReverse(LinkedNode root)
            {
                if (root == null) return;
                Stack<LinkedNode> stack = new Stack<LinkedNode>();

                while (root != null)
                {
                    stack.Push(root);
                    root = root.next;
                }

                while (stack.Count > 0)
                {
                    Console.WriteLine(stack.Pop().value + "->");
                }

            }

            /**
             * reverse linked list using recursion
             * @param root
             */
            private static void recursiveReverse(LinkedNode root)
            {
                if (root == null) return;
                recursiveReverse(root.next);
                Console.WriteLine(root.value + "->");
            }
        }

        class LinkedNode
        {
            public LinkedNode next;
            public String value;
            public LinkedNode(String value)
            {
                this.value = value;
            }
        }
    }

    // [1,2, 2, 3, 4, 7] , [0, 2 , 6, 7, 8]

    public class MyTest
    {
        public List<int> FindCommonNumbers(List<int> arr1, List<int> arr2)
        {
            List<int> res = new List<int>();

            if (arr1 != null || arr2 == null)
                return res;

            for (int i = 0; i < arr1.Count; i++)
            {

                int found = BinarySearch(arr2, 0, arr2.Count, arr1[i]);
                if (found >= 0)
                    res.Add(found);
            }

            return res;
        }

        private int BinarySearch(List<int> arr, int left, int right, int n)
        {
            int found = -1;

            if (left < right)
            {
                int mid = left + (right - left) / 2;

                if (n < arr[mid])
                    found = BinarySearch(arr, left, mid, n);
                else if (n > arr[mid])
                    found = BinarySearch(arr, mid + 1, right, n);
                else
                    found = arr[mid];
            }

            return found;
        }
    }

    }
