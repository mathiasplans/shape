using System.Collections.Generic;
using System;

namespace Shape {
    public class Epsilon : IShape {
        public string Symbol {get { return "Epsilon";}}
        public Attributes Attributes {get {return null;}}

        public List<IShape> NextShapes() {
            return new List<IShape>();
        }

        public List<float[]> GetVertices() {
            return new List<float[]>();
        }
    }
}