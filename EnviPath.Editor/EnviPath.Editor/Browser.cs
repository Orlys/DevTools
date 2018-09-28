
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
    public partial class Browser : Form
    {
        private readonly Editor _editor1;
        private readonly Editor _editor2;
        private readonly FolderBrowserDialog _folderBrowserDialog;

        private Logger _logger;
        internal Browser(Logger logger) : this()
        {
            this._logger = logger;
            this.Assert(listBox1, this.Load(EnvironmentVariableTarget.Machine));
            this.Assert(listBox2, this.Load(EnvironmentVariableTarget.User));
            
        }
        public Browser()
        {
            InitializeComponent();

            this._editor1 = new Editor();
            this._editor2 = new Editor();
            this._folderBrowserDialog = new FolderBrowserDialog();

            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox2.ContextMenuStrip = this.contextMenuStrip2;


            this.listBox1.LostFocus += delegate
            {
                this.listBox1.ClearSelected();
            };
            this.listBox2.LostFocus += delegate
            {
                this.listBox2.ClearSelected();
            };

            this.listBox1.MouseDoubleClick += (sender, e) =>
            {
                var index = this.listBox1.SelectedIndex;
                if (index == -1)
                    return;
                if (this.listBox1.Items[index] is string oldValue)
                {
                    this._editor1.Init(this, oldValue);

                    //edit value
                    if (this._editor1.IsUpdated && this.DoubleCheck())
                    {
                        
                        var newValue = this._editor1.Result;

                        this.listBox1.Items[index] = newValue;
                       this.Update(EnvironmentVariableTarget.Machine, oldValue, newValue);
                    }
                }
            };

            this.listBox2.MouseDoubleClick += (sender, e) =>
            {
                var index = this.listBox2.SelectedIndex;
                if (index == -1)
                    return;
                if (this.listBox2.Items[index] is string oldValue)
                {
                    this._editor2.Init(this, oldValue);

                    //edit value
                    if (this._editor2.IsUpdated)
                    {
                        var newValue = this._editor2.Result;

                        this.listBox2.Items[index] = newValue;
                        this.Update(EnvironmentVariableTarget.User, oldValue, newValue);
                    }
                }
            };

            this.addToolStripMenuItem1.Click += (sender, e) =>
            {
                if (this._folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var folder = this._folderBrowserDialog.SelectedPath;
                    this.AddItem(this.listBox1, folder);
                    this.Append(EnvironmentVariableTarget.Machine, folder);
                }
            };

            this.addToolStripMenuItem2.Click += (sender, e) =>
            {
                if (this._folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var folder = this._folderBrowserDialog.SelectedPath;

                    this.AddItem(this.listBox2, folder);
                    this.Append(EnvironmentVariableTarget.User, folder);
                }
            };

            this.removeToolStripMenuItem1.Click += (sender, e) =>
            {
                var index = listBox1.SelectedIndex;
                if (index != -1 && this.DoubleCheck() )
                {
                   
                    var target = listBox1.Items[index] as string;
                    listBox1.Items.RemoveAt(index);
                    this.Remove(EnvironmentVariableTarget.Machine, target);
                }
            };
            this.removeToolStripMenuItem2.Click += (sender, e) =>
            {
                var index = listBox2.SelectedIndex;
                if (index != -1)
                {
                    var target = listBox2.Items[index] as string;
                    listBox2.Items.RemoveAt(index);
                    this.Remove(EnvironmentVariableTarget.User, target);
                }
            };

        }

        private void Assert(ListBox listBox, string[] list)
        {
            listBox.Items.Clear();
            listBox.Items.AddRange(list);
        }

        private const string PATH = "PATH";
        private const char SEPARATOR = ';';

        private string[] Load(EnvironmentVariableTarget environmentVariableTarget)
        {
            if (environmentVariableTarget == EnvironmentVariableTarget.Process)
                throw new NotSupportedException();
            var pathString = Environment.GetEnvironmentVariable(PATH, environmentVariableTarget);

            this._logger.WriteLine(environmentVariableTarget, pathString);
            return pathString.Split(SEPARATOR);
        }

        private void Update(EnvironmentVariableTarget environmentVariableTarget, string oldValue, string newValue)
        {
            var pathString = Environment.GetEnvironmentVariable(PATH, environmentVariableTarget);
            pathString = pathString.Replace(oldValue, newValue);

            if (environmentVariableTarget == EnvironmentVariableTarget.Process)
                throw new NotSupportedException();

            this._logger.WriteLine(environmentVariableTarget, pathString);
            Environment.SetEnvironmentVariable(PATH, pathString, environmentVariableTarget);

        }
        private void Remove(EnvironmentVariableTarget environmentVariableTarget, string oldValue)
        {
            var pathString = Environment.GetEnvironmentVariable(PATH, environmentVariableTarget);
            pathString = pathString.Replace(oldValue + SEPARATOR, null);

            if (environmentVariableTarget == EnvironmentVariableTarget.Process)
                throw new NotSupportedException();

            this._logger.WriteLine(environmentVariableTarget, pathString);
            Environment.SetEnvironmentVariable(PATH, pathString, environmentVariableTarget);

        }

        private void Append(EnvironmentVariableTarget environmentVariableTarget, string newValue)
        {
            var pathString = Environment.GetEnvironmentVariable(PATH, environmentVariableTarget);
            pathString = pathString + newValue + SEPARATOR;

            if (environmentVariableTarget == EnvironmentVariableTarget.Process)
                throw new NotSupportedException();

            this._logger.WriteLine(environmentVariableTarget, pathString);
Environment.SetEnvironmentVariable(PATH, pathString, environmentVariableTarget);

        }


        private void AddItem(ListBox listBox, string value)
        {
            if (value.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                var lastIndex = listBox.Items.Count - 1;
                if (string.IsNullOrWhiteSpace(listBox.Items[lastIndex] as string))
                {
                    listBox.Items[lastIndex] = value;
                }
                else
                {
                    listBox.Items.Add(value);
                }
            }
        }

        private bool DoubleCheck()
        {
            return (MessageBox.Show("ARE YOU SURE? Changing or deleting this path may cause system problems.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
        }

    }
}
