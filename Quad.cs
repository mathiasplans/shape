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
        private VirtualConnection vc = null;
        private string name = "Quad";

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
        public Vertex Center {get {
            Vertex c = new Vertex(0f, 0f);
            foreach (Vertex vert in this.v) {
                c += vert;
            }

            c /= 4f;

            return c;
        }}
        public string Name {get {return this.name;} set {this.name = value;}}


        public static ShapeGraph Prototype() {
            return new ShapeGraph(typeof(Quad));
        }

        public static IShape Etalon(Rules rules, float width) {
            Vertex v1 = new Vertex(0, 0);
            Vertex v2 = new Vertex(width, 0);
            Vertex v3 = new Vertex(width, width);
            Vertex v4 = new Vertex(0, width);

            return new Quad(rules, (v1, v2, v3, v4));
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
                List<IShape> vcShapes = shapeGetter(other);

                if (this.VC.Persistant) {
                    for (int i = 0; i < newShapes.Count; ++i) {
                        VirtualConnection.Connect(newShapes[i], vcShapes[i], false, false, true);
                    }
                }

                newShapes.AddRange(vcShapes);
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

        public IShape Copy() {
            return new Quad(this.rules, this.attributes.Copy(), this.locator, (this.v1.Copy(), this.v2.Copy(), this.v3.Copy(), this.v4.Copy()));
        }

        public override string ToString() {
            return "Q";
        }
    }
}