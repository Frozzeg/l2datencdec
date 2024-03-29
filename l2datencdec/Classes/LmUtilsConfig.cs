#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

#endregion

namespace LmUtils
{
    public class Config
    {
        public Xml xml;
        protected bool inited = false;

        #region Constructors, destructor...
        public Config(string filename)
        {
            this.inited = false;

            this.xml = new Xml(filename);
            this.xml.ThisCanThrowExeptions = true;
            this.Reload();

            this.inited = true;
        }
        #endregion

        #region Last folders

        public void LoadLastFolder(OpenFileDialog dlg, string id)
        {
            this.LoadLastFolder(dlg, id, null);
        }

        public void LoadLastFolder(OpenFileDialog dlg, string id, string default_folder)
        {
            try
            {
                dlg.InitialDirectory = this.xml["LastFolder." + id];
            }
            catch
            {
                dlg.InitialDirectory = String.IsNullOrEmpty(default_folder) ? LmUtils.GlobalUtilities.GetStartDirectory(false) : default_folder;
            }
        }

        public void LoadLastFolder(SaveFileDialog dlg, string id)
        {
            this.LoadLastFolder(dlg, id, null);
        }

        public void LoadLastFolder(SaveFileDialog dlg, string id, string default_folder)
        {
            try
            {
                dlg.InitialDirectory = this.xml["LastFolder." + id];
            }
            catch
            {
                dlg.InitialDirectory = String.IsNullOrEmpty(default_folder) ? LmUtils.GlobalUtilities.GetStartDirectory(false) : default_folder;
            }
        }

        public void LoadLastFolder(FolderBrowserDialog dlg, string id)
        {
            this.LoadLastFolder(dlg, id, null);
        }

        public void LoadLastFolder(FolderBrowserDialog dlg, string id, string default_folder)
        {
            try
            {
                dlg.SelectedPath = this.xml["LastFolder." + id];
            }
            catch
            {
                dlg.SelectedPath = String.IsNullOrEmpty(default_folder) ? LmUtils.GlobalUtilities.GetStartDirectory(false) : default_folder;
            }
        }

        public void SaveLastFolder(OpenFileDialog dlg, string id)
        {
            try
            {
                this.xml["LastFolder." + id] = Path.GetDirectoryName(dlg.Multiselect ? dlg.FileNames[0] : dlg.FileName);
            }
            catch
            {
            }
        }

        public void SaveLastFolder(SaveFileDialog dlg, string id)
        {
            try
            {
                this.xml["LastFolder." + id] = Path.GetDirectoryName(dlg.FileName);
            }
            catch
            {
            }
        }

        public void SaveLastFolder(FolderBrowserDialog dlg, string id)
        {
            try
            {
                this.xml["LastFolder." + id] = dlg.SelectedPath;
            }
            catch
            {
            }
        }

        #endregion

        #region Form state

        public void SaveFormState(ColorDialog form, string id)
        {
            try
            {
                id = "FormState." + id;
                for (int k = 0; k < form.CustomColors.Length; k++)
                {
                    this.xml[id + ".CustomColors[]"] = form.CustomColors[k].ToString();
                }
            }
            catch
            {
            }
        }

        public void LoadFormState(ColorDialog dlg, string id)
        {
            try
            {
                id = "FormState." + id;
                XmlNodeList nodes = this.xml.GetNodes(id + ".CustomColors[]");

                if (nodes.Count > 0)
                {
                    int[] colors = new int[nodes.Count];
                    for (int k = 0; k < nodes.Count; k++)
                    {
                        colors[k] = Convert.ToInt32(nodes[k].InnerText);
                    }

                    dlg.CustomColors = colors;
                }
            }
            catch
            {
            }
        }

        public void SaveFormState(Form form, string id)
        {
            try
            {
                id = "FormState." + id;
                this.xml[id + ".X"] = form.Location.X.ToString();
                this.xml[id + ".Y"] = form.Location.Y.ToString();
                this.xml[id + ".Width"] = form.Size.Width.ToString();
                this.xml[id + ".Height"] = form.Size.Height.ToString();
                this.xml[id + ".State"] = form.WindowState.ToString();
            }
            catch
            {
            }
        }

        public void LoadFormState(Form form, string id)
        {
            try
            {
                id = "FormState." + id;

                if (this.xml.GetNode(id + ".Width") == null)
                    return;

                int x = Convert.ToInt32(this.xml[id + ".X"]);
                int y = Convert.ToInt32(this.xml[id + ".Y"]);
                int width = Convert.ToInt32(this.xml[id + ".Width"]);
                int height = Convert.ToInt32(this.xml[id + ".Height"]);

                form.StartPosition = FormStartPosition.Manual;
                form.SetBounds(x, y, width, height);
                form.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), this.xml[id + ".State"]);
            }
            catch
            {
            }
        }

        #endregion

        #region Save (virtual) / load (virtual)
        public virtual void Reload()
        {
            this.xml.Reload();
        }

        public virtual void Save()
        {
            this.xml.Save();
        }
        #endregion

        public virtual void Defaults()
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
            {
                try
                {
                    prop.SetValue(this, prop.GetValue(this));
                }
                catch
                {
                    prop.ResetValue(this);
                }
            }
        }

        public virtual void ResetDefaults()
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
            {
                prop.ResetValue(this);
            }
        }

        public object GetDefault(string property_name)
        {
            return (TypeDescriptor.GetProperties(this)[property_name].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute).Value;
        }
    }
}