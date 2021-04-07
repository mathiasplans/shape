using System.Collections;
using System.Collections.Generic;
using System;
using Shape;

public abstract class ShapeGraph {
    protected class SGVertex {
        public Type symbol;
        public Attributes attributes;
        public List<Tuple<SGVertex, Attributes>> connections;
        public List<float[]> geometry;
    }

    HashSet<SGVertex> vertices = new HashSet<SGVertex>();

    protected abstract bool AreConnected(SGVertex first, SGVertex second);

    /**
     * Function for converting a collection of shapes
     * into the shape graph
     */
    public abstract void ToGraph(List<IShape> shapes);

    /**
     * Function for converting the shape graph into
     * a collection of shapes
     */
    public abstract List<IShape> ToShapes();
}