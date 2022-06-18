using System.Text;
using SunAnalyzer.Data;
using SunAnalyzer.Version;

namespace SunAnalyzer.Analyze.Labels {
    public class NpcListLabel : Label
    {
        // FIXME: SIZE
        public override int Size => NpcEntries.Count * 24;

        public List<NpcEntry> NpcEntries { get; set; }

        public override LabelType Type => LabelType.DATA;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{Name}:");
            for (int i = 0; i < NpcEntries.Count; ++i) {
                builder.AppendLine(NpcEntries[i].ToString());
            }
            return builder.ToString();
        }
        public NpcListLabel(int address, string name, MapCodeAssembly baseAssembly) : base(address, name, baseAssembly)
        {
            NpcEntries = new List<NpcEntry>();
            int offset = address - VersionConstants.MAP_CODE_BASE_ADDRESS;
            MemoryStream dataStream = new MemoryStream(baseAssembly.ByteCode);
            dataStream.Position = offset;
            do {
                NpcEntries.Add(NpcEntry.FromStream(dataStream));
            } while (NpcEntries.Last().SpriteId != 0xFFFF);
        }
    }
}