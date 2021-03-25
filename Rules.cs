using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using Shape;

public class Rules {
    private Dictionary<Type, List<Func<IShape, List<IShape>>>> rules;
    private Dictionary<Func<IShape, List<IShape>>, Attributes> attributes;
    private Random rng;

    public Rules() {
        this.rules = new Dictionary<Type, List<Func<IShape, List<IShape>>>>();
        this.attributes = new Dictionary<Func<IShape, List<IShape>>, Attributes>();
        this.rng = new Random();

        // Add an empty rule
        this.rules.Add(typeof(void), new List<Func<IShape, List<IShape>>>());
    }

    public void AddRule<T>(Attributes attrs, Func<T, List<IShape>> ruleFunctor) where T: IShape {
        if (!this.rules.ContainsKey(typeof(T))) {
            this.rules.Add(typeof(T), new List<Func<IShape, List<IShape>>>());
        }

        Func<IShape, List<IShape>> func = (IShape s) => ruleFunctor((T) s);

        this.rules[typeof(T)].Add(func);

        // Data connected to the rule
        if (attrs == null)
            attrs = new Attributes();

        this.attributes.Add(func, attrs);
    }
    
    public List<Func<IShape, List<IShape>>> GetRules<T>() {
        if (!this.rules.ContainsKey(typeof(T)))
            return this.rules[typeof(void)];

        return this.rules[typeof(T)];
    }

    public Func<IShape, List<IShape>> Next<T>() {
        if (!this.rules.ContainsKey(typeof(T)))
            throw new Exception($"This symbol does not exist in the rules database: {typeof(T).ToString()}");

        List<Func<IShape, List<IShape>>> r = this.GetRules<T>();
        if (r.Count > 0) {
            int index = this.rng.Next(r.Count);
            return r[index];
        }

        return null;
    }

    public Func<IShape, List<IShape>> Next<T>(Attributes attrs) {
        if (!this.rules.ContainsKey(typeof(T)))
            throw new Exception($"This symbol does not exist in the rules database: {typeof(T).ToString()}");
 
        List<Func<IShape, List<IShape>>> rawRules = this.GetRules<T>();

        // Filter the rules
        List<Func<IShape, List<IShape>>> r = new List<Func<IShape, List<IShape>>>();
        foreach (var rawRule in rawRules) {
            if (this.attributes[rawRule].Matches(attrs))
                r.Add(rawRule);
        }

        if (r.Count > 0) {
            int index = this.rng.Next(r.Count);
            return r[index];
        }

        return null;
    }
}