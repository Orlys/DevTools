using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnviPath.Editor
{
    public partial class Editor : Form
    {
        public Editor()
        {
            InitializeComponent();

            

            this.FormClosing += (sender, e) =>
            {
                if (this.richTextBox1.Text.Equals(this._oldValue))
                {
                    return;
                }

                var replacedWritespace = this.richTextBox1.Text.Replace("\r", null).Replace("\n", null);
                if (replacedWritespace.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                {
                    var choice = MessageBox.Show("Invalid characters in path. Do you want to continue to modify?", "Error", MessageBoxButtons.YesNo);
                    this.richTextBox1.Text = this._oldValue;
                    if (choice == DialogResult.Yes)
                    {
                        this.richTextBox1.SelectAll();
                        e.Cancel = true;
                    }
                }
                else if (string.IsNullOrWhiteSpace(replacedWritespace))
                    return;
                else if (Directory.Exists(replacedWritespace))
                {
                    this.IsUpdated = true;
                    this.Result = replacedWritespace;
                }
                else
                {
                    var choice = MessageBox.Show("Directory not found. Do you want to continue to modify?", "Error", MessageBoxButtons.YesNo);
                    this.richTextBox1.Text = this._oldValue;
                    if (choice == DialogResult.Yes)
                    {
                        this.richTextBox1.SelectAll();
                        e.Cancel = true;
                    }
                }
            };

        }

        public void Init(Form parent)
        {
            this.Result = string.Empty;
            this.IsUpdated = false;
            this.ShowDialog(parent);
        }
        public void Init(Form parent, string value)
        {
            this.Result = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                return;
            this._oldValue = value ;
            this.richTextBox1.Text = value;
            this.IsUpdated = false;
            
            this.richTextBox1.SelectAll();

            this.ShowDialog(parent);
        }

        private string _oldValue;
        public string Result { get; private set; }
        public bool IsUpdated { get; private set; }
        

        
    }
}
