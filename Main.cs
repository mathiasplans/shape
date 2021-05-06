using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System;

using Shape;

public class MainClass {
    static int sf = 3;

    public static void Main() {
       CreateImage(32 * 12 * sf);
    }

    private static (List<IShape>, List<(IShape, List<IShape>)>) GetShapes(int min, int max) {
        return ExampleGrammar3.Run(min, max, 20);
    }

    public static Graphics GetGraphics(Bitmap bmp) {
        Graphics gr = Graphics.FromImage(bmp);
        gr.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
        return gr;
    }

    public static void DrawLines(Graphics gr, List<IShape> shapes, float factor) {
        Pen pen = new Pen(Color.Black, factor);

        foreach (IShape shape in shapes) {
            List<Line> lines = shape.GetLines();
            List<Vertex> vertices = shape.GetVertices();

            if (vertices.Count == 0)
                continue;

            List<Point> points = new List<Point>();
            foreach (Vertex v in vertices) {
                points.Add(v);
            }

            // Fill the shape
            // Color
            int red = 255, green = 255, blue = 255;
            if (shape.Attributes.ContainsKey("R"))
                red = (int) shape.Attributes.Get("R").Start;

            if (shape.Attributes.ContainsKey("G"))
                green = (int) shape.Attributes.Get("G").Start;

            if (shape.Attributes.ContainsKey("B"))
                blue = (int) shape.Attributes.Get("B").Start;

            Brush br = new SolidBrush(Color.FromArgb(red, green, blue));

            // Points
            Point p1 = points[0];

            for (int i = 2; i < points.Count; ++i) {
                gr.FillPolygon(br, new Point[] {p1, points[i - 1], points[i]});
            }
            
            // Draw the lines
            foreach (Line line in lines) {
                gr.DrawLine(pen, (Point) line.p1, (Point) line.p2);
            }
        }
        
    }

    public static void DrawSymbols(Graphics gr, List<IShape> shapes, Font font, Brush brush, float offx, float offy) {
        foreach (IShape shape in shapes) {
            Vertex center = shape.Center;
            string symbol = shape.ToString();
            // if (symbol != "Q" && symbol != "T")
                gr.DrawString(symbol, font, brush, new PointF(center.x - offx, center.y - offy));
        }
    }

    public static void DrawGraph(Graphics gr, List<IShape> shapes, float factor) {
        Font symbolfont = new Font("Times New Roman", 14f * factor);
        Brush brush = new SolidBrush(Color.Black);
        Brush brush2 = new SolidBrush(Color.White);

        Pen pen = new Pen(Color.Black, factor / 2f);

        // Create the shape graph
        ShapeGraph sg = new ShapeGraph(shapes);

        HashSet<ShapeGraph.Node> done = new HashSet<ShapeGraph.Node>();
        foreach (ShapeGraph.Node node in sg) {
            Vertex nodeCenter = node.Shape.Center;
            // Draw lines between the vertices  
            foreach ((ShapeGraph.Node, Attributes) conn in node.Connections) {
                if (done.Contains(conn.Item1))
                    continue;

                Vertex connCenter = conn.Item1.Shape.Center;
                gr.DrawLines(pen, new PointF[] {connCenter, nodeCenter});
            }

            // Draw the point
            gr.FillEllipse(brush2, new Rectangle((int) (nodeCenter.x - 9 * factor), (int) (nodeCenter.y - 9 * factor), (int) (19 * factor), (int) (19 * factor)));


            done.Add(node);
        }

        DrawSymbols(gr, shapes, symbolfont, brush, 6f * factor, 9f * factor);
    }

    public static void DrawRules(List<(IShape, List<IShape>)> rules) {
        int count = 0;
        float factor = 6f;

        HashSet<IShape> lhsshapes = new HashSet<IShape>();

        foreach ((IShape lhs, List<IShape> rhs) in rules) {
            // TODO: change the size of the image depending on the size of the rhs
            Bitmap shapeBmp = new Bitmap((int) (300 * factor), (int) (120 * factor));
            Bitmap graphBmp = new Bitmap((int) (300 * factor), (int) (120 * factor));

            Graphics shapeGr = GetGraphics(shapeBmp);
            Graphics graphGr = GetGraphics(graphBmp);

            float[,] commontranslate = new[,] {
                {factor, 0f, 10f * factor},
                {0f, factor, 10f * factor},
                {0f, 0f, 1f}      
            };

            float[,] translate = new[,] {
                {1f, 0f, 180f * factor},
                {0f, 1f, 0f},
                {0f, 0f, 1f}
            };

            if (!lhsshapes.Contains(lhs)) {
                lhs.Transform(commontranslate);
                lhsshapes.Add(lhs);
            }

            foreach (IShape shape in rhs) {
                shape.Transform(commontranslate);
                shape.Transform(translate);
            }

            List<IShape> allShapes = new List<IShape>(rhs);
            lhs.Name = "LHS";
            allShapes.Add(lhs);

            Font symbolfont = new Font("Times New Roman", 14f * factor);
            Font arrowfont = new Font("Times New Roman", 60f * factor, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Black);

            if (rhs.Count == 0) {
                IShape terminalShape = lhs.Copy();
                terminalShape.Transform(translate);
                allShapes.Add(terminalShape);
            }

            DrawLines(shapeGr, allShapes, factor / 2f);
            DrawSymbols(shapeGr, allShapes, symbolfont, brush, 7f * factor, 11f * factor);
            DrawGraph(graphGr, allShapes, factor);
            
            shapeGr.DrawString("→", arrowfont, brush, new PointF(111f * factor, 3f * factor));
            graphGr.DrawString("→", arrowfont, brush, new PointF(111f * factor, 3f * factor));

            shapeBmp.Save($"rule{count}.png", ImageFormat.Png);
            graphBmp.Save($"rule{count}_graph.png", ImageFormat.Png);

            count += 1;
        }
    }

    private static void CreateImage(int size) {
        Bitmap bmp = new Bitmap(size, size);

        (List<IShape> shapes, List<(IShape, List<IShape>)> examples) = GetShapes(size / 12, 11 * size / 12);
    
        Graphics gr = GetGraphics(bmp);

        DrawLines(gr, shapes, sf);

        DrawRules(examples);

        bmp.Save("split.png", ImageFormat.Png);

        // Graph of the result
        bmp = new Bitmap(size, size);

        gr = GetGraphics(bmp);

        DrawGraph(gr, shapes, size / 200f);

        bmp.Save("split_graph.png", ImageFormat.Png);
    }
}