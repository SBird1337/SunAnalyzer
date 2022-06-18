namespace SunAnalyzer.Analyze.Labels {
    public abstract class Label : IComparable<Label> {
        private string _name;
        public enum LabelType {
            FUNCTION,
            POOL,
            BRANCH,
            DATA
        }
        public int Address { get; set; }
        public string Name { 
            get {
                if (string.IsNullOrEmpty(_name)) {
                    return $"_{Address.ToString("X8")}";
                }
                return _name;
            }
            set {
                _name = value;
            } 
        }
        public abstract int Size { get; }
        public abstract LabelType Type { get; }
        public MapCodeAssembly BaseAssembly {get; }

        public Label(int address, string name, MapCodeAssembly baseAssembly) {
            Address = address;
            _name = name;
            BaseAssembly = baseAssembly;
        }

        public int CompareTo(Label? other) {
            if (other == null)
                return 1;
            return Address.CompareTo(other.Address);
        }

        public abstract override string ToString();
    }
}