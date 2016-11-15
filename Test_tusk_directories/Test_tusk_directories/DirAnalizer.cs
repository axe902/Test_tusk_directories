using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Test_tusk_directories
{
    class DirAnalizer
    {
        Directories dir = new Directories();
        private List<string> ListVer = new List<string>();// Список названий папок (формат версий)
        private List<string> ListDate = new List<string>();//Список названий папок (формат дат)

        public DirAnalizer(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                dir.CreateTestData(path);
            }
            else
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string d in dirs)
                {
                    dir.Add(d);
                }
            }
        }

        public string Latest()
        {
            dir.SubDirectory.Sort( (a,b) => a.CreationTime.CompareTo(b.CreationTime) ); //Сортировка по дате создания

            return dir.SubDirectory.Last().Name;
        }

        public string Version(string parameter)
        {
            ToListVer();

            if (ListVer.Count == 0)
            {
                return Latest();//Алгоритм Latest, когда нет подходящих папок
            }

            ListVer.Add(parameter);

            //Натуральная сортировка
            string[] tl = ListVer.ToArray();
            Array.Sort(tl, new LogicalStringComparer());
            ListVer = tl.ToList();

            List<string> resList = new List<string>();
            resList = ListVer.FindAll(x => x == parameter);
            var result = resList.First();
            int i = ListVer.IndexOf(result);

            if (resList.Count != 1) //Точное совпадение с одной из версий
            {
                return result;
            }
            else if (i == 0)
            {
                return ListVer[i + 1];
            }
            else if (i == ListVer.Count - 1)
            {
                return ListVer[i - 1];
            }
            else  //Максимальное приближение (сравнение между двумя близкими версиями)
            {
                string[] prev = new string[4];
                string[] next = new string[4];
                string[] tempPar = new string[4];
                double lengthToPrev, lengthToNext;
                prev = ListVer[i - 1].Split('.');
                tempPar = parameter.Split('.');
                next = ListVer[i + 1].Split('.');

                for (int j = 0; j < 4; j++)
                {
                    lengthToPrev = Math.Abs(Convert.ToDouble(tempPar[j]) - Convert.ToDouble(prev[j]));
                    lengthToNext = Math.Abs(Convert.ToDouble(tempPar[j]) - Convert.ToDouble(next[j]));
                    if (lengthToPrev != lengthToNext)
                    {
                        if (lengthToPrev < lengthToNext)
                        {
                            return ListVer[i - 1];
                        }
                        else
                        {
                            return ListVer[i + 1];
                        }
                    }
                }
                return ListVer[i - 1]; //Предыдущую версию, в случает когда prev и next на одинаковом расстоянии
            }
        }

        public string Data(string parameter)
        {
            ToListDate();

            if (ListDate.Count == 0)
            {
                return Latest();//Алгоритм Latest, когда нет подходящих папок
            }

            ListDate.Add(parameter);
            
            //Натуральная сортировка
            string[] tl = ListDate.ToArray();
            Array.Sort(tl, new LogicalStringComparer());
            ListDate = tl.ToList();

            List<string> resList = new List<string>();
            resList = ListDate.FindAll(x => x == parameter);
            var result = resList.First();
            int i = ListDate.IndexOf(result);

            if (resList.Count != 1) //Точное совпадение с одной из дат
            {
                return result;
            }
            else if (i == 0)
            {
                return ListDate[i + 1];
            }
            else if (i == ListDate.Count - 1)
            {
                return ListDate[i - 1];
            }
            else  //Максимальное приближение (сравнение между двумя близкими датами)
            {
                string[] prev = new string[3];
                string[] next = new string[3];
                string[] tempPar = new string[3];
                double lengthToPrev, lengthToNext;
                prev = ListDate[i - 1].Split('-');
                tempPar = parameter.Split('-');
                next = ListDate[i + 1].Split('-');

                // Сумма дней в предыдущей дате, отсчет от prev
                double prevSum =(Convert.ToDouble(prev[1]) - 1)*30 + Math.Ceiling( Convert.ToDouble(prev[1])/2 ) - 1 + Convert.ToDouble(prev[2]);
                // Сумма дней в параметре, отчет от prev
                double parSum = (Convert.ToDouble(tempPar[0]) - Convert.ToDouble(prev[0])) * 366 + (Convert.ToDouble(tempPar[1]) - 1) * 30 + Math.Ceiling(Convert.ToDouble(tempPar[1]) / 2) - 1 + Convert.ToDouble(tempPar[2]);
                // Сумма дней в следующей дате, отчет от prev
                double nextSum = (Convert.ToDouble(next[0]) - Convert.ToDouble(prev[0])) * 366 + (Convert.ToDouble(next[1]) - 1) * 30 + Math.Ceiling(Convert.ToDouble(next[1]) / 2) - 1 + Convert.ToDouble(next[2]);
                
                lengthToPrev = Math.Abs(parSum - prevSum); //Разница сумм
                lengthToNext = Math.Abs(nextSum - parSum); //Разница сумм
                if (lengthToPrev != lengthToNext)
                {
                    if (lengthToPrev < lengthToNext)
                    {
                        return ListDate[i - 1]; //Предыдущая дата ближе
                    }
                    else
                    {
                        return ListDate[i + 1]; //Следующая дата ближе
                    }
                }
                return ListDate[i - 1]; //Предыдущую версию, в случает когда prev и next на одинаковом расстоянии
            }
        }

        //Сформировать список из названий по версии
        private void ToListVer()
        {
            Regex regex = new Regex(@"^(\d+[.]\d+[.]\d+[.]\d+)$");

            foreach (DirectoryInfo di in dir.SubDirectory)
            {
                if (regex.IsMatch(di.Name))
                {
                    ListVer.Add(di.Name);
                }
            }
        }

        private void ToListDate()
        {
            Regex regex = new Regex(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$");

            foreach (DirectoryInfo di in dir.SubDirectory)
            {
                if (regex.IsMatch(di.Name))
                {
                    ListDate.Add(di.Name);
                }
            }
        }

        //Натуральная сортировка
        class LogicalStringComparer : IComparer<string>
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            static extern int StrCmpLogicalW(string x, string y);

            public int Compare(string x, string y)
            {
                return StrCmpLogicalW(x, y);
            }
        }
    }
}
