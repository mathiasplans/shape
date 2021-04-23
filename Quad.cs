using System.Collections.Generic;
using System;

namespace Shape {
    public class Quad : IShape {
        private static readonly List<IShape> dummyList = new List<IShape>();
        private Attributes attributes;
        private Vertex[] v;
        private Line[] l;
        private Rules rules;
        private Random rnd;
        private ShapeGraph shapeGraph;
        private (uint x, uint y) locator;
        private string control = "";
        private VirtualConnection vc;

        public Vertex v1 {get {return this.v[0];}}
        public Vertex v2 {get {return this.v[1];}}
        public Vertex v3 {get {return this.v[2];}}
        public Vertex v4 {get {return this.v[3];}}
        public Line l1 {get {return this.l[0];}}
        public Line l2 {get {return this.l[1];}}
        public Line l3 {get {return this.l[2];}}
        public Line l4 {get {return this.l[3];}}


        public Type Symbol {get { return typeof(Quad);}}
        public Attributes Attributes {get {return this.attributes;}}
        public ShapeGraph Graph {get {return this.shapeGraph;}}
        public (uint, uint) Locator {get {return this.locator;}}
        public string Control {get {return this.control;} set {this.control = value;}}
        public VirtualConnection VC {get {return this.vc;} set {this.vc = value;}}


        public static ShapeGraph Prototype() {
            return new ShapeGraph(typeof(Quad));
        }

        private void CalculateLines() {
            this.l = new Line[] {
                new Line(this.v1, this.v2), 
                new Line(this.v2, this.v3), 
                new Line(this.v3, this.v4), 
                new Line(this.v4, this.v1)
            };
        }

        private void InitializeVertices((Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) {
            this.v = new Vertex[] {vertices.v1, vertices.v2, vertices.v3, vertices.v4};
            this.CalculateLines();

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
            var shapeGetter = this.rules.Next<Quad>(this.attributes);
            List<IShape> newShapes = shapeGetter(this);

            if (this.VC != null) {
                IShape other = this.VC.Other(this);
                this.VC.Transform(other);
                newShapes.AddRange(shapeGetter(other));
            }
            
            return newShapes;
        }

        public List<Vertex> GetVertices() {
            return new List<Vertex>(this.v);
        }

        public List<Line> GetLines() {
            return new List<Line>(this.l);
        }

        public void SetVertices(List<Vertex> newVerts) {
            for (int i = 0; i < newVerts.Count; ++i) {
                this.v[i] = newVerts[i];
            }

            this.CalculateLines();
        }
    }
}