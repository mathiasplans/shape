using System.Collections;
using System.Collections.Generic;
using System;

namespace Shape {
    public class ShapeGraph {
        public class Node {
            private Type symbol;
            private IShape shape;
            private HashSet<(Node, Attributes)> connections;
            public IShape Shape {get {return this.shape;}}

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
                return one.symbol == other.symbol;
            }

            public static bool operator !=(Node one, Node other) {
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

        public HashSet<Node>.Enumerator GetEnumerator() {
            return this.nodes.GetEnumerator();
        }

        private HashSet<Node> AddGraph(ShapeGraph other, HashSet<Node> edges) {
            HashSet<Node> shapeNodes = other.nodes;
            
            if (other == null)
                return shapeNodes;

            // For each node in the other shape graph, check
            // if there are connections to this shape graph
            foreach (Node otherNode in shapeNodes) {
                foreach (Node ownNode in edges) {
                    // NOTE: LineOverlap is valid for 2D only!
                    if (otherNode.Shape.LineOverlap(ownNode.Shape))
                        otherNode.Connect(ownNode, null);
                } 
            }

            // Then add the nodes in the other graph to own graph
            foreach (Node otherNode in shapeNodes) {
                this.nodes.Add(otherNode);
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

            // Disconnect the edges
            other.Remove();
        }

        public ShapeGraph() {
            this.nodes = new HashSet<Node>();
        }

        public ShapeGraph(IShape shape) {
            this.nodes = new HashSet<Node>();

            this.nodes.Add(new Node(shape));
        }

        public ShapeGraph(Type symbol) {
            this.nodes = new HashSet<Node>();

            this.nodes.Add(new Node(symbol));
        }

        public ShapeGraph(List<IShape> shapes) {
            this.nodes = new HashSet<Node>();

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
                        if (node == protonode)
                            o.Add((node, prototype.Item2));
                    }
                }
            }

            return o;
        }

        public void Interpret(HashSet<(ShapeGraph, Type)> prototypes, Control control) {
            // Get all the non-terminals
            List<(Node, Type)> nonterminals = this.GetNonTerminals(prototypes);

            // If no non-terminals exist, it is finished
            // TODO: return boolean value
            if (nonterminals.Count == 0)
                return;

            // Take a random
            Random rnd = new Random();
            int r = rnd.Next(nonterminals.Count);
            (Node, Type) randomNonTerminal = nonterminals[r];

            // Get the shape
            IShape shape = randomNonTerminal.Item1.Shape;

            // Apply a rule on it
            List<IShape> newShapes = shape.NextShapes();

            // If the size of new shapes is 0, it is a terminal
            if (newShapes.Count == 0)
                return;

            // Get the edges of that node
            HashSet<Node> removedEdges = new HashSet<Node>();
            foreach ((Node node, Attributes a) c in randomNonTerminal.Item1.Connections) {
                removedEdges.Add(c.node);
            }

            // Remove the original node from the graph
            this.RemoveNode(randomNonTerminal.Item1);

            // We need to get all the new nodes
            ShapeGraph splitGraph = new ShapeGraph();

            // Add all the new shapes to the graph
            foreach (IShape ns in newShapes) {
                splitGraph.AddGraph(ns.Graph);
            }

            control.Inerpret(shape, splitGraph);

            // Merge the split graph with this graph
            this.AddGraph(splitGraph, removedEdges);
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