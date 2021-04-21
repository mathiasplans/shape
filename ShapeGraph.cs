using System.Collections;
using System.Collections.Generic;
using System;

namespace Shape {
    public class ShapeGraph {
        protected class SGNode {
            private Type symbol;
            private IShape shape;
            private HashSet<(SGNode, Attributes)> connections;

            public IShape Shape {get {return this.shape;}}

            public HashSet<(SGNode, Attributes)> Connections {get {return this.connections;}}

            public SGNode(IShape shape) {
                this.shape = shape;
                this.symbol = shape.Symbol;
                this.connections = new HashSet<(SGNode, Attributes)>();
            }

            public SGNode(Type symbol) {
                this.shape = null;
                this.symbol = symbol;
                this.connections = new HashSet<(SGNode, Attributes)>();
            }

            public void Connect(SGNode other, Attributes attr) {
                this.connections.Add((other, attr));
                other.connections.Add((this, attr));
            }

            public void Remove() {
                foreach ((SGNode, Attributes) connection in this.connections) {
                    connection.Item1.connections.Remove((this, connection.Item2));
                }
            }

            public static bool operator ==(SGNode one, SGNode other) {
                return one.symbol == other.symbol;
            }

            public static bool operator !=(SGNode one, SGNode other) {
                return one.symbol != other.symbol;
            }

            public override bool Equals(object obj) {
                return base.Equals(obj);
            }

            public override int GetHashCode() {
                return base.GetHashCode();
            }
        }

        protected HashSet<SGNode> nodes;

        private HashSet<SGNode> AddGraph(ShapeGraph other, HashSet<SGNode> edges) {
            HashSet<SGNode> shapeNodes = other.nodes;
            
            if (other == null)
                return shapeNodes;

            // For each node in the other shape graph, check
            // if there are connections to this shape graph
            foreach (SGNode otherNode in shapeNodes) {
                foreach (SGNode ownNode in edges) {
                    // NOTE: LineOverlap is valid for 2D only!
                    if (otherNode.Shape.LineOverlap(ownNode.Shape))
                        otherNode.Connect(ownNode, null);
                } 
            }

            // Then add the nodes in the other graph to own graph
            foreach (SGNode otherNode in shapeNodes) {
                this.nodes.Add(otherNode);
            }

            return shapeNodes;
        }

        private HashSet<SGNode> AddGraph(ShapeGraph sg) {
            return this.AddGraph(sg, this.nodes);
        }

        private void RemoveGraph(ShapeGraph other) {
            // TODO
        }

        private void RemoveNode(SGNode other) {
            // Remove it from the set
            this.nodes.Remove(other);

            // Disconnect the edges
            other.Remove();
        }

        public ShapeGraph() {
            this.nodes = new HashSet<SGNode>();
        }

        public ShapeGraph(IShape shape) {
            this.nodes = new HashSet<SGNode>();

            this.nodes.Add(new SGNode(shape));
        }

        public ShapeGraph(Type symbol) {
            this.nodes = new HashSet<SGNode>();

            this.nodes.Add(new SGNode(symbol));
        }

        public ShapeGraph(List<IShape> shapes) {
            this.nodes = new HashSet<SGNode>();

            foreach (IShape shape in shapes) {
                this.AddGraph(shape.Graph);
            }
        }

        private List<(SGNode, Type)> GetNonTerminals(HashSet<(ShapeGraph, Type)> prototypes) {
            List<(SGNode, Type)> o = new List<(SGNode, Type)>();
            foreach (SGNode node in this.nodes) {
                foreach ((ShapeGraph, Type) prototype in prototypes) {
                    // NOTE: This only works with one node!
                    foreach (SGNode protonode in prototype.Item1.nodes) {
                        if (node == protonode)
                            o.Add((node, prototype.Item2));
                    }
                }
            }

            return o;
        }

        public void Interpret(HashSet<(ShapeGraph, Type)> prototypes, ControlGrammar cg) {
            // Get all the non-terminals
            List<(SGNode, Type)> nonterminals = this.GetNonTerminals(prototypes);

            // Take a random
            Random rnd = new Random();
            int r = rnd.Next(nonterminals.Count);
            (SGNode, Type) randomNonTerminal = nonterminals[r];

            // Get the shape
            IShape shape = randomNonTerminal.Item1.Shape;

            // Apply a rule on it
            List<IShape> newShapes = shape.NextShapes();

            // Get the edges of that node
            HashSet<SGNode> removedEdges = new HashSet<SGNode>();
            foreach ((SGNode node, Attributes a) c in randomNonTerminal.Item1.Connections) {
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

            // Control grammar
            if (shape.Control != "") {
                HashSet<((uint, uint), string, float, string)> attributeOverwrite = cg.Interpret(shape.Control);

                Dictionary<(uint, uint), SGNode> nodePlacement = new Dictionary<(uint, uint), SGNode>();
                List<(uint, uint)> locators = new List<(uint, uint)>();
                foreach (SGNode node in splitGraph.nodes) {
                    nodePlacement.Add(node.Shape.Locator, node);
                    locators.Add(node.Shape.Locator);
                }

                HashSet<(uint, uint)> handledLocators = new HashSet<(uint, uint)>();

                // Console.WriteLine("Control");
                foreach (((uint x, uint y) loc, string attr, float val, string next) in attributeOverwrite) {
                    // Get all the locators meant by the locator
                    List<(uint, uint)> reqLocators = new List<(uint, uint)>();
                    foreach ((uint x, uint y) all in locators) {
                        if ((loc.x == all.x || loc.x == ~0u) && (loc.y == all.y || loc.y == ~0u)) {
                            if (loc.x == all.x && loc.y == all.y)
                                reqLocators.Add(all);

                            else if (!handledLocators.Contains(all))
                                reqLocators.Add(all);

                            handledLocators.Add(all);
                        }
                    }

                    foreach ((uint x, uint y) all in reqLocators) {
                        SGNode node = nodePlacement[all];
                        
                        // Set the attribute
                        float before = node.Shape.Attributes.Get(attr).Start;
                        node.Shape.Attributes.Set(attr, new ScalarAttribute(val));
                        float after = node.Shape.Attributes.Get(attr).Start;

                        // Add the control symbol
                        node.Shape.Control = next;
                        // Console.WriteLine($"{node.GetHashCode()} {all.x} {all.y}: {before} -> {val} ({after}) {next}");
                    }
                }
            }

            // TODO: WFC

            // Merge the split graph with this graph
            this.AddGraph(splitGraph, removedEdges);
        }

        public List<IShape> GetShapes() {
            List<IShape> shapes = new List<IShape>();
            foreach (SGNode node in this.nodes) {
                shapes.Add(node.Shape);
            }

            return shapes;
        }
    }
}