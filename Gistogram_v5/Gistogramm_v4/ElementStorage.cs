using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    class ElementStorage
    {
        public Element[] Elem { get; private set; }
        private bool isMax = false;
        private bool isSorted = false;
        private int MaximumValue { get; set; }

        public ElementStorage(int count)
        {
            Elem = new Element[count];
        }
        public int MaxValue()
        {
            int max = 0;
            if (!isMax)
            {
                int countStr = Elem.Length;

                for (int i = 0; i < countStr; i++)
                {
                    for (int j = 0; j < Elem[i].Items.Length; j++)
                    {
                        if (max < Elem[i].Items[j].Value)
                        {
                            max = Elem[i].Items[j].Value;
                        }
                    }
                }
                isMax = true;
            }
            else
            {
                max = MaximumValue;
            }
            return max;
        }
        public void SortByName()
        {
            if (isSorted) return;

            for (int i = 0; i < Elem.Length; i++)
            {
                Elem[i].Sort();
            }
            isSorted = true;
            return;
        }
    }
}
