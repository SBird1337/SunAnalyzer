namespace SunAnalyzer.Analyze {
    public abstract class Label : IComparable<Label> {
        private string _name;
        public enum LabelType {
            FUNCTION,
            DATA,
            BRANCH
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
        public int Size { get; set; }
        public abstract LabelType Type { get; }
        public MapCodeAssembly BaseAssembly {get; }

        public Label(int address, string name, MapCodeAssembly baseAssembly) {
            Address = address;
            _name = name;
            Size = -1;
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