using SunAnalyzer.Analyze;

namespace SunAnalyzer.Data {
    public class NpcEntry : DataEntry {
        public ushort SpriteId { get; set; }
        public ushort Flag {get; set; }
        public int ScriptAddress { get; set; }
        public short X { get; set; }
        public short Z { get; set; }
        public short Y { get; set; }
        public ushort Unknown { get; set; }
        public ushort Direction { get; set; }
        public override int Size => 24;

        public override bool IsEndElement => (SpriteId == 0xFFFF);

        public NpcEntry() : base () {}
        public override void Initialize(Stream stream, MapCodeAssembly assembly)
        {
            base.Initialize(stream, assembly);
            BinaryReader reader = new BinaryReader(stream);
            SpriteId = reader.ReadUInt16();
            Flag = reader.ReadUInt16();
            ScriptAddress = reader.ReadInt32();
            reader.ReadUInt16(); // Padding X
            X = reader.ReadInt16();
            reader.ReadUInt16(); // Padding Z
            Z = reader.ReadInt16();
            reader.ReadUInt16(); // Padding Y
            Y = reader.ReadInt16();
            Direction = reader.ReadUInt16();
            Unknown = reader.ReadUInt16();
        }

        public override string ToString() {
            return $"\tnpc_entry 0x{SpriteId.ToString("X4")}, 0x{Flag.ToString("X4")}, 0x{ScriptAddress.ToString("X8")}, 0x{X.ToString("X4")}, 0x{Z.ToString("X4")}, 0x{Y.ToString("X4")}, 0x{Direction.ToString("X4")}, 0x{Unknown.ToString("X4")}";
        }
    }
}