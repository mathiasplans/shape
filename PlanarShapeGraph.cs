using System.Collections;
using System.Collections.Generic;
using System;
using Shape;

public class PlanarShapeGraph : ShapeGraph {

    protected override bool AreConnected(SGVertex first, SGVertex second) {
        return false;
    }

    public override void ToGraph(List<IShape> shapes) {
        HashSet<SGVertex> lookingForNeighbors = new HashSet<SGVertex>();

        foreach (IShape shape in shapes) {
            // Create
        }
    }

    public override List<IShape> ToShapes() {
        return null;
    }
}