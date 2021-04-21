using System.Collections.Generic;
using System;
using Shape;

public class Interpreter2 {
    public static List<IShape> Interpret(IShape start, Rules rules, ControlGrammar cg, uint max) {
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

        // Find all the subgraphs of the shape graph that are prototypes
        for (int i = 0; i < max; ++i) {
            sg.Interpret(prototypes, cg);
        }

        return sg.GetShapes();
    }
}