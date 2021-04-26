using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System;

using Shape;

public class MainClass {
    public static void Main() {
       CreateImage(32 * 12);
    }

    private static (List<IShape>, List<(IShape, List<IShape>)>) GetShapes(int min, int max) {
        return ExampleGrammar3.Run(min, max, 20);
    }

    public static Graphics GetGraphics(Bitmap bmp) {
        Graphics gr = Graphics.FromImage(bmp);
        gr.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
        return gr;
    }

    public static void DrawLines(Graphics gr, List<IShape> shapes) {
        Pen pen = new Pen(Color.Black, 3);

        foreach (IShape shape in shapes) {
            List<Line> lines = shape.GetLines();
            List<Point> points = new List<Point>();
            
            foreach (Line line in lines) {
                gr.DrawLine(pen, (Point) line.p1, (Point) line.p2);
            }
        }
        
    }

    public static void DrawSymbols(Graphics gr, List<IShape> shapes, Font font, Brush brush) {
        foreach (IShape shape in shapes) {
            Vertex center = shape.Center;
            gr.DrawString(shape.ToString(), font, brush, new PointF(center.x - 7f, center.y - 11f));
        }
    }

    public static void DrawRules(List<(IShape, List<IShape>)> rules) {
        int count = 0;

        HashSet<IShape> lhsshapes = new HashSet<IShape>();

        foreach ((IShape lhs, List<IShape> rhs) in rules) {
            // TODO: change the size of the image depending on the size of the rhs
            Bitmap bmp = new Bitmap(300, 120);

            Graphics gr = GetGraphics(bmp);

            float[,] commontranslate = new[,] {
                {1f, 0f, 10f},
                {0f, 1f, 10f},
                {0f, 0f, 1f}      
            };

            float[,] translate = new[,] {
                {1f, 0f, 180f},
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
            allShapes.Add(lhs);

            Font symbolfont = new Font("Times New Roman", 14f);
            Font arrowfont = new Font("Times New Roman", 60f, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Black);

            DrawSymbols(gr, allShapes, symbolfont, brush);

            if (rhs.Count == 0) {
                IShape terminalShape = lhs.Copy();
                terminalShape.Transform(translate);
                allShapes.Add(terminalShape);
            }

            DrawLines(gr, allShapes);

            gr.DrawString("→", arrowfont, brush, new PointF(111f, 3f));

            bmp.Save($"rule{count}.png", ImageFormat.Png);

            count += 1;
        }
    }

    private static void CreateImage(int size) {
        Bitmap bmp = new Bitmap(size, size);

        (List<IShape> shapes, List<(IShape, List<IShape>)> examples) = GetShapes(size / 12, 11 * size / 12);
        
        Graphics gr = GetGraphics(bmp);

        DrawLines(gr, shapes);

        DrawRules(examples);

        bmp.Save("split.png", ImageFormat.Png);
    }
}