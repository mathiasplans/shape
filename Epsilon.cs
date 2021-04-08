using System.Collections.Generic;
using System;

namespace Shape {
    public class Epsilon : IShape {
        public Type Symbol {get { return typeof(Epsilon);}}
        public Attributes Attributes {get {return null;}}

        public ShapeGraph Graph {get {return new ShapeGraph();}}

        public static ShapeGraph Prototype() {
            return new ShapeGraph();
        }

        public List<IShape> NextShapes() {
            return new List<IShape>();
        }

        public List<Vertex> GetVertices() {
            return new List<Vertex>();
        }

        public List<Line> GetLines() {
            return new List<Line>();
        }
    }
}