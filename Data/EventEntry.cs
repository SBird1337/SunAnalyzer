using SunAnalyzer.Analyze;
using SunAnalyzer.Analyze.Labels;
using SunAnalyzer.Version;

namespace SunAnalyzer.Data {
    public class EventEntry : DataEntry {
        public uint BitFlags { get; set; } // I don't exactly know what those are for yet, should classify the whole bitfield if known
        public ushort EventFlag { get; set; }
        public ushort ObjectId { get; set; }
        public int ScriptAddress { get; set; }
        public override int Size => 12;
        public override bool IsEndElement => (BitFlags == 0xFFFFFFFF);
        public EventEntry() : base () {}

        public override void Initialize(Stream stream, MapCodeAssembly assembly) {
            base.Initialize(stream, assembly);
            BinaryReader reader = new BinaryReader(stream);
            BitFlags = reader.ReadUInt32();
            EventFlag = reader.ReadUInt16();
            ObjectId = reader.ReadUInt16();
            ScriptAddress = reader.ReadInt32();
        }

        public override string ToString() {
            string scriptIdentifier = $"0x{ScriptAddress.ToString("X8")}";
            if (ScriptAddress > VersionConstants.MAP_CODE_BASE_ADDRESS && ScriptAddress < (VersionConstants.MAP_CODE_BASE_ADDRESS + Assembly!.ByteCode.Length)) {
                int thumbCodeAddress = ScriptAddress - 1;
                Label? l = Assembly!.Labels.FirstOrDefault(l => l.Address == thumbCodeAddress);
                if (l != null)
                    scriptIdentifier = $"{l.Name}+1";
            }
            return $"\tevent_entry 0x{BitFlags.ToString("X8")}, 0x{EventFlag.ToString("X4")}, 0x{ObjectId.ToString("X4")}, {scriptIdentifier}";
        }
    }
}