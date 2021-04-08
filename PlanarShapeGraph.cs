// using System.Collections;
// using System.Collections.Generic;
// using System;
// using Shape;

// public class PlanarShapeGraph : ShapeGraph {

//     protected override bool AreConnected(SGVertex first, SGVertex second) {
//         return false;
//     }

//     public override void ToGraph(List<IShape> shapes) {
//         HashSet<SGVertex> lookingForConnections = new HashSet<SGVertex>();

//         foreach (IShape shape in shapes) {
//             // Create a vertex for the shape
//             SGVertex newVert = new SGVertex(shape);

//             // Check the connections with other shapes
//             foreach (SGVertex sgv in lookingForConnections) {
//                 // Check if the shapes are coincidental
//                 // if (newVert.Shape.IsCoincidentalWith(sgv.Shape)) {
//                 //     // Connect the vertices

//                 // }
//             }

//             // Add the vertex to the connection set
//             lookingForConnections.Add(newVert);
//         }
//     }

//     public override List<IShape> ToShapes() {
//         return null;
//     }
// }