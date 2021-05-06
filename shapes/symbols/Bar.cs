using System.Collections.Generic;
using System.Collections;
using System;
using Shape;

public class Bar : Quad {
        public Bar(Rules rules, Attributes attributes, (uint, uint) locator, (Vertex v1, Vertex v2, Vertex v3, Vertex v4) vertices) :
        base (rules, attributes, locator, vertices) {
            this.name = "Bar";
        }

        public override Type Symbol {get { return typeof(Bar);}}

        public override string ToString() {
            return "B";
        }

        new public static ShapeGraph Prototype() {
            return new ShapeGraph(typeof(Bar));
        }

        new public static IShape Etalon(Rules rules, float width) {
            Vertex v1 = new Vertex(0, width);
            Vertex v2 = new Vertex(width, width);
            Vertex v3 = new Vertex(width, 0);
            Vertex v4 = new Vertex(0, 0);

            return new Bar(rules, new Attributes(), (0, 0), (v1, v2, v3, v4));
        }

        public override List<IShape> NextShapes() {
            return this.NextShape<Bar>();
        }
}