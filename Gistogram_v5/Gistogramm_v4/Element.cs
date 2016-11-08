using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    class Element
    {
        public string Name { get; private set; }
        public Attribute[] Items { get; private set; }

        public Element(string name, int count)
        {
            Name = name;
            Items = new Attribute[count];
        }
        public void AddAttribute(string name, int value)
        {
            List<Attribute> ls = Items.ToList();
            ls.Add(new Attribute(name, value));
            Items = ls.ToArray();
        }
        public int IndexOf(string name)
        {
            return Array.IndexOf(Items, Items.FirstOrDefault(n => n.Name == name));
        }
        public void Sort()
        {
            Array.Sort(Items);
        }
    }
}
