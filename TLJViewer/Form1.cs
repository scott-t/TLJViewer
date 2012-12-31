using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TLJViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //delegate string stringFormatter (byte[] data);
        //stringFormatter[] functions = new stringFormatter()[0x26];

        //public static string gamepath = "d:/games/tlj demo";
        public static string gamepath = "d:/games/tlj";

        private void Form1_Load(object sender, EventArgs e)
        {
            TreeNode node = readSubDir(new System.IO.DirectoryInfo(gamepath));
            fileTree.Nodes.Add(node);
        }

        private TreeNode readSubDir(System.IO.DirectoryInfo dir)
        {
            TreeNode node = new TreeNode(dir.Name);
            node.Tag = dir;

            System.IO.DirectoryInfo[] sd = dir.GetDirectories();
            foreach (System.IO.DirectoryInfo subdir in sd)
                node.Nodes.Add(readSubDir(subdir));

            System.IO.FileInfo[] files = dir.GetFiles();
            foreach (System.IO.FileInfo file in files)
            {
                TreeNode child = new TreeNode();
                child.Name = child.Text = file.Name;
                child.Tag = file;

                if (file.Extension.ToLower() == ".xarc")
                {
                    TreeNode res = XARC.ParseFile(file.FullName);
                    TreeNode[] children = new TreeNode[res.Nodes.Count];
                    res.Nodes.CopyTo(children, 0);
                    child.Nodes.AddRange(children);
                }

                node.Nodes.Add(child);
            }

            return node;
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = fileTree.SelectedNode;
            string tmp = "";
            while (node != null && node.Parent != null)
            {
                tmp = "/" + node.Text + tmp;
                node = node.Parent;
            }

            lblPath.Text = tmp;
            if (fileTree.SelectedNode.Tag is System.IO.FileInfo)
            {
                System.IO.FileInfo file = (System.IO.FileInfo)fileTree.SelectedNode.Tag;

                scriptTree.Nodes.Clear();
                if (scriptNodeBinding.Count > 0)
                    scriptNodeBinding.Clear();

                if (file.Name.ToLower().EndsWith(".xrc"))
                    scriptTree.Nodes.Add(XRC.ParseXRC(file.OpenRead()));
            }
            else if (fileTree.SelectedNode.Tag is XARC.FileEntry)
            {
                XARC.FileEntry fe = (XARC.FileEntry)fileTree.SelectedNode.Tag;
                // Parse XRC node
                System.IO.FileStream fs = System.IO.File.OpenRead(((System.IO.FileInfo)(fileTree.SelectedNode.Parent.Tag)).FullName);
                byte[] buf = new byte[fe.length];
                fs.Seek(fe.offset, System.IO.SeekOrigin.Begin);
                fs.Read(buf, 0, (int)fe.length);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(buf);

                scriptTree.Nodes.Clear();

                if (scriptNodeBinding.Count > 0)
                    scriptNodeBinding.Clear();

                if (fe.name.ToLower().Contains(".xrc"))
                    scriptTree.Nodes.Add(XRC.ParseXRC(stream));

                lblPath.Text = fe.idx.ToString() + " - " + lblPath.Text;
            }
        }

        private void archiveTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (scriptNodeBinding.Count > 0)
                scriptNodeBinding.Clear();

            scriptNodeBinding.Add(scriptTree.SelectedNode.Tag);

            scriptNodeBinding.MoveFirst();
        }

    }
}
