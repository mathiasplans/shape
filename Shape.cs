using System.Collections.Generic;
using System.Drawing;

namespace Shape {
    public interface IShape {
        string Symbol {get;}
        Attributes Attributes {get;}
        List<IShape> NextShapes();
        List<Point> GetVertices();
    }
}