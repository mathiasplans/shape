using System.Collections.Generic;
using System.Collections;
using System;

public class ControlGrammar {
    Dictionary<string, List<string[]>> rules;
    Dictionary<string, List<ControlAssignment[]>> terminals;
    Random rnd;

    public ControlGrammar() {
        this.rules = new Dictionary<string, List<string[]>>();
        this.terminals = new Dictionary<string, List<ControlAssignment[]>>();
        this.rnd = new Random();
    }

    public void AddRule(string lhs, string[] rhs) {
        if (!this.rules.ContainsKey(lhs)) {
            this.rules.Add(lhs, new List<string[]>());
        }

        this.rules[lhs].Add(rhs);
    }

    public void AddRule(string lhs, List<string[]> rhs) {
        foreach (var r in rhs) {
            this.AddRule(lhs, r);
        }
    }

    public void AddRule(string lhs, ControlAssignment[] rhs) {
        if (!this.terminals.ContainsKey(lhs)) {
            this.terminals.Add(lhs, new List<ControlAssignment[]>());
        }

        this.terminals[lhs].Add(rhs);
    }

    public void AddRule(string lhs, ControlAssignment rhs) {
        this.AddRule(lhs, new ControlAssignment[] {rhs});
    }

    public void AddRule(string lhs, List<ControlAssignment[]> rhs) {
        foreach (var r in rhs) {
            this.AddRule(lhs, r);
        }
    }

    public HashSet<ControlAssignment> Interpret(string start) {
        if (start == "") {
            return new HashSet<ControlAssignment>();
        }

        int rand = 0;
        if (this.terminals.ContainsKey(start)) {
            rand = this.rnd.Next(this.terminals[start].Count);
            ControlAssignment[] aslist = this.terminals[start][rand];
            return new HashSet<ControlAssignment>(aslist);
        }

        if (!this.rules.ContainsKey(start))
            throw new Exception($"This symbol is unknow to the control grammar: \"{start}\"");

        rand = this.rnd.Next(this.rules[start].Count);
        string[] derivation = this.rules[start][rand];
        HashSet<ControlAssignment> r = new HashSet<ControlAssignment>();
        foreach (string symbol in derivation) {
            r.UnionWith(this.Interpret(symbol));
        }

        return r;
    }
}