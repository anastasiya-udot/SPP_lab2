using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TraceParser
{
    class IOWorker
    {
        public string FilePath
        {
            get; private set;
        }

        public string FileName
        {
            get;  private set;
        }


        private OpenFileDialog CreateOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;
            return openFileDialog;
        }

        private SaveFileDialog CreateSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-File | *.xml";
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.InitialDirectory = this.FilePath;
            return saveFileDialog;
        }

        public bool SaveFile(TreeViewBuilder treeViewBuilder)
        {
            SaveFileDialog saveFileDialog = CreateSaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                XDocument xmlDocument = new XDocument();

                XElement root = new XElement("root");
                AddNodes(root, treeViewBuilder.RootNode);
                xmlDocument.Add(root);

                xmlDocument.Save(saveFileDialog.FileName);
                return true;
            }
            return false;
        }

        private void AddNodes(XElement root, Node node)
        {
            foreach (Node currentNode in node.ListNodes)
            {
                if (currentNode.Tag == "thread")
                {
                    XElement thread = new XElement("thread");
                    XAttribute id = new XAttribute("id", currentNode.Id);

                    thread.Add(id);

                    root.Add(thread);

                    if (currentNode.ListNodes.Count != 0)
                        AddNodes(thread, currentNode);

                }
                if (currentNode.Tag == "method")
                {
                    XElement method = new XElement("method");

                    XAttribute methodName = new XAttribute("name", currentNode.Name);
                    XAttribute className = new XAttribute("package", currentNode.Package);
                    XAttribute parameters = new XAttribute("paramsCount", currentNode.ParamsCount);
                    XAttribute totalTime = new XAttribute("time", currentNode.Time);

                    method.Add(methodName);
                    method.Add(className);
                    method.Add(parameters);
                    method.Add(totalTime);

                    root.Add(method);

                    if (currentNode.ListNodes.Count != 0)
                        AddNodes(method, currentNode);
                }
            }
        }
        public void SetIOWorkerProperties(TabPageManager tabPageManager, string filePath)
        {
            this.FilePath = filePath;
            this.FileName = Path.GetFileName(this.FilePath);
        }

        public bool OpenFile(TabPageManager tabPageManager)
        {
            System.IO.Stream stream = null;
            OpenFileDialog openFileDialog = CreateOpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = openFileDialog.OpenFile()) != null)
                    {
                        SetIOWorkerProperties(tabPageManager, (stream as FileStream).Name);
                        tabPageManager.TreeViewBuilder = new TreeViewBuilder(this.FilePath);
                        return true;
                    }
                }
                catch (XmlException xExc)
                {
                    MessageBox.Show("Error: XML Error. Original error: " + xExc.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                finally
                {
                    stream.Close();
                }
            }
            return false;
        }

    }
}
