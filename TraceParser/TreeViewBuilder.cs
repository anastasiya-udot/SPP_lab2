using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace TraceParser
{
    class TreeViewBuilder
    {
        private Node currentNode;

        public TreeView TreeView
        {
            get;
            private set;
        }

        public Node RootNode
        {
            get;  private set;
        }

        public TreeViewBuilder(string filePath)
        {
            this.TreeView = CreateTreeViewFromXMLDocument(filePath);
        }

        private TreeView CreateTreeViewFromXMLDocument(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            TreeView treeView = CreateTreeView();
            treeView.Nodes.Add(new TreeNode(xmlDocument.DocumentElement.Name));

            TreeNode tNode = new TreeNode();
            tNode = (TreeNode)treeView.Nodes[0];

            this.RootNode = new Node();

            AddTreeNode(xmlDocument.DocumentElement, tNode, this.RootNode);
            return treeView;
        }
        private TreeView CreateTreeView()
        {
            TreeView treeView = new TreeView();
            treeView.Dock = DockStyle.Fill;
            treeView.NodeMouseDoubleClick += treeView_MouseDoubleClick;
            treeView.Nodes.Clear();
            return treeView;
        }


        public void treeView_MouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            currentNode = (Node)e.Node.Tag;
            if (currentNode != null)
            {
                PropertyAssistant propertyAssistant = new PropertyAssistant(currentNode, e.Node);        
            }
        }

        private void AddTreeNode(XmlNode xmlNode, TreeNode treeNode, Node node)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;

            if (xmlNode.HasChildNodes)
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                {
                    xNode = xmlNode.ChildNodes[x];

                    Node childNode = new Node();
                    node.ListNodes.Add(childNode);
                    childNode.SetProperties(xNode);
                    treeNode.Nodes.Add(new TreeNode(childNode.FormTreeViewString()));

                    tNode = treeNode.Nodes[x];
                    tNode.Tag = childNode;

                    AddTreeNode(xNode, tNode, childNode);
                }
            }
            else
            {
                node.SetProperties(xmlNode);
                treeNode.Text = node.FormTreeViewString();
                treeNode.Tag = node;
            }

        }
    }
}
