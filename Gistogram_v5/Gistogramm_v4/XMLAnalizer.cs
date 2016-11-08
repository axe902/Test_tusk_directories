using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Gistogramm_v4
{
    static class XMLAnalizer
    {
        private enum Branch { EL, AT };

        public static ElementStorage ConvertFrom(string allText)
        {
            ElementStorage es = null;
            XDocument doc = XDocument.Parse(allText);

            es = new ElementStorage(doc.Root.Elements().Count());
            int i = 0;
            foreach (XElement el in doc.Root.Elements())
            {
                es.Elem[i] = new Element(el.Name.ToString(), 0);
                foreach (XElement element in el.Elements())
                {
                    int pos = es.Elem[i].IndexOf(element.Name.ToString());
                    int add;

                    if (element.Value.Length == 0) { add = 1; }
                    else { add = Convert.ToInt32(element.Value); }

                    if (pos == -1)
                    {
                        es.Elem[i].AddAttribute(element.Name.ToString(), add);
                    }
                    else
                    {
                        es.Elem[i].Items[pos].AddValue(add);
                    }
                }
                i++;
            }

            return es;
        }
        public static bool IsFormat(string allText)
        {
            string temp = "";
            int versionInfoStart;
            int versionInfoEnd;

            allText=allText.Trim();

            //поиск информации о формате
            versionInfoStart = allText.IndexOf("<?");
            versionInfoEnd = allText.IndexOf("?>");

            //если она есть
            if ((versionInfoStart == 0) && (versionInfoEnd > 0))
            {//удаляем
                for (int i = versionInfoEnd + 2; i < allText.Length; i++)
                {
                    temp += allText[i];
                }
            }
            else temp = allText;

            return CheckSubElements(temp, Branch.EL);
        }

        private static bool CheckSubElements(string allText, Branch root)
        {
            bool isXML = true;
            int posStartNameElem, posEndNameElem, posStartEndElem, posEndElem;
            string elemName; //имя объекта
            string temp;
            string elem;//содержимое объекта

            do
            {
                allText = allText.Trim();
                //проверка завершилась успехом
                if (allText.Length == 0) break;
                //поиск начала элемента
                posStartNameElem = allText.IndexOf('<');
                posEndNameElem = allText.IndexOf('>');
                //элемента нет - не XML
                if ((posEndNameElem <= posStartNameElem) ||
                    (posStartNameElem + 1 == posEndNameElem) ||
                    (allText[posEndNameElem + 1] == '/') ||
                    (posStartNameElem != 0))
                {
                    isXML = false;
                    break;
                }
                //имя элемента
                elemName = allText.Substring(posStartNameElem + 1, posEndNameElem - posStartNameElem - 1);
                temp = "</" + elemName + ">";
                //поиск конца элемента
                posStartEndElem = allText.IndexOf(temp);
                //конца нет
                if ((posStartEndElem < 0) || (posEndNameElem + 1 == posStartEndElem))
                {
                    isXML = false;
                    break;
                }
                posEndElem = posStartEndElem + temp.Length - 1;
                //выделяем содержимое элемента
                elem = allText.Substring(posEndNameElem + 1, posStartEndElem - posEndNameElem - 1);
                //проверка аттрибутов
                if (root == Branch.EL)
                {
                    isXML = CheckSubElements(elem, Branch.AT);
                    if (!isXML) break;
                }

                //удаление проверенной части
                temp = "";
                for (int i = posEndElem + 1; i < allText.Length; i++)
                {
                    temp += allText[i];
                }
                allText = temp;
            }
            while (true);

            return isXML;
        }
 
    }
}
