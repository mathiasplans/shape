using System.Collections.Generic;
using System.Drawing;
using System;

namespace Shape {
    public class Epsilon : IShape {
        public string Symbol {get { return "Epsilon";}}
        public Attributes Attributes {get {return null;}}

        public List<IShape> NextShapes() {
            return new List<IShape>();
        }

        public List<Point> GetVertices() {
            return new List<Point>();
        }
    }
}