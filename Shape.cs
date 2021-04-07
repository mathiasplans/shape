using System.Collections.Generic;

namespace Shape {
    public interface IShape {
        string Symbol {get;}
        Attributes Attributes {get;}
        List<IShape> NextShapes();
        List<float[]> GetVertices();
    }
}