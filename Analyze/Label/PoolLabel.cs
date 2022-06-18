using System.Text;
using SunAnalyzer.Version;

namespace SunAnalyzer.Analyze.Labels {
    public class PoolLabel : Label
    {
        public override LabelType Type => LabelType.POOL;

        public override int Size => 4;

        public PoolLabel(int address, string name, MapCodeAssembly baseAssembly) : base(address, name, baseAssembly) {
            
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{Name}:");
            int offset = Address - VersionConstants.MAP_CODE_BASE_ADDRESS;
            int dataValue = BitConverter.ToInt32(BaseAssembly.ByteCode, offset);
            Label? dataLabel = BaseAssembly.Labels.FirstOrDefault(l => l.Address == dataValue);
            Label? codeLabel = BaseAssembly.Labels.FirstOrDefault(l => l.Address+1 == dataValue);
            if (dataLabel != null)
                builder.AppendLine($"\t.word {dataLabel.Name}");
            else if (codeLabel != null)
                builder.AppendLine($"\t.word {codeLabel.Name}+1");
            else
                builder.AppendLine($"\t.word 0x{dataValue.ToString("X8")}");
            return builder.ToString();
        }
    }
}