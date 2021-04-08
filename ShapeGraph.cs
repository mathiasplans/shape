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

            public SGNode(IShape shape) {
                this.shape = shape;
                this.symbol = shape.Symbol;
                this.connections = new HashSet<(SGNode, Attributes)>();
            }

            public void Connect(SGNode other, Attributes attr) {
                this.connections.Add((other, attr));
                other.connections.Add((this, attr));
            }
        }

        protected HashSet<SGNode> nodes;

        private void AddGraph(ShapeGraph other) {
            if (other == null)
                return;

            // Get the nodes
            HashSet<SGNode> shapeNodes = other.nodes;

            // For each node in the other shape graph, check
            // if there are connections to this shape graph
            foreach (SGNode otherNode in shapeNodes) {
                foreach (SGNode ownNode in this.nodes) {
                    // NOTE: LineOverlap is valid for 2D only!
                    if (otherNode.Shape.LineOverlap(ownNode.Shape))
                        otherNode.Connect(ownNode, null);
                } 
            }

            // Then add the nodes in the other graph to own graph
            foreach (SGNode otherNode in shapeNodes) {
                this.nodes.Add(otherNode);
            }
        }

        public ShapeGraph() {
            this.nodes = new HashSet<SGNode>();
        }

        public ShapeGraph(IShape shape) {
            this.nodes = new HashSet<SGNode>();

            this.AddGraph(shape.Graph);
        }

        public ShapeGraph(List<IShape> shapes) {
            this.nodes = new HashSet<SGNode>();

            foreach (IShape shape in shapes) {
                this.AddGraph(shape.Graph);
            }
        }
    }
}