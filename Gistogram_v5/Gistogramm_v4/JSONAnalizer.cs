using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    static class JSONAnalizer
    {
        public static ElementStorage ConvertFrom(string allText)
        {
            ElementStorage es = null;

            int counter = 0, elemCount;
            int elStart, elEnd;
            string elName;

            elStart = allText.IndexOf('{');
            elEnd = allText.LastIndexOf('}');
            char[] arr = allText.ToCharArray();
            arr[elStart] = ' ';
            arr[elEnd] = ' ';
            allText = new string(arr);

            allText = allText.Trim();

            elemCount = Count(allText);
            es = new ElementStorage(elemCount);

            for (int k = 0; k < elemCount; k++)
            {
                elStart = allText.IndexOf('{');
                elEnd = allText.IndexOf('}');

                elName = allText.Substring(0, elStart).Trim().Trim(':').Trim().Trim('"');
                /*elName = allText.Substring(0, elEnd).Trim();
                elName = elName.Trim(':');
                elName = elName.Trim();
                elName = elName.Trim('"');*/
                //выделяем содержимое элемента
                string objInfo = allText.Substring(elStart + 1, elEnd - elStart - 1);

                if (elEnd != allText.Length - 1 && allText[elEnd + 1] == ',')
                    elEnd++;
                //разбиваем на аттрибуты
                string[] attr = objInfo.Split(new char[] { ',' });

                // es.Elem[elemCount - counter - 1] = new Element("", attr.Length);
                es.Elem[k] = new Element(elName, 0);
                for (int i = 0; i < attr.Length; i++)
                {
                    attr[i] = attr[i].Trim();

                    //разбиваем аттрибут на Name и Value
                    
                    string[] temp = attr[i].Split(new char[] { ':' });
                    for (int m = 0; m < temp.Length; m++) temp[m] = temp[m].Trim().Trim('"');

                    int pos = es.Elem[k].IndexOf(temp[0]);
                    int add;

                    if (temp.Length == 1) { add = 1; }
                    else { add = Convert.ToInt32(temp[1]); }

                    if (pos == -1)
                    {
                        es.Elem[k].AddAttribute(temp[0], add);
                    }
                    else
                    {
                        es.Elem[k].Items[pos].AddValue(add);
                    }
                }

                string tempr = "";
                for (int j = elEnd + 1; j < allText.Length; j++)
                {
                    tempr += allText[j];
                }
                //исследование нового элемента
                allText = tempr;
                counter++;
            }

            return es;
        }
        public static bool IsFormat(string allText)
        {
            bool isJSON = true;
            int i, j, k;
            int elStart, elEnd;

            //удаление root
            elStart = allText.IndexOf('{');
            elEnd = allText.LastIndexOf('}');
            if (elEnd == elStart) return false;
            char[] arr = allText.ToCharArray();
            arr[elStart] = ' ';
            arr[elEnd] = ' ';
            allText = new string(arr);

            allText = allText.Trim();
            int count = Count(allText);

            for (int l = 0; l < count; l++)
            {
                allText = allText.Trim();
                //проверказавершилась успехом
                if (allText.Length == 0) break;
                //поиск начало и конец элемента
                elStart = allText.LastIndexOf('{');
                elEnd = allText.LastIndexOf('}');
                if ((elStart >= elEnd) ||
                    (l == 0 && !allText.EndsWith("}")))
                {
                    //если элемента нет или между ними есть недопустимые символы
                    isJSON = false;
                    break;
                }

                //выделяем содержимое элемента
                string objInfo = allText.Substring(elStart + 1, elEnd - elStart - 1);
                //разбиваем на аттрибуты
                string[] attr = objInfo.Split(new char[] { ',' });
                for (i = 0; i < attr.Length; i++)
                {
                    attr[i] = attr[i].Trim();
                    for (j = 0; j < attr.Length; j++)
                    {
                        //разбиваем аттрибут на Name и Value
                        string[] temp = attr[j].Split(new char[] { ':' });
                        for (k = 0; k < temp.Length; k++)
                        {
                            temp[k] = temp[k].Trim();
                            int tryParse;
                            //если их не 2 или не в кавычках
                            if ((temp.Length != 2 && temp.Length != 1) || 
                                (temp[0][0] != '"') || 
                                (temp[0][temp[0].Length - 1] != '"') || // если не число
                                (temp.Length == 2 && !Int32.TryParse(temp[1].Trim('"'),out tryParse)))
                            {
                                isJSON = false;
                                break;
                            }
                                    
                        }
                    }
                }
                //исследование нового элемента
                allText = allText.Remove(elStart);
            }

            return isJSON;
        }

        private static int Count(string allText)
        {
            int count = 0;
            int elStart, elEnd;

            do
            {
                allText = allText.Trim();
                elStart = allText.LastIndexOf('{');
                elEnd = allText.LastIndexOf('}');
                if (elEnd == elStart) break;
                allText = allText.Remove(elStart);
                count++;
            }
            while (true);

            return count;
        }
    }
}
