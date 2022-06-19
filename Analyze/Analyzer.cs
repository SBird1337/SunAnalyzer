using Gee.External.Capstone;
using Gee.External.Capstone.Arm;
using SunAnalyzer.Version;
using SunAnalyzer.Extension;
using SunAnalyzer.Analyze.Labels;
using SunAnalyzer.Data;

namespace SunAnalyzer.Analyze {
    public class Analyzer {
        private CapstoneArmDisassembler Disassembler { get; set; }
        public Analyzer()
        {
            Disassembler = CapstoneDisassembler.CreateArmDisassembler(ArmDisassembleMode.Thumb);
            Disassembler.EnableInstructionDetails = true;
            Disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
        }

        public MapCodeAssembly AnalyzeMapCode(byte[] byteCode, GameVersion version) {
            MapCodeAssembly assembly = new MapCodeAssembly(version, byteCode);
            for (int i = 0; i < assembly.InitFunctionAddresses.Length; ++i) {
                int functionAddress = 8 * i;
                int dataAddress = 8 * i + 4;
                int veneerDestination;
                ArmInstruction[] initVeneer = Disassembler.Disassemble(byteCode.Skip(functionAddress).ToArray(), functionAddress, 2);
                veneerDestination = BitConverter.ToInt32(byteCode, dataAddress);
                assembly.InitFunctionAddresses[i] = veneerDestination - 1; // thumb
                assembly.Labels.Add(new FunctionLabel(functionAddress + VersionConstants.MAP_CODE_BASE_ADDRESS, $"Thunk_{VersionConstants.INIT_FUNCTION_NAMES[i]}", initVeneer, assembly));
                assembly.Labels.Add(new PoolLabel(dataAddress + VersionConstants.MAP_CODE_BASE_ADDRESS, "", assembly));
            }
            for (int i = 0; i < assembly.InitFunctionAddresses.Length; ++i) {
                HashSet<int> ignored = new HashSet<int>();
                assembly.Labels.ForEach(l => ignored.Add(l.Address));
                assembly.Labels.AddRange(AnalyzeFunction(assembly.InitFunctionAddresses[i], byteCode, assembly, ignored, VersionConstants.INIT_FUNCTION_NAMES[i]));
            }

            FunctionLabel? npcInitLabel = assembly.Labels.FirstOrDefault(l => l.Address == assembly.InitFunctionAddresses[3]) as FunctionLabel;
            FunctionLabel? eventInitLabel = assembly.Labels.FirstOrDefault(l => l.Address == assembly.InitFunctionAddresses[4]) as FunctionLabel;

            if (npcInitLabel != null) {
                int[] npcAddresses = AnalyzePoolReturnsInFile(npcInitLabel, assembly);
                for (int i = 0; i < npcAddresses.Length; ++i) {
                    assembly.Labels.Add(new DataLabel<NpcEntry>(npcAddresses[i], $"NpcData_{i}", assembly));
                }
            } else {
                throw new Exception("npcInitLabel is NULL");
            }
            if (eventInitLabel != null) {
                int[] eventAddresses = AnalyzePoolReturnsInFile(eventInitLabel, assembly);
                for (int i = 0; i < eventAddresses.Length; ++i) {
                    DataLabel<EventEntry> label = new DataLabel<EventEntry>(eventAddresses[i], $"EventData_{i}", assembly);
                    assembly.Labels.Add(label);
                    for (int j = 0; j < label.DataEntries.Count; ++j) {
                        EventEntry entry = label.DataEntries[j];
                        if (entry.ScriptAddress > VersionConstants.MAP_CODE_BASE_ADDRESS && entry.ScriptAddress < (VersionConstants.MAP_CODE_BASE_ADDRESS + assembly.ByteCode.Length)) {
                            HashSet<int> ignored = new HashSet<int>();
                            assembly.Labels.ForEach(l => ignored.Add(l.Address));
                            assembly.Labels.AddRange(AnalyzeFunction(entry.ScriptAddress - 1, byteCode, assembly, ignored, $"EventData_{i}_Action_{j}"));
                        }
                    }
                }
            } else {
                throw new Exception("eventInitLabel is NULL");
            }

            return assembly;
        }

        private void TryAddLabel(List<Label> labels, HashSet<int> ignoredAddresses, Label newLabel) {
            if (labels.Any(l => l.Address == newLabel.Address) ||ignoredAddresses.Contains(newLabel.Address))
                return;
            ignoredAddresses.Add(newLabel.Address);
            labels.Add(newLabel);
        }

        private Label[] AnalyzeFunction(int startAddress, byte[] byteCode, MapCodeAssembly assembly, HashSet<int> ignoredAddresses, string name = "") {
            List<Label> labels = new List<Label>();
            if (ignoredAddresses.Contains(startAddress))
                return labels.ToArray();

            ignoredAddresses.Add(startAddress);
            List<ArmInstruction> functionInstructions = new List<ArmInstruction>();
            int currentAddress = startAddress - VersionConstants.MAP_CODE_BASE_ADDRESS;
            ArmInstruction instruction;
            do {
                ArmInstruction[] instructions = Disassembler.Disassemble(byteCode.Skip(currentAddress).ToArray(), currentAddress, 1);
                instruction = instructions[0];
                functionInstructions.Add(instruction);
                if (instruction.IsPoolLoad()) {
                    int poolAddress = instruction.GetPoolLoadAddress(currentAddress + VersionConstants.MAP_CODE_BASE_ADDRESS);
                    TryAddLabel(labels, ignoredAddresses, new PoolLabel(poolAddress, "", assembly));
                }
                if (instruction.Id == ArmInstructionId.ARM_INS_B) {
                    int targetAddress = VersionConstants.MAP_CODE_BASE_ADDRESS + instruction.GetBranchTarget();
                    TryAddLabel(labels, ignoredAddresses, new BranchLabel(targetAddress, "", assembly));
                }
                if (instruction.Id == ArmInstructionId.ARM_INS_BL) {
                    int targetAddress = VersionConstants.MAP_CODE_BASE_ADDRESS + instruction.GetBranchTarget();
                    labels.AddRange(AnalyzeFunction(targetAddress, byteCode, assembly, ignoredAddresses, $"sub_{targetAddress.ToString("X8")}"));
                }
                currentAddress += instruction.Bytes.Length;
            } while (!instruction.IsFunctionReturn());
            labels.Add(new FunctionLabel(startAddress, name, functionInstructions.ToArray(), assembly));
            return labels.ToArray();
        }

        private int[] AnalyzePoolReturnsInFile(FunctionLabel scannedFunction, MapCodeAssembly assembly) {
            List<int> targets = new List<int>();
            // TODO: Sophisticated approach: Exhaustively scan each code branch of the function and check the resulting return value.

            // Simplified approach: Scan every pool load and assume we found an NPC Array if the pool constant is inside the code file.
            //                      And the pool load targets r0
            int currentAddress = scannedFunction.Address;
            for (int i = 0; i < scannedFunction.Instructions.Length; ++i) {
                ArmInstruction currentInstruction = scannedFunction.Instructions[i];
                if (currentInstruction.IsPoolLoad() && currentInstruction.Details.Operands[0].Register.Id == ArmRegisterId.ARM_REG_R0) {
                    int poolAddress = assembly.GetPoolValue(currentInstruction, currentAddress);
                    if (poolAddress > VersionConstants.MAP_CODE_BASE_ADDRESS && poolAddress < (VersionConstants.MAP_CODE_BASE_ADDRESS + assembly.ByteCode.Length))
                        targets.Add(poolAddress);
                }
                currentAddress += currentInstruction.Bytes.Length;
            }
            return targets.ToArray();
        }
    }
}