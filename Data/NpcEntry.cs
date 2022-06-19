using SunAnalyzer.Analyze;

namespace SunAnalyzer.Data {
    public class NpcEntry : DataEntry {
        public ushort SpriteId { get; set; }
        public ushort Flag {get; set; }
        public int ScriptAddress { get; set; }
        public short X { get; set; }
        public ushort Unknown1 {get; set; }
        public short Z { get; set; }
        public ushort Unknown2 {get; set; }
        public short Y { get; set; }
        public ushort Unknown3 { get; set; }
        public ushort Direction { get; set; }
        public ushort Unknown4 {get; set; }
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
            Unknown1 = reader.ReadUInt16();
            X = reader.ReadInt16();
            Unknown2 = reader.ReadUInt16();
            Z = reader.ReadInt16();
            Unknown3 = reader.ReadUInt16();
            Y = reader.ReadInt16();
            Direction = reader.ReadUInt16();
            Unknown4 = reader.ReadUInt16();
        }

        public override string ToString() {
            return $"\tnpc_entry 0x{SpriteId.ToString("X4")}, 0x{Flag.ToString("X4")}, 0x{ScriptAddress.ToString("X8")}, 0x{Unknown1.ToString("X4")}, 0x{X.ToString("X4")}, 0x{Unknown2.ToString("X4")}, 0x{Z.ToString("X4")}, 0x{Unknown3.ToString("X4")}, 0x{Y.ToString("X4")}, 0x{Direction.ToString("X4")}, 0x{Unknown4.ToString("X4")}";
        }
    }
}