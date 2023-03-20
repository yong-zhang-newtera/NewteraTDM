using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.Test
{
    public class Node<TKey> where TKey : IComparable<TKey>
    {
        public TKey Key { get; set; }
        public Node<TKey> Left { get; set; }
        public Node<TKey> Right { get; set; }
    }

    public class Tree<TKey> where TKey : IComparable<TKey>
    {
        private Node<TKey> root = null;

        public Node<TKey> Find(TKey key)
        {
            return BinarySearch(this.root, key);
        }

        private Node<TKey> BinarySearch(Node<TKey> node, TKey key)
        {
            Node<TKey> found = null;

            if (node != null)
            {
                int result = node.Key.CompareTo(key);
                if (result == 0)
                    found = node;
                else if (result > 0)
                    found = BinarySearch(node.Left, key);
                else
                    found = BinarySearch(node.Right, key);
            }

            return found;
        }
    }

    // Binary Tree

    public class TreeNode<T> where T : IComparable<T>
    {
        public T Key;
        public TreeNode<T> Left = null;
        public TreeNode<T> Right = null;
        public TreeNode<T> Parent = null;
    }

    public class BinaryTree<T> where T : IComparable<T>
    {
        private TreeNode<T> root;

        public BinaryTree()
        {
            root = null;
        }

        public TreeNode<T> Find(T key)
        {
            TreeNode<T> found = null;

            if (this.root != null)
            {
                TreeNode<T> current = this.root;

                while (current != null && found == null)
                {
                    int result = current.Key.CompareTo(key);
                    if (result == 0)
                        found = current;
                    else if (result > 0)
                        current = current.Left;
                    else
                        current = current.Right;
                }
            }

            return found;
        }

        public void Insert(T key)
        {
            if (key != null)
            {
                TreeNode<T> parent = null;
                TreeNode<T> current = this.root;
                int result = 0;

                while (current != null)
                {
                    result = current.Key.CompareTo(key);
                    if (result >= 0)
                    {
                        parent = current;
                        current = parent.Left;
                    }
                    else
                    {
                        parent = current;
                        current = parent.Right;
                    }
                }
                TreeNode<T> newNode = new TreeNode<T>() { Key = key };
                if (parent != null)
                {
                    if (result > 0)
                        parent.Left = newNode;
                    else
                        parent.Right = newNode;
                }
                else
                    this.root = newNode;
            }
        }

        public void Delete(T key)
        {
            // one or zero child, easy
            // two children, tricky
        }


        private void BFT(TreeNode<T> root)
        {
            Queue<TreeNode<T>> q = new Queue<TreeNode<T>>();

            if (root == null)
                return;

            q.Enqueue(root);

            while (q.Count > 0)
            {
                TreeNode<T> current = q.Dequeue();
                if (current == null)
                    continue;

                q.Enqueue(current.Left);
                q.Enqueue(current.Right);
            }

        }

        private void DFS(TreeNode<T> root)
        {
            Stack<TreeNode<T>> s = new Stack<TreeNode<T>>();
            s.Push(root);

            while (s.Count > 0)
            {
                TreeNode<T> current = s.Pop();

                // do something

                if (current.Left != null)
                    s.Push(current.Left);

                if (current.Right != null)
                    s.Push(current.Right);
            }
        }

        public void ProcessNode(T key)
        {

        }

        // found leaf nodes from a BST preorder sequence
        public List<int> leaves(int[] arr)
        {
            List<int> result = new List<int>();
            if (arr == null || arr.Length == 0)
            {
                return result;
            }

            Stack<int> st = new Stack<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                int leaf = int.MinValue;
                bool leafFound = false;
                while (st.Count > 0 && arr[i] > st.Peek())
                {
                    if (leaf > int.MinValue && !leafFound)
                    {
                        result.Add(leaf);
                        leafFound = true;
                    }
                    leaf = st.Pop();
                }

                st.Push(arr[i]);

            }
            result.Add(st.Pop());
            return result;
        }
    }

    public class Node
    {
        public int Value;
        public Node Left;
        public Node Right;

        public Node(int val)
        {
            Value = val;
            Left = null;
            Right = null;
        }
    }

    class Trees
    {
        static int maxLevel = 0;

        static void LeftViewUtil(Node node, int level)
        {
            if (node == null)
            {
                return;
            }

            if (level > maxLevel)
            {
                Console.Write(node.Value);
                maxLevel = level;
            }

            LeftViewUtil(node.Left, level + 1);
            LeftViewUtil(node.Right, level + 1);

        }
        static void InsertTree(ref Tree tree, int x, Tree parent)
        {
            if (tree == null)
            {
                Tree node = new Tree();
                node.Value = x;
                node.Parent = parent;
                tree = node;
                return;
            }

            if (x < tree.Value)
                InsertTree(ref tree.Left, x, tree);
            else
                InsertTree(ref tree.Right, x, tree);
        }

        // Diameter of a binary tree, longest path between two nodes in a b tree
        public int Diameter(Node root)
        {
            /* base case if tree is empty */
            if (root == null)
                return 0;

            /* get the height of left and right sub trees */
            int lheight = Height(root.Left);
            int rheight = Height(root.Right);

            /* get the diameter of left and right subtrees */
            int ldiameter = Diameter(root.Left);
            int rdiameter = Diameter(root.Right);

            /* Return max of following three
              1) Diameter of left subtree
             2) Diameter of right subtree
             3) Height of left subtree + height of right subtree + 1 */
            return Math.Max(lheight + rheight + 1,
                            Math.Max(ldiameter, rdiameter));

        }

        /*The function Compute the "height" of a tree. Height is the
        number f nodes along the longest path from the root node
        down to the farthest leaf node.*/
        static int Height(Node node)
        {
            /* base case tree is empty */
            if (node == null)
                return 0;

            /* If tree is not empty then height = 1 + max of left
               height and right heights */
            return (1 + Math.Max(Height(node.Left), Height(node.Right)));
        }

        // find a path with sum equals to a target
        public static void HasPath(Node root, int sum, String path)
        {
            if (root != null)
            {
                if (root.Value > sum)
                { // if root is greater than Sum required, return
                    return;
                }
                else
                {
                    path += " " + root.Value; //add root to path
                    sum = sum - root.Value; // update the required sum
                    if (sum == 0)
                    { //if sum required =0, means we have found the path
                        Console.WriteLine(path);
                    }
                    HasPath(root.Left, sum, path);
                    HasPath(root.Right, sum, path);
                }
            }
        }

        // Check if it is a full BT, no node has a single child
        public static bool IsFullBT(Node root)
        {
            if (root == null)
                return true;

            if (root.Left == null && root.Right == null)
                return true;

            if (root.Left != null && root.Right != null)
                return IsFullBT(root.Left) && IsFullBT(root.Right);

            return false;
        }


        // Find LCA in BST
        public static Node LCS(Node root, int n1, int n2)
        {
            if (root == null)
                return null;

            if (root.Value > n1 && root.Value > n2)
                return LCS(root.Left, n1, n2);

            if (root.Value < n1 && root.Value < n2)
                return LCS(root.Right, n1, n2);

            return root;
        }

        // find LCA in binary tree
        public static Node FindLCA(Node root, int n1, int n2)
        {
            if (root == null)
                return null;

            if (root.Value == n1 || root.Value == n2)
                return root;

            Node leftLCA = FindLCA(root.Left, n1, n2);
            Node rightLCA = FindLCA(root.Right, n1, n2);

            if (leftLCA != null && rightLCA != null)
                return root;

            return (leftLCA != null ? leftLCA : rightLCA);
        }


        public static void PrintPath()
        {
            Node root = null;
            int sum = 20;
            string path = "";
            HasPath(root, sum, path);
        }


        //Distance between 2 node a, b in a tree is given as:
        // dis(a, b) = dis(root, a) + dis(root, b) - 2*dis(root, lca(a, b));
        // where
        // lca(x, y) is Least Common Ancestor of node x and node y
        public int FindDistanceOfTwoNode(Node root, int n1, int n2)
        {
            int distance = -1;

            Node lca = LCA(root, n1, n2);
            int lcaDistance = 0;
            if (lca != null)
            {
                lcaDistance = FindDistance(root, lca.Value);
            }
            distance = FindDistance(root, n1) + FindDistance(root, n2) - 2 * lcaDistance;

            return distance;
        }

        // Returns -1 if x doesn't exist in tree. Else
        // returns distance of x from root
        public static int FindDistance(Node root, int x)
        {
            // Base case
            if (root == null)
                return -1;

            // Initialize distance
            int dist = -1;

            // Check if x is present at root or in left
            // subtree or right subtree.
            if ((root.Value == x) ||
                (dist = FindDistance(root.Left, x)) >= 0 ||
                (dist = FindDistance(root.Right, x)) >= 0)
                return dist + 1;

            return dist;
        }

        /* Function to find LCA of n1 and n2 in BST. The function assumes that both
            n1 and n2 are present in BST */
        public Node LCA(Node node, int n1, int n2)
        {
            if (node == null)
                return null;

            // If both n1 and n2 are smaller than root, then LCA lies in left
            if (node.Value > n1 && node.Value > n2)
                return LCA(node.Left, n1, n2);

            // If both n1 and n2 are greater than root, then LCA lies in right
            if (node.Value < n1 && node.Value < n2)
                return LCA(node.Right, n1, n2);

            return node;
        }

        // Given a preorder sequence of BST, print number inoder sequence
        public static void PrintInorderFromPreorderStackVersion(int[] nums)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(nums[0]);
            int index = 1;
            while (index < nums.Length)
            {
                if (stack.Count == 0 || stack.Last() > nums[index])
                {
                    stack.Push(nums[index]);
                }
                else
                {
                    while (stack.Count > 0 && stack.Last() < nums[index])
                    {
                        Console.WriteLine(stack.Pop());
                    }

                    stack.Push(nums[index]);
                }

                index++;
            }
        }

        // conver binary tree to a list
        public static List<int> Store(Node root)
        {
            List<int> res = new List<int>();

            PreOrder(root, res);

            return res;

        }

        public static void PreOrder(Node root, List<int> res)
        {
            if (root == null)
                res.Add(int.MinValue);
            else
            {
                res.Add(root.Value);
                PreOrder(root.Left, res);
                PreOrder(root.Right, res);
            }
        }

        public static Node Restore(List<int> list)
        {
            if (list == null)
                return null;

            return ToTree(list);
        }

        public static Node ToTree(List<int> list)
        {
            if (list.Count == 0)
                return null;
            if (list[0] == int.MinValue)
                return null;

            Node root = new Node(list[0]);

            list.RemoveAt(0);

            root.Left = ToTree(list);
            root.Right = ToTree(list);

            return root;
        }

        public void Flattern(Node node, StringBuilder builder)
        {
            if (node == null)
                return;

            if (node.Left != null && node.Right == null)
            {
                builder.Append(":").Append(node.Value);
                return;
            }

            builder.Append(node.Value).Append("?");
            Flattern(node.Left, builder);
            Flattern(node.Right, builder);
        }

        // from a?b?c:d:e (teneray expression) to binary tree
        public static Node Build(string s, int idx)
        {
	        if (idx >= s.Length) {
		        return null;
	        }

            Node node = new Node(s[idx]);
            idx += 2;
	        if (idx < s.Length && s[idx - 1] != ':')
	        {
		        node.Left = Build(s, idx);
                node.Right = Build(s, idx);
            }
	        return node;
        }

        // all node numbers are 1 -9, find sum of node paths
        public static int PathSumUtil(Node node, int v)
        {
            if (node == null)
                return 0;

            v = v * 10 + node.Value;

            if (node.Left == null && node.Right == null)
                return v;

            return PathSumUtil(node.Left, v) + PathSumUtil(node.Right, v);
        }

        // Insert a node to a complete binary tree
        public TreeNode insert(TreeNode root, int val)
        {
            Queue<TreeNode> queue = new LinkedList<>();

            TreeNode candidate = null;
            queue.add(root);
            while (!queue.isEmpty())
            {
                TreeNode node = queue.pop();

                if (node.left != null && node.right == null) (return node.right = new TreeNode(val));
                if (node.left == null && node.right != null) (return node.left = new TreeNode(val));
                if (node.left == null && node.right == null) candidate = node;
                queue.add(node.left);
                queue.add(node.right);
            }
            return (candidate.left = new TreeNode(val));
        }
    }
 }
