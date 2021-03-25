using System;

public class StrictAttribute : IAttribute {
    private readonly float start, end;

    public StrictAttribute(float start, float end) {
        this.start = start;
        this.end = end;
    }

    public float Start {get {return this.start;}}
    public float End {get {return this.end;}}

    public bool Matches(IAttribute other) {
        return this.Start <= other.Start && this.End >= other.End;
    }
}