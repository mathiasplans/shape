using System.Collections.Generic;
using System.Drawing;

public interface Shape {
    string Symbol {get;}
    List<Shape> NextShapes();
    List<Point> GetVertices();
}