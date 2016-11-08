using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    static class CSVAnalizer
    {
        private enum DataType { STRING, INT };

        public static  ElementStorage ConvertFrom(string fileText)
        {
             ElementStorage es = null;
            string[] names;

            string[] str = fileText.Split(new char[] { '\n' });

            for (int i = 0; i < str.Length; i++) str[i] = str[i].Trim();
            //удаление пустых строк из массива
            str = str.Where(n => !string.IsNullOrEmpty(n)).ToArray();

            DataType dt = FileType(str, out names, ref str);
            int countElem = str.Length;

            es = new ElementStorage(countElem);
            for (int i = 0; i < str.Length; i++)
            {
                string[] temp = CSVReadLine(str[i]);
                es.Elem[i] = new Element(temp[0], 0);
                for (int j = 1; j < temp.Length; j++)
                {

                    if (dt == DataType.INT)
                    {
                        if (temp[j].Length == 0) temp[j] = "0";
                        es.Elem[i].AddAttribute(names[j], Convert.ToInt32(temp[j]));
                    }
                    else
                    {
                        int pos = es.Elem[i].IndexOf(temp[j]);

                        if (pos == -1)
                        {
                            es.Elem[i].AddAttribute(temp[j], 1);
                        }
                        else
                        {
                            es.Elem[i].Items[pos].AddValue(1);
                        }
                    }
                }
            }
            return es;
        }
        public static bool IsFormat(string fileText)
        {
            string[] str;
            bool isCSV = true;

            str = fileText.Split(new char[] { '\n' });
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = str[i].Trim();
                if (!IsStringCSV(str[i]))
                {
                    isCSV = false;
                    break;
                }
            }

            return isCSV;
        }

        private static  string[] CSVReadLine(string str)
        {
            int j = 0;

            char[] arrOfChars = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] == ',') && (j % 2 == 0))
                {
                    arrOfChars[i] = ';';
                }
                if (str[i] == '"')
                {
                    j++;
                }
            }
            str = new string(arrOfChars);
            str = str.Replace("\"", "");
            string[] temp = str.Split(new char[] { ';' });
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = temp[i].Trim();
            }

            return temp;
        }
        private static bool IsStringCSV(string str)
        {
            bool isCSV = true;
            bool flagQuote = false;

            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] == ' ') && (!flagQuote))
                {
                    isCSV = false;
                    break;
                }
                if (str[i] == '"')
                {
                    flagQuote = !flagQuote;
                }
            }

            return isCSV;
        }
        private static DataType FileType(string[] str, out string[] names, ref string[] elements)
        {
            names = null;

            if (str.Length < 2)
            {
                return DataType.STRING;
            }

            for (int i = 1; i < str.Length; i++)
            {
                string[] temp = CSVReadLine(str[i]);
                int value;

                for (int j = 1; j < temp.Length; j++)
                {
                    if (temp[j].Length != 0 && !Int32.TryParse(temp[j], out value))
                    {
                        return DataType.STRING;
                    }
                }
            }

            names = CSVReadLine(str[0]);
            str[0] = "";
            elements = str.Where(n => !string.IsNullOrEmpty(n)).ToArray();

            return DataType.INT;
        }
    }
}
