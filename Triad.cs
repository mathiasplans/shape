using System.Collections.Generic;
using System.Drawing;
using System;

namespace Shape {
    public class Triad : IShape {
        private static readonly List<IShape> dummyList = new List<IShape>();
        private Attributes attributes;
        public Point v1, v2, v3;
        private Rules rules;
        private Random rnd;
        
        public string Symbol {get { return "Triad";}}
        public Attributes Attributes {get {return this.attributes;}}

        private void InitializeVertices((Point v1, Point v2, Point v3) vertices) {
            this.v1 = vertices.v1;
            this.v2 = vertices.v2;
            this.v3 = vertices.v3;
        }
        
        public Triad(Rules rules, (Point v1, Point v2, Point v3) vertices) {
            this.InitializeVertices(vertices);
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = new Attributes();
        }

        public Triad(Rules rules, Attributes attributes, (Point v1, Point v2, Point v3) vertices) {
            this.InitializeVertices(vertices);
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = attributes;
        }

        public List<IShape> NextShapes() {
            return this.rules.Next<Triad>(this.attributes)(this);
        }

        public List<Point> GetVertices() {
            return new List<Point> {this.v1, this.v2, this.v3};
        }
    }
}