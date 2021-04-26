using System.Collections.Generic;
using System;

namespace Shape {
    public class Epsilon : IShape {
        private VirtualConnection vc;
        public Type Symbol {get { return typeof(Epsilon);}}
        public Attributes Attributes {get {return null;}}
        public ShapeGraph Graph {get {return new ShapeGraph();}}
        public (uint, uint) Locator {get {return (0u, 0u);}}
        public string Control {get {return "";} set {}}
        public VirtualConnection VC {get {return this.vc;} set {this.vc = value;}}
        public Vertex Center {get {return new Vertex(0f, 0f);}}
        public static ShapeGraph Prototype() {
            return new ShapeGraph();
        }

        public static IShape Etalon(Rules rules, float width) {
            return new Epsilon();
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

        public void SetVertices(List<Vertex> newVerts) {

        }

        public void Transform(float[,] transformation) {

        }

        public override string ToString() {
            return "ε";
        }

        public IShape Copy() {
            return new Epsilon();
        }
    }
}