using System.IO;

namespace LogicOfStatements
{
    internal class History
    {
        private readonly string outputFileName;
        public History(string fileName) => outputFileName = fileName + ".txt";
        public void Add(string output)
        {
            using (StreamWriter sw = new StreamWriter(outputFileName, true)) sw.WriteLine(output + "\r\n\r\n");
        }
    }
}
