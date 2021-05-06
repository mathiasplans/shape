using System;

public class ControlAssignment {
    private (uint x, uint y) loc;
    private string attrname;
    private float value;
    private string next;
    private readonly bool terminal;

    public (uint x, uint y) Loc {get {return this.loc;}}
    public string Attr {get {return this.attrname;}}
    public float Value {get {return this.value;}}
    public string Next {get {return this.next;}}
    public bool IsTerminal {get {return this.terminal;}}

    private ControlAssignment((uint, uint) loc, string attrname, float value, string next, bool terminal) {
        this.loc = loc;
        this.attrname = attrname;
        this.value = value;
        this.next = next;
        this.terminal = terminal;
    }

    public ControlAssignment((uint x, uint y) loc, string attrname, float value, string next) {
        this.loc = loc;
        this.attrname = attrname;
        this.value = value;
        this.next = next;
        this.terminal = true;
    }


    public ControlAssignment((uint x, uint y) loc, string next) {
        this.loc = loc;
        this.attrname = "";
        this.value = 0f;
        this.next = next;
        this.terminal = true;
    }

    public ControlAssignment((uint x, uint y) loc, string wfc, string next) {
        this.loc = loc;
        this.attrname = wfc;
        this.value = 0f;
        this.next = next;
        this.terminal = false;
    }

    public ControlAssignment Copy() {
        return new ControlAssignment(this.loc, this.attrname, this.value, this.next, this.terminal);
    }

    public void SetLoc((uint x, uint y) loc) {
        this.loc = loc;
    }

    public static implicit operator ControlAssignment(
        ((uint, uint) loc, string attrname, float value, string next) a)
        => new ControlAssignment(a.loc, a.attrname, a.value, a.next);

    public static implicit operator ControlAssignment(
        (string attrname, float value, string next) a)
        => new ControlAssignment((~0u, ~0u), a.attrname, a.value, a.next);

    public static implicit operator ControlAssignment(
        ((uint, uint) loc, string next) a)
        => new ControlAssignment(a.loc, a.next);

    public static implicit operator ControlAssignment(
        ((uint, uint) loc, string wfc, string next) a)
        => new ControlAssignment(a.loc, a.wfc, a.next);
    public static implicit operator ControlAssignment(
        string next) 
        => new ControlAssignment((~0u, ~0u), "", 0f, next);

}