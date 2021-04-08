using System;
using System.Drawing;

public class Vertex {
    private (float, float) point;

    public Vertex(float x, float y) {
        this.point = (x, y);
    }

    public float x {
        get {return this.point.Item1;}
    }

    public float y {
        get {return this.point.Item2;}
    }

    public static Vertex operator -(Vertex v1) {
        return new Vertex(-v1.x, -v1.y);
    }

    public static Vertex operator +(Vertex v1, Vertex v2) {
        return new Vertex(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vertex operator +(Vertex v1, float s1) {
        return new Vertex(v1.x + s1, v1.y + s1);
    }

    public static Vertex operator -(Vertex v1, Vertex v2) {
        return new Vertex(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vertex operator -(Vertex v1, float s1) {
        return new Vertex(v1.x - s1, v1.y - s1);
    }

    public static Vertex operator *(Vertex v1, Vertex v2) {
        return new Vertex(v1.x * v2.x, v1.y * v2.y);
    }

    public static Vertex operator *(Vertex v1, float s1) {
        return new Vertex(v1.x * s1, v1.y * s1);
    }

    public static Vertex operator /(Vertex v1, Vertex v2) {
        return new Vertex(v1.x / v2.x, v1.y / v2.y);
    }

    public static Vertex operator /(Vertex v1, float s1) {
        return new Vertex(v1.x / s1, v1.y / s1);
    }

    public static implicit operator Vertex(Point p) => new Vertex(p.X, p.Y);
    public static implicit operator Point(Vertex v) => new Point((int)v.x, (int)v.y);
    public static implicit operator Vertex((float, float) cord) => new Vertex(cord.Item1, cord.Item2);
    public static implicit operator (float, float)(Vertex v) => (v.x, v.y);

    public float Magnitude() {
        return (float) Math.Sqrt(this.x * this.x + this.y * this.y);
    }
}