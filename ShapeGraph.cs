// using System.Collections;
// using System.Collections.Generic;
// using System;
// using Shape;

// public abstract class ShapeGraph {
//     protected class SGVertex {
//         public readonly IShape Shape;
//         public readonly HashSet<(SGVertex, Attributes)> Connections;
//         public int Rank {get {return this.connections.Count;}}
//         public SGVertex(IShape shape) {
//             this.Shape = shape;
//             this.Connections = new HashSet<(SGVertex, Attributes)>();
//         }

//         public void Connect(SGVertex other, Attributes attributes) {
//             this.Connections.Add((other, attributes));
//             other.Connections.Add((this, attributes));
//         }
//     }

//     HashSet<SGVertex> vertices = new HashSet<SGVertex>();

//     protected abstract bool AreConnected(SGVertex first, SGVertex second);

//     /**
//      * Function for converting a collection of shapes
//      * into the shape graph
//      */
//     public abstract void ToGraph(List<IShape> shapes);

//     /**
//      * Function for converting the shape graph into
//      * a collection of shapes
//      */
//     public abstract List<IShape> ToShapes();
// }