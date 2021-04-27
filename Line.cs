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

    public Vertex Bisect(float x) {
        return this.p1 * x + this.p2 * (1f - x);
    }

    public Vertex Bisect() {
        return this.Bisect(0.5f);
    }

    public Vertex[] Sections(int n) {
        float len = 1f / (float) (n + 1);
        Vertex[] sections = new Vertex[n];

        for (int i = 0; i < n; ++i) {
            sections[i] = this.Bisect(len * (i + 1));
        }

        return sections;
    }

    public Vertex[] Sections(params float[] x) {
        Vertex[] sections = new Vertex[x.Length];
        for (int i = 0; i < x.Length; ++i) {
            sections[i] = this.Bisect(x[i]);
        }

        return sections;
    }

    private static bool LengthMatch(Line l1, Line l2, Line l3) {
        float combinedLength = l1.Length + l2.Length;
        return ((combinedLength + Line.error) > l3.Length) && ((combinedLength - Line.error) < l3.Length);
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

        // See if all the points of both lines are coincidental with the large line
        bool coincidentRule = largest.Coincident(this.p1) && largest.Coincident(this.p2) && largest.Coincident(other.p1) && largest.Coincident(other.p2);

        // See if the length of the largest doesn't exceed the combined length of the two lines
        bool lengthRule = this.Length + other.Length >= largest.Length - Line.error;

        // If true collinear is checked, the lengths should not match
        bool trueRule = true;
        if (t)
            trueRule = !LengthMatch(this, other, largest);

        return coincidentRule && lengthRule && trueRule;
    }

    public bool Colinear(Line other) {
        return this.Colinear(other, false);
    }

    public bool TrueColinear(Line other) {
        return this.Colinear(other, true);
    }

    public override string ToString() {
        return $"{this.p1} - {this.p2}";
    }
}