using System;

public class RangeAttribute : IAttribute {
    private readonly float start, end;

    public RangeAttribute(float start, float end) {
        this.start = start;
        this.end = end;
    }

    public float Start {get {return this.start;}}
    public float End {get {return this.end;}}

    public bool Matches(IAttribute other) {
        return this.Start <= other.End && this.End >= other.Start;
    }
}