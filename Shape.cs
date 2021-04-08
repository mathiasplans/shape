using System.Collections.Generic;
using System;

namespace Shape {
    public interface IShape {
        Type Symbol {get;}
        Attributes Attributes {get;}
        ShapeGraph Graph {get;}
        static ShapeGraph Prototype() {
            return null;
        }
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