using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gistogramm_v4
{
    class UserControlIO
    {
        private ElementStorage es;
        private FormatReader reader;
        private string dataType;

        public void Start()
        {
            if (!Input())
            {
                Console.WriteLine();
                Console.WriteLine("Choose action (\"open\", \"exit\", \"save\", \"print\", \"help\"): ");
                while (!ChooseAction()) ;
            }
        }

        private bool Input()
        {
            string fileName;
            bool flagExit = false;

            Console.WriteLine();
            Console.Write("Enter file name (\"exit\" - close application):\n> ");
            fileName = Console.ReadLine();
            if (fileName == "exit") return true;
            Console.Write("Enter diagramm type ('v' - vertical, 'h' - horizontal): ");
            dataType = Console.ReadLine();

            try
            {
                reader = new FormatReader(fileName);
                es = reader.GetStorage();
            }
            catch (Exception)
            {
                Console.WriteLine("File can not be readed.");
                es = null;
            }

            if ((es == null) || ((dataType.ToLower() != "v") && (dataType.ToLower() != "h")))
            {
                Console.WriteLine("Not appropriated format.");
                flagExit = Input();
            }
            return flagExit;
        }
        
        private bool ChooseAction()
        {
            bool flagExit = false;
            string fileName;
            string command;

            Console.Write("> ");
            command = Console.ReadLine();
            switch (command)
            {
                    //открытие файла
                case "open":
                    flagExit = Input();
                    break;

                    //выход из программы
                case "exit":
                    flagExit = true;
                    break;

                    //сохранение в файл
                case "save":
                    Console.Write("Enter path: ");
                    fileName = Console.ReadLine();
                    try
                    {
                        using (ChartWriter writer = new ChartWriter(fileName, es))
                        {
                            writer.PrintChart(ConsoleColor.Blue, ConsoleColor.Red, dataType);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Chart could not be saved.");
                    }
                    break;


                    //печать на консоль
                case "print":
                    ChartWriter wr = new ChartWriter(null, es);
                    wr.PrintChart(ConsoleColor.Blue, ConsoleColor.Red, dataType);
                    break;

                    //справка
                case "help":
                    Help();
                    break;

                case "":
                    break;

                default:
                    Console.WriteLine("Unknown action.");
                    break;
            }

            return flagExit;
        }

        private void Help()
        {
            Console.WriteLine("\t open  - enter new file and chart type");
            Console.WriteLine("\t exit  - finish application");
            Console.WriteLine("\t save  - store data in file");
            Console.WriteLine("\t print - write chart");
            Console.WriteLine("\t help  - write this information");
        }      
    }
}
