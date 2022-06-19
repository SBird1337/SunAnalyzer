using System.Text;
using SunAnalyzer.Data;
using SunAnalyzer.Version;

namespace SunAnalyzer.Analyze.Labels {
    public class DataLabel<T> : Label where T : DataEntry, new()
    {
        public override int Size => DataEntries.Count * DataEntries[0].Size;

        public List<T> DataEntries { get; set; }

        public override LabelType Type => LabelType.DATA;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{Name}: @0x{Address.ToString("X8")}");
            for (int i = 0; i < DataEntries.Count; ++i) {
                builder.AppendLine(DataEntries[i].ToString());
            }
            return builder.ToString();
        }
        public DataLabel(int address, string name, MapCodeAssembly baseAssembly) : base(address, name, baseAssembly)
        {
            DataEntries = new List<T>();
            int offset = address - VersionConstants.MAP_CODE_BASE_ADDRESS;
            MemoryStream dataStream = new MemoryStream(baseAssembly.ByteCode);
            dataStream.Position = offset;
            do {
                T entry = new T();
                entry.Initialize(dataStream, baseAssembly);
                DataEntries.Add(entry);
            } while (!DataEntries.Last().IsEndElement);
        }
    }
}