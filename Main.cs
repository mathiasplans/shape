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

    private static List<IShape> GetShapes(int min, int max) {
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

    private static void CreateImage(int size) {
        Bitmap bmp = new Bitmap(size, size);

        List<IShape> shapes = GetShapes(size / 12, 11 * size / 12);
        
        Graphics gr = GetGraphics(bmp);

        DrawLines(gr, shapes);

        bmp.Save("split.png", ImageFormat.Png);
    }
}