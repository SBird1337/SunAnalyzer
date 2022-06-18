namespace SunAnalyzer.Data {
    public class NpcEntry {
        public ushort SpriteId { get; set; }
        public ushort Flag {get; set; }
        public int ScriptAddress { get; set; }
        public short X { get; set; }
        public short Z { get; set; }
        public short Y { get; set; }
        public ushort Unknown { get; set; }
        public ushort Direction { get; set; }

        private NpcEntry() {}

        public static NpcEntry FromStream(Stream stream) {
            BinaryReader reader = new BinaryReader(stream);
            NpcEntry newEntry = new NpcEntry();
            newEntry.SpriteId = reader.ReadUInt16();
            newEntry.Flag = reader.ReadUInt16();
            newEntry.ScriptAddress = reader.ReadInt32();
            reader.ReadUInt16(); // Padding X
            newEntry.X = reader.ReadInt16();
            reader.ReadUInt16(); // Padding Z
            newEntry.Z = reader.ReadInt16();
            reader.ReadUInt16(); // Padding Y
            newEntry.Y = reader.ReadInt16();
            newEntry.Direction = reader.ReadUInt16();
            newEntry.Unknown = reader.ReadUInt16();

            return newEntry;
        }

        public override string ToString() {
            return $"\tnpc_entry 0x{SpriteId.ToString("X4")}, 0x{Flag.ToString("X4")}, 0x{ScriptAddress.ToString("X8")}, 0x{X.ToString("X4")}, 0x{Z.ToString("X4")}, 0x{Y.ToString("X4")}, 0x{Direction.ToString("X4")}, 0x{Unknown.ToString("X4")}";
        }
    }
}