using System.Collections.Generic;

public class Interpreter {
    public static List<Shape> Interpret(Shape start, uint max) {
        Queue<Shape> shapeQueue = new Queue<Shape>();
        shapeQueue.Enqueue(start);

        List<Shape> output = new List<Shape>();
        List<Shape> righthand;
        for (uint i = 0; i < max; ++i) {
            Shape s = shapeQueue.Dequeue();

            righthand = s.NextShapes();

            if (righthand.Count == 0) {
                output.Add(s);
                i -= 1;
            }

            else foreach(Shape shape in righthand)
                shapeQueue.Enqueue(shape);
        }

        // Add the remaining shapes to the output
        while (shapeQueue.Count > 0) 
            output.Add(shapeQueue.Dequeue());

        return output;
    }
}