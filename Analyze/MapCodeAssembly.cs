using System.Text;
using SunAnalyzer.Version;
using SunAnalyzer.Analyze.Labels;
using Gee.External.Capstone.Arm;
using SunAnalyzer.Extension;

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

        public int GetPoolValue(ArmInstruction instruction, int currentAddress) {
            if (!instruction.IsPoolLoad())
                throw new ArgumentException("Instruction is no pool load", "instruction");
            int poolAddress = instruction.GetPoolLoadAddress(currentAddress);
            int poolOffset = poolAddress - VersionConstants.MAP_CODE_BASE_ADDRESS;
            return BitConverter.ToInt32(ByteCode, poolOffset);
        }

        private string RenderGap(int address, int length) {
            int offset = address - VersionConstants.MAP_CODE_BASE_ADDRESS;
            int end = offset + length;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"_{address.ToString("X8")}:");
            while (true) {
                byte[] bytesToPrint;
                if (end - offset < 8) {
                    if (offset == end)
                        break;
                    bytesToPrint = ByteCode.Take(new Range(offset, end)).ToArray();
                    builder.AppendLine($"\t.byte {string.Join(", ", bytesToPrint.Select(b => "0x" + b.ToString("X2")))}");
                    break;
                } else {
                    bytesToPrint = ByteCode.Take(new Range(offset, offset + 8)).ToArray();
                    builder.AppendLine($"\t.byte {string.Join(", ", bytesToPrint.Select(b => "0x" + b.ToString("X2")))}");
                    offset += 8;
                }
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(".thumb");
            builder.AppendLine(".syntax unified");
            builder.AppendLine(".include \"macros/event.inc\"");
            builder.AppendLine();
            Labels.Sort();
            for (int i = 0; i < Labels.Count; ++i) {
                Label current = Labels[i];
                if (current.Type != Label.LabelType.BRANCH)
                    builder.AppendLine(current.ToString());
                
                if (i == (Labels.Count - 1)) {
                    // Hit the last label, print remainder as raw bytes if any
                    int fileOffset = current.Address + current.Size - VersionConstants.MAP_CODE_BASE_ADDRESS;
                    if (fileOffset < ByteCode.Length) {
                        builder.AppendLine(RenderGap(current.Address + current.Size, ByteCode.Length - fileOffset));
                    }
                } else {
                    // Check if there is a gap between this and the next label
                    Label next = Labels[i+1];
                    if (next.Type == Label.LabelType.BRANCH || current.Type == Label.LabelType.BRANCH)
                        continue;
                    int fileOffset = current.Address + current.Size - VersionConstants.MAP_CODE_BASE_ADDRESS;
                    int nextOffset = next.Address - VersionConstants.MAP_CODE_BASE_ADDRESS;

                    // TODO: Check if the gap is only alignment
                    if (fileOffset < nextOffset) {
                        builder.AppendLine(RenderGap(current.Address + current.Size, nextOffset - fileOffset));
                    }
                }
            }
            return builder.ToString();
        }
    }
}