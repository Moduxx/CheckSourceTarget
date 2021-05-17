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

                var fileLines = File.ReadAllLines(pathTextBox.Text);
                var lines = new List<String>(fileLines);

                string[] targetValues = new string[(lines.Count() + 1)];
                string[] sourceValues = new string[(lines.Count() + 1)];

                int k = 0;

                foreach (var line in lines)
                {
                    if (line.Contains("<source>"))
                    {
                        string[] firstSplit = line.Split('>');
                        string[] secondSplit = firstSplit[1].Split('<');
                        sourceValues[k] = secondSplit[0];
                    }
                    else if (line.StartsWith("          <target"))
                    {
                        string[] firstSplit = line.Split('>');
                        string[] secondSplit = firstSplit[1].Split('<');
                        targetValues[k] = secondSplit[0];
                        k++;
                    }
                }

                int correctMatches = 0;
                int incorrectMatches = 0;
                int duplicateValues = 0;

                if (sourceValues.Any())
                {
                    var myList = new List<string>();
                    var duplicates = new List<String>();

                    foreach (var sourceValue in sourceValues)
                    {
                        if (!myList.Contains(sourceValue))
                        {
                            myList.Add(sourceValue);
                        }
                        else
                        {
                            duplicates.Add(sourceValue);
                        }
                    }

                    if (duplicates.Any())
                    {
                        var uniqueItems = new HashSet<string>(duplicates);

                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                        saveFileDialog1.FileName = "SourceValueDuplicates.txt";
                        saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                        saveFileDialog1.FilterIndex = 2;
                        saveFileDialog1.RestoreDirectory = true;

                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());

                            foreach (string s in uniqueItems)
                            {
                                writer.WriteLine(s);
                                duplicateValues++;
                            }
                            if (duplicateValues > 0)
                            {
                                duplicateValues--;
                            }
                            writer.Dispose();
                            writer.Close();
                        }
                    }
                }

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

                MessageBox.Show(string.Format("Exact matches found: {0}, Differencies found: {1}, Duplicates found: {2}", correctMatches, incorrectMatches, duplicateValues));
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
