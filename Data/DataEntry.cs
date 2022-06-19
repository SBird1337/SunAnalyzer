using SunAnalyzer.Analyze;

namespace SunAnalyzer.Data {
    public abstract class DataEntry : IDataEntry
    {
        public MapCodeAssembly? Assembly { get; set; }
        public DataEntry() {}
        public virtual void Initialize(Stream stream, MapCodeAssembly assembly) {
            Assembly = assembly;
        }
        public abstract int Size { get; }
        public abstract bool IsEndElement { get; }

        public override abstract string ToString();
    }
}