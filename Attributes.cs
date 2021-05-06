using System.Collections.Generic;
using System;

public class Attributes {
    private Dictionary<string, IAttribute> attributes;

    public Dictionary<string, IAttribute>.Enumerator GetEnumerator() {
        return this.attributes.GetEnumerator();
    }

    public bool ContainsKey(string key) {
        return this.attributes.ContainsKey(key);
    }

    public Attributes() {
        this.attributes = new Dictionary<string, IAttribute>();
    }

    public Attributes(Attributes source) {
        this.attributes = new Dictionary<string, IAttribute>(source.attributes);
    }

    public Attributes(params (string, IAttribute)[] kvps) {
        this.attributes = new Dictionary<string, IAttribute>();
        this.Add(kvps);
    }

    public Attributes Copy() {
        return new Attributes(this);
    }

    public void Add(params (string, IAttribute)[] kvps) {
        foreach ((string name, IAttribute attr) kvp in kvps) {
            this.Add(kvp.name, kvp.attr);
        }
    }

    public void Add(string name, IAttribute attribute) {
        if (this.attributes.ContainsKey(name))
            throw new Exception($"This attribute already exists: {name}");

        this.attributes.Add(name, attribute);
    }

    public void Overwrite(params (string, IAttribute)[] kvps) {
        foreach ((string name, IAttribute attr) kvp in kvps) {
            this.Overwrite(kvp.name, kvp.attr);
        }
    }

    public void Overwrite(string name, IAttribute attribute) {
        if (!this.attributes.ContainsKey(name))
            throw new Exception($"This attribute does not exist: {name}");

        this.attributes[name] = attribute;
    }

    public void Set(string name, IAttribute attribute) {
        if (!this.attributes.ContainsKey(name)) {
            this.attributes.Add(name, attribute);
        }

        else {
            this.attributes[name] = attribute;
        }
    }

    public IAttribute Get(string name) {
        return this.attributes[name];
    }

    public bool Matches(Attributes other) {
        bool matches = true;

        // Check common attributes
        HashSet<string> commonKeys = new HashSet<string>(this.attributes.Keys);
        commonKeys.IntersectWith(other.attributes.Keys);
        foreach (string key in commonKeys) {
            matches &= this.attributes[key].Matches(other.attributes[key]);
        }

        if (!matches)
            return false;

        DefaultAttribute da = new DefaultAttribute();

        // Check the attributes that are only here
        HashSet<string> hereKeys = new HashSet<string>(this.attributes.Keys);
        hereKeys.ExceptWith(other.attributes.Keys);
        foreach (string key in hereKeys) {
            this.attributes[key].Matches(da);
        }

        if (!matches)
            return false;

        // Check the attributes that are only in the other
        HashSet<string> thereKeys = new HashSet<string>(other.attributes.Keys);
        hereKeys.ExceptWith(this.attributes.Keys);
        foreach (string key in thereKeys) {
            other.attributes[key].Matches(da);
        }

        return matches;
    }
}