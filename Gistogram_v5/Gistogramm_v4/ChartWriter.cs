using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gistogramm_v4
{
    class ChartWriter: IDisposable
    {
        private TextWriter consoleStandardOutput;   //стандартный поток вывода
        private String[] colorNames;
        private ElementStorage es;
        private StreamWriter strOut;
        private int CurrColumn { get; set; }        //текущая позиция от начала строки

        private const int diff = 12;                //смещение между столбцами таблицы
        private const int countColumn = 6;          //количество столбцов таблицы
        private int maxPrintLength = countColumn * (diff + 1) - 1;//ширина таблицы

        public ChartWriter(string path, ElementStorage es)
        {
            colorNames = ConsoleColor.GetNames(typeof(ConsoleColor));
            consoleStandardOutput = Console.Out;
            if (path != null)
            {
                strOut = new StreamWriter(new FileStream(path, FileMode.Truncate, FileAccess.Write));
                Console.SetOut(strOut);
            }
            else
            {
                strOut = null;
            }

            CurrColumn = 1;
            this.es = es;
        }
        public void PrintChart(ConsoleColor tableColor, ConsoleColor gistColor, string type)
        {
            WriteTable(tableColor, 0);
            if (type.ToLower() == "v")
            {
                VerticalChart(gistColor, 0);
            }
            else
            {
                HorizontChart(gistColor);
            }
            WriteLegend(tableColor);
        }
        private void WriteLegend(ConsoleColor borderColor)
        {
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
            for (int i = 0; i < es.Elem[0].Items.Length; i++)
            {
                Write("|", borderColor);
                if (i == 0)
                {
                    Write("Legend:", ConsoleColor.Yellow);
                }
                WriteBorder(" ", diff + 2 - CurrColumn, borderColor);

                ConsoleColor lineColor = ChooseColor(i);
                Console.BackgroundColor = lineColor;
                Write("*", lineColor);

                Write(" " + es.Elem[0].Items[i].Name, ConsoleColor.White);
                WriteBorder(" ", maxPrintLength + 2 - CurrColumn, borderColor);
                WriteLine("|", borderColor);
            }
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
        }
        private void HorizontChart(ConsoleColor borderColor)
        {
            int maxValue = es.MaxValue();

            for (int i = 0; i < es.Elem.Length; i++)
            {
                Write(" ", borderColor);
                WriteBorder("-", maxPrintLength, borderColor);
                WriteLine();

                Write("|", borderColor);
                Write(es.Elem[i].Name, ConsoleColor.White);
                WriteBorder(" ", maxPrintLength + 2 - CurrColumn, ConsoleColor.White);
                WriteLine("|", borderColor);

                for (int j = 0; j < es.Elem[i].Items.Length; j++)
                {
                    Write("|", borderColor);
                    WriteBorder(" ", diff, borderColor);

                    //печать гистограммы
                    ConsoleColor lineColor = ChooseColor(j);
                    Console.BackgroundColor = lineColor;
                    WriteBorder("*", ((maxPrintLength - diff - 2) * es.Elem[i].Items[j].Value) / maxValue + 1, lineColor);

                    WriteBorder(" ", maxPrintLength + 2 - CurrColumn, ConsoleColor.White);
                    WriteLine("|", borderColor);
                }
            }
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
            Write("|", borderColor);
            Write("Value:", ConsoleColor.Yellow);

            int cntTemp = maxPrintLength / diff;
            int temp = maxValue / (cntTemp - 1);

            for (int i = 0; i < cntTemp; i++)
            {
                WriteBorder(" ", diff * (i + 1) + 2 - CurrColumn, ConsoleColor.White);

                if (i != cntTemp - 1)
                {
                    if (i == 0 || cntTemp <= maxValue + 1)
                    {
                        Write((i * temp).ToString(), ConsoleColor.White);
                    }
                }
                else
                {
                    WriteBorder(" ", maxPrintLength - CurrColumn - maxValue.ToString().Length + 1, borderColor);
                    Write(maxValue.ToString(), ConsoleColor.White);
                }
            }
            WriteBorder(" ", maxPrintLength + 2 - CurrColumn, ConsoleColor.White);
            WriteLine("|", borderColor);
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
        }
        private void VerticalChart(ConsoleColor borderColor, int startIndex)
        {
            int maxPrintHigh = 25;
            int maxValue = es.MaxValue();
            int[] scale;
            int[] posScale;

            int endIndex = startIndex + countColumn - 1;

            if (maxValue > 5)
            {
                scale = new int[5];
                posScale = new int[5] { 1, 6, 11, 16, 21 };
            }
            else
            {
                scale = new int[2];
                posScale = new int[2] { 1, 21 };
            }

            int index;
            int scaleLength = posScale[posScale.Length - 1] - posScale[0];

            for (int i = 0; i < scale.Length; i++)
            {
                scale[scale.Length - i - 1] = i * (maxValue / (scale.Length - 1));
            }
            scale[0] = maxValue;
            scale[scale.Length - 1] = 0;

            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);

            for (int i = 0; i < maxPrintHigh - 2; i++)
            {
                WriteLine();
                Write("|", borderColor);
                if (i == 0) Write("Value:", ConsoleColor.Yellow);
                if (IsExist(posScale, i, out index))
                {
                    WriteBorder(" ", diff - scale[index].ToString().Length, ConsoleColor.White);
                    Write(scale[index].ToString(), ConsoleColor.White);
                }

                for (int j = startIndex; j < endIndex + 1; j++)
                {

                    if ((i >= posScale[0]) && (i <= posScale[posScale.Length - 1]) && (j != startIndex) && (j < es.Elem.Length + 1))
                    {
                        Write(" ", ConsoleColor.White);
                        int countAttr = es.Elem[0].Items.Length;

                        for (int k = 0; k < countAttr; k++)
                        {
                            if ((scaleLength * es.Elem[j - 1].Items[k].Value) / maxValue + 1 < scaleLength - i + 2)
                            {
                                Write(" ", ConsoleColor.White);
                            }
                            else
                            {
                                Console.BackgroundColor = ChooseColor(k);
                                Write("*", ChooseColor(k));
                                Console.ResetColor();
                            }
                        }
                    }

                    WriteBorder(" ", (j + 1 - startIndex) * (diff + 1) + 1 - CurrColumn, borderColor);
                    Write("|", borderColor);
                }
            }

            //Печать имен элементов
            WriteLine();
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
            Write("|", borderColor);

            for (int i = startIndex; i < endIndex + 1; i++)
            {
                if (i == startIndex || i > es.Elem.Length)
                {
                    WriteBorder(" ", (i + 1 - startIndex) * (diff + 1) + 1 - CurrColumn, borderColor);
                }
                else
                {
                    Write(es.Elem[i - 1].Name.ToString(), ConsoleColor.White);
                    WriteBorder(" ", (i + 1 - startIndex) * (diff + 1) + 1 - CurrColumn, borderColor);
                }
                Write("|", borderColor);
            }

            WriteLine();
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();

            if (endIndex < es.Elem.Length)
            {
                VerticalChart(borderColor, endIndex);
            }
        }      
        private void WriteTable(ConsoleColor borderColor, int startIndex)
        {
            int countAttr = es.Elem[0].Items.Length;
            int endIndex = startIndex + countColumn - 1;

            //Спиисок аттрибутов
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                Write("|", borderColor);
                if ((i != startIndex) && (i < countAttr + 1))
                {
                    string temp = ShortString(es.Elem[0].Items[i - 1].Name);
                    Write(temp, ConsoleColor.White);
                    WriteBorder(" ", diff - temp.Length, borderColor);
                }
                else
                {
                    WriteBorder(" ", diff, borderColor);
                }
            }
            WriteLine("|", borderColor);

            //Список элементов
            for (int j = 0; j < es.Elem.Length; j++)
            {
                Write(" ", borderColor);
                WriteBorder("-", maxPrintLength, borderColor);
                WriteLine();
                for (int i = startIndex; i < endIndex + 1; i++)
                {
                    string temp;
                    Write("|", borderColor);
                    if (i == startIndex)
                    {
                        temp = ShortString(es.Elem[j].Name);
                        Write(temp, ConsoleColor.White);
                        WriteBorder(" ", diff - temp.Length, borderColor);
                    }
                    else
                        if ((i != startIndex) && (i < countAttr + 1))
                        {
                            temp = ShortString(es.Elem[j].Items[i - 1].Value.ToString());
                            Write(temp, ConsoleColor.White);
                            WriteBorder(" ", diff - temp.Length, borderColor);
                        }
                        else
                        {
                            WriteBorder(" ", diff, borderColor);
                        }
                }
                WriteLine("|", borderColor);
            }
            Write(" ", borderColor);
            WriteBorder("-", maxPrintLength, borderColor);
            WriteLine();
            WriteLine();

            if (endIndex < countAttr)
            {
                WriteTable(borderColor, endIndex);
            }
        }

        private void Write(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ResetColor();
            CurrColumn += str.Length;
        } 
        private void WriteLine(string str, ConsoleColor color)
        {
            Write(str, color);
            WriteLine();
        }
        private void WriteLine()
        {
            Console.WriteLine();
            CurrColumn = 1;
        }
        private void WriteBorder(string ch, int length, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < length; i++)
            {
                Console.Write(ch);
            }
            Console.ResetColor();
            CurrColumn += ch.Length * length;
        }
        private string ShortString(string str)
        {
            if (str.Length > diff)
            {
                str = str.Remove(diff - 3);
                str += "...";
            }
            return str;
        }
        
        private ConsoleColor ChooseColor(int number)
        {
            return (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorNames[colorNames.Length - number- 1]);
        }
        private bool IsExist(int[] arr, int value, out int index)
        {
            bool result = false;

            index = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    result = true;
                    index = i;
                    break;
                }
            }
            return result;
        }
        
        public void Dispose()
        {
            if (strOut != null)
            {
                strOut.Close();
            }
            Console.SetOut(consoleStandardOutput);
        }
    }
}
