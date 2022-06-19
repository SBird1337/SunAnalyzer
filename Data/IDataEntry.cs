namespace SunAnalyzer.Data {
    public interface IDataEntry {

        public bool IsEndElement { get; }
        public int Size { get; }

        public string ToString();
    }
}