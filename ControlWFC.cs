using System.Collections.Generic;
using System.Collections;
using System;
using Shape;

public class ControlWFC {
    private List<Wave> waves;
    private Dictionary<string, Wave> waveMap;
    private WaveFunction core;
    private Dictionary<string, (ICollection<ControlAssignment>, string[])> rules;
    private HashSet<string> symbols;
    private bool hasBeenEncoded = false;

    public ControlWFC() {
        this.waves = new List<Wave>();
        this.waveMap = new Dictionary<string, Wave>();
        this.rules = new Dictionary<string, (ICollection<ControlAssignment>, string[])>();
        this.symbols = new HashSet<string>();
    }

    public void AddRule(string wave, ICollection<ControlAssignment> cas, float weight, params string[] adjacencies) {
        this.rules.Add(wave, (cas, adjacencies));

        Wave nw = new Wave(1, wave, weight);
        this.waves.Add(nw);
        this.waveMap.Add(wave, nw);

        this.symbols.Add(wave);
    }

    public void AddRule(string wave, ControlAssignment ca, float weight, params string[] adjacencies) {
        List<ControlAssignment> cas = new List<ControlAssignment> {ca};
        this.AddRule(wave, cas, weight, adjacencies);
    }

    private void Encode() {
        // Add the constraints to the waves
        foreach (string symbol in this.symbols) {
            (ICollection<ControlAssignment> ca, string[] adjacencies) = this.rules[symbol];

            // Get the wave
            Wave w = this.waveMap[symbol];

            // Compose the adjacencies
            Wave[] adjc = new Wave[adjacencies.Length];

            for (uint i = 0; i < adjacencies.Length; ++i) {
                adjc[i] = this.waveMap[adjacencies[i]];
            }

            // Add the adjacencies
            w.AddConstraints(0, adjc);
        }

        // Create the WFC core that uses given waves
        this.core = new WaveFunction(this.waves.ToArray(), 1); 

        this.hasBeenEncoded = true;
    }

    private class Node {
        public uint[] wave;
        public ShapeGraph.Node node;
        public uint entropy;
        public HashSet<Node> connections;

        public Node(uint[] wave, ShapeGraph.Node node, uint entropy) {
            this.node = node;
            this.wave = wave;
            this.entropy = entropy;

            this.connections = new HashSet<Node>();
        }

        public void Connect(Node other) {
            this.connections.Add(other);
            other.connections.Add(this);
        }

        public override int GetHashCode() {
            return this.node.GetHashCode();
        }
    }

    private uint Propagate(Node lowestEntropyNode, Heap<Node> entropyClasses) {
        Heap<Node> propagation = new Heap<Node>(Comparer<Node>.Create((x, y) => {return (int)y.entropy - (int)x.entropy;}));
        uint done = 0;

        // Now we need to propagate the change
        lowestEntropyNode.entropy = 0U;
        propagation.Add(lowestEntropyNode);

        // Propagate the node
        Node pn;

        // Old and new entropy
        uint propageeEntropy;
        uint newEntropy;

        // Run until the state has been completly propagated.
        // In other words, propagate until there is nothing more
        // to propagate. AKA until propagation stack has no more
        // elements.
        while (propagation.Count > 0) {
            // Get the propagator. Note that it has the lowest entropy
            pn = propagation.Pop();

            // Propagate to each edge
            foreach (Node connection in pn.connections) {
                // Old and new entropy (after propagating the state)
                propageeEntropy = connection.entropy;
                newEntropy = this.core.Collapse(pn.wave, connection.wave);

#if(DEBUG)
                // Throw an exception if the new entropy is negative
                // This usually indicates that the logic is faulty, not
                // that the programmer made some mistake.
                if (newEntropy > ~0U - 100)
                    throw new Exception($"New entropy is negative ({newEntropy}, was {propageeEntropy}): ({pn}) propagated to ({connection}), propagator {Convert.ToString(pn.wave[0], 2)}");
#endif

                // The wavefunction changed due to propagation
                if (propageeEntropy != newEntropy) {
                    // Update done
                    if (newEntropy == 0)
                        done += 1;

                    // Update the entropy
                    connection.entropy = newEntropy;

                    propagation.Add(connection);

                    // Change the entropy class
                    if (entropyClasses.Contains(connection))
                        entropyClasses.Decrease(connection, connection);
                }
            }
        }

        return done;
    }

    public HashSet<ControlAssignment> Collapse(ShapeGraph sg, Dictionary<ShapeGraph.Node, HashSet<string>> constraints) {
        if (!this.hasBeenEncoded)
            this.Encode();

        // Create a new graph of wavefunctions
        // that has the same topology as the ShapeGraph
        HashSet<Node> graph = new HashSet<Node>();
        Dictionary<ShapeGraph.Node, Node> nodemap = new Dictionary<ShapeGraph.Node, Node>();

        foreach (ShapeGraph.Node node in sg) {
            // Create a new wave for this node
            uint[] wave;

            // If there are constraints
            if (constraints != null && constraints.ContainsKey(node)) {
                HashSet<string> symbols = constraints[node];
                List<Wave> cwaves = new List<Wave>();
                foreach (string symbol in symbols) {
                    cwaves.Add(this.waveMap[symbol]);
                }

                wave = this.core.wencoder.GetPossibilitySpace(cwaves);
            }

            // If no constraints are given
            else 
                wave = this.core.wencoder.GetFull();

            // Add it to the graph
            Node wfcnode = new Node(wave, node, 1);
            wfcnode.entropy = this.core.wencoder.GetEntropy(wave);
            graph.Add(wfcnode);
            nodemap.Add(node, wfcnode);
        }

        // Add the connections
        foreach (Node node in graph) {
            foreach ((ShapeGraph.Node conn, Attributes a) in node.node.Connections) {
                node.Connect(nodemap[conn]);
            }
        }

        // Now that we have the graph, we can start collapsing the state
        uint done = 0;
        uint need = (uint) graph.Count;

        // Count the initial finished states
        foreach (Node node in graph) {
            if (node.entropy == 0)
                done += 1;
        }

        Heap<Node> entropyClasses = new Heap<Node>(Comparer<Node>.Create((x, y) => {return (int)y.entropy - (int)x.entropy;}));

        // Fill the classes
        foreach (Node node in graph) {
            entropyClasses.Add(node);
        }

        Node lowestEntropyNode = null;

        // Execute until every tile has 0 entropy
        while (done < need) {
            // Get the tile with lowest entropy
            // The value of lowestEntropyNode should be non-default after
            // the loop. If it is not, then there is a severe bug
            // somewhere in this loop.
            lowestEntropyNode = entropyClasses.Pop();

            // If the tile has not been propagated yet
            uint did = this.Propagate(lowestEntropyNode, entropyClasses);
            if (did != 0) {
                done += did;
                continue;
            }

            // Collapse the wave on the tile
            this.core.Collapse(lowestEntropyNode.wave);
            
            // Change the entropy class
            // entropyClasses.ChangeClass(lowestEntropy, 0, lowestEntropyNode);
            done += 1;
            lowestEntropyNode.entropy = 0;

            // Propagate the collapsed state
            done += this.Propagate(lowestEntropyNode, entropyClasses);
        }

        // Now get all the assignments
        HashSet<ControlAssignment> cas = new HashSet<ControlAssignment>();
        foreach (Node node in graph) {
            Wave[] waves = this.core.wencoder.GetWaves(node.wave);
            ICollection<ControlAssignment> wcas = this.rules[waves[0].name].Item1;

            foreach(ControlAssignment ca in wcas) {
                ControlAssignment nca = ca.Copy();

                // Set the location of the assignement to the shape
                nca.SetLoc(node.node.Shape.Locator);

                // Add to the output
                cas.Add(nca);
            }
        }

        return cas;
    }
}