using System.Collections.Generic;
using System;

namespace Shape {
    public class Quad : IShape {
        private static readonly List<IShape> dummyList = new List<IShape>();
        private Attributes attributes;
        public float[] v1, v2, v3, v4;
        private Rules rules;
        private Random rnd;

        public string Symbol {get { return "Quad";}}
        public Attributes Attributes {get {return this.attributes;}}

        private void InitializeVertices((float[] v1, float[] v2, float[] v3, float[] v4) vertices) {
            this.v1 = vertices.v1;
            this.v2 = vertices.v2;
            this.v3 = vertices.v3;
            this.v4 = vertices.v4;
        }
        
        public Quad(Rules rules, (float[] v1, float[] v2, float[] v3, float[] v4) vertices) {
            this.InitializeVertices(vertices);
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = new Attributes();
        }

        public Quad(Rules rules, Attributes attributes, (float[] v1, float[] v2, float[] v3, float[] v4) vertices) {
            this.InitializeVertices(vertices);
            this.rules = rules;
            this.rnd = new Random();
            this.attributes = attributes;
        }

        public List<IShape> NextShapes() {
            return this.rules.Next<Quad>(this.attributes)(this);
        }

        public List<float[]> GetVertices() {
            return new List<float[]> {this.v1, this.v2, this.v3, this.v4};
        }
    }
}