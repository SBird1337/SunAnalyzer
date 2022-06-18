using Gee.External.Capstone.Arm;

namespace SunAnalyzer.Extension {
    public static class ArmInstructionExtensions {
        private static readonly ArmInstructionId[] ValidBranchInstructions = { ArmInstructionId.ARM_INS_B, ArmInstructionId.ARM_INS_BX, ArmInstructionId.ARM_INS_BL };
        public static bool IsPoolLoad(this ArmInstruction instruction) {
            if (!instruction.HasDetails)
                throw new ArgumentException("Instruction has no detail information", "instruction");
            var details = instruction.Details;
            return instruction.Id == ArmInstructionId.ARM_INS_LDR 
                && details.Operands[0].Type == ArmOperandType.Register
                && details.Operands[1].Type == ArmOperandType.Memory
                && !details.Operands[1].IsSubtracted
                && details.Operands[1].Memory.Base.Id == ArmRegisterId.ARM_REG_PC
                && ((details.Operands[1].Memory.Index == null) || (details.Operands[1].Memory.Index.Id == ArmRegisterId.Invalid));
        }

        public static int GetPoolLoadAddress(this ArmInstruction instruction, int currentAddress) {
            if (!instruction.HasDetails)
                throw new ArgumentException("Instruction has no detail information", "instruction");
            if (!instruction.IsPoolLoad())
                throw new ArgumentException("Instruction is not a pool load", "instruction");
            return (currentAddress & ~3) + instruction.Details.Operands[1].Memory.Displacement + 4;
        }

        public static bool IsFunctionReturn(this ArmInstruction instruction) {
            if (!instruction.HasDetails)
                throw new ArgumentException("Instruction has no detail information", "instruction");
            var details = instruction.Details;
            
            // 'bx' instruction
            if (instruction.Id == ArmInstructionId.ARM_INS_BX)
                return details.ConditionCode == ArmConditionCode.ARM_CC_AL;
            
            // 'mov' instruction with pc as the destination
            if (instruction.Id == ArmInstructionId.ARM_INS_MOV
             && details.Operands[0].Type == ArmOperandType.Register
             && details.Operands[0].Register.Id == ArmRegisterId.ARM_REG_PC)
                return true;
            
            // 'pop' with pc in the register list
            if (instruction.Id == ArmInstructionId.ARM_INS_POP) {
                foreach (ArmOperand operand in details.Operands) {
                    if (operand.Type == ArmOperandType.Register && operand.Register.Id == ArmRegisterId.ARM_REG_PC)
                        return true;
                }
            }

            return false;
        }

        public static bool IsBranch(this ArmInstruction instruction) {
            return ValidBranchInstructions.Contains(instruction.Id);
        }

        public static int GetBranchTarget(this ArmInstruction instruction) {
            if (!instruction.HasDetails)
                throw new ArgumentException("Instruction has no detail information", "instruction");
            if (!instruction.IsBranch())
                throw new ArgumentException("Instruction is not a branch", "instruction");
            return instruction.Details.Operands[0].Immediate;
        }
    }
}