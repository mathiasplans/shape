using System.Collections.Generic;
using System.Collections;
using System;

using Shape;

public class ExampleGrammar4 {
    public static (List<IShape>, List<(IShape, List<IShape>)>) Run(int min, int max, uint times) {
        Rules rules = new Rules();

        rules.AddRule(
            new Attributes(
                ("Floors", new ScalarAttribute(3f))
            ),
            (Start start) => {
                Vertex[] v1 = start.l2.Sections(0.3f, 0.65f);
                Vertex[] v2 = start.l4.Sections(0.35f, 0.7f);

                Floor f1 = new Floor(rules, start.Attributes.Copy(), (0, 2), (start.v1, start.v2, v1[1], v2[0]));
                Floor f2 = new Floor(rules, start.Attributes.Copy(), (0, 1), (v2[0], v1[1], v1[0], v2[1]));
                Floor f3 = new Floor(rules, start.Attributes.Copy(), (0, 0), (v2[1], v1[0], start.v3, start.v4));
              
                return new List<IShape> {f1, f2, f3};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Floors", new ScalarAttribute(2f)),
                ("Wide", new ScalarAttribute(0f))
            ),
            (Start start) => {
                Vertex[] v1 = start.l1.Sections(4);
                Vertex[] v2 = start.l3.Sections(4);

                Attributes a = start.Attributes.Copy();
                a.Set("RoofSlanted", new ScalarAttribute(1f));

                Vertical s1 = new Vertical(rules, a.Copy(), (0, 0), (start.v1, v1[3], v2[0], start.v4));
                Vertical s2 = new Vertical(rules, start.Attributes.Copy(), (1, 0), (v1[3], v1[2], v2[1], v2[0]));
                Vertical s3 = new Vertical(rules, start.Attributes.Copy(), (2, 0), (v1[2], v1[1], v2[2], v2[1]));
                Vertical s4 = new Vertical(rules, start.Attributes.Copy(), (3, 0), (v1[1], v1[0], v2[3], v2[2]));
                Vertical s5 = new Vertical(rules, a.Copy(), (4, 0), (v1[0], start.v2, start.v3, v2[3]));

                VirtualConnection.Connect(s1, s5, true, false, true);

                return new List<IShape> {s1, s2, s3, s4, s5};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Floors", new ScalarAttribute(2f)),
                ("Wide", new ScalarAttribute(1f))
            ),
            (Start start) => {
                Vertex[] v1 = start.l1.Sections(10);
                Vertex[] v2 = start.l3.Sections(10);

                Vertical s1 = new Vertical(rules, start.Attributes.Copy(), (0, 0), (start.v1, v1[9], v2[0], start.v4));
                Vertical s2 = new Vertical(rules, start.Attributes.Copy(), (1, 0), (v1[9], v1[8], v2[1], v2[0]));
                Vertical s3 = new Vertical(rules, start.Attributes.Copy(), (2, 0), (v1[8], v1[7], v2[2], v2[1]));
                Vertical s4 = new Vertical(rules, start.Attributes.Copy(), (3, 0), (v1[7], v1[6], v2[3], v2[2]));
                Vertical s5 = new Vertical(rules, start.Attributes.Copy(), (4, 0), (v1[6], v1[5], v2[4], v2[3]));
                Vertical s6 = new Vertical(rules, start.Attributes.Copy(), (5, 0), (v1[5], v1[4], v2[5], v2[4]));
                Vertical s7 = new Vertical(rules, start.Attributes.Copy(), (6, 0), (v1[4], v1[3], v2[6], v2[5]));
                Vertical s8 = new Vertical(rules, start.Attributes.Copy(), (7, 0), (v1[3], v1[2], v2[7], v2[6]));
                Vertical s9 = new Vertical(rules, start.Attributes.Copy(), (8, 0), (v1[2], v1[1], v2[8], v2[7]));
                Vertical s10 = new Vertical(rules, start.Attributes.Copy(), (9, 0), (v1[1], v1[0], v2[9], v2[8]));
                Vertical s11 = new Vertical(rules, start.Attributes.Copy(), (10, 0), (v1[0], start.v2, start.v3, v2[9]));

                return new List<IShape> {s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11};
            }
        );

        rules.AddRule(
            new Attributes (
                ("RoofSlanted", new ScalarAttribute(0f))
            ),
            (Vertical vertical) => {
                Vertex[] v1 = vertical.l2.Sections(0.3f, 0.65f);
                Vertex[] v2 = vertical.l4.Sections(0.35f, 0.7f);

                Section s1 = new Section(rules, vertical.Attributes.Copy(), (0, 2), (vertical.v1, vertical.v2, v1[1], v2[0]));
                Section s2 = new Section(rules, vertical.Attributes.Copy(), (0, 1), (v2[0], v1[1], v1[0], v2[1]));
                Roof r = new Roof(rules, vertical.Attributes.Copy(), (0, 0), (v2[1], v1[0], vertical.v3, vertical.v4));
              
                return new List<IShape> {s1, s2, r};
            }
        );

        rules.AddRule(
            new Attributes (
                ("RoofSlanted", new ScalarAttribute(1f))
            ),
            (Vertical vertical) => {
                Vertex[] v1 = vertical.l2.Sections(0.3f, 0.65f);
                Vertex[] v2 = vertical.l4.Sections(0.35f, 0.7f);

                Section s = new Section(rules, vertical.Attributes.Copy(), (0, 2), (vertical.v1, vertical.v2, v1[1], v2[0]));
                Roof r = new Roof(rules, vertical.Attributes.Copy(), (0, 1), (v2[0], v1[1], v1[0], v2[1]));

                return new List<IShape> {s, r};
            }
        );

        rules.AddRule(
            new Attributes(
                ("RoofSlanted", new ScalarAttribute(1f))
            ),
            (Roof roof) => {
                Vertex v = roof.l2.Bisect();
                Triad t = new Triad(rules, roof.Attributes.Copy(), (0, 0), (v, roof.v1, roof.v2));

                return new List<IShape> {t};
            }
        );

        rules.AddRule(
            new Attributes(
                ("RoofSize", new ScalarAttribute(1f)),
                ("RoofSlanted", new ScalarAttribute(0f))
            ),
            (Roof roof) => {
                Vertex v1 = roof.l2.Bisect(0.6f);
                Vertex v2 = roof.l4.Bisect(0.4f);

                Quad q = new Quad(rules, roof.Attributes.Copy(), (0, 0), (roof.v2, v1, v2, roof.v1));

                return new List<IShape> {q};
            }
        );

        rules.AddRule(
            new Attributes(
                ("RoofSize", new ScalarAttribute(2f)),
                ("RoofSlanted", new ScalarAttribute(0f))
            ),
            (Roof roof) => {
                Vertex v1 = roof.l2.Bisect(0.7f);
                Vertex v2 = roof.l4.Bisect(0.3f);

                Quad q = new Quad(rules, roof.Attributes.Copy(), (0, 0), (roof.v2, v1, v2, roof.v1));

                return new List<IShape> {q};
            }
        );

        rules.AddRule(
            new Attributes(
                ("RoofSize", new ScalarAttribute(3f)),
                ("RoofSlanted", new ScalarAttribute(0f))
            ),
            (Roof roof) => {
                Vertex v1 = roof.l2.Bisect(0.8f);
                Vertex v2 = roof.l4.Bisect(0.2f);

                Quad q = new Quad(rules, roof.Attributes.Copy(), (0, 0), (roof.v2, v1, v2, roof.v1));

                return new List<IShape> {q};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Sections", new ScalarAttribute(5f))
            ),
            (Floor floor) => {
                Vertex[] v1 = floor.l1.Sections(4);
                Vertex[] v2 = floor.l3.Sections(4);

                Section s1 = new Section(rules, floor.Attributes.Copy(), (0, 0), (floor.v1, v1[3], v2[0], floor.v4));
                Section s2 = new Section(rules, floor.Attributes.Copy(), (1, 0), (v1[3], v1[2], v2[1], v2[0]));
                Section s3 = new Section(rules, floor.Attributes.Copy(), (2, 0), (v1[2], v1[1], v2[2], v2[1]));
                Section s4 = new Section(rules, floor.Attributes.Copy(), (3, 0), (v1[1], v1[0], v2[3], v2[2]));
                Section s5 = new Section(rules, floor.Attributes.Copy(), (4, 0), (v1[0], floor.v2, floor.v3, v2[3]));

                return new List<IShape> {s1, s2, s3, s4, s5};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Sections", new ScalarAttribute(4f))
            ),
            (Floor floor) => {
                Vertex[] v1 = floor.l1.Sections(3);
                Vertex[] v2 = floor.l3.Sections(3);

                Section s1 = new Section(rules, floor.Attributes.Copy(), (0, 0), (floor.v1, v1[2], v2[0], floor.v4));
                Section s2 = new Section(rules, floor.Attributes.Copy(), (1, 0), (v1[2], v1[1], v2[1], v2[0]));
                Section s3 = new Section(rules, floor.Attributes.Copy(), (2, 0), (v1[1], v1[0], v2[2], v2[1]));
                Section s4 = new Section(rules, floor.Attributes.Copy(), (4, 0), (v1[0], floor.v2, floor.v3, v2[2]));

                return new List<IShape> {s1, s2, s3, s4};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Door", new ScalarAttribute(0f)),
                ("Corn", new ScalarAttribute(1f)),
                ("Bar", new ScalarAttribute(1f)),
                ("Stairs", new ScalarAttribute(0f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l2.Sections(0.1f, 0.85f);
                Vertex[] v2 = section.l4.Sections(0.15f, 0.9f);

                Attributes a = section.Attributes.Copy();
                a.Set("Corn", new ScalarAttribute(0f));
                a.Set("Bar", new ScalarAttribute(0f));
                
                Attributes b = a.Copy();
                b.Set("R", new ScalarAttribute(255f));
                b.Set("G", new ScalarAttribute(255f));
                b.Set("B", new ScalarAttribute(255f));

                Cornice c = new Cornice(rules, b.Copy(), (0, 0), (section.v1, section.v2, v1[1], v2[0]));
                Section s = new Section(rules, a, (0, 1), (v2[0], v1[1], v1[0], v2[1]));
                Bar bar = new Bar(rules, b.Copy(), (0, 2), (v2[1], v1[0], section.v3, section.v4));

                return new List<IShape> {c, s, bar};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Door", new ScalarAttribute(0f)),
                ("Corn", new ScalarAttribute(1f)),
                ("Bar", new ScalarAttribute(0f)),
                ("Stairs", new ScalarAttribute(0f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l2.Sections(0.1f);
                Vertex[] v2 = section.l4.Sections(0.9f);

                Attributes a = section.Attributes.Copy();
                a.Set("Corn", new ScalarAttribute(0f));
                a.Set("Bar", new ScalarAttribute(0f));

                Attributes b = a.Copy();
                b.Set("R", new ScalarAttribute(255f));
                b.Set("G", new ScalarAttribute(255f));
                b.Set("B", new ScalarAttribute(255f));

                Section s = new Section(rules, a, (0, 0), (section.v1, section.v2, v1[0], v2[0]));
                Cornice c = new Cornice(rules, b, (0, 1), (v2[0], v1[0], section.v3, section.v4));

                return new List<IShape> {s, c};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Corn", new ScalarAttribute(0f)),
                ("Bar", new ScalarAttribute(0f)),
                ("Door", new ScalarAttribute(0f)),
                ("Sections", new ScalarAttribute(4f)),
                ("Stairs", new ScalarAttribute(0f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l1.Sections(0.25f, 0.75f);
                Vertex[] v2 = section.l3.Sections(0.25f, 0.75f);

                Line l1 = new Line(v1[0], v2[1]);
                Line l2 = new Line(v1[1], v2[0]);

                Vertex[] v3 = l1.Sections(0.1f, 0.82f);
                Vertex[] v4 = l2.Sections(0.1f, 0.82f);

                Quad q1 = new Quad(rules, section.Attributes.Copy(), (0, 0), (section.v1, section.v4, v2[0], v1[1]));
                Quad q2 = new Quad(rules, section.Attributes.Copy(), (0, 2), (v1[0], v2[1], section.v3, section.v2));

                Quad q3 = new Quad(rules, section.Attributes.Copy(), (0, 3), (v2[0], v4[0], v3[0], v2[1]));
                Quad q4 = new Quad(rules, section.Attributes.Copy(), (0, 4), (v1[1], v4[1], v3[1], v1[0]));
         
                Attributes a = section.Attributes.Copy();
                a.Set("R", new ScalarAttribute(255f));
                a.Set("G", new ScalarAttribute(255f));
                a.Set("B", new ScalarAttribute(255f));

                Window w = new Window(rules, a, (1, 0), (v4[0], v4[1], v3[1], v3[0]));

                return new List<IShape> {q1, q2, q3, q4, w};
            }
        );
    
        rules.AddRule(
            new Attributes(
                ("Corn", new ScalarAttribute(0f)),
                ("Bar", new ScalarAttribute(0f)),
                ("Door", new ScalarAttribute(0f)),
                ("Sections", new ScalarAttribute(5f)),
                ("Stairs", new ScalarAttribute(0f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l1.Sections(0.2f, 0.8f);
                Vertex[] v2 = section.l3.Sections(0.2f, 0.8f);

                Line l1 = new Line(v1[0], v2[1]);
                Line l2 = new Line(v1[1], v2[0]);

                Vertex[] v3 = l1.Sections(0.1f, 0.82f);
                Vertex[] v4 = l2.Sections(0.1f, 0.82f);

                Quad q1 = new Quad(rules, section.Attributes.Copy(), (0, 0), (section.v1, section.v4, v2[0], v1[1]));
                Quad q2 = new Quad(rules, section.Attributes.Copy(), (0, 2), (v1[0], v2[1], section.v3, section.v2));

                Quad q3 = new Quad(rules, section.Attributes.Copy(), (0, 3), (v2[0], v4[0], v3[0], v2[1]));
                Quad q4 = new Quad(rules, section.Attributes.Copy(), (0, 4), (v1[1], v4[1], v3[1], v1[0]));
         
                Window w = new Window(rules, section.Attributes.Copy(), (1, 0), (v4[0], v4[1], v3[1], v3[0]));

                return new List<IShape> {q1, q2, q3, q4, w};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Stairs", new ScalarAttribute(1f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l1.Sections(0.15f, 0.85f);
                Vertex[] v2 = section.l3.Sections(0.15f, 0.85f);

                Quad q1 = new Quad(rules, section.Attributes.Copy(), (0, 0), (section.v1, section.v4, v2[0], v1[1]));
                Quad q2 = new Quad(rules, section.Attributes.Copy(), (0, 2), (v1[0], v2[1], section.v3, section.v2));
         
                Window w = new Window(rules, section.Attributes.Copy(), (1, 0), (v2[0], v1[1], v1[0], v2[1]));

                return new List<IShape> {q1, q2, w};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Door", new RangeAttribute(1f, 2f))
            ),
            (Section section) => {
                Vertex[] v1 = section.l2.Sections(0.20f, 0.25f);
                Vertex[] v2 = section.l4.Sections(0.75f, 0.8f);

                Line l = new Line(v1[1], v2[0]);

                Vertex[] v3 = l.Sections(0.07f, 0.93f);
                Vertex[] v4 = section.l1.Sections(0.07f, 0.93f);


                // Upper area
                Door door = new Door(rules, section.Attributes.Copy(), (1, 0), (v4[1], v4[0], v3[1], v3[0]));
                Quad mid = new Quad(rules, section.Attributes.Copy(), (2, 0), (v2[0], v1[1], v1[0], v2[1]));
                Quad upper = new Quad(rules, section.Attributes.Copy(), (3, 0), (v2[1], v1[0], section.v3, section.v4));

                // Sides
                Quad side1 = new Quad(rules, section.Attributes.Copy(), (0, 1), (section.v1, v2[0], v3[0], v4[1]));
                Quad side2 = new Quad(rules, section.Attributes.Copy(), (1, 1), (section.v2, v4[0], v3[1], v1[1]));

                return new List<IShape> {door, mid, upper, side1, side2};
            }
        );

        rules.AddRule(
            new Attributes(
                
            ),
            (Window window) => {
                Vertex[] v1 = window.l1.Sections(0.05f, 0.95f);
                Vertex[] v2 = window.l3.Sections(0.05f, 0.95f);

                Line l1 = new Line(v1[0], v2[1]);
                Line l2 = new Line(v1[1], v2[0]);

                Vertex[] v3 = l1.Sections(0.1f, 0.9f);
                Vertex[] v4 = l2.Sections(0.1f, 0.9f);

                Quad q1 = new Quad(rules, window.Attributes.Copy(), (0, 0), (window.v1, window.v4, v2[0], v1[1]));
                Quad q2 = new Quad(rules, window.Attributes.Copy(), (0, 2), (v1[0], v2[1], window.v3, window.v2));

                Quad q3 = new Quad(rules, window.Attributes.Copy(), (0, 3), (v2[0], v4[0], v3[0], v2[1]));
                Quad q4 = new Quad(rules, window.Attributes.Copy(), (0, 4), (v1[1], v4[1], v3[1], v1[0]));
         
                Glass g = new Glass(rules, window.Attributes.Copy(), (1, 0), (v4[0], v4[1], v3[1], v3[0]));

                return new List<IShape> {q1, q2, q3, q4, g};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Win", new ScalarAttribute(0f))
            ),
            (Glass glass) => {
                Quad q = new Quad(rules, glass.Attributes.Copy(), (0, 0), (glass.v1, glass.v2, glass.v3, glass.v4));

                return new List<IShape> {q};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Win", new ScalarAttribute(0.3f))
            ),
            (Glass glass) => {
                Vertex[] v1 = glass.l2.Sections(0.7f);
                Vertex[] v2 = glass.l4.Sections(0.3f);

                Quad b = new Quad(rules, glass.Attributes.Copy(), (0, 0), (glass.v1, glass.v2, v1[0], v2[0]));
                Quad t = new Quad(rules, glass.Attributes.Copy(), (0, 1), (v2[0], v1[0], glass.v3, glass.v4));

                return new List<IShape> {t, b};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Win", new ScalarAttribute(0.5f))
            ),
            (Glass glass) => {
                Vertex v1 = glass.l2.Bisect(0.7f);
                Vertex v2 = glass.l4.Bisect(0.3f);

                Line l = new Line(v1, v2);

                Vertex v3 = l.Bisect(0.3f);
                Vertex v4 = glass.l3.Bisect(0.3f);

                Quad t = new Quad(rules, glass.Attributes.Copy(), (0, 0), (glass.v1, glass.v2, v1, v2));
                // Quad t = new Quad(rules, glass.Attributes.Copy(), (0, 1), (v2, v1, glass.v3, glass.v4));

                Quad b1 = new Quad(rules, glass.Attributes.Copy(), (1, 0), (glass.v4, v2, v3, v4));
                Quad b2 = new Quad(rules, glass.Attributes.Copy(), (1, 1), (v4, v3, v1, glass.v3));

                return new List<IShape> {t, b1, b2};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Win", new ScalarAttribute(0.8f))
            ),
            (Glass glass) => {
                Vertex v1 = glass.l1.Bisect();
                Vertex v2 = glass.l3.Bisect();

                Line l = new Line(v1, v2);

                Vertex[] v3 = glass.l2.Sections(3);
                Vertex[] v4 = glass.l4.Sections(3);
                Vertex[] v5 = l.Sections(3);

                Quad q1 = new Quad(rules, glass.Attributes.Copy(), (0, 0), (glass.v2, v1, v5[2], v3[2]));
                Quad q2 = new Quad(rules, glass.Attributes.Copy(), (0, 1), (v1, v5[2], v4[0], glass.v1));
                Quad q3 = new Quad(rules, glass.Attributes.Copy(), (0, 2), (v3[2], v5[2], v5[1], v3[1]));
                Quad q4 = new Quad(rules, glass.Attributes.Copy(), (0, 3), (v3[1], v5[1], v5[0], v3[0]));
                Quad q5 = new Quad(rules, glass.Attributes.Copy(), (0, 4), (v3[0], v5[0], v2, glass.v3));
                Quad q6 = new Quad(rules, glass.Attributes.Copy(), (0, 5), (v5[2], v5[1], v4[1], v4[0]));
                Quad q7 = new Quad(rules, glass.Attributes.Copy(), (0, 6), (v5[1], v5[0], v4[2], v4[1]));
                Quad q8 = new Quad(rules, glass.Attributes.Copy(), (0, 7), (v5[0], v2, glass.v4, v4[2]));

                return new List<IShape> {q1, q2, q3, q4, q5, q6, q7, q8};
            }
        );

        rules.AddRule(
            new Attributes(
                ("Door", new ScalarAttribute(1f))
            ),
            (Door door) => {
                return new List<IShape>();
            }
        );

        rules.AddRule(
            new Attributes(
                ("Door", new ScalarAttribute(2f))
            ),
            (Door door) => {
                Vertex v1 = door.l1.Bisect();
                Vertex v2 = door.l3.Bisect();

                Quad q1 = new Quad(rules, door.Attributes.Copy(), (0, 0), (door.v2, door.v3, v2, v1));
                Quad q2 = new Quad(rules, door.Attributes.Copy(), (0, 1), (v1, v2, door.v4, door.v1));

                return new List<IShape> {q1, q2};
            }
        );

        rules.AddRule(null, (Cornice cornice) => {return new List<IShape>();});
        rules.AddRule(null, (Bar bar) => {return new List<IShape>();});
        rules.AddRule(null, (Quad bar) => {return new List<IShape>();});    

        Start start = new Start(
            rules,
            new Attributes(
                ("Floors", new ScalarAttribute(2f)),
                ("Door", new ScalarAttribute(0f)),
                ("Corn", new ScalarAttribute(1f)),
                ("Bar", new ScalarAttribute(0f)),
                ("Win", new ScalarAttribute(0.8f)),
                ("Sections", new ScalarAttribute(4f)),
                ("Stairs", new ScalarAttribute(0f)),
                ("R", new ScalarAttribute(255f)),
                ("G", new ScalarAttribute(255f)),
                ("B", new ScalarAttribute(255f)),
                ("WFC", new ScalarAttribute(1f)),
                ("RoofSize", new ScalarAttribute(3f)),
                ("Wide", new ScalarAttribute(0f)),
                ("Roof", new ScalarAttribute(0f)),
                ("RoofSlanted", new ScalarAttribute(0f))
            ),
            (0, 0),
           (new Vertex(min, max), 
            new Vertex(max, max), 
            new Vertex(max, min), 
            new Vertex(min, min))
        );

        // Define the control grammar
        Control control = new Control();
        control.Grammar.AddRule("Start", new string[] {"Door", "Bar", "Cornice", "W1", "W2", "W3", "Sections"});
        control.Grammar.AddRule("Door", ((0, 2), "D0"));
        control.Grammar.AddRule("Door", ((0, 2), "D1"));
        control.Grammar.AddRule("Door", ((0, 2), "D2"));
        control.Grammar.AddRule("Door", new ControlAssignment[] {((0, 2), "D0"), ((0, 1), "S0"), ((0, 0), "S0")});
        control.Grammar.AddRule("Door", new ControlAssignment[] {((0, 2), "D1"), ((0, 1), "S1"), ((0, 0), "S1")});
        control.Grammar.AddRule("Door", new ControlAssignment[] {((0, 2), "D2"), ((0, 1), "S2"), ((0, 0), "S2")});
        control.Grammar.AddRule("Cornice", ((0, ~0u), "Corn", 1f, ""));
        control.Grammar.AddRule("Bar", ((0, 2), "Bar", 1f, ""));
        control.Grammar.AddRule("W1", ((0, 1), "Win", 0.5f, ""));
        control.Grammar.AddRule("W1", ((0, 1), "Win", 0.8f, ""));
        control.Grammar.AddRule("W2", ((0, 2), "Win", 0.3f, ""));
        control.Grammar.AddRule("W2", ((0, 2), "Win", 0.5f, ""));
        control.Grammar.AddRule("W3", ((0, 3), "Win", 0.5f, ""));
        control.Grammar.AddRule("W3", ((0, 3), "Win", 0.8f, ""));
        control.Grammar.AddRule("Sections", ((~0u, ~0u), "Sections", 4f, ""));
        control.Grammar.AddRule("Sections", ((~0u, ~0u), "Sections", 5f, ""));

        control.Grammar.AddRule("D0", ((0, 0), "Door", 1f, ""));
        control.Grammar.AddRule("D0", ((0, 0), "Door", 2f, ""));
        control.Grammar.AddRule("D1", ((1, 0), "Door", 1f, ""));
        control.Grammar.AddRule("D1", ((1, 0), "Door", 2f, ""));
        control.Grammar.AddRule("D2", ((3, 0), "Door", 1f, ""));
        control.Grammar.AddRule("D2", ((3, 0), "Door", 2f, ""));

        control.Grammar.AddRule("S0", new ControlAssignment[] {((0, 0), "Stairs", 1f, ""), ((0, 0), "Win", 0.8f, "")});
        control.Grammar.AddRule("S1", new ControlAssignment[] {((1, 0), "Stairs", 1f, ""), ((1, 0), "Win", 0.8f, "")});
        control.Grammar.AddRule("S2", new ControlAssignment[] {((3, 0), "Stairs", 1f, ""), ((3, 0), "Win", 0.8f, "")});

        control.Grammar.AddRule("Start2", new ControlAssignment[] {((0, 0), "WFC1", ""), ((0, 0), "WFC2", "")});

        control.AddWFC("WFC1");
        ControlAssignment[] black = new ControlAssignment[] {("R", 40f, ""), ("G", 40f, ""), ("B", 40f, "")};
        ControlAssignment[] bronze = new ControlAssignment[] {("R", 205f, ""), ("G", 127f, ""), ("B", 50f, "")};
        ControlAssignment[] brick = new ControlAssignment[] {("R", 132, ""), ("G", 31f, ""), ("B", 39, "")};
        ControlAssignment[] cement = new ControlAssignment[] {("R", 206f, ""), ("G", 205f, ""), ("B", 203f, "")};
        ControlAssignment[] wood = new ControlAssignment[] {("R", 202f, ""), ("G", 164f, ""), ("B", 114f, "")};
        control.WFC["WFC1"].AddRule("Black", black, 0.3f, "Bronze", "Brick", "Cement", "Wood");
        control.WFC["WFC1"].AddRule("Bronze", bronze, 1f, "Black", "Brick", "Cement", "Wood");
        control.WFC["WFC1"].AddRule("Brick", brick, 1f, "Black", "Bronze", "Cement", "Wood");
        control.WFC["WFC1"].AddRule("Cement", cement, 1f, "Black", "Bronze", "Brick", "Wood");
        control.WFC["WFC1"].AddRule("Wood", wood, 1f, "Black", "Bronze", "Brick", "Cement");

        control.AddWFC("WFC2");
        control.WFC["WFC2"].AddRule("R1", ("RoofSize", 1f, ""), 1f, "R2");
        control.WFC["WFC2"].AddRule("R2", ("RoofSize", 2f, ""), 1f, "R1", "R3");
        control.WFC["WFC2"].AddRule("R3", ("RoofSize", 3f, ""), 1f, "R2");

        start.Control.Add("Start");

        var ruleexamples = rules.RuleExamples(100f);

        List<IShape> shapes = Interpreter2.Interpret(start, rules, control, 0);

        return (shapes, ruleexamples);
    }
}