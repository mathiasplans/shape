using System.Collections.Generic;
using System;
using Shape;

public class Interpreter2 {
    public static List<List<IShape>> Interpret(IShape start, Rules rules, Control control, uint max) {
        ShapeGraph sg = new ShapeGraph(start);

        // Get all the LHSes, so we can search for them in the graph
        HashSet<Type> lhs = rules.GetAllLHS();
        HashSet<(ShapeGraph, Type)> prototypes = new HashSet<(ShapeGraph, Type)>();

        foreach (Type t in rules.GetAllLHS()) {
            // We know that Type t is IShape, which implements a static function
            // 'Prototype'. This piece of reflection is for calling that static
            // function.
            ShapeGraph protoSG = (ShapeGraph)t.GetMethod("Prototype").Invoke(null, new object[]{});
            prototypes.Add((protoSG, t));
        }

        List<List<IShape>> progress = new List<List<IShape>>();

        // Find all the subgraphs of the shape graph that are prototypes
        if (max > 0)
            for (int i = 0; i < max; ++i) {
                if (sg.Interpret(prototypes, control).Item2)
                    progress.Add(sg.GetShapes());
            }

        else {
            bool cont = false;
            bool intr = false;
            while (((cont, intr) = sg.Interpret(prototypes, control)).Item1) {
                if (intr)
                    progress.Add(sg.GetShapes());
            }
        }

        return progress;
    }
}
