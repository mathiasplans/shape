using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System;

public class MainClass {
    private static Point BisectLine(Point v1, Point v2, float x) {
        return new Point((int) (v1.X * x + v2.X * (1f - x)), (int) (v1.Y * x + v2.Y * (1f - x)));
    }

    public static void Main() {
       CreateImage(32 * 12);
    }

    private static List<Shape> GetShapes(int min, int max) {
        Rules rules = new Rules();
        rules.AddRule((Quad quad) => {
            Point nv1 = BisectLine(quad.v1, quad.v2, 0.5f); 
            Point nv2 = BisectLine(quad.v2, quad.v3, 0.5f); 
            Point nv3 = BisectLine(quad.v3, quad.v4, 0.5f); 
            Point nv4 = BisectLine(quad.v4, quad.v1, 0.5f); 

            // A quad with the vertices as bipartitions of originals edges
            Quad q = new Quad(rules, nv1, nv2, nv3, nv4);

            // Triangles
            Triad t1 = new Triad(rules, nv1, quad.v2, nv2);
            Triad t2 = new Triad(rules, nv2, quad.v3, nv3);
            Triad t3 = new Triad(rules, nv3, quad.v4, nv4);
            Triad t4 = new Triad(rules, nv4, quad.v1, nv1);

            return new List<Shape> {q, t1, t2, t3, t4};
        });

        rules.AddRule((Quad quad) => {
            Point nv1 = BisectLine(quad.v1, quad.v2, 0.5f);
            Point nv2 = BisectLine(quad.v1, nv1, 0.5f);
            Point nv3 = BisectLine(nv1, quad.v2, 0.5f);

            Point nv4 = BisectLine(quad.v4, quad.v3, 0.5f);
            Point nv5 = BisectLine(quad.v4, nv4, 0.5f);
            Point nv6 = BisectLine(nv4, quad.v3, 0.5f);

            Quad q1 = new Quad(rules, quad.v1, nv2, nv5, quad.v4);
            Quad q2 = new Quad(rules, nv2, nv1, nv4, nv5);
            Quad q3 = new Quad(rules, nv1, nv3, nv6, nv4);
            Quad q4 = new Quad(rules, nv3, quad.v2, quad.v3, nv6);

            return new List<Shape> {q1, q2, q3, q4};
        });

        Quad start = new Quad(
            rules, 
            new Point(min, max), 
            new Point(max, max), 
            new Point(max, min), 
            new Point(min, min)
        );

        List<Shape> shapes = Interpreter.Interpret(start, 6);

        return shapes;
    }

    public static Graphics GetGraphics(Bitmap bmp) {
        Graphics gr = Graphics.FromImage(bmp);
        gr.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
        return gr;
    }

    public static void DrawLines(Graphics gr, List<Shape> shapes) {
        Pen pen = new Pen(Color.Black, 3);

        foreach (Shape shape in shapes) {
            List<Point> vertices = shape.GetVertices();

            for (int i = 1; i < vertices.Count; ++i) {
                // Draws the line 
                gr.DrawLine(pen, vertices[i - 1], vertices[i]); 
            }

            // Last line wraps around
            gr.DrawLine(pen, vertices[vertices.Count - 1], vertices[0]);
        }
        
    }

    private static void CreateImage(int size) {
        Bitmap bmp = new Bitmap(size, size);

        List<Shape> shapes = GetShapes(size / 12, 11 * size / 12);
        
        Graphics gr = GetGraphics(bmp);

        DrawLines(gr, shapes);

        bmp.Save("split.png", ImageFormat.Png);
    }
}