public interface IAttribute {
    float Start {get;}
    float End {get;}
    bool Matches(IAttribute other);

    string Range() {
        return $"[{this.Start}, {this.End}]";
    }
}