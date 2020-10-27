using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CheckSourceTarget
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            if (pathTextBox.Text != "")
            {
                var filePath = string.Empty;
                var fileContent = string.Empty;

                using (StreamReader reader = new StreamReader(pathTextBox.Text))
                {
                    fileContent = reader.ReadToEnd();
                }

                string[] lines = Regex.Split(fileContent, Environment.NewLine);

                string[] targetValues = new string[(lines.Count() + 1)];
                string[] sourceValues = new string[(lines.Count() + 1)];

                int k = 0;

                foreach (var line in lines)
                {
                    if (line.StartsWith("          <source>"))
                    {
                        string[] firstSplit = line.Split('>');
                        string[] secondSplit = firstSplit[1].Split('<');
                        sourceValues[k] = secondSplit[0];
                    }
                    else if (line.StartsWith("          <target>"))
                    {
                        string[] firstSplit = line.Split('>');
                        string[] secondSplit = firstSplit[1].Split('<');
                        targetValues[k] = secondSplit[0];
                        k++;
                    }
                }

                int correctMatches = 0;
                int incorrectMatches = 0;

                for (int i = 0; i < k; i++)
                {
                    if (string.Equals(sourceValues[i], targetValues[i]))
                    {
                        correctMatches++;
                    }
                    else
                    {
                        incorrectMatches++;
                    }
                }

                MessageBox.Show(string.Format("Exact matches found: {0}, Differencies found: {1}", correctMatches, incorrectMatches));
            }
            else
                MessageBox.Show("Please select translation file path!");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "XLIFF (*.xlf)|*.xlf|All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = ofd.FileName;
                }
            }
        }
    }
}
