using System;
using System.Collections.Generic;
using System.Text;

namespace TLJViewer
{
    class XRC
    {
        //public static string[][] subtypeLookup = new string[][] 
        //{ new string[] { "" }, 
        //  new string[] { "4", "4", "4" } };
        public static string[] typeLookup = new string[]{
            "", // 00
            "W_Root", 
            "Level",
            "Load Sub-Scene?",
            "Layer", // 04
            "Camera",
            "Floor",
            "Face",
            "W_Item", // 08
            "Script node", 
            "AnimHierarchy",
            "Anim",
            "Direction", // 0c
            "Image", 
            "Anim Script",
            "Anim Script Item",
            "Sound Item", // 10
            "Path", 
            "FloorField",
            "W_Bookmark",
            "",  // 14
            "W_Knowledge",
            "ScriptCommand",
            "PATTable",
            "", // 18
            "",
            "W_Container",
            "W_Dialog",
            "", // 1c
            "W_Speech",
            "W_Light",
            "",
            "Mesh", // 20
            "W_Scroll",
            "FMV",
            "Lipsync",
            "AnimScrptBonesTrigger", // 24
            "String",
            "Textures",
        };

        static Func<System.IO.BinaryReader, byte, string>[] functions = 
            { new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), //00
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), 
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatLayer), //04
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatCamera),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatItem), //08
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatScript),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatAnim0B),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatDirection), //0C
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatWImage),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatSoundItem0F),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatSound10), //10
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatBookmark),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), //14
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatScriptCommand),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatPAT),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), //18
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatDialog),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), //1C
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatWSpeech),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatLight),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatModel20), //20
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatScroll),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatFMV),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatLipsync),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric), //24
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatGeneric),
              new Func<System.IO.BinaryReader, byte, string>(XRC.formatTexture26)};
    

        public class ScriptEntry
        {
            private string name;
            public byte type;
            public byte subType;

            public ushort unknown2;

            public uint dataLen;
            public byte[] data;

            public ushort unknown3;

            public String Name { 
                get { return name; }
                set { name = value; }
            }

            public string SubType
            {
                get { return String.Format("0x{0:X2}", subType); }
                set{}
            }

            public string RootType
            {
                get
                {
                    if (type < typeLookup.Length)
                        return string.Format("0x{0:X2} {1}", type, typeLookup[type]);
                    else
                        return String.Format("0x{0:X2}", type);
                }
                set{}

            }

            public string Unknown2
            {
                get { return String.Format("0x{0:X2}", unknown2); }
                set { }
            }

            public string Unknown3
            {
                get { return String.Format("0x{0:X2}", unknown3); }
                set { }
            }

            public uint DataLen
            {
                get { return dataLen; }
                set { dataLen = value; }
            }

            public string DataFormatted
            {
                get
                {
                    return functions[type](new System.IO.BinaryReader(new System.IO.MemoryStream(data)), subType);
                }
                set
                {
                    data = new byte[4];
                }
            }
        }

        public enum StrType{
            LEN_Byte,
            LEN_UShort,
            LEN_Uint};

        public static string readString(System.IO.BinaryReader data, StrType type)
        {
            uint len;
            if (type == StrType.LEN_Byte)
                len = data.ReadByte();
            else if (type == StrType.LEN_UShort)
                len = data.ReadUInt16();
            else
                len = data.ReadUInt32();

            byte[] tmp = data.ReadBytes((int)len);
            return System.Text.ASCIIEncoding.ASCII.GetString(tmp);
            
        }

        public static string readHex32(System.IO.BinaryReader data)
        {
            return String.Format("{0:X8}", data.ReadUInt32());
        }

        public static string readResID(System.IO.BinaryReader data)
        {
            string ret = "";
            int size = data.ReadInt32();
            ret += " Node ID [" + size + "]\n";

            for (int i = 0; i < size; ++i)
            {
                ret += "     idx " + i + ": " + String.Format("0x{0:X2}, 0x{1:X2}", data.ReadSByte(), data.ReadInt16()) + "\n";
            }

            return ret;
        }

        public static string formatGeneric(System.IO.BinaryReader data, byte subType = 0)
        {
            return formatG(int.MaxValue, data);
        }

        public static string formatG(System.IO.BinaryReader data)
        {
            return formatG(int.MaxValue, data);
        }

        public static string formatG(int count, System.IO.BinaryReader data)
        {
            string ret = "";
            if (count > (data.BaseStream.Length - data.BaseStream.Position))
                count = (int)(data.BaseStream.Length - data.BaseStream.Position);

            for (int i = 0; i < count; ++i)
                ret += String.Format("{0:X2} ", data.ReadByte());
            return ret;
        }

        public static string formatLayer(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x44: " + data.ReadInt32();
            if (subType == 2)
            {
                ret += "\nm_0x54: " + data.ReadInt32() +
                       "\nm_0x74: " + data.ReadSingle() +
                       "\nm_0x78: " + data.ReadSingle() + /* if something,  */ "\n" + formatG(data) /* endif */;
                
            }
            else
                ret += "\n" + formatG(data);
            return ret;
        }

        public static string formatCamera(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x38: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle() +
                         "\nm_0x44: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle() +
                         "\nm_0x50: " + data.ReadSingle();
            ret += "\nm_0x54: " + data.ReadSingle() +
                   "\nm_0x58: " + data.ReadInt32() + ", " + data.ReadInt32() + ", " + data.ReadInt32() + ", " + data.ReadInt32() +
                   "\nm_0x2C: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle() + "\n" + formatG(data); 
            return ret;
        }

        public enum itemType
        {
            ItemInternalGlobal = 0x01,
            ItemInternalLevel = 0x03,

            ItemVis3DImported = 0x05,
            ItemVis3DPickup = 0x06,
            ItemVisInventory = 0x02,
            ItemVis2D = 0x07,
            ItemVis3DChar = 0x0A,

            ItemVis3D,
            ItemVis,

            ItemInternal,

            ItemBase
        }

        public static string formatItem(System.IO.BinaryReader data, byte subType)
        {
            if (subType == 0x08)
                subType = 0x07; // Catches both

            string ret = formatItemPriv(data, (itemType)(subType)) + "\n leftover? " + formatG(data);
            return ret;
        }

        protected static string formatItemPriv(System.IO.BinaryReader data, itemType type)
        {
            string ret = "";
            int val = 0;
            switch (type)
            {
                case itemType.ItemVis3DImported:
                    // itemvis3dimported
                    ret = formatItemPriv(data, itemType.ItemVis3D);

                    val = data.ReadInt32();
                    ret += "m_0x78: " + data.ReadInt32() + ", " + data.ReadInt32();
                    ret += "\nm_0x6C: " + val;
                    break;

                case itemType.ItemVis3DPickup:
                    ret = formatItemPriv(data, itemType.ItemVis3D);

                    val = data.ReadInt32();
                    ret += "m_0x78: " + data.ReadInt32() + ", " + data.ReadInt32();
                    ret += "\nm_0x6C: " + val;
                    break;

                case itemType.ItemVis3DChar:
                    ret = formatItemPriv(data, itemType.ItemVis3D);
                    // ??  (*(int (__thiscall **)(_DWORD, _DWORD))(*((_DWORD *)v2 + 30) + 4))((char *)v2 + 120, a2);
                    break;

                case itemType.ItemVis2D:
                    ret = formatItemPriv(data, itemType.ItemVis);

                    ret += "m_0x60: " + data.ReadInt32() + ", " + data.ReadInt32();
                    //  (*(int (__thiscall **)(_DWORD, _DWORD))(*((_DWORD *)v2 + 21) + 4))((char *)v2 + 84, a2);
                    break;

                case itemType.ItemVis3D: // no other body
                case itemType.ItemVisInventory: // no other body
                case itemType.ItemVis:
                    ret = formatItemPriv(data, itemType.ItemBase);

                    ret += "m_0x44: " + data.ReadInt32();
                    break;

                case itemType.ItemInternalLevel:
                    ret = formatItemPriv(data, itemType.ItemInternal);

                    // (*(int (__thiscall **)(_DWORD, _DWORD))(*((_DWORD *)v2 + 20) + 4))((char *)v2 + 80, a2)
                    break;

                case itemType.ItemInternalGlobal: // no other obdy
                case itemType.ItemInternal: // no other body
                case itemType.ItemBase:
                    ret = "m_0x34: " + data.ReadInt32() + "\nm_0x38: " + data.ReadInt32();
                    break;

                default:
                    ret += formatG(data);
                    break;
            }

            return ret += "\n";
        }

        public static string formatScript(System.IO.BinaryReader data, byte subType)
        {
            int v9 = data.ReadInt32();
            int v8 = data.ReadInt32();
            
            string ret = "read v9: " + v9 + ", v8: " + v8 + "\nm_0x34: " + data.ReadInt32() + "\nm_0x38: " + data.ReadInt32();
            int a2 = data.ReadInt32();
            ret += "\n read a2 " + a2;

            ret += "\n " + formatG(data);

            return ret;
        }

        public static string formatAnim0B(System.IO.BinaryReader data, byte subType)
        {
            string ret = formatG(8, data) + "\ntext: " + readString(data, StrType.LEN_UShort);
            return ret + "\n" + formatG(data);
        }

        public static string formatDirection(System.IO.BinaryReader data, byte subType)
        {
            return "m_0x2C: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle();
        }

        public static string formatWImage(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x30: " + readString(data, StrType.LEN_UShort);
            ret += "\nm_0x34: " + data.ReadInt32() + ", " + data.ReadInt32() +
                   "\nm_0x3C: " + data.ReadInt32() + "\nm_0x40: " + data.ReadInt32() + "\n0x4C: Arr[" + data.ReadInt32() + "]";
            return ret + "\n" + formatG(data);
        }

        public static string formatSoundItem0F(System.IO.BinaryReader data, byte subType)
        {
            return formatG(4, data) + "\nlen " + data.ReadUInt32() + " (ms)\n" + formatG(data);
        }

        public static string formatSound10(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x8C: " + readString(data, StrType.LEN_UShort);
            ret += "\nm_0x60: " + data.ReadInt32() + ", " + data.ReadInt32() + ", " + data.ReadInt32() + ", " + data.ReadInt32() + "\n";
            ret += "m_0x48: " + data.ReadInt32() + ", m_0x70: " + data.ReadInt32() + ", m_0x50: " + data.ReadInt32() + "\n";
            ret += "m_0x90: " + readString(data, StrType.LEN_UShort);
            ret += "\nm_0x74: " + data.ReadInt32() + ", 0x54: " + data.ReadInt32() + ", 0x38: " + data.ReadInt32() + ", 0x34: " + data.ReadSingle();
            return ret + "\n" + formatG(data);
        }

        public static string formatBookmark(System.IO.BinaryReader data, byte subType)
        {
            return "m_0x2C: " + data.ReadSingle() + "\nm_0x30: " + data.ReadSingle() + "\n   " + formatG(data);
        }

        public static string formatScriptCommand(System.IO.BinaryReader data, byte subType)
        {
            string ret = "";

            int val = data.ReadInt32();
            if (val != 0)
            {

            }
            else
            {

            }

            if (val != 0 /* > v3 */)
            {
                for (int v16 = 0; v16 < val; ++v16)
                {
                    // sub 1004b3b0
                    {
                        int v = data.ReadInt32();
                        switch (v)
                        {
                            case 1:
                            case 2:
                                ret += data.ReadInt32() + "\n";
                                break;

                            case 3:
                                {
                                    ret += readResID(data);
                                }
                                break;

                            case 4:
                                ret += readString(data, StrType.LEN_UShort) + "\n";
                                break;

                            case 6:
                                // literally empty
                                break;

                            default:
                                System.Windows.Forms.MessageBox.Show("heh");
                                break;
                        }
                    }

                    // other si just debug
                }
            }

            return ret + "\n  leftover?" + formatG(data);
        }

        public static string formatPAT(System.IO.BinaryReader data, byte subType)
        {
            int val = data.ReadInt32();
            string ret = "";

            if (val > 0)
            {

            }
            else
            {

            }
            
            if (val > 0)
            {
                ret += "m_0xYY: Arr[" + val.ToString() + "]";
                for (int i = 0; i < val; ++i)
                {
                    ret += "\n     <" + data.ReadInt32() + ", " + data.ReadInt32() + ">";
                }
            }

            return ret + "\n" + formatG(data);
        }

        public static string formatDialog(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x44: " + data.ReadInt32() + "\nm_0x40: " + data.ReadInt32() + "\n";
            int val = data.ReadInt32();

            int v4 = 0;

            if (val != 0)
            {

            }
            else
            {

            }

            for (int k = 0; k < val; ++k)
            {
                ret += data.ReadInt32() + "\n{\n";
                int count = data.ReadInt32();

                for (int i = 0; i < count; ++i)
                {
                    ret += " inner: " + data.ReadInt32() + "\n";
                    ret += "  " + readResID(data) + "  " + readResID(data);
                    ret += "    " + data.ReadInt32() + " " + data.ReadInt32() + " " + data.ReadInt32() + " " + data.ReadInt32() + " " + data.ReadInt32() + " " + data.ReadInt32() + "\n";
                    ret += "  " + readResID(data) + "\n  {\n";
                    int count2 = data.ReadInt32();
                    for (int j = 0; j < count2; ++j)
                    {
                        ret += "   " + readResID(data) + "   " + readResID(data);
                    }
                    ret += "  }\n";
                }
                ret += "}\n";
            }

            return ret + "\n " + formatG(data);
        }

        public static string formatWSpeech(System.IO.BinaryReader data, byte subType)
        {
            string ret = "text: " + readString(data, StrType.LEN_UShort);
            return ret + "\n" + formatG(data);
        }

        public static string formatLight(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x30: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle() +
                "\nm_0x3C: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle() +
                "\nm_0x48: " + data.ReadSingle() + ", " + data.ReadSingle() + ", " + data.ReadSingle();

            ret += "\n att m_0x58: " + data.ReadSingle() + "\nm_0x54: " + data.ReadSingle();

            return ret + "\n" + formatG(data);
        }

        public static string formatModel20(System.IO.BinaryReader data, byte subType)
        {
            string ret = "model: " + readString(data, StrType.LEN_UShort);
            return ret + "\n" + formatG(data);
        }

        public static string formatScroll(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x2C: " + data.ReadInt32() + "\nm_0x34: " + data.ReadInt32() +
                "\nm_0x30: " + data.ReadInt32() + "\nm_0x38: " + data.ReadInt32();
            return ret + "\n" + formatG(data);
        }

        public static string formatFMV(System.IO.BinaryReader data, byte subType)
        {
            string ret = "m_0x2C: " + readString(data, StrType.LEN_UShort);
            ret += "\nm_0x34: " + data.ReadInt32();
            if (data.BaseStream.Position < data.BaseStream.Length)
                ret += "\nm_0x38: " + data.ReadInt32();
            ret += "\n    " + formatG(data);
            return ret;
        }

        public static string formatLipsync(System.IO.BinaryReader data, byte subType)
        {
            string ret = "{\n";
            int count = data.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                int val = data.ReadInt32();
                ret += "  " + val + " (" + (char)(val) + "), " + data.ReadInt32() + "\n";
            }

            ret += "}\n" + formatG(data);
            return ret;
        }

        public static string formatTexture26(System.IO.BinaryReader data, byte subType)
        {
            string ret = "tex: " + readString(data, StrType.LEN_UShort);
            return ret + "\n" + formatG(data);
        }

        public static System.Windows.Forms.TreeNode ParseXRC(System.IO.Stream s)
        {
            System.IO.BinaryReader stream;
            stream = new System.IO.BinaryReader(s);

            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
            return readNode(stream);
        }

        private static System.Windows.Forms.TreeNode readNode(System.IO.BinaryReader stream, int parent = 0){
            ScriptEntry entry = new ScriptEntry();

            entry.type = stream.ReadByte();
            entry.subType = stream.ReadByte();

            entry.unknown2 = stream.ReadUInt16();

            uint len = stream.ReadUInt16();
            for (int i = 0; i < len; i++)
                entry.Name += (char)stream.ReadByte();

            entry.dataLen = stream.ReadUInt32();
            entry.data = stream.ReadBytes((int)entry.dataLen);

            len = stream.ReadUInt16();
            entry.unknown3 = stream.ReadUInt16();

            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode(entry.Name);
            node.Tag = entry;

            if (entry.type == 2 && len == 0)
            {
                string file = String.Format("{0:X2}", entry.unknown2) + "/" + String.Format("{0:X2}", entry.unknown2);
                if (entry.unknown2 == 0)
                    file = "Global/Global";
                System.Windows.Forms.TreeNode n = XARC.ParseFile(Form1.gamepath + "/" + file + ".xarc");
                n = n.FirstNode;// Nodes.Find(entry.Name + ".xrc", false)[0];

                XARC.FileEntry fe = (XARC.FileEntry)n.Tag;
                // Parse XRC node
                System.IO.FileStream fs = System.IO.File.OpenRead(Form1.gamepath + "/" + file + ".xarc");
                byte[] buf = new byte[fe.length];
                fs.Seek(fe.offset, System.IO.SeekOrigin.Begin);
                fs.Read(buf, 0, (int)fe.length);
                return readNode(new System.IO.BinaryReader(new System.IO.MemoryStream(buf)));
            }
            else if (entry.type == 3 && len == 0)
            {
                System.Windows.Forms.TreeNode n = XARC.ParseFile(Form1.gamepath + "/" + String.Format("{0:X2}", parent) + "/" + String.Format("{0:X2}", entry.unknown2) + "/" + String.Format("{0:X2}", entry.unknown2) + ".xarc");
                n = n.FirstNode;// Nodes.Find(entry.Name, false)[0];

                XARC.FileEntry fe = (XARC.FileEntry)n.Tag;
                // Parse XRC node
                System.IO.FileStream fs = System.IO.File.OpenRead(Form1.gamepath + "/" + String.Format("{0:X2}", parent) + "/" + String.Format("{0:X2}", entry.unknown2) + "/" + String.Format("{0:X2}", entry.unknown2) + ".xarc");
                byte[] buf = new byte[fe.length];
                fs.Seek(fe.offset, System.IO.SeekOrigin.Begin);
                fs.Read(buf, 0, (int)fe.length);
                return readNode(new System.IO.BinaryReader(new System.IO.MemoryStream(buf)));
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    node.Nodes.Add(readNode(stream, entry.unknown2));
                }

            }

            return node;
        }
    }
}
