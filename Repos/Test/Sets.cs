using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class Sets
    {
        // subsets or substring
        static void PrintAllSubstrings()
        {
            string str = "shkdkkww";

            FindSubstrings(str);
        }

        static void FindSubstrings(string str)
        {
            DoFindSubstring("", str);
        }

        static void DoFindSubstring(string sofar, string rest)
        {
            if (rest == "")
                Console.WriteLine(sofar);
            else
            {
                DoFindSubstring(sofar + rest[0], rest.Substring(1)); // rest is getting smaller

                DoFindSubstring(sofar, rest.Substring(1));
            }
        }

        // paper cut problem, minimum cuts of square
        // assume A <= B
        static int recursiveCompute(int A, int B)
        {
            if (A == B) return 1;
            if (A == 1) return B;
            return (B / A) + recursiveCompute((B % A), A);
        }

        // given string of digits, get the largest odd number by using the digits in the string
        static int GetLargestNumber(string str)
        {
            string sortedStr = SortDigits(str);

            return GetLargestNumUtil(sortedStr);
        }

        static int GetLargestNumUtil(string remianing)
        {
            if (string.IsNullOrEmpty(remianing)) return 0;

            int number = int.Parse(remianing);

            if ((number % 3) == 0) return number;

            for (int i = remianing.Length - 1; i >= 0; i--)
            {
                number = GetLargestNumUtil(remianing.Substring(0, i) + remianing.Substring(i + 1));
                if (number > 0)
                {
                    return number;
                }
            }

            return 0;
        }

        static string SortDigits(string str)
        {
            char[] digits = str.ToCharArray();
            Array.Sort(digits);

            return new string(digits);
        }

        // given an array of number, use +. -, *, or / on any of two numbers, find the max number you can get
        static double FindMaxNumber(double[] arr)
        {
            return DoFindMaxNumber(0, 0, arr);
        }

        static double DoFindMaxNumber(double prod, int i, double[] arr)
        {
            if (i == arr.Length)
                return prod;

            double temp = GetMax(prod, i, arr);
            if (prod != 0)
            {
                double temp2 = prod / DoFindMaxNumber(arr[i], i++, arr);
                if (temp2 > temp)
                    return temp2;
                else
                    return temp;
            }
            else
                return prod;
        }

        static double GetMax(double prod, int i, double[] arr)
        {
            double tempAdd = prod + DoFindMaxNumber(arr[i], i + 1, arr);
            double tempSubstract = prod - DoFindMaxNumber(arr[i], i + 1, arr);
            double tempMultiple = prod * DoFindMaxNumber(arr[i], i + 1, arr);

            double temp = tempAdd;
            if (tempSubstract > temp)
            {
                temp = tempSubstract;
            }

            if (tempMultiple > temp)
            {
                temp = tempMultiple;
            }

            return temp;
        }


        // Use DP to find longest increase order sub array in an array
        public static void LongestSubArray(int[] arr, int n)
        {
            int[] dp = new int[n];

            dp[0] = 1;

            for (int i = 1; i < n; i++)
            {
                if (arr[i] > arr[i - 1])
                    dp[i] = dp[i - 1] + 1;
                else
                    dp[i] = 1;
            }

            int max = 1;
            int p = 0;
            for (int i = 0; i < n; i++)
            {
                if (dp[i] > max)
                {
                    max = dp[i];
                    p = i;
                }
            }

            Console.WriteLine(max + "" + (p - max) + " " + p);
        }


        // print the repeating numbers (num < n) in the sets with O(1) space
        // idea, move the number to its index postion, scan the rest of the numbers
        public void PrintRepeatingNumbers()
        {
            // 	std::vector<int> inp = {4, 2, 0, 5, 2, 0, 1} ;
            List<int> arr = new List<int>() { 1, 2, 3, 0, 0, 1, 3, 6, 6, 6 };

            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] == i) continue;

                if (arr[arr[i]] == arr[i])
                    Console.WriteLine(arr[i]);
                else
                {
                    int temp = arr[arr[i]];
                    arr[arr[i]] = arr[i];
                    arr[i] = temp;
                    i--;
                }
            }
        }

        // find the numbers in an array so that numbers on the left are smaller and on the right are greater
        public static void Order(int[] num, int size)
        {
            int max = 0;

            Stack<int> s = new Stack<int>();
            for (int i = 0; i < size; i++)
            {
                if (num[i] > max)
                {
                    max = num[i];
                    s.Push(max);
                }

                while (s.Count > 0 && num[i] < s.Peek())
                {
                    s.Pop();
                }
            }

            while (s.Count > 0)
            {
                Console.WriteLine("%d ", s.Peek());
                s.Pop();
            }

        }
    }
}
