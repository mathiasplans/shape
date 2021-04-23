using System.Collections.Generic;
using System.Collections;
using System;

namespace Shape {
    public class VirtualConnection {
        private IShape shape1;
        private IShape shape2;
        private Vertex center;
        private Dictionary<int, int> xsymmetry;
        private Dictionary<int, int> ysymmetry;

        private VirtualConnection(IShape shape1, IShape shape2, Vertex center, Dictionary<int, int> xsymmetry, Dictionary<int, int> ysymmetry) {
            this.shape1 = shape1;
            this.shape2 = shape2;
            this.center = center;

            this.xsymmetry = xsymmetry;
            this.ysymmetry = ysymmetry;
        }

        public static void Connect(IShape shape1, IShape shape2, bool symx, bool symy) {
            if (shape1.VC != null || shape2.VC != null)
                throw new Exception("Shape already has a virtual connection!");

            if (shape1.Symbol != shape2.Symbol)
                throw new Exception($"Shapes with different symbold can not be virtually connected: {shape1.Symbol}, {shape2.Symbol}");

            // Check whether the shape is symmetric..
            Vertex center = new Vertex(0f, 0f);
            foreach (Vertex vert in shape1.GetVertices()) {
                center += vert;
            }

            center /= shape1.GetVertices().Count;

            List<float> xcoords = new List<float>();
            List<float> ycoords = new List<float>();
            foreach (Vertex vert in shape1.GetVertices()) {
                xcoords.Add(vert.x - center.x);
                ycoords.Add(vert.y - center.y);
            }

            Dictionary<int, int> xmap = new Dictionary<int, int>();
            Dictionary<int, int> ymap = new Dictionary<int, int>();

            // ..in respect to x-axis
            if (symx) {
                for (int i = 0; i < xcoords.Count; ++i) {
                    if (xmap.ContainsKey(i))
                        continue;

                    bool hasMatch = false;
                    float x1 = xcoords[i];
                    for (int j = 0; j < xcoords.Count; ++j) {
                        if (i == j)
                            continue;
                            
                        if (xmap.ContainsKey(j)) {
                            hasMatch = true;
                            continue;
                        }

                        if (Math.Abs(ycoords[i] - ycoords[j]) > 0.05)
                            continue;

                        float x2 = xcoords[j];
                                                
                        float bigger = Math.Max(x1, x2);
                        float smaller = Math.Min(x1, x2);
                        
                        hasMatch = (bigger + smaller) < 0.05;
                        if (hasMatch) {
                            xmap.Add(i, j);
                            xmap.Add(j, i);
                            break;
                        }
                    }

                    if (!hasMatch)
                        throw new Exception("This shape is not symmetric in respect to X-axis!");
                }
            }

            else {
                for (int i = 0; i < xcoords.Count; ++i) {
                    xmap.Add(i, i);
                }
            }

            // ..in respect to y-axis
            if (symy) {
                for (int i = 0; i < ycoords.Count - 1; ++i) {
                    if (ymap.ContainsKey(i))
                        continue;

                    bool hasMatch = false;
                    float y1 = ycoords[i];
                    for (int j = i + 1; j < ycoords.Count; ++j) {
                        if (ymap.ContainsKey(j))
                            continue;

                        float y2 = ycoords[j];

                        if (Math.Abs(xcoords[i] - xcoords[j]) > 0.05)
                            continue;

                        float bigger = Math.Max(y1, y2);
                        float smaller = Math.Min(y1, y2);
                        
                        hasMatch = (bigger - smaller) < 0.05;
                        if (hasMatch) {
                            ymap.Add(i, j);
                            ymap.Add(j, i);
                            break;
                        }
                    }

                    if (!hasMatch)
                        throw new Exception("This shape is not symmetric in respect to Y-axis!");
                }
            }

            else {
                for (int i = 0; i < ycoords.Count; ++i) {
                    ymap.Add(i, i);
                }
            }

            VirtualConnection vc = new VirtualConnection(shape1, shape2, center, xmap, ymap);

            shape1.VC = vc;
            shape2.VC = vc;
        }

        public IShape Other(IShape from) {
            if (from == this.shape1)
                return this.shape2;

            return this.shape1;
        }

        public void Transform(IShape shape) {
            // TODO: this only works on x-axis atm
            List<Vertex> oldVertices = shape.GetVertices();
            List<Vertex> newVertices = new List<Vertex>(new Vertex[oldVertices.Count]);
            for (int i = 0; i < oldVertices.Count; ++i) {
                Vertex old = oldVertices[i];
                int newIndex = this.xsymmetry[i];
                newVertices[newIndex] = old; 
            }

            shape.SetVertices(newVertices);
        }
    }
}