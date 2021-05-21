using System.Collections;
using System.Collections.Generic;
using System;

namespace Shape {
    public class ShapeGraph {
        public class Node {
            private Type symbol;
            private IShape shape;
            private HashSet<(Node, Attributes)> connections;
            private bool isterm = false;
            public IShape Shape {get {return this.shape;}}
            public bool IsTerminal {get {return this.isterm;} set {this.isterm = value;}}

            public HashSet<(Node, Attributes)> Connections {get {return this.connections;}}

            public Node(IShape shape) {
                this.shape = shape;
                this.symbol = shape.Symbol;
                this.connections = new HashSet<(Node, Attributes)>();
            }

            public Node(Type symbol) {
                this.shape = null;
                this.symbol = symbol;
                this.connections = new HashSet<(Node, Attributes)>();
            }

            public void Connect(Node other, Attributes attr) {
                this.connections.Add((other, attr));
                other.connections.Add((this, attr));
            }

            public void Remove() {
                foreach ((Node, Attributes) connection in this.connections) {
                    connection.Item1.connections.Remove((this, connection.Item2));
                }
            }

            public static bool operator ==(Node one, Node other) {
                if (one is null)
                    return other is null;

                if (other is null)
                    return false;

                return one.symbol == other.symbol;
            }

            public static bool operator !=(Node one, Node other) {
                if (one is null)
                    return !(other is null);

                if (other is null)
                    return true;

                return one.symbol != other.symbol;
            }

            public override bool Equals(object obj) {
                return base.Equals(obj);
            }

            public override int GetHashCode() {
                return base.GetHashCode();
            }
        }

        protected HashSet<Node> nodes;
        protected Dictionary<IShape, Node> shapeMap;

        public HashSet<Node>.Enumerator GetEnumerator() {
            return this.nodes.GetEnumerator();
        }

        public int Count {get {return this.nodes.Count;}}

        private HashSet<Node> AddGraph(ShapeGraph other, HashSet<Node> edges) {
            if (other == null)
                return new HashSet<Node>();

            HashSet<Node> shapeNodes = other.nodes;

            // For each node in the other shape graph, check
            // if there are connections to this shape graph
            foreach (Node otherNode in shapeNodes) {
                foreach (Node ownNode in edges) {
                    // NOTE: LineOverlap is valid for 2D only!
                    if (otherNode.Shape.LineOverlap(ownNode.Shape)) {
                        otherNode.Connect(ownNode, null);
                    }
                }
            }

            // Then add the nodes in the other graph to own graph
            foreach (Node otherNode in shapeNodes) {
                this.nodes.Add(otherNode);
                this.shapeMap.Add(otherNode.Shape, otherNode);
            }

            return shapeNodes;
        }

        private HashSet<Node> AddGraph(ShapeGraph sg) {
            return this.AddGraph(sg, this.nodes);
        }

        private void RemoveGraph(ShapeGraph other) {
            // TODO
        }

        private void RemoveNode(Node other) {
            // Remove it from the set
            this.nodes.Remove(other);
            this.shapeMap.Remove(other.Shape);

            // Disconnect the edges
            other.Remove();
        }

        public ShapeGraph() {
            this.nodes = new HashSet<Node>();
            this.shapeMap = new Dictionary<IShape, Node>();
        }

        public ShapeGraph(IShape shape) {
            this.nodes = new HashSet<Node>();
            this.shapeMap = new Dictionary<IShape, Node>();

            Node newNode = new Node(shape);
            this.nodes.Add(newNode);
            this.shapeMap.Add(shape, newNode);
        }

        public ShapeGraph(Type symbol) {
            this.nodes = new HashSet<Node>();
            this.shapeMap = new Dictionary<IShape, Node>();

            Node newNode = new Node(symbol);
            this.nodes.Add(newNode);
        }

        public ShapeGraph(List<IShape> shapes) {
            this.nodes = new HashSet<Node>();
            this.shapeMap = new Dictionary<IShape, Node>();

            foreach (IShape shape in shapes) {
                this.AddGraph(shape.Graph);
            }
        }

        private List<(Node, Type)> GetNonTerminals(HashSet<(ShapeGraph, Type)> prototypes) {
            List<(Node, Type)> o = new List<(Node, Type)>();
            foreach (Node node in this.nodes) {
                foreach ((ShapeGraph, Type) prototype in prototypes) {
                    // NOTE: This only works with one node!
                    foreach (Node protonode in prototype.Item1.nodes) {
                        if (node == protonode && !node.IsTerminal)
                            o.Add((node, prototype.Item2));
                    }
                }
            }

            return o;
        }

        public (bool, bool) Interpret(HashSet<(ShapeGraph, Type)> prototypes, Control control) {
            // Get all the non-terminals
            List<(Node, Type)> nonterminals = this.GetNonTerminals(prototypes);

            // If no non-terminals exist, it is finished
            // TODO: return boolean value
            if (nonterminals.Count == 0)
                return (false, true);

            // Take a random
            Random rnd = new Random();
            int r = rnd.Next(nonterminals.Count);
            (Node, Type) randomNonTerminal = nonterminals[r];

            // Get the shape
            IShape shape = randomNonTerminal.Item1.Shape;

            // Apply a rule on it
            List<IShape> newShapes = shape.NextShapes();

            // If the size of new shapes is 0, it is a terminal
            if (newShapes.Count == 0) {
                randomNonTerminal.Item1.IsTerminal = true;
                return (true, false);
            }

            // Get the virtual connection
            IShape vcshape = null;
            Node vcnode = null;
            bool vc = false;
            if (shape.VC != null) {
                vcshape = shape.VC.Other(shape);
                vcnode = this.shapeMap[vcshape];
                vc = vcnode != null;
            }

            // Get the edges of that node
            HashSet<Node> removedEdges = new HashSet<Node>();
            foreach ((Node node, Attributes a) c in randomNonTerminal.Item1.Connections) {
                if (c.node != vcnode)
                    removedEdges.Add(c.node);
            }

            // Also add virtual connection edges
            if (vc) {
                foreach ((Node node, Attributes a) c in vcnode.Connections) {
                    if (c.node != randomNonTerminal.Item1)
                        removedEdges.Add(c.node);
                }
            }

            // Remove the original node from the graph
            this.RemoveNode(randomNonTerminal.Item1);

            if (vc) {
                this.RemoveNode(vcnode);
            }

            // We need to get all the new nodes
            ShapeGraph splitGraph = new ShapeGraph();
            ShapeGraph vcGraph = new ShapeGraph();

            // Add all the new shapes to the graph
            if (vc) {
                int half = newShapes.Count / 2;
                for (int i = 0; i < half; ++i) {
                    splitGraph.AddGraph(newShapes[i].Graph);
                    vcGraph.AddGraph(newShapes[i + half].Graph);
                }
            }

            else {
                foreach (IShape ns in newShapes) {
                    splitGraph.AddGraph(ns.Graph);
                }
            }

            ShapeGraph mergedGraph = new ShapeGraph();
            mergedGraph.AddGraph(splitGraph);
            mergedGraph.AddGraph(vcGraph);

            if (!vc)
                control.Interpret(shape, mergedGraph);

            else {
                control.Interpret(shape, splitGraph, vcGraph);
            }

            // Merge the split graph with this graph
            this.AddGraph(mergedGraph, removedEdges);

            return (true, true);
        }

        public List<IShape> GetShapes() {
            List<IShape> shapes = new List<IShape>();
            foreach (Node node in this.nodes) {
                shapes.Add(node.Shape);
            }

            return shapes;
        }
    }
}
