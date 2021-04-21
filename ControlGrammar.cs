using System.Collections.Generic;
using System.Collections;
using System;

public class ControlGrammar {
    Dictionary<string, List<string[]>> rules;
    Dictionary<string, List<((uint, uint), string, float, string)[]>> terminals;
    Random rnd;

    public ControlGrammar() {
        this.rules = new Dictionary<string, List<string[]>>();
        this.terminals = new Dictionary<string, List<((uint, uint), string, float, string)[]>>();
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

    public void AddRule(string lhs, ((uint, uint), string, float, string)[] rhs) {
        if (!this.terminals.ContainsKey(lhs)) {
            this.terminals.Add(lhs, new List<((uint, uint), string, float, string)[]>());
        }

        this.terminals[lhs].Add(rhs);
    }
    
    public void AddRule(string lhs, ((uint, uint), string, float, string) rhs) {
        this.AddRule(lhs, new ((uint, uint), string, float, string)[] {rhs});
    }

    public void AddRule(string lhs, List<((uint, uint), string, float, string)[]> rhs) {
        foreach (var r in rhs) {
            this.AddRule(lhs, r);
        }
    }

    public HashSet<((uint, uint), string, float, string)> Interpret(string start) {
        if (start == "") {
            return new HashSet<((uint, uint), string, float, string)>();
        }

        int rand = 0;
        if (this.terminals.ContainsKey(start)) {
            rand = this.rnd.Next(this.terminals[start].Count);
            ((uint, uint), string, float, string)[] aslist = this.terminals[start][rand];
            return new HashSet<((uint, uint), string, float, string)>(aslist);
        }

        if (!this.rules.ContainsKey(start))
            throw new Exception($"This symbol is unknow to the control grammar: \"{start}\"");

        rand = this.rnd.Next(this.rules[start].Count);
        string[] derivation = this.rules[start][rand];
        HashSet<((uint, uint), string, float, string)> r = new HashSet<((uint, uint), string, float, string)>();
        foreach (string symbol in derivation) {
            r.UnionWith(this.Interpret(symbol));
        }

        return r;
    }
}