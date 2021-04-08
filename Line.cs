using System;

public class Line {
    private static readonly float error = 0.001f;

    public Vertex p1, p2;
    private Vertex vector;
    float length;

    public float Length {get {return this.length;}}

    public Line(Vertex p1, Vertex p2) {
        this.p1 = p1;
        this.p2 = p2;

        this.vector = p2 - p1;
        this.length = this.vector.Magnitude();
    }

    private static bool LengthMatch(Line l1, Line l2, Line l3) {
        float combinedLength = l1.Length + l2.Length;
        return (combinedLength + Line.error) > l3.Length && (combinedLength - Line.error) < l3.Length;
    }

    public bool Coincident(Vertex p3) {
        Line l1 = new Line(this.p1, p3);
        Line l2 = new Line(this.p2, p3);

        return LengthMatch(l1, l2, this);
    }

    private bool Colinear(Line other, bool t) {
        // Create two pseudo-lines
        Line l1 = new Line(this.p1, other.p2);
        Line l2 = new Line(other.p1, this.p2);

        // Find the line with the largest length
        Line largest = this;

        if (other.Length > largest.Length)
            largest = other;

        if (l1.Length > largest.Length)
            largest = l1;
        
        if (l2.Length > largest.Length)
            largest = l2;

        if (t && LengthMatch(this, other, largest))
            return false;

        // See if all the points of both lines are coincidental with the large line
        return largest.Coincident(this.p1) && largest.Coincident(this.p2) && largest.Coincident(other.p1) && largest.Coincident(other.p2);
    }

    public bool Colinear(Line other) {
        return this.Colinear(other, false);
    }

    public bool TrueColinear(Line other) {
        return this.Colinear(other, true);
    }
}