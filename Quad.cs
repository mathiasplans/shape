using System.Collections.Generic;
using System.Drawing;
using System;

public class Quad : Shape {
    public string Symbol {get { return "Quad";}}
    private static readonly List<Shape> dummyList = new List<Shape>();

    public Point v1, v2, v3, v4;
    private Rules rules;
    private Random rnd;

    
    public Quad(Rules rules, Point v1, Point v2, Point v3, Point v4) {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        this.v4 = v4;

        this.rules = rules;
        
        this.rnd = new Random();
    }

    public List<Shape> NextShapes() {
        List<Func<Shape, List<Shape>>> r = this.rules.GetRules<Quad>();
        if (r.Count > 0) {
            int index = this.rnd.Next(r.Count);
            return r[index](this);
        }

        return dummyList;
    }

    public List<Point> GetVertices() {
        return new List<Point> {this.v1, this.v2, this.v3, this.v4};
    }
}