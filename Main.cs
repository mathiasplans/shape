using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System;

using Shape;

public class MainClass {
    private static float[] BisectLine(float[] v1, float[] v2, float x) {
        // return new Point((int) (v1.X * x + v2.X * (1f - x)), (int) (v1.Y * x + v2.Y * (1f - x)));
        float conx = 1f - x;
        return new float[] {v1[0] * x + v2[0] * conx, v1[1] * x + v2[1] * conx};
    }

    public static void Main() {
       CreateImage(32 * 12);
    }

    private static List<IShape> GetShapes(int min, int max) {
        Rules rules = new Rules();
        rules.AddRule(null,
            (Quad quad) => {
                float[] nv1 = BisectLine(quad.v1, quad.v2, 0.5f); 
                float[] nv2 = BisectLine(quad.v2, quad.v3, 0.5f); 
                float[] nv3 = BisectLine(quad.v3, quad.v4, 0.5f); 
                float[] nv4 = BisectLine(quad.v4, quad.v1, 0.5f); 

                // A quad with the vertices as bipartitions of originals edges

                Quad q;
                if (quad.Attributes.Get("Angle").Start == 1f)
                    q = new Quad(
                        rules,
                        new Attributes(("Angle", new ScalarAttribute(0f))),
                        (nv1, nv2, nv3, nv4)
                    );

                else
                    q = new Quad(
                        rules,
                        new Attributes(("Angle", new ScalarAttribute(1f))),
                        (nv1, nv2, nv3, nv4)
                    );

                // Triangles
                Triad t1 = new Triad(rules, (nv1, quad.v2, nv2));
                Triad t2 = new Triad(rules, (nv2, quad.v3, nv3));
                Triad t3 = new Triad(rules, (nv3, quad.v4, nv4));
                Triad t4 = new Triad(rules, (nv4, quad.v1, nv1));

                return new List<IShape> {q, t1, t2, t3, t4};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Angle", new ScalarAttribute(0f))
            ), 

            (Quad quad) => {
                float[] nv1 = BisectLine(quad.v1, quad.v2, 0.5f);
                float[] nv2 = BisectLine(quad.v1, nv1, 0.5f);
                float[] nv3 = BisectLine(nv1, quad.v2, 0.5f);

                float[] nv4 = BisectLine(quad.v4, quad.v3, 0.5f);
                float[] nv5 = BisectLine(quad.v4, nv4, 0.5f);
                float[] nv6 = BisectLine(nv4, quad.v3, 0.5f);

                Quad q1 = new Quad(rules, quad.Attributes, (quad.v1, nv2, nv5, quad.v4));
                Quad q2 = new Quad(rules, quad.Attributes, (nv2, nv1, nv4, nv5));
                Quad q3 = new Quad(rules, quad.Attributes, (nv1, nv3, nv6, nv4));
                Quad q4 = new Quad(rules, quad.Attributes, (nv3, quad.v2, quad.v3, nv6));

                return new List<IShape> {q1, q2, q3, q4};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Angle", new ScalarAttribute(1f))
            ),

            (Quad quad) => {
                float[] nv1 = BisectLine(quad.v1, quad.v2, 0.5f);
                float[] nv2 = BisectLine(quad.v4, quad.v3, 0.5f);

                Quad q1 = new Quad(rules, quad.Attributes, (quad.v1, nv1, nv2, quad.v4));
                Quad q2 = new Quad(rules, quad.Attributes, (nv1, quad.v2, quad.v3, nv2));

                return new List<IShape> {q1, q2};
            }
        );

        rules.AddRule(null, (Triad triad) => {
            return new List<IShape> {new Epsilon()};
        });

        rules.AddRule(null, (Triad triad) => {
            return new List<IShape>();
        });

        Quad start = new Quad(
            rules,
            new Attributes(
                ("Angle", new ScalarAttribute(0f))
            ),
           (new float[]{min, max}, 
            new float[]{max, max}, 
            new float[]{max, min}, 
            new float[]{min, min})
        );

        List<IShape> shapes = Interpreter.Interpret(start, 10);

        return shapes;
    }

    public static Graphics GetGraphics(Bitmap bmp) {
        Graphics gr = Graphics.FromImage(bmp);
        gr.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
        return gr;
    }

    public static void DrawLines(Graphics gr, List<IShape> shapes) {
        Pen pen = new Pen(Color.Black, 3);

        foreach (IShape shape in shapes) {
            List<float[]> vertices = shape.GetVertices();
            List<Point> points = new List<Point>();

            foreach (float[] vertex in vertices) {
                points.Add(new Point((int)vertex[0], (int)vertex[1]));
            }
            

            for (int i = 1; i < points.Count; ++i) {
                // Draws the line 
                gr.DrawLine(pen, points[i - 1], points[i]); 
            }

            // Last line wraps around
            gr.DrawLine(pen, points[points.Count - 1], points[0]);
        }
        
    }

    private static void CreateImage(int size) {
        Bitmap bmp = new Bitmap(size, size);

        List<IShape> shapes = GetShapes(size / 12, 11 * size / 12);
        
        Graphics gr = GetGraphics(bmp);

        DrawLines(gr, shapes);

        bmp.Save("split.png", ImageFormat.Png);
    }
}