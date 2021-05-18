using CheckSourceTarget.src.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
                    var myList = new List<MultipleStrings>();
                    var duplicates = new List<MultipleStrings>();
                    int i = 0;

                    foreach (var sourceValue in sourceValues)
                    {
                        MultipleStrings ms = new MultipleStrings();
                        ms.SourceValue = sourceValue;
                        ms.TargetValue = targetValues[i];

                        var tempMyList = myList.FindAll(x => x.SourceValue == ms.SourceValue);

                        if (!tempMyList.Any())
                        {
                            myList.Add(ms);
                        }
                        else
                        {
                            duplicates.Add(ms);
                            Console.WriteLine("Kazka Rado " + ms.SourceValue + " " + ms.TargetValue);
                        }
                        i++;
                    }

                    if (duplicates.Any())
                    {
                        var filteredSources = new List<String>();
                        Boolean IsNotMatch;

                        foreach (var item in myList)
                        {
                            IsNotMatch = false;
                            var tempDuplicates = duplicates.FindAll(x => (x.SourceValue == item.SourceValue));
                            if (tempDuplicates.Any())
                            {
                                foreach (var tempDuplicate in tempDuplicates)
                                {
                                    if (item.TargetValue != tempDuplicate.TargetValue)
                                    {
                                        IsNotMatch = true;
                                    }
                                }
                            }
                            if (IsNotMatch)
                            {
                                filteredSources.Add(item.SourceValue);
                            }
                        }


                        if (filteredSources.Any())
                        {
                            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                            saveFileDialog1.FileName = "SourceValueDuplicates.txt";
                            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                            saveFileDialog1.FilterIndex = 2;
                            saveFileDialog1.RestoreDirectory = true;

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile());

                                foreach (string s in filteredSources)
                                {
                                    writer.WriteLine("<source>" + s + "</source>");
                                    duplicateValues++;
                                }
                                writer.Dispose();
                                writer.Close();
                            }
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
