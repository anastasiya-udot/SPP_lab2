using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TraceParser
{
    class PropertyAssistant
    {
        ComboBox attributesBox;
        TextBox inputBox;
        private Node currentNode;
        private TreeNode treeNode;

        public PropertyAssistant(Node node, TreeNode treeNode)
        {
            this.currentNode = node;
            this.treeNode = treeNode;
            Form attrForm = CreateNewForm(node);
            attrForm.ShowDialog();
        } 

        private Form CreateNewForm(Node node)
        {
            Form form = new Form();
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            attributesBox = CreateComboBox(node, form);
            inputBox = CreateTextBox(attributesBox, form);
            attributesBox.SelectedIndexChanged += attributesBox_SelectedIndexChanged;
            form.Controls.AddRange(new Control[] { attributesBox, inputBox });
            return form;
        }


        private void attributesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputBox.Text = String.Empty;
        }

        private TextBox CreateTextBox(ComboBox attributesBox, Form form)
        {
            TextBox box = new TextBox();
            box.Location = new Point(0, attributesBox.Height * 4);
            box.Width = form.Width;
            box.KeyDown += inputBox_KeyDown;
            return box;
        }


        private void ParseAttributeValue(string value)
        {
            try
            {
                double previousTime = currentNode.Time;
                currentNode.DefineProperty(attributesBox.SelectedItem.ToString(), value);
                treeNode.Text = currentNode.FormTreeViewString();
                if (attributesBox.SelectedItem.ToString() == "time")
                    ChangeNodeParentsTime(treeNode, currentNode.Time - previousTime);

            }
            catch(Exception e){
                MessageBox.Show("Enter valid value");
            }
        }

        private void ChangeNodeParentsTime(TreeNode treeNode, double timeDifference)
        {
            TreeNode parentTreeNode = treeNode.Parent;
            Node parentNode = (Node)parentTreeNode.Tag;
            while (parentNode.Tag != "thread")
            {
                parentNode.Time += timeDifference;
                parentTreeNode.Text = parentNode.FormTreeViewString();
                parentTreeNode = parentTreeNode.Parent;
                parentNode = (Node)parentTreeNode.Tag;
            }
        }

        private ComboBox CreateComboBox(Node node, Form form)
        {
            ComboBox box = new ComboBox();
            string[] attributes = node.GetAttributes();
            box.Items.AddRange(attributes);
            box.SelectedIndex = 0;
            box.Dock = DockStyle.Top;
            return box;
        }

        private void inputBox_KeyDown(object seder, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ParseAttributeValue(inputBox.Text);
                inputBox.Text = String.Empty;
            }
        }
    }
}
