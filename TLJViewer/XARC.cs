using System;
using System.Collections.Generic;
using System.Text;

namespace TLJViewer
{
    class XARC
    {
        public struct FileEntry
	    {
		    public string name;
            public uint offset;
            public uint length;

            public uint idx;
	    }

        public static System.Windows.Forms.TreeNode ParseFile(string path)
        {
            // Read archive tree
            uint nFiles, baseOffset;
            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();

            System.IO.BinaryReader stream = new System.IO.BinaryReader(System.IO.File.OpenRead(path));
            stream.ReadUInt32();
            nFiles = stream.ReadUInt32();
            baseOffset = stream.ReadUInt32();

            for (int i = 0; i < nFiles; i++)
            {
                char b;
                FileEntry f = new FileEntry();
                do
                {
                    b = (char)stream.ReadByte();
                    if (b != 0)
                        f.name += b;
                } while (b != 0);
                f.length = stream.ReadUInt32();
                stream.ReadUInt32();

                f.offset = baseOffset;
                baseOffset += f.length;

                f.idx = (uint)i;

                System.Windows.Forms.TreeNode n = new System.Windows.Forms.TreeNode(f.name);
                n.Tag = f;

                node.Nodes.Add(n);
            }

            return node;
        }


    }
}
