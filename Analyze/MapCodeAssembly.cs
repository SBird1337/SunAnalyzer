using System.Text;
using SunAnalyzer.Version;

namespace SunAnalyzer.Analyze {
    public class MapCodeAssembly {
        public int[] InitFunctionAddresses { get; set; }

        public List<Label> Labels { get; set; }
        public byte[] ByteCode { get; set; }
        public MapCodeAssembly(GameVersion version, byte[] byteCode) {
            if (version == GameVersion.BROKEN_SEAL)
                InitFunctionAddresses = new int[VersionConstants.BROKEN_SEAL_INIT_COUNT];
            else
                InitFunctionAddresses = new int[VersionConstants.LOST_AGE_INIT_COUNT];
            Labels = new List<Label>();
            ByteCode = byteCode;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Labels.Sort();
            for (int i = 0; i < Labels.Count; ++i) {
                Label current = Labels[i];
                if (current.Type != Label.LabelType.BRANCH)
                    builder.AppendLine(current.ToString());
            }
            return builder.ToString();
        }
    }
}