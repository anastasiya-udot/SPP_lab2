using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace TraceParser
{

    public partial class Form1 : Form
    {
        TabControl tabControl;
        TabPage currentPage;
       
        public Form1()
        {
  
            InitializeComponent();
            tabControl = CreateTabArea();
            CreateContextMenu(null);            
        }

        private TabControl CreateTabArea()
        {
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Selecting += tabControl_Selecting;
            tabControl.AllowDrop = true;
            tabControl.DragEnter += tabControl_DragEnter;
            tabControl.DragDrop += tabControl_DragDrop;
            this.Controls.Add(tabControl);
            return tabControl;

        }

        private void tabControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))

                e.Effect = DragDropEffects.Move;
        }
        private void tabControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
    
            foreach (string file in droppedFiles)
            {
                if (CheckFileName(file))
                {
                    TabPageManager tabPageManager = new TabPageManager();
                    IOWorker io = new IOWorker();
                    tabPageManager.IOWorker = io;
                    io.SetIOWorkerProperties(Path.GetFullPath(file));
                    CreateNewTabPage(tabPageManager);
                    CreateContextMenu(tabPageManager);
                }
            }
        }

        private bool CheckFileName(string file)
        {
            if ((Path.GetExtension(file) == ".xml") || (Path.GetExtension(file) == ".XML"))
            {
                foreach (TabPage tabPage in tabControl.TabPages)
                {
                    TabPageManager tabPageManager = (TabPageManager)tabPage.Tag;
                    if (tabPageManager.IOWorker.FilePath == Path.GetFullPath(file))
                    {
                        tabControl.SelectedTab = tabPage;
                        return false;
                    }
                                
                }
                return true;
            }
            return false;
        }

        private void tabControl_Selecting(object sender, EventArgs e)
        {
            ChangeCurrentPage();
        }

        private void ChangeCurrentPage()
        {
            if (tabControl.SelectedTab != null)
            {
                currentPage = tabControl.SelectedTab;
            }
        }

        private void CreateNewTabPage(TabPageManager tabPageManager)
        {

            TabPage newTabPage = new TabPage(tabPageManager.IOWorker.FileName);
            tabPageManager.TreeViewBuilder = new TreeViewBuilder(tabPageManager.IOWorker.FilePath);
            newTabPage.Controls.Add(tabPageManager.TreeViewBuilder.TreeView);
            tabControl.TabPages.Add(newTabPage);
            tabControl.SelectedTab = newTabPage;
            newTabPage.Tag = tabPageManager;
            ChangeCurrentPage();

        }

        private void CreateContextMenu(TabPageManager tabPageManager){

            ContextMenuStrip contextMenu = new ContextMenuStrip();
           
            ToolStripMenuItem openFile = new ToolStripMenuItem();
            openFile.Text = "New...";
            openFile.MouseUp += openFile_MouseUp;

            ToolStripMenuItem saveFile = new ToolStripMenuItem();
            saveFile.Text = "Save";
            saveFile.MouseUp += saveFile_MouseUp;

            ToolStripMenuItem closeFile = new ToolStripMenuItem();
            closeFile.Text = "Close";
            closeFile.MouseUp += closeFile_MouseUp;
           
            contextMenu.Items.AddRange( new [] {openFile, saveFile, closeFile} );
            
            if (tabPageManager == null) 
            {
                saveFile.Available = false;
                closeFile.Available = false;
                this.ContextMenuStrip = contextMenu;
            }
            else
            {
                tabControl.ContextMenuStrip = contextMenu;
            }
        }

        private void closeFile_MouseUp(object sender, EventArgs e)
        {
            if (tabControl.TabPages.Count == 1)
            {
                tabControl.TabPages.Remove(currentPage);
                currentPage = null;
                CreateContextMenu(null);
            }

            foreach (TabPage tab in tabControl.TabPages)
            {
                if (tab != currentPage)
                {
                    tabControl.TabPages.Remove(currentPage);
                    return;
                }
            }
        }

        private void openFile_MouseUp(object sender, EventArgs e)
        {
            TabPageManager tabPageManager = new TabPageManager();
            IOWorker io = new IOWorker();
            tabPageManager.IOWorker = io;
            if (io.OpenFile(tabPageManager, tabControl))
            {
                tabPageManager.TreeViewBuilder = new TreeViewBuilder(io.FilePath);
                CreateNewTabPage(tabPageManager);
                CreateContextMenu(tabPageManager);
            }
        }


        private void saveFile_MouseUp(object sender, EventArgs e)
        {
            TabPageManager tabPageManager = (TabPageManager)currentPage.Tag;
            tabPageManager.IOWorker.SaveFile(tabPageManager.TreeViewBuilder);
        }
 
    }
}
