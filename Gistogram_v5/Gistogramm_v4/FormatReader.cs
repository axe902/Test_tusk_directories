using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    class FormatReader : IFormatReader
    {
        private string fileText;
        private delegate ElementStorage DelegateFileReader(string str);

        //Конструктор
        public FormatReader(string path)
        {
            fileText = System.IO.File.ReadAllText(path, Encoding.GetEncoding(1251));
        }

        //Указатель на анализатор
        private DelegateFileReader ChooseAnalizer()
        {
            DelegateFileReader analizator = NOPConverter;

            if (XMLAnalizer.IsFormat(fileText)) analizator = XMLAnalizer.ConvertFrom;
            else
                if (JSONAnalizer.IsFormat(fileText)) analizator = JSONAnalizer.ConvertFrom;
                else
                    if (CSVAnalizer.IsFormat(fileText)) analizator = CSVAnalizer.ConvertFrom;

            return analizator;
        }

        //Считывание содержимого файла
        public ElementStorage GetStorage()
        {
            ElementStorage es = Correct(ChooseAnalizer()(fileText));
            if (es != null) es.SortByName();

            return es;
        }

        //проверка на количество аттрибутов
        private ElementStorage Correct(ElementStorage es)
        {
            ElementStorage el = es;
            int attrCount;

            if (el != null)
            {
                if (el.Elem[0].Items == null)
                {
                    return null;
                }
                attrCount = el.Elem[0].Items.Length;
                if (attrCount == 0) return null;

                for (int i = 0; i < el.Elem.Length; i++)
                {
                    if (el.Elem[i].Items.Length != attrCount)
                    {
                        return null;
                    }
                }
            }

            return el;
        }

        private ElementStorage NOPConverter(string str)
        {
            return null;
        }

    }
}
