using System;

public class ScalarAttribute : IAttribute {
    private readonly float scalar;

    public ScalarAttribute(float scalar) {
        this.scalar = scalar;
    }

    public float Start {get {return this.scalar;}}
    public float End {get {return this.scalar;}}

    public bool Matches(IAttribute other) {
        return this.scalar <= other.End && this.scalar >= other.Start;
    }
}