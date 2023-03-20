using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DijkstraAlgorithm
{
    class Dijkstra
    {

        private static int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private static void Print(int[] distance, int verticesCount)
        {
            Console.WriteLine("Vertex    Distance from source");

            for (int i = 0; i < verticesCount; ++i)
                Console.WriteLine("{0}\t  {1}", i, distance[i]);
        }

        public static void DijkstraAlgo(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];

            for (int i = 0; i < verticesCount; ++i)
            {
                distance[i] = int.MaxValue;
                shortestPathTreeSet[i] = false;
            }

            distance[source] = 0;

            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < verticesCount; ++v)
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MaxValue && distance[u] + graph[u, v] < distance[v])
                        distance[v] = distance[u] + graph[u, v];
            }

            Print(distance, verticesCount);
        }

        static void Main(string[] args)
        {
            int[,] graph =  {
                         { 0, 6, 0, 0, 0, 0, 0, 9, 0 },
                         { 6, 0, 9, 0, 0, 0, 0, 11, 0 },
                         { 0, 9, 0, 5, 0, 6, 0, 0, 2 },
                         { 0, 0, 5, 0, 9, 16, 0, 0, 0 },
                         { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
                         { 0, 0, 6, 0, 10, 0, 2, 0, 0 },
                         { 0, 0, 0, 16, 0, 2, 0, 1, 6 },
                         { 9, 11, 0, 0, 0, 0, 1, 0, 5 },
                         { 0, 0, 2, 0, 0, 0, 6, 5, 0 }
                            };

            DijkstraAlgo(graph, 0, 9);
        }
    }
}


namespace Dijkstra
{
    class Algorithm
    {
        public List<int> Dijkstra(ref int[] pi, ref List<Node> G, int s)
        {
            InitializeSingleSource(ref pi, ref G, s);

            List<int> S = new List<int>();
            PriorityQueue Q = new PriorityQueue(G);

            Q.buildHeap();

            while (Q.size() != 0)
            {
                Node u = Q.extractMin();

                S.Add(u.Id);

                for (int i = 0; i < u.Adjacency.Count; i++)
                {
                    Node v = u.Adjacency[i];
                    int w = u.Weights[i];

                    Relax(ref pi, u, ref v, w);
                }
            }

            return S;
        }

        void InitializeSingleSource(ref int[] pi, ref List<Node> nodeList, int s)
        {
            pi = new int[nodeList.Count];

            for (int i = 0; i < pi.Length; i++)
                pi[i] = -1;

            nodeList[s].Distance = 0;

            string x = "2222";
        }

        void Relax(ref int[] pi, Node u, ref Node v, int w)
        {
            if (v.Distance > u.Distance + w)
            {
                v.Distance = u.Distance + w;
                pi[v.Id] = u.Id;
            }
        }
    }
}

// Graph
public class Graph
{
    int num;
    List<List<int>> vertices;

    public Graph(int num)
    {
        this.num = num;
        this.vertices = new List<List<int>>(num);
        for (int i = 0; i < this.num; i++)
        {
            this.vertices[i] = new List<int>();
        }
    }

    public void AddEdge(int x, int y)
    {
        this.vertices[x].Add(y);
    }

    public void BFS(int s)
    {
        bool[] visited = new bool[this.num]; // default is false

        Queue<int> q = new Queue<int>();

        visited[s] = true;
        q.Enqueue(s);

        while (q.Count > 0)
        {
            int v = q.Dequeue();
            Console.Write(v + ",");
            foreach (int n in this.vertices[v])
            {
                if (!visited[n])
                {
                    visited[n] = true;
                    q.Enqueue(n);
                }
            }
        }
    }

    public void DFS(int s)
    {
        bool[] visited = new bool[this.num]; // default is false

        DFSUtil(s, visited);
    }

    private void DFSUtil(int s, bool[] visited)
    {
        visited[s] = true;

        Console.Write(s + ",");
        foreach (int n in this.vertices[s])
        {
            if (!visited[n])
            {
                DFSUtil(n, visited);
            }
        }
    }
}

public struct Cell
{
    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Matrix
{
    int m;
    int n;
    char[,] matrix;

    public Matrix(int m, int n)
    {
        this.m = m;
        this.n = n;
        matrix = new Char[m, n]; 
    }

    public void SetNode(int x, int y, char c)
    {
        this.matrix[x, y] = c;
    }

    public void BFS(int x, int y, bool[,] visited)
    {
        Queue<Cell> q = new Queue<Cell>();

        q.Enqueue(new Cell(x, y));
        visited[x, y] = true;

        while (q.Count > 0)
        {
            Cell c = q.Dequeue();
            int cx = c.x;
            int cy = c.y;

            if (IsValidCell(cx, cy + 1) && !visited[cx, cy + 1])
            {
                visited[cx, cy + 1] = true;
                q.Enqueue(new Cell(cx, cy + 1));
            }

            if (IsValidCell(cx, cy - 1) && !visited[cx, cy - 1])
            {
                visited[cx, cy - 1] = true;
                q.Enqueue(new Cell(cx, cy - 1));
            }
            
            if (IsValidCell(cx + 1, cy) && !visited[cx + 1, cy])
            {
                visited[cx + 1, cy] = true;
                q.Enqueue(new Cell(cx + 1, cy));
            }

            if (IsValidCell(cx - 1, cy) && !visited[cx - 1, cy])
            {
                visited[cx - 1, cy] = true;
                q.Enqueue(new Cell(cx - 1, cy));
            }
        }
    }

    public class DetectCyclicDirectGraph
    {
        int v;
        List<int>[] adj;

        public DetectCyclicDirectGraph(int v)
        {
            this.v = v;
            this.adj = new List<int>[v];

            for (int i = 0; i < v; i++)
                this.adj[i] = new List<int>();
        }

        public void AddEdge(int x, int y)
        {
            if (x < this.v && y < this.v && x >= 0 && y >= 0)
                this.adj[x].Add(y);
        }

        public bool IsCyclic(DetectCyclicDirectGraph graph)
        {
            bool[] visited = new bool[this.v];
            bool[] recStack = new bool[this.v];

            for (int i = 0; i < this.v; i++)
            {
                if (IsCyclicUtil(i, visited, recStack))
                    return true;
            }

            return false;
        }

        private bool IsCyclicUtil(int n, bool[] visited, bool[] recStack)
        {
            visited[n] = true;
            recStack[n] = true;

            foreach (int i in this.adj[n])
            {
                if (!visited[i])
                    return IsCyclicUtil(i, visited, recStack);
                else if (recStack[i])
                    return true;
            }

            recStack[n] = false;

            return false;
        }

        public static void Main()
        {
            DetectCyclicDirectGraph g = new DetectCyclicDirectGraph(3);
            // add edges

            if (g.IsCyclic(g))
                Console.WriteLine("Is cyclic");
            else
                Console.WriteLine("Is not cyclic");
        }
    }

    public class AllPairShortestPath
    {
        static int IFN = int.MaxValue;

        public void FloydWarshall(int[,] graph, int dim)
        {
            int[,] dist = new int[dim, dim];
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                    dist[i, j] = graph[i, j];

            for (int k = 0; k < dim; k++)
                for (int i = 0; i < dim; i++)
                    for (int j = 0; j < dim; j++)
                        if (dist[i,k] != IFN && dist[k, j] != IFN &&
                            dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];

            PrintSolution(dist, dim);
        }

        private void PrintSolution(int[,] dist, int dim)
        {
            for (int i = 0; i < dim; i++)

                for (int j = 0; j < dim; j++)
                {
                    if (dist[i, j] == IFN)
                        Console.Write("IFN");
                    else
                        Console.Write(dist[i, j] + " ");
                }

            Console.WriteLine();
        }

        public static void Main()
        {
            int dim = 3;
            int[,] graph = new int[3, 3] { { 0, 1, IFN }, { 2, 0, IFN }, { 3, 4, 0 } };

            AllPairShortestPath obj = new AllPairShortestPath();

            obj.FloydWarshall(graph, dim);
        }


    }

    private bool IsValidCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= this.m || y >= this.n || this.matrix[x, y] != 'X')
            return false;

        return true;
    }

    public void FindAreas()
    {
        int answers = 0;
        bool[,] visited = new bool[this.m, this.n];

        for (int i = 0; i < this.m; i++)
            for (int j = 0; i < this.n; j++)
            {
                if (this.matrix[i, j] == 'X' && !visited[i, j])
                {
                    answers++;
                    BFS(i, j, visited);
                }
            }

        Console.WriteLine(answers);
    }
}


// matrix graph search
public class MatrixTraverse
{
    public MatrixTraverse()
    {

    }

    private bool IsInvalidOrReached(int[,] v, int cx, int cy)
    {
        if (cx < 0 || cy < 0 || (cx >= v.GetLength(0)) ||
           (cy >= v.GetLength(1) || v[cx, cy] == 1))
        {
            return false;
        }

        return true;

    }
    private bool FindPath(int[,] v, int cx, int cy)
    {
        if (!IsInvalidOrReached(v, cx, cy))
        {
            return false;
        }
        if (v[cx, cy] == 2)
        {
            return true;
        }

        // cx and cy is valid
        v[cx, cy] = 1;
        return FindPath(v, cx - 1, cy) || FindPath(v, cx, cy - 1) ||
           FindPath(v, cx + 1, cy) || FindPath(v, cx, cy + 1);

    }

    bool FindNode()
    {
        int[,] i = new int[10, 10];
        i[8,6] = 2;
        return FindPath(i, 5, 2);
    }
}

// Toplogical sort
public class TGraph
{
    private int V;   // No. of vertices
    private List<int>[] adj; // Adjacency List

    //Constructor
    public TGraph(int v)
    {
        V = v;
        adj = new List<int>[v];
        for (int i = 0; i < v; ++i)
            adj[i] = new List<int>();
    }

    // Function to add an edge into the graph
    public void AddEdge(int v, int w) { adj[v].Add(w); }

    // A recursive function used by topologicalSort
    private void topologicalSortUtil(int v, bool[] visited,
                             Stack<int> stack)
    {
        // Mark the current node as visited.
        visited[v] = true;
       
        // Recur for all the vertices adjacent to this
        // vertex
        foreach (int i in this.adj[v])
        {
            if (!visited[i])
                topologicalSortUtil(i, visited, stack);
        }

        // Push current vertex to stack which stores result
        stack.Push(v);
    }

    // The function to do Topological Sort. It uses
    // recursive topologicalSortUtil()
    public void topologicalSort()
    {
        Stack<int> stack = new Stack<int>();

        // Mark all the vertices as not visited
        bool[] visited = new bool[V];
      
        // Call the recursive helper function to store
        // Topological Sort starting from all vertices
        // one by one
        for (int i = 0; i < V; i++)
            if (visited[i] == false)
                topologicalSortUtil(i, visited, stack);

        // Print contents of stack
        while (stack.Count > 0)
            Console.WriteLine(stack.Pop() + " ");
    }

    // Driver method
    public static void main()
    {
        // Create a graph given in the above diagram
        TGraph g = new TGraph(6);
        g.AddEdge(5, 2);
        g.AddEdge(5, 0);
        g.AddEdge(4, 0);
        g.AddEdge(4, 1);
        g.AddEdge(2, 3);
        g.AddEdge(3, 1);

        g.topologicalSort();
    }
}

public class MinSum
{
    public static int FindLongestPathForAll(int[,] mat, int n)
    {
        int result = 1;
        int[,] dp = new int[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                dp[i, j] = -1;

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                if (dp[i, j] == -1)
                    dp[i, j] = FindLongestPath(mat, n, i, j, dp);

                result = Math.Max(result, dp[i, j]);
            }

        return result;
    }

    private static int FindLongestPath(int[,] mat, int n, int i, int j, int[,] dp)
    {
        if (i < 0 || i >= n || j < 0 || j >= n)
            return 0;

        if (dp[i, j] != -1)
            return dp[i, j];

        if (i < n - 1 && (mat[i, j] + 1) == mat[i + 1, j])
            return dp[i, j] = 1 + FindLongestPath(mat, n, i + 1, j, dp);

        if (i > 0 && (mat[i, j] + 1) == mat[i - 1, j])
            return dp[i, j] = i + FindLongestPath(mat, n, i - 1, j, dp);

        return dp[i, j] = 1;
    }
}
