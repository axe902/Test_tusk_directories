using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    class Attribute : IComparable<Attribute>
    {
        public string Name { get; private set; }
        public int Value { get; private set; }

        public Attribute(string name, int value)
        {
            Name = name;
            Value = value;
        }
        public void AddValue(int value)
        {
            Value += value;
        }
        public int CompareTo(Attribute y)
        {
            return Name.CompareTo(y.Name);
        }
    }
}
