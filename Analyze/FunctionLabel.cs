using System.Text;
using Gee.External.Capstone.Arm;
using SunAnalyzer.Extension;
using SunAnalyzer.Version;

namespace SunAnalyzer.Analyze {
    public class FunctionLabel : Label
    {
        public override LabelType Type => LabelType.FUNCTION;
        public ArmInstruction[] Instructions { get; set; }
        public FunctionLabel(int address, string name, ArmInstruction[] instructions, MapCodeAssembly baseAssembly) : base(address, name, baseAssembly) {
            Instructions = instructions;
            Size = instructions.Length * 2;
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{Name}:");
            IEnumerable<Label> branches = BaseAssembly.Labels.Where(l => l.Type == LabelType.BRANCH).ToArray();
            int currentAddressOffset = 0;
            for (int i = 0; i < Instructions.Length; ++i) {
                ArmInstruction instruction = Instructions[i];
                Label? branchLabel = branches.FirstOrDefault(l => l.Address == (Address + currentAddressOffset));
                if (branchLabel != null)
                    builder.AppendLine($"{branchLabel.Name}:");
                if (instruction.IsPoolLoad()) {
                    int address = instruction.GetPoolLoadAddress(Address + currentAddressOffset);
                    
                    Label? poolLabel = BaseAssembly.Labels.FirstOrDefault(l => l.Address == address);
                    if (poolLabel != null) {
                        builder.AppendLine($"\t{instruction.Mnemonic} {instruction.Details.Operands[0].Register.Name}, {poolLabel.Name}");
                        currentAddressOffset += instruction.Bytes.Length;
                        continue;
                    }
                }
                if (instruction.Id == ArmInstructionId.ARM_INS_B) {
                    Label? branchTargetLabel = branches.FirstOrDefault(l => l.Address == (VersionConstants.MAP_CODE_BASE_ADDRESS + instruction.GetBranchTarget()));
                    if (branchTargetLabel != null) {
                        builder.AppendLine($"\t{instruction.Mnemonic} {branchTargetLabel.Name}");
                        currentAddressOffset += instruction.Bytes.Length;
                        continue;
                    }
                }
                if (instruction.Id == ArmInstructionId.ARM_INS_BL) {
                    Label? branchTargetLabel = BaseAssembly.Labels.FirstOrDefault(l => l.Address == (VersionConstants.MAP_CODE_BASE_ADDRESS + instruction.GetBranchTarget()));
                    if (branchTargetLabel != null) {
                        builder.AppendLine($"\t{instruction.Mnemonic} {branchTargetLabel.Name}");
                        currentAddressOffset += instruction.Bytes.Length;
                        continue;
                    }
                }
                builder.AppendLine($"\t{instruction.Mnemonic} {instruction.Operand}");
                currentAddressOffset += instruction.Bytes.Length;
            }
            return builder.ToString();
        }
    }
}