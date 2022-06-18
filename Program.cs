using SunAnalyzer.Analyze;
using SunAnalyzer.Version;

namespace SunAnalyzer {
    internal class Program {
        private static void Main(string[] args) {
            byte[] code = File.ReadAllBytes("test.bin");
            Analyzer analyzer = new Analyzer();
            MapCodeAssembly assembly = analyzer.AnalyzeMapCode(code, GameVersion.LOST_AGE);
            Console.WriteLine(assembly.ToString());
        }
    }

}