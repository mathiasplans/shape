public interface IAttribute {
    float Start {get;}
    float End {get;}
    bool Matches(IAttribute other);
}