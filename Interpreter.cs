using System.Collections.Generic;
using Shape;

public class Interpreter {
    public static List<IShape> Interpret(IShape start, uint max) {
        Queue<IShape> shapeQueue = new Queue<IShape>();
        shapeQueue.Enqueue(start);

        List<IShape> output = new List<IShape>();
        List<IShape> righthand;
        for (uint i = 0; i < max; ++i) {
            IShape s = shapeQueue.Dequeue();

            righthand = s.NextShapes();

            if (righthand.Count == 0) {
                output.Add(s);
                i -= 1;
            }

            else if (righthand[0].Symbol == typeof(Epsilon)) {
                // In case of epsilon, we want to remove the shape
                // For that, we have to not add the shapes into the queue, nor
                // to the output
            }

            else foreach(IShape shape in righthand)
                shapeQueue.Enqueue(shape);
        }

        // Add the remaining shapes to the output
        while (shapeQueue.Count > 0) 
            output.Add(shapeQueue.Dequeue());

        return output;
    }
}