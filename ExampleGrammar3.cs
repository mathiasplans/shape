using System.Collections.Generic;
using System.Collections;
using System;

using Shape;

public class ExampleGrammar3 {
    public static (List<List<IShape>>, List<(IShape, List<IShape>)>) Run(int min, int max, uint times) {
        Rules rules = new Rules();

        rules.AddRule(
            new Attributes(

            ),
            (Quad quad) => {
                Vertex b1 = quad.l1.Bisect();
                Vertex b2 = quad.l2.Bisect();
                Vertex b3 = quad.l3.Bisect();
                Vertex b4 = quad.l4.Bisect();

                Vertex c = (b1 + b2 + b3 + b4) / 4f;

                Quad q4 = new Quad(rules, quad.Attributes.Copy(), (0, 0), (b4, c, b3, quad.v4));
                Quad q1 = new Quad(rules, quad.Attributes.Copy(), (1, 0), (quad.v1, b1, c, b4));
                Quad q3 = new Quad(rules, quad.Attributes.Copy(), (0, 1), (c, b2, quad.v3, b3));
                Quad q2 = new Quad(rules, quad.Attributes.Copy(), (1, 1), (b1, quad.v2, b2, c));

                if (quad.VC == null || !quad.VC.Persistant) {
                    VirtualConnection.Connect(q4, q1, false, false, true);
                    VirtualConnection.Connect(q3, q2, true, false, true);
                }

                return new List<IShape> {q1, q2, q3, q4};
            }
        );

        rules.AddRule(
            new Attributes(

            ),
            (Quad quad) => {
                Vertex th = quad.l1.Bisect();

                Triad t1 = new Triad(rules, quad.Attributes.Copy(), (0, 0), (quad.v1, th, quad.v4));
                Triad t2 = new Triad(rules, quad.Attributes.Copy(), (0, 1), (quad.v4, th, quad.v3));
                Triad t3 = new Triad(rules, quad.Attributes.Copy(), (0, 2), (th, quad.v2, quad.v3));

                return new List<IShape> {t1, t2, t3};
            }
        );

        rules.AddRule(
            new Attributes(

            ),
            (Quad quad) => {
                Vertex[] v1 = quad.l1.Sections(2);
                Vertex[] v2 = quad.l3.Sections(2);

                Attributes na = quad.Attributes.Copy();
                na.Set("Wave", new ScalarAttribute(3f));

                Quad q1 = new Quad(rules, na.Copy(), (0, 0), (quad.v1, v1[1], v2[0], quad.v4));
                Quad q2 = new Quad(rules, na.Copy(), (0, 1), (v1[1], v1[0], v2[1], v2[0]));
                Quad q3 = new Quad(rules, na.Copy(), (0, 2), (v1[0], quad.v2, quad.v3, v2[1]));

                return new List<IShape> {q1, q2, q3};
            }
        );

        rules.AddRule(null, (Quad quad) => {
            Triad t1 = new Triad(rules, quad.Attributes.Copy(), (0, 0), (quad.v1, quad.v2, quad.v3));
            Triad t2 = new Triad(rules, quad.Attributes.Copy(), (0, 1), (quad.v3, quad.v4, quad.v1));

            return new List<IShape> {t1, t2};
        });

        rules.AddRule(
            new Attributes(
                ("Wave", new ScalarAttribute(3f))
            ),
            (Quad quad) => {
                return new List<IShape>();
            }
        );

        rules.AddRule(
            new Attributes(

            ),
            (Quad quad) => {
                Vertex[] v1 = quad.l1.Sections(2);
                Vertex[] v2 = quad.l2.Sections(2);
                Vertex[] v3 = quad.l3.Sections(2);
                Vertex[] v4 = quad.l4.Sections(2);

                Vertex c1 = v1[1] * 0.75f + v2[0] * 0.25f;
                Vertex c2 = v1[1] * 0.25f + v2[0] * 0.75f;
                Vertex c3 = v3[1] * 0.75f + v4[0] * 0.25f;
                Vertex c4 = v3[1] * 0.25f + v4[0] * 0.75f;

                Attributes qa = quad.Attributes.Copy();
                qa.Set("Wave", new ScalarAttribute(3f));

                // Quads
                // Quad q1 = new Quad(rules, qa.Copy(), (v1[0], c1, c4, v4[1]));
                // Quad q2 = new Quad(rules, qa.Copy(), (c1, v1[1], v2[0], c2));
                // Quad q3 = new Quad(rules, qa.Copy(), (v3[0], c3, c2, v2[1]));
                // Quad q4 = new Quad(rules, qa.Copy(), (v4[0], c4, c3, v3[1]));
                // Quad q5 = new Quad(rules, qa.Copy(), (c1, c2, c3, c4));

                // Triangles
                Triad t1 = new Triad(rules, qa.Copy(), (0, 0), (quad.v1, v1[1], v4[0]));
                Triad t2 = new Triad(rules, qa.Copy(), (0, 1), (v1[1], v1[0], c1));
                Triad t3 = new Triad(rules, qa.Copy(), (0, 2), (v1[0], quad.v2, v2[1]));
                Triad t4 = new Triad(rules, qa.Copy(), (0, 3), (c2, v2[1], v2[0]));
                Triad t5 = new Triad(rules, qa.Copy(), (0, 4), (v3[1], v2[0], quad.v3));
                Triad t6 = new Triad(rules, qa.Copy(), (0, 5), (v3[0], c3, v3[1]));
                Triad t7 = new Triad(rules, qa.Copy(), (0, 6), (quad.v4, v4[1], v3[0]));
                Triad t8 = new Triad(rules, qa.Copy(), (0, 7), (v4[1], v4[0], c4));

                // return new List<IShape> {q1, q2, q3, q4, t1, t2, t3, t4, t5, t6, t7, t8};
                return new List<IShape> {t1, t2, t3, t4, t5, t6, t7, t8};
            }
        );

        rules.AddRule(null, (Triad triad) => {
            return new List<IShape>();
        });

        Quad start = new Quad(
            rules,
            new Attributes(
                ("Wave", new ScalarAttribute(0f))
            ),
           (new Vertex(min, max), 
            new Vertex(max, max), 
            new Vertex(max, min), 
            new Vertex(min, min))
        );

        // Define the control grammar
        Control control = new Control();


        var ruleexamples = rules.RuleExamples(100f);

        List<List<IShape>> shapes = Interpreter2.Interpret(start, rules, control, times);

        return (shapes, ruleexamples);
    }
}