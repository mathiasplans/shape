using System;

public class DefaultAttribute : IAttribute {
    private readonly float start, end;

    public DefaultAttribute() {
        this.start = float.MinValue;
        this.end = float.MaxValue;
    }

    public float Start {get {return this.start;}}
    public float End {get {return this.end;}}

    public bool Matches(IAttribute other) {
        return true;
    }
}