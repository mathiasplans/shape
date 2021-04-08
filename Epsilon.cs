using System.Collections.Generic;
using System;

namespace Shape {
    public class Epsilon : IShape {
        public string Symbol {get { return "Epsilon";}}
        public Attributes Attributes {get {return null;}}

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