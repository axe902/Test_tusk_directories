using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test_tusk_directories
{
    class Directories
    {
        public List<DirectoryInfo> SubDirectory { get; private set; }

        public Directories()
        {
            SubDirectory = new List<DirectoryInfo>();
        }

        //Заполнить тестовыми папками
        public void CreateTestData(string path)
        {
            Directory.CreateDirectory(path + "\\1.0.1.0");
            this.Add(path + "\\1.0.1.0");
            Directory.CreateDirectory(path + "\\1.0.0.1");
            this.Add(path + "\\1.0.0.1");
            Directory.CreateDirectory(path + "\\2.1.0.0");
            this.Add(path + "\\2.1.0.0");
            Directory.CreateDirectory(path + "\\8.2.10.0");
            this.Add(path + "\\8.2.10.0");
            Directory.CreateDirectory(path + "\\4.0.21.5");
            this.Add(path + "\\4.0.21.5");
            Directory.CreateDirectory(path + "\\2015-01-07");
            this.Add(path + "\\2015-01-07");
            Directory.CreateDirectory(path + "\\2016-05-13");
            this.Add(path + "\\2016-05-13");
            Directory.CreateDirectory(path + "\\2016-06-04");
            this.Add(path + "\\2016-06-04");
            Directory.CreateDirectory(path + "\\2016-09-29");
            this.Add(path + "\\2016-09-29");
            Directory.CreateDirectory(path + "\\2016-10-19");
            this.Add(path + "\\2016-10-19");
        }

        public void Add(string path)
        {
            DirectoryInfo sd = new DirectoryInfo(path);
            SubDirectory.Add(sd);
        }

    }
}
