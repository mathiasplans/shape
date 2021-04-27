using System.Collections.Generic;
using System.Collections;
using System;
using Shape;

using ForwardCarry = System.Collections.Generic.Dictionary<Shape.ShapeGraph.Node, System.Collections.Generic.HashSet<string>>;
using NodeMap = System.Collections.Generic.Dictionary<(uint, uint), System.Collections.Generic.List<Shape.ShapeGraph.Node>>;

public class Control {
    private ControlGrammar cg;
    private Dictionary<string, ControlWFC> cwfc;
    private HashSet<string> activationSymbols;

    public ControlGrammar Grammar {get {return this.cg;}}
    public Dictionary<string, ControlWFC> WFC {get {return this.cwfc;}}

    public Control() {
        this.cg = new ControlGrammar();
        this.cwfc = new Dictionary<string, ControlWFC>();
        this.activationSymbols = new HashSet<string>();
    }

    public void AddWFC(string activationSymbol) {
        this.cwfc.Add(activationSymbol, new ControlWFC());
        this.activationSymbols.Add(activationSymbol);
    }

    private (ForwardCarry, string) Assign(ICollection<ControlAssignment> assignments, NodeMap nodePlacement) {
        ForwardCarry forwardCarry = new ForwardCarry();
        string activationSymbol = "";
        HashSet<((uint, uint), string)> handledLocators = new HashSet<((uint, uint), string)>();

        // Console.WriteLine("Control");
        foreach (ControlAssignment ca in assignments) {
            // Get all the locators meant by the locator
            List<(uint, uint)> reqLocators = new List<(uint, uint)>();
            foreach ((uint x, uint y) all in nodePlacement.Keys) {
                if ((ca.Loc.x == all.x || ca.Loc.x == ~0u) && (ca.Loc.y == all.y || ca.Loc.y == ~0u)) {
                    if (ca.Loc.x == all.x && ca.Loc.y == all.y)
                        reqLocators.Add(all);

                    else if (!handledLocators.Contains((all, ca.Attr)))
                        reqLocators.Add(all);

                    handledLocators.Add((all, ca.Attr));
                }
            }

            foreach ((uint x, uint y) all in reqLocators) {
                ShapeGraph.Node node = nodePlacement[all][0];

                // If the assignment is nonterminal, it will be used by
                // some following steps in the control pipeline
                if (!ca.IsTerminal) {
                    // If the assignment evokes WFC
                    if (activationSymbol == "" && this.activationSymbols.Contains(ca.Next)) {
                        activationSymbol = ca.Next;
                    }

                    // If the assignment sets a WFC constraint
                    else {
                        if (!forwardCarry.ContainsKey(node)) {
                            forwardCarry.Add(node, new HashSet<string>());
                        }

                        forwardCarry[node].Add(ca.Next);
                    }
                }
                
                else {
                    foreach (ShapeGraph.Node n in nodePlacement[all]) {
                        // Set the attribute
                        if (ca.Attr != "")
                            n.Shape.Attributes.Set(ca.Attr, new ScalarAttribute(ca.Value));

                        // Add the control symbol
                        n.Shape.Control = ca.Next;
                    }
                }
            }
        }

        return (forwardCarry, activationSymbol);
    }

    private (ForwardCarry, string) DoGrammar(IShape shape, NodeMap nodePlacement) {
        // Control grammar
        if (shape.Control != "") {
            HashSet<ControlAssignment> attributeOverwrite = cg.Interpret(shape.Control);
            return this.Assign(attributeOverwrite, nodePlacement);
        }

        return (new ForwardCarry(), "");
    }

    private (ForwardCarry, string) DoWFC((ForwardCarry startState, string actSymbol) state, IShape shape, ShapeGraph splitGraph, NodeMap nodePlacement) {
        // WFC was not activated
        if (!this.cwfc.ContainsKey(state.actSymbol))
            return (new ForwardCarry(), "");

        // Collapse the WFC
        HashSet<ControlAssignment> assignments = this.WFC[state.actSymbol].Collapse(splitGraph, state.startState);
        return this.Assign(assignments, nodePlacement);
    }

    public void Interpret(IShape shape, params ShapeGraph[] splitGraph) {
        NodeMap nodePlacement = new NodeMap();
        foreach (ShapeGraph sg in splitGraph) {
            foreach (ShapeGraph.Node node in sg) {
                // There are duplicate locations
                if (!nodePlacement.ContainsKey(node.Shape.Locator))
                    nodePlacement.Add(node.Shape.Locator, new List<ShapeGraph.Node>());

                nodePlacement[node.Shape.Locator].Add(node);
            }
        }

        (ForwardCarry carry, string act) state;
        state = this.DoGrammar(shape, nodePlacement);
        state = this.DoWFC(state, shape, splitGraph[0], nodePlacement);
        // TODO: Can add more steps into this pipeline
    }
}