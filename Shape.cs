using System.Collections.Generic;

namespace Shape {
    public interface IShape {
        string Symbol {get;}
        Attributes Attributes {get;}
        List<IShape> NextShapes();
        List<Vertex> GetVertices();
        List<Line> GetLines();

        bool LineOverlap(IShape other) {
            foreach (Line line1 in this.GetLines()) {
                foreach (Line line2 in other.GetLines()) {
                    if (line1.TrueColinear(line2))
                        return true;
                }
            }

            return false;
        }
    }
}