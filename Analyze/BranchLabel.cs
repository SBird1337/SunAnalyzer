namespace SunAnalyzer.Analyze {
    public class BranchLabel : Label
    {
        public override LabelType Type => LabelType.BRANCH;
        public BranchLabel(int address, string name, MapCodeAssembly baseAssembly) : base(address, name, baseAssembly)
        {
        }

        

        public override string ToString()
        {
            return $"{Name}:";
        }
    }
}