using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    public class CSV : IEnumerable<List<string>>
    {
        private static readonly string defaultPath = "../../Resources/";
        private static readonly string defaultFormat = ".csv";

        private List<List<string>> data = new List<List<string>>();
        private string path;
        public string Path
        {
            get => path;
            set => path = System.IO.Path.GetFullPath(value.Replace("!", defaultPath) + (System.IO.Path.HasExtension(value) ? "" : defaultFormat));
        }

        public CSV(string path)
        {
            Path = path;
            if (File.Exists(Path))            
                Read();
        }

        public void Read() => File.ReadAllLines(Path).ToList().ForEach(l => data.Add(new List<string>(l.Split(',').Select(s => s.Trim()))));
        public void Save() => File.WriteAllLines(Path, data.Select(l => l.Aggregate((a, b) => a + ", " + b)));
        public override string ToString() => data.Any() ? data.Select((List<string> l) => l.Aggregate((a, b) => a + ", " + b) + "   \n").Aggregate((a, b) => a + "\n" + b) : System.IO.Path.GetFileNameWithoutExtension(Path) + " is Empty";

        public IEnumerator<List<string>> GetEnumerator() => ((IEnumerable<List<string>>)data).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<List<string>>)data).GetEnumerator();

        public List<string> this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }
        public List<string> this[string s, int i]
        {
            get => data.FirstOrDefault(l => l[i].Equals(s));
            set => data[data.FindIndex(l => l[i].Equals(s))] = value;
        }
        public List<string> this[string s]
        {
            get => this[s, 0];
            set => this[s, 0] = value;
        }
    }
}
