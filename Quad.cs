using System.Collections.Generic;
using System;

namespace Shape {
    public class Quad : IShape {
        private static readonly List<IShape> dummyList = new List<IShape>();
        private Attributes attributes;
        public Vertex v1, v2, v3, v4;
        public Line l1, l2, l3, l4;
        private Rules rules;
        private Random rnd;
        private ShapeGraph shapeGraph;
        private (uint x, uint y) locator;
        private string control = "";

        public Type Symbol {get { return typeof(Quad);}}
        public Attributes Attributes {get {return this.attributes;}}
        public ShapeGraph Graph {get {return this.shapeGraph;}}
        public (uint, uint) Locator {get {return this.locator;}}
        public string Control {get {return this.control;} set {this.control = value;}}
        public static ShapeGraph Prototype() {
            return new ShapeGraph(typeof(Quad));
        }


        private void InitializeVertices((Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) {
            this.v1 = vertices.v1;
            this.v2 = vertices.v2;
            this.v3 = vertices.v3;
            this.v4 = vertices.v4;

            this.l1 = new Line(this.v1, this.v2);
            this.l2 = new Line(this.v2, this.v3);
            this.l3 = new Line(this.v3, this.v4);
            this.l4 = new Line(this.v4, this.v1);

            this.shapeGraph = new ShapeGraph(this);
        }
        
        public Quad(Rules rules, (Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) {
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = new Attributes();
            this.locator = (~0u, ~0u);
            this.InitializeVertices(vertices);
        }

        public Quad(Rules rules, Attributes attributes, (Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) {
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = attributes;
            this.locator = (~0u, ~0u);
            this.InitializeVertices(vertices);
        }

        public Quad(Rules rules, Attributes attributes, (uint, uint) locator, (Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) {
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = attributes;
            this.locator = locator;
            this.InitializeVertices(vertices);
        }

        public List<IShape> NextShapes() {
            return this.rules.Next<Quad>(this.attributes)(this);
        }

        public List<Vertex> GetVertices() {
            return new List<Vertex> {this.v1, this.v2, this.v3, this.v4};
        }

        public List<Line> GetLines() {
            return new List<Line> {this.l1, this.l2, this.l3, this.l4};
        }
    }
}