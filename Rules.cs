using System.Collections.Generic;
using System;

public class Rules {
    private Dictionary<Type, List<Func<Shape, List<Shape>>>> rules;

    public Rules() {
        this.rules = new Dictionary<Type, List<Func<Shape, List<Shape>>>>();

        // Add an empty rule
        this.rules.Add(typeof(void), new List<Func<Shape, List<Shape>>>());
    }

    public void AddRule<T>(Func<T, List<Shape>> ruleFunctor) where T: Shape {
        if (!this.rules.ContainsKey(typeof(T))) {
            this.rules.Add(typeof(T), new List<Func<Shape, List<Shape>>>());
        }

        Func<Shape, List<Shape>> func = (Shape s) => ruleFunctor((T) s);

        this.rules[typeof(T)].Add(func);
    }
    
    public List<Func<Shape, List<Shape>>> GetRules<T>() {
        if (!this.rules.ContainsKey(typeof(T)))
            return this.rules[typeof(void)];

        return this.rules[typeof(T)];
    }
}