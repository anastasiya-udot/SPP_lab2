using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace TraceParser
{
    class Node
    {
        public List<Node> ListNodes { get; set; }

        public int Id
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Package
        {
            get; set;
        }
        public int ParamsCount
        {
            get; set;
        }
        public double Time
        {
            get; set;
        }

        public string Tag
        {
            get; private set;
        }

        public Node()
        {
            ListNodes = new List<Node>();
        }

        public void SetProperties(XmlNode xNode)
        {
            this.Tag = xNode.Name;
            for (int counter = 0; counter < xNode.Attributes.Count; counter++)
            {
                DefineProperty(xNode.Attributes[counter].LocalName, xNode.Attributes[counter].Value);
            }
        }

        public void DefineProperty(string attributeString, string valueString)
        {
            try
            {
                switch (attributeString)
                {
                    case "id":
                        {
                            int id = Convert.ToInt32(valueString);
                            if (id < 0) throw new Exception();
                            this.Id = id;
                        } break;
                    case "name":
                        {
                            this.Name = valueString;
                        } break;
                    case "package":
                        {
                            this.Package = valueString;
                        } break;
                    case "paramsCount":
                        {
                            int paramsCount = Convert.ToInt32(valueString);
                            if (paramsCount < 0) throw new Exception();
                            this.ParamsCount = paramsCount;
                        } break;
                    case "time":
                        {
                            double time = Convert.ToDouble(valueString.Replace(".", ","));
                            if (time < 0) throw new Exception();
                            this.Time = time;
                        } break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("XML data are not correct: {0} = \"{1}\"", attributeString, valueString) );
            }
        }

        public string FormTreeViewString()
        {
            if (this.Tag == "method")
            {
                string name = " name=\"" + this.Name + "\"";
                string package = " package=\"" + this.Package + "\"";
                string paramsCount = " paramsCount=\"" + this.ParamsCount.ToString() + "\"";
                string time = " time=\"" + this.Time.ToString() + "\"";
                return this.Tag + " " + name + package + paramsCount + time;
            }
            if (this.Tag == "thread")
            {
                string id = " id=\"" + this.Id + "\"";
                return this.Tag + " " + id;
            }
            return this.Tag;
        }

        public string[] GetAttributes()
        {
            string[] attributes;
            if (this.Tag == "method")
                return attributes =  new string [] {"name", "package","paramsCount","time"};               
            else
                return attributes = new string[] { "id" };
        }

    }
}
