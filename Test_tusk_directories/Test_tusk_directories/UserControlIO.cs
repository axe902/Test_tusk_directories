using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Test_tusk_directories
{
    class UserControlIO
    {
        private DirAnalizer da;
        private string systemDrive = Environment.ExpandEnvironmentVariables("%SYSTEMDRIVE%");
        Regex regex = new Regex(@"^%SYSTEMDRIVE%(\\(\w+ *\w*))+[|](\b(Latest)\b|\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])|(\d+[.]\d+[.]\d+[.]\d+))$");

        public void Start()
        {
            while (!Input()) ;
        }

        private bool Input()
        {
            string command;
            bool flagExit = false;
            string result;

            Console.WriteLine();
            Console.Write("Enter command (\"exit\" - close application):\n> ");
            command = Console.ReadLine();
            if (command == "exit") return true;

            if (!regex.IsMatch(command))
            {
                Console.WriteLine("Incorrect command.");
                return flagExit;
            }

            string[] paths = new string[2];
            paths = command.Split('|');

            /*
            Пометка для меня №1
            */
            paths[0] = paths[0].Replace("%SYSTEMDRIVE%", systemDrive);
            /*
            Пометка для меня №2
            */
            try
            {
                da = new DirAnalizer(paths[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Папка не создалась
                return flagExit;
            }

            try
            {
                result = ChooseAction(paths[1]);
            }
            catch (Exception)
            {
                Console.WriteLine("No directories in "+paths[0]); // 0 папок изначально
                return flagExit;
            }
            Output(paths[0], result);//Вывод

            return flagExit;
        }

        //Выбор. По версии, дате или Latest
        private string ChooseAction(string parameter)
        {
            if (parameter.Contains('.'))
            {
                return da.Version(parameter);
            }
            if (parameter.Contains('-'))
            {
                return da.Data(parameter);
            }

            return da.Latest();
        }

        private void Output(string path1, string path2)
        {
            Console.WriteLine(path1+'\\'+path2);
        }
    }
}
