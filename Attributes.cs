// using System.Collections.Generic;
// using System;

// public class Attributes {
//     private Dictionary<string, IAttribute> attributes;

//     public Attributes() {
//         this.attributes = new Dictionary<string, IAttribute>();
//     }

//     public Attributes Clone() {
//         Attributes na = new Attributes();
//     }

//     public void AddAttribute(IAttribute attribute) {
//         string name = attribute.GetName();
//         if (this.attributes.ContainsKey(name))
//             throw new Exception($"This attribute already exists: {name}");

//         this.attributes.Add(name, attribute);
//     }

//     public void Overwrite(IAttribute attribute) {
//         string name = attribute.GetName();
//         if (!this.attributes.ContainsKey(name))
//             throw new Exception($"This attribute does not exist: {name}");

//         this.attributes[name] = attribute;
//     }

//     public IAttribute GetAttribute(string name) {
//         return this.attributes[name];
//     }

//     public bool Matches(Attributes other) {
//         bool matches = true;

//         // Check common attributes
//         foreach (string key in this.attributes.Keys & other.attributes.Keys) {
//             mathches &= this.attributes[key].Matches(other);
//         }

//         if (!matches)
//             return false;

//         DefaultAttribute da = new DefaultAttribute("dummyattribute");

//         // Check the attributes that are only here
//         foreach (string key in this.attributes.Keys - other.attributes.Keys) {
//             this.attributes[key].Matches(da);
//         }

//         if (!matches)
//             return false;

//         // Check the attributes that are only in the other
//         foreach (string key in other.attributes.Keys - this.attributes.Keys) {
//             other.attributes[key].Matches(da);
//         }

//         return matches;
//     }
// }