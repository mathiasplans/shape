using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using Shape;

public class Rules {
    private Dictionary<Type, List<(Func<IShape, List<IShape>>, Attributes)>> rules;
    private Random rng;

    public Rules() {
        this.rules = new Dictionary<Type, List<(Func<IShape, List<IShape>>, Attributes)>>();
        this.rng = new Random();

        // Add an empty rule
        this.rules.Add(typeof(void), new List<(Func<IShape, List<IShape>>, Attributes)>());
    }

    public void AddRule<T>(Attributes attrs, Func<T, List<IShape>> ruleFunctor) where T: IShape {
        if (!this.rules.ContainsKey(typeof(T))) {
            this.rules.Add(typeof(T), new List<(Func<IShape, List<IShape>>, Attributes)>());
        }

        Func<IShape, List<IShape>> func = (IShape s) => ruleFunctor((T) s);

        // Data connected to the rule
        if (attrs == null)
            attrs = new Attributes();

        this.rules[typeof(T)].Add((func, attrs));
    }
    
    public List<(Func<IShape, List<IShape>>, Attributes)> GetRules<T>() {
        return this.GetRules(typeof(T));
    }

    public List<(Func<IShape, List<IShape>>, Attributes)> GetRules(Type key) {
        if (!this.rules.ContainsKey(key))
            return this.rules[typeof(void)];

        return this.rules[key];
    }

    public Func<IShape, List<IShape>> Next<T>() {
        if (!this.rules.ContainsKey(typeof(T)))
            throw new Exception($"This symbol does not exist in the rules database: {typeof(T).ToString()}");

        List<(Func<IShape, List<IShape>>, Attributes)> r = this.GetRules<T>();
        if (r.Count > 0) {
            int index = this.rng.Next(r.Count);
            return r[index].Item1;
        }

        return null;
    }

    public Func<IShape, List<IShape>> Next<T>(Attributes attrs) {
        if (!this.rules.ContainsKey(typeof(T)))
            throw new Exception($"This symbol does not exist in the rules database: {typeof(T).ToString()}");
 
        List<(Func<IShape, List<IShape>>, Attributes)> rawRules = this.GetRules<T>();

        // Filter the rules
        List<Func<IShape, List<IShape>>> r = new List<Func<IShape, List<IShape>>>();
        foreach (var rawRule in rawRules) {
            if (rawRule.Item2.Matches(attrs))
                r.Add(rawRule.Item1);
        }

        if (r.Count > 0) {
            int index = this.rng.Next(r.Count);
            return r[index];
        }

        return null;
    }

    public HashSet<Type> GetAllLHS() {
        HashSet<Type> o = new HashSet<Type>();
        foreach(Type key in this.rules.Keys) {
            if (key != typeof(void))
                o.Add(key);
        }

        return o;
    }

    public List<(IShape, List<IShape>)> RuleExamples(float width) {
        List<(IShape, List<IShape>)> productions = new List<(IShape, List<IShape>)>();
        foreach (Type key in this.rules.Keys) {
            if (key == typeof(void))
                continue;

            // Get the etalon shape
            IShape etalon = (IShape) key.GetMethod("Etalon").Invoke(null, new object[]{this, width});

            // Use all the rules on the etalon and record them
            foreach ((Func<IShape, List<IShape>> del, Attributes a) in this.rules[key]) {
                productions.Add((etalon, del(etalon)));
            }
        }

        return productions;
    }
}