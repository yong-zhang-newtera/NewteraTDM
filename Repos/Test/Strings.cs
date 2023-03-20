using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    class Strings
    {
        public int NumUniqueCharacters(StringBuilder sb)
        {
            string str = sb.ToString();
            return str.Distinct().Count();
        }

        public string KthLongestDistinctSubString(string str, int k)
        {
            StringBuilder sb = new StringBuilder();
            string largestSubstr = "";

            for (int i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);

                if (NumUniqueCharacters(sb) > k)
                    sb.Remove(0, 1);

                if (largestSubstr.Length < sb.Length)
                    largestSubstr = sb.ToString();
            }

            return largestSubstr;
        }

        // string permutations recursive
        static void Main(string[] args)
        {
            string str = "ABC";
            Permute(str);
        }

        static void Permute(string str)
        {
            DoPermute("", str);
        }

        static void DoPermute(string sofar, string rest)
        {
            if (string.IsNullOrEmpty(rest))
                Console.WriteLine(sofar);
            else
            {
                for (int i = 0; i < rest.Length; i++)
                {
                    string next = sofar + rest[i];

                    string remaining = rest.Substring(0, i) + rest.Substring(i + 1);

                    DoPermute(next, remaining);
                }
            }
        }

        // Remove mimum parenthesis in a string to make parenthesis balanced
        public static string GetBalancedParenthese(string s)
        {
            Stack<int> iStack = new Stack<int>();
            Stack<char> pStack = new Stack<char>();

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {

                char c = s[i];

                builder.Append(c);

                switch (c)
                {
                    case '(':
                        iStack.Push(i);
                        break;

                    case ')':

                        if (iStack.Count > 0)
                        {
                            iStack.Pop();
                        }
                        break;
                }
            }

            while (iStack.Count > 0)
            {
                builder.Remove(iStack.Pop(), 1);
            }

            return builder.ToString();
        }

        // print prime factors, (2, 3, 5, 7, 9)
        public static void PrimeFactors(int n)
        {
            int i = 2;
            while (n >= 1)
            {
                if (n % i == 0)
                {
                    Console.WriteLine("factor: " + i);
                    n /= i;
                }
                else
                {
                    i++;
                }
            }
        }

        // given an odd number, draw diamond
        public static void DrawDiamond(int n)
        {
            int star = 1;
            int increment = 2;
            int space = (n - 1) / 2;

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(GetString(space, " ") + GetString(star, "*") + GetString(space, " "));

                star = star + increment;
                space = (n - star) / 2;

                if (star == n)
                {
                    increment = -2;
                }
            }
        }

        private static string GetString(int l, string s)
        {
            string result = "";

            for (int i = 0; i < l; i++)
            {
                result += s;
            }

            return result;
        }

        // Use DP to sove string edit distance problem in O(m * n), rather that O(3^M)
        static int EditDistDP(String str1, String str2, int m, int n)
        {
            // Create a table to store results of subproblems
            int[,] dp = new int[m + 1, n + 1];

            // Fill d[][] in bottom up manner
            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    // If first string is empty, only option is to
                    // isnert all characters of second string
                    if (i == 0)
                        dp[i, j] = j;  // Min. operations = j

                    // If second string is empty, only option is to
                    // remove all characters of first string
                    else if (j == 0)
                        dp[i, j] = i; // Min. operations = i

                    // If last characters are same, ignore last char
                    // and recur for remaining string
                    else if (str1[i - 1] == str2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];

                    // If last character are different, consider all
                    // possibilities and find minimum
                    else
                        dp[i, j] = 1 + Math.Min(dp[i, j - 1],  // Insert
                                           Math.Min(dp[i - 1, j],  // Remove
                                           dp[i - 1, j - 1])); // Replace
                }
            }

            return dp[m, n];
        }

        public static int LCSWithDP(char[] x, char[] y, int m, int n)
        {
            int[,] l = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                        l[i, j] = 0;
                    else if (x[i - 1] == y[j - 1])
                        l[i, j] = 1 + l[i - 1, j - 1];
                    else
                        l[i, j] = Math.Max(l[i - 1, j], l[i, j - 1]);
                }

            return l[m, n];
        }

        public static int LISWithDP(int[] arr, int n)
        {
            int[] lis = new int[n];

            for (int i = 0; i < n; i++)
            {
                lis[i] = 1;
            }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < i; j++)
                {
                    if (arr[i] > arr[j] && lis[i] < lis[j] + 1)
                        lis[i] = lis[j] + 1;
                }

            int max = 0;
            for (int i = 0; i < n; i++)
            {
                if (lis[i] > max)
                    max = lis[i];
            }

            return max;
        }


        /* A Naive recursive implementation of LCS problem in java, going backward */
        public class LongestCommonSubsequence
        {

            /* Returns length of LCS for X[0..m-1], Y[0..n-1] */
            int lcs(char[] X, char[] Y, int m, int n)
            {
                if (m == 0 || n == 0)
                    return 0;
                if (X[m - 1] == Y[n - 1])
                    return 1 + lcs(X, Y, m - 1, n - 1);
                else
                    return max(lcs(X, Y, m, n - 1), lcs(X, Y, m - 1, n));
            }

            /* Utility function to get max of 2 integers */
            int max(int a, int b)
            {
                return (a > b) ? a : b;
            }

            public static void main(String[] args)
            {
                LongestCommonSubsequence lcs = new LongestCommonSubsequence();
                String s1 = "AGGTAB";
                String s2 = "GXTXAYB";

                char[] X = s1.ToCharArray();
                char[] Y = s2.ToCharArray();
                int m = X.Length;
                int n = Y.Length;

                Console.WriteLine("Length of LCS is" + " " +
                                              lcs.lcs(X, Y, m, n));
            }

            // combinations of merging two string
            public void DFS(string s1, int m, int i, string s2, int n, int j, string path, List<string> ret)
            {
                if (i == m && j == n)
                {
                    ret.Add(path);
                    return;
                }
                if (i < m) DFS(s1, m, i + 1, s2, n, j, path + s1, ret);
                if (j < n) DFS(s1, m, i, s2, n, j + 1, path + s2[j], ret);
            }

            public void ReverseCharArray(char[] chars)
            {
                int start = 0;
                int end = chars.Length - 1;

                while (start < end)
                {
                    char tmp = chars[end];
                    chars[end] = chars[start];
                    chars[start] = tmp;
                    start++;
                    end--;
                }
            }
        }

        // check meta data
        // Returns true if str1 and str2 are meta strings
        public bool AreMetaStrings(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;

            // Return false if both are not of equal length
            if (len1 != len2)
                return false;

            // To store indexes of previously mismatched
            // characters
            int prev = -1, curr = -1;

            int count = 0;
            for (int i = 0; i < len1; i++)
            {
                // If current character doesn't match
                if (str1[i] != str2[i])
                {
                    // Count number of unmatched character
                    count++;

                    // If unmatched are greater than 2,
                    // then return false
                    if (count > 2)
                        return false;

                    // Store both unmatched characters of
                    // both strings
                    prev = curr;
                    curr = i;
                }
            }

            // Check if previous unmatched of string1
            // is equal to curr unmatched of string2
            // and also check for curr unmatched character,
            // if both are same, then return true
            return (count == 2 &&
                    str1[prev] == str2[curr] &&
                    str1[curr] == str2[prev]);
        }
    }
}
