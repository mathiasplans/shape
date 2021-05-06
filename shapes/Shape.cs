using System.Collections.Generic;
using System;

namespace Shape {
    public interface IShape {
        Type Symbol {get;}
        Attributes Attributes {get;}
        ShapeGraph Graph {get;}
        (uint, uint) Locator {get;}
        HashSet<string> Control {get;}
        VirtualConnection VC {get; set;}
        Vertex Center {get;}
        string Name {get; set;}
        static ShapeGraph Prototype() {
            return null;
        }
        static IShape Etalon(Rules rules, float width) {
            return null;
        }

        List<IShape> NextShapes();
        List<Vertex> GetVertices();
        List<Line> GetLines();
        void SetVertices(List<Vertex> newVerts);
        bool LineOverlap(IShape other) {
            foreach (Line line1 in this.GetLines()) {
                foreach (Line line2 in other.GetLines()) {
                    if (line1.TrueColinear(line2))
                        return true;
                }
            }

            return false;
        }

        void Transform(float[,] transformation) {
            List<Vertex> vertices = this.GetVertices();
            for (int i = 0; i < vertices.Count; ++i) {
                Vertex vert = vertices[i];
                float[] coord = new float[] {vert.x, vert.y, 1f};
                float[] newcoord = new float[coord.Length];

                for (int col = 0; col < transformation.GetLength(1); ++col) {
                    for (int row = 0; row < coord.Length; ++row) {
                        newcoord[row] += coord[col] * transformation[row, col];
                    }
                }

                vertices[i] = new Vertex(newcoord[0], newcoord[1]);
            }

            this.SetVertices(vertices);
        }

        IShape Copy();
    }
}