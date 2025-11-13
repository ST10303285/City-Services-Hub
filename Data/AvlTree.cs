using System;
using System.Collections.Generic;
using MunicipalServicesApp.Model;

namespace MunicipalServicesApp.Data
{
    // AVL Tree to store Service Requests in sorted order by date (oldest → newest)
    public class AvlTree
    {
        // internal node type for the AVL tree
        private class Node
        {
            public ServiceRequest Data;
            public Node Left;
            public Node Right;
            public int Height;

            public Node(ServiceRequest data)
            {
                Data = data;
                Height = 1; // new node has height 1
            }
        }

        private Node root;

        //---------------------------------------------------------------- helper methods --------------------------------------------------//

        // Return height of a node (0 if null)
        private int Height(Node n) => n?.Height ?? 0;

        // Balance factor = height(left) - height(right)
        private int BalanceFactor(Node n) => (n == null) ? 0 : Height(n.Left) - Height(n.Right);

        // Right rotation:
        //      y                              x
        //     / \     Right Rotate (y)       / \
        //    x   T3   - - - - - - - >       T1  y
        //   / \                                / \
        //  T1  T2                             T2 T3
        private Node RotateRight(Node y)
        {
            var x = y.Left;
            var t2 = x.Right;

            // perform rotation
            x.Right = y;
            y.Left = t2;

            // update heights
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            // new root of this subtree
            return x;
        }

        // Left rotation:
        //    x                             y
        //   / \     Left Rotate (x)       / \
        //  T1  y   - - - - - - - - >     x  T3
        //     / \                       / \
        //    T2 T3                     T1 T2
        private Node RotateLeft(Node x)
        {
            var y = x.Right;
            var t2 = y.Left;

            // perform rotation
            y.Left = x;
            x.Right = t2;

            // update heights
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            // new root of this subtree
            return y;
        }

        // Compare two ServiceRequest objects by DateReported then Id as tiebreaker
        private int Compare(ServiceRequest a, ServiceRequest b)
        {
            int c = a.DateReported.CompareTo(b.DateReported);
            if (c != 0) return c;
            return string.Compare(a.Id, b.Id, StringComparison.Ordinal);
        }

        //---------------------------------------------------- public API ------------------------------------------------//

        // Insert a request into the AVL tree (no-op if item is null)
        public void Insert(ServiceRequest item)
        {
            if (item == null) return;
            root = InsertInternal(root, item);
        }

        // Search by Id (returns matching ServiceRequest or null)
        public ServiceRequest SearchById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return SearchByIdInternal(root, id);
        }

        // Traverse in-order (oldest -> newest). Accepts an action to call for each node.
        public void InOrderTraverse(Action<ServiceRequest> visit)
        {
            InOrderInternal(root, visit);
        }

        // Get most recent N requests (newest first)
        public List<ServiceRequest> GetMostRecent(int n)
        {
            var result = new List<ServiceRequest>();
            if (n <= 0) return result;
            GetMostRecentInternal(root, result, n);
            return result;
        }

        //--------------------------------------------- Internal implementation --------------------------------------------//

        // Recursive insert that maintains AVL balance
        private Node InsertInternal(Node node, ServiceRequest item)
        {
            // normal BST insert
            if (node == null)
                return new Node(item);

            if (Compare(item, node.Data) < 0)
                node.Left = InsertInternal(node.Left, item);
            else
                node.Right = InsertInternal(node.Right, item);

            // update height
            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            // check balance
            int balance = BalanceFactor(node);

            // Four rebalancing cases:

            // Left Left Case
            if (balance > 1 && Compare(item, node.Left.Data) < 0)
                return RotateRight(node);

            // Right Right Case
            if (balance < -1 && Compare(item, node.Right.Data) > 0)
                return RotateLeft(node);

            // Left Right Case
            if (balance > 1 && Compare(item, node.Left.Data) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right Left Case
            if (balance < -1 && Compare(item, node.Right.Data) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            // already balanced
            return node;
        }

        // Simple tree-wide search by id (DFS). Returns first match found.
        private ServiceRequest SearchByIdInternal(Node node, string id)
        {
            if (node == null) return null;
            if (node.Data.Id == id) return node.Data;

            var left = SearchByIdInternal(node.Left, id);
            if (left != null) return left;
            return SearchByIdInternal(node.Right, id);
        }

        // In-order traversal helper
        private void InOrderInternal(Node node, Action<ServiceRequest> visit)
        {
            if (node == null) return;
            InOrderInternal(node.Left, visit);
            visit?.Invoke(node.Data);
            InOrderInternal(node.Right, visit);
        }

        // Collect most recent items by traversing right (newest) first
        private void GetMostRecentInternal(Node node, List<ServiceRequest> acc, int n)
        {
            if (node == null || acc.Count >= n) return;
            GetMostRecentInternal(node.Right, acc, n);
            if (acc.Count < n) acc.Add(node.Data);
            GetMostRecentInternal(node.Left, acc, n);
        }
    }
}
