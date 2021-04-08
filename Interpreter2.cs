using System.Collections.Generic;
using System;
using Shape;

public class Interpreter2 {
    public static List<IShape> Interpret(IShape start, Rules rules, uint max) {
        ShapeGraph sg = new ShapeGraph(start);

        // Get all the LHSes, so we can search for them in the graph
        HashSet<Type> lhs = rules.GetAllLHS();
        HashSet<ShapeGraph> prototypes = new HashSet<ShapeGraph>();
        Dictionary<ShapeGraph, Type> graphToShape = new Dictionary<ShapeGraph, Type>(); // Not needed?

        foreach (Type t in rules.GetAllLHS()) {
            // We know that Type t is IShape, which implements a static function
            // 'Prototype'. This piece of reflection is for calling that static
            // function.
            ShapeGraph protoSG = (ShapeGraph)t.GetMethod("Prototype").Invoke(null, new object[]{});
            prototypes.Add(protoSG);
            graphToShape.Add(protoSG, t);
        }

        return null;

        // List<IShape> output = new List<IShape>();
        // List<IShape> righthand;
        // for (uint i = 0; i < max; ++i) {
        //     IShape s = shapeQueue.Dequeue();

        //     righthand = s.NextShapes();

        //     if (righthand.Count == 0) {
        //         output.Add(s);
        //         i -= 1;
        //     }

        //     else if (righthand[0].Symbol == "Epsilon") {
        //         // In case of epsilon, we want to remove the shape
        //         // For that, we have to not add the shapes into the queue, nor
        //         // to the output
        //     }

        //     else foreach(IShape shape in righthand)
        //         shapeQueue.Enqueue(shape);
        // }

        // // Add the remaining shapes to the output
        // while (shapeQueue.Count > 0) 
        //     output.Add(shapeQueue.Dequeue());

        // return output;
    }
}