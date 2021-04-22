using System.Collections.Generic;
using System.Collections;
using System;

using Shape;

public class ExampleGrammar1 {
    public static List<IShape> Run(int min, int max, uint times) {
        Rules rules = new Rules();
        rules.AddRule(
            new Attributes(
                ("Diamond", new ScalarAttribute(1f))
            ),
            (Quad quad) => {
                Vertex nv1 = quad.l1.Bisect(0.5f);
                Vertex nv2 = quad.l2.Bisect(0.5f);
                Vertex nv3 = quad.l3.Bisect(0.5f);
                Vertex nv4 = quad.l4.Bisect(0.5f);

                // A quad with the vertices as bipartitions of original's edges
                Attributes attrs = new Attributes(quad.Attributes);
                if (quad.Attributes.Get("Angle").Start == 1f)
                    attrs.Set("Angle", new ScalarAttribute(0f));


                else
                    attrs.Set("Angle", new ScalarAttribute(1f));

                Quad q = new Quad(
                    rules,
                    attrs,
                    (1, 0),
                    (nv1, nv2, nv3, nv4)
                );

                // Triangles
                Triad t1 = new Triad(rules, quad.Attributes.Copy(), (0, 0), (nv1, quad.v2, nv2));
                Triad t2 = new Triad(rules, quad.Attributes.Copy(), (0, 1), (nv2, quad.v3, nv3));
                Triad t3 = new Triad(rules, quad.Attributes.Copy(), (0, 2), (nv3, quad.v4, nv4));
                Triad t4 = new Triad(rules, quad.Attributes.Copy(), (0, 3), (nv4, quad.v1, nv1));

                return new List<IShape> {q, t1, t2, t3, t4};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Angle", new ScalarAttribute(0f))
            ), 

            (Quad quad) => {
                Vertex[] nv1 = quad.l1.Sections(3);
                Vertex[] nv2 = quad.l3.Sections(3);

                Quad q1 = new Quad(rules, quad.Attributes.Copy(), (0, 0), (quad.v1, nv1[2], nv2[0], quad.v4));
                Quad q2 = new Quad(rules, quad.Attributes.Copy(), (1, 0), (nv1[2], nv1[1], nv2[1], nv2[0]));
                Quad q3 = new Quad(rules, quad.Attributes.Copy(), (2, 0), (nv1[1], nv1[0], nv2[2], nv2[1]));
                Quad q4 = new Quad(rules, quad.Attributes.Copy(), (3, 0), (nv1[0], quad.v2, quad.v3, nv2[2]));

                return new List<IShape> {q1, q2, q3, q4};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Angle", new ScalarAttribute(1f))
            ),

            (Quad quad) => {
                Vertex nv1 = quad.l1.Bisect(0.5f);
                Vertex nv2 = quad.l3.Bisect(0.5f);

                Quad q1 = new Quad(rules, quad.Attributes.Copy(), (0, 0), (quad.v1, nv1, nv2, quad.v4));
                Quad q2 = new Quad(rules, quad.Attributes.Copy(), (1, 0), (nv1, quad.v2, quad.v3, nv2));

                return new List<IShape> {q1, q2};
            }
        );

        rules.AddRule(null, (Triad triad) => {
            return new List<IShape> {new Epsilon()};
        });

        rules.AddRule(null, (Triad triad) => {
            return new List<IShape>();
        });

        Quad start = new Quad(
            rules,
            new Attributes(
                ("Angle", new ScalarAttribute(0f)),
                ("Diamond", new ScalarAttribute(0f))
            ),
           (new Vertex(min, max), 
            new Vertex(max, max), 
            new Vertex(max, min), 
            new Vertex(min, min))
        );

        start.Control = "A";

        // Define the control grammar
        Control control = new Control();
        control.Grammar.AddRule("A", new string[]{"N", "Y", "Y"});
        control.Grammar.AddRule("N", ((~0u, 0), "Diamond", 0f, ""));
        control.Grammar.AddRule("Y", ((1, 0), "Diamond", 1f, "A"));
        control.Grammar.AddRule("Y", ((2, 0), "Diamond", 1f, "A"));

        // List<IShape> shapes = Interpreter.Interpret(start, 10);
        List<IShape> shapes = Interpreter2.Interpret(start, rules, control, times);

        return shapes;
    }
}