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
        Button button;
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
            button = CreateButton(attributesBox, form, inputBox);
            form.Controls.AddRange(new Control[] { attributesBox, inputBox, button });
            return form;
        }

        private TextBox CreateTextBox(ComboBox attributesBox, Form form)
        {
            TextBox box = new TextBox();
            box.Location = new Point(0, attributesBox.Height * 4);
            box.Width = form.Width - form.Width / 2;
            box.KeyDown += inputBox_KeyDown;
            box.Text = currentNode.Name;
        
            return box;
        }

        private Button CreateButton(ComboBox attributesBox, Form form, TextBox inputBox)
        {
            Button button = new Button();
            button.Location = new Point(inputBox.Width + 10, attributesBox.Height * 4);
            button.Click += button_Click;
            button.Text = "Confirm";
            return button;
        }

        private void button_Click(object seder, EventArgs e)
        {
            ParseAttributeValue(inputBox.Text);
            SetInputBoxText();
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

        private void SetInputBoxText()
        {
            switch(attributesBox.GetItemText(attributesBox.SelectedItem)){
                case "name":{
                    inputBox.Text = currentNode.Name;
                } break;
                case "package":{
                    inputBox.Text = currentNode.Package;
                } break;
                case "paramsCount": {
                    inputBox.Text = currentNode.ParamsCount.ToString();
                } break;
                case "time": {
                    inputBox.Text = currentNode.Time.ToString();
                }break;
        
            }
        }

        private void ChangeNodeParentsTime(TreeNode treeNode, double timeDifference)
        {
            TreeNode parentTreeNode = treeNode.Parent;
            Node parentNode = (Node)parentTreeNode.Tag;
      //      timeDifference = Math.Round(timeDifference, 4);
                while (parentNode.Tag != null)
                {
                    if ((parentNode.Time + timeDifference) > 0)
                    {
                        parentNode.Time += timeDifference;
                    }
                    else
                    {
                        parentNode.Time = 0;
                    }
                    parentTreeNode.Text = parentNode.FormTreeViewString();
                    parentTreeNode = parentTreeNode.Parent;
                    parentNode = (Node)parentTreeNode.Tag;
                    if (parentNode == null) break;
                }

        }

        private ComboBox CreateComboBox(Node node, Form form)
        {
            ComboBox box = new ComboBox();
            string[] attributes = node.GetAttributes();
            box.Items.AddRange(attributes);
            box.SelectedIndex = 0;
            box.Dock = DockStyle.Top;
            box.SelectionChangeCommitted += box_SelectionChangeCommitted;
            return box;
        }

        private void box_SelectionChangeCommitted(object seder, EventArgs e)
        {
            SetInputBoxText();
        }

        private void inputBox_KeyDown(object seder, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ParseAttributeValue(inputBox.Text);
                SetInputBoxText();
            }
        }
    }
}
