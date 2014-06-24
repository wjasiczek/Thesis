using Roslyn.Compilers.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PracaInzynierska
{
    public partial class Form1 : Form
    {
        private PathForm path;
        private RoslynWorker roslynWorker;
        private string solutionPath;

        public Form1()
        {
            Directory.CreateDirectory("Dependency graphs");
            Directory.CreateDirectory("Dependency text");
            InitializeComponent();
        }

        private void buttonAddSolution_Click(object sender, EventArgs e)
        {
            DirectoryInfo downloadedMessageInfo = new DirectoryInfo("Dependency graphs");
            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }
            downloadedMessageInfo = new DirectoryInfo("Dependency text");
            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }
            path = new PathForm();
            path.ShowDialog();
            if (path.confirmed)
            {
                labelSolutionName.Text = Path.GetFileName(path.solutionPath);
                listBoxLog.Items.Clear();
                buttonFindLoops.Visible = true;
                buttonAllTextRepresentation.Visible = false;
                buttonGenerateAllGraphs.Visible = false;
                buttonShowCode.Visible = false;
                buttonShowGraph.Visible = false;
                buttonTextRepresentation.Visible = false;
                solutionPath = path.solutionPath;
            }
        }

        public class PathForm
        {
            private Form path;
            private Label pathLabel;
            private TextBox pathTextBox;
            private Button confirmButton;
            private Button browseButton;
            private OpenFileDialog browser;
            public String solutionPath { get; set; }
            public bool confirmed { get; set; }

            public PathForm()
            {
                path = new Form() { Width = 360, Height = 180, Text = "Add solution" };
                pathLabel = new Label() { Left = 20, Top = 13, Text = "Path to solution", Width = 125, Height = 17 };
                pathTextBox = new TextBox() { Left = 20, Top = 33, Width = 300, Height = 22 };
                confirmButton = new Button() { Text = "Confirm", Left = 220, Top = 70, Width = 100, Height = 30 };
                browseButton = new Button() { Text = "Browse", Left = 20, Top = 70, Width = 100, Height = 30 };
                browser = new OpenFileDialog();
                confirmed = false;
            }

            public void ShowDialog()
            {
                path.Load += new System.EventHandler(path_Load);
                browseButton.Click += new System.EventHandler(buttonBrowse_Click);
                confirmButton.Click += new System.EventHandler(buttonConfirm_Click);
                path.FormBorderStyle = FormBorderStyle.FixedSingle;
                path.Controls.Add(pathLabel);
                path.Controls.Add(pathTextBox);
                path.Controls.Add(confirmButton);
                path.Controls.Add(browseButton);
                path.ShowDialog(); 
            }

            private void path_Load(object sender, EventArgs e) { }

            private void buttonBrowse_Click(object sender, EventArgs e)
            {
                if (browser.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = browser.FileName;
                }
            }

            private void buttonConfirm_Click(object sender, EventArgs e)
            {
                if (File.Exists(pathTextBox.Text))
                {
                    if (Path.GetExtension(pathTextBox.Text) == ".sln")
                    {
                        solutionPath = pathTextBox.Text;
                        confirmed = true;
                        path.Close();
                    }
                    else
                    {
                        MessageBox.Show("File not viable");
                    }                  
                }
                else
                {
                    MessageBox.Show("File not found");
                }
            }
        }

        private void buttonFindLoops_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
            buttonShowCode.Visible = false;
            buttonShowGraph.Visible = false;
            buttonTextRepresentation.Visible = false;
            roslynWorker = new RoslynWorker(solutionPath);

            for (var i = 0; i < roslynWorker.ForLoops.Count(); i++)
            {
                String fileName = "ForLoop" + i.ToString();
                listBoxLog.Items.Add(fileName);
            }

            for (var i = 0; i < roslynWorker.WhileLoops.Count(); i++)
            {
                String fileName = "WhileLoop" + i.ToString();
                listBoxLog.Items.Add(fileName);
            }

            buttonGenerateAllGraphs.Visible = true;
            buttonAllTextRepresentation.Visible = true;
        }

        private void listBoxLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLog.SelectedIndex != -1)
            {
                buttonShowGraph.Visible = true;
                buttonShowCode.Visible = true;
                buttonTextRepresentation.Visible = true;
            }
        }

        private void buttonShowGraph_Click(object sender, EventArgs e)
        {
            int innerIterationCount = 0;
            int outerIterationCount = 0;
            String outerArgumentName = "";
            String innerArgumentName = "";
            bool isCutHorizontal = false;
            bool isCutVertical = false;
            List<KeyValuePair<String, String>> dependencies = new List<KeyValuePair<String, String>>();
            List<KeyValuePair<String, String>> leftDependencies = new List<KeyValuePair<String, String>>();

            var index = Int32.Parse(Regex.Match(listBoxLog.SelectedItem.ToString(), @"\d+").Value);
            roslynWorker.arrayExpressions = null;

            if (listBoxLog.SelectedItem.ToString().StartsWith("For"))
            {
                innerIterationCount = roslynWorker.GetInnerIterationCount(index, "For");
                if (innerIterationCount > 10)
                {
                    innerIterationCount = 10;
                    isCutHorizontal = true;
                }
                outerIterationCount = roslynWorker.GetOuterIterationCount(index, "For");
                if (outerIterationCount > 10)
                {
                    outerIterationCount = 10;
                    isCutVertical = true;
                }
                outerArgumentName = roslynWorker.GetOuterArgumentName(index, "For");
                innerArgumentName = roslynWorker.GetInnerArgumentName(index, "For");
                roslynWorker.SetArrayExpressions(index, "For");

                for (var i = 0; i < roslynWorker.arrayExpressions.Count(); i++)
                {
                    dependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetOuterIterationDependency(i),
                        roslynWorker.GetInnerIterationDependency(i)));
                    leftDependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetLeftOuterDependency(i),
                        roslynWorker.GetLeftInnerDependency(i)));
                }
            }

            if (listBoxLog.SelectedItem.ToString().StartsWith("While"))
            {
                innerIterationCount = roslynWorker.GetInnerIterationCount(index, "While");
                if (innerIterationCount > 10)
                {
                    innerIterationCount = 10;
                    isCutHorizontal = true;
                }
                outerIterationCount = roslynWorker.GetOuterIterationCount(index, "While");
                if (outerIterationCount > 10)
                {
                    outerIterationCount = 10;
                    isCutVertical = true;
                }
                outerArgumentName = roslynWorker.GetOuterArgumentName(index, "While");
                innerArgumentName = roslynWorker.GetInnerArgumentName(index, "While");
                roslynWorker.SetArrayExpressions(index, "While");

                for (var i = 0; i < roslynWorker.arrayExpressions.Count(); i++)
                {
                    dependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetOuterIterationDependency(i),
                        roslynWorker.GetInnerIterationDependency(i)));
                    leftDependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetLeftOuterDependency(i),
                        roslynWorker.GetLeftInnerDependency(i)));
                }
            }

            bool overlap = true;

            if (checkBoxNoOverlapping.Checked)
                overlap = false;

            bool isSmall = false;

            if (checkBoxSmallNodes.Checked)
                isSmall = true;

            GraphVizWorker.GenerateGraph(outerIterationCount, innerIterationCount, listBoxLog.SelectedItem.ToString(), dependencies,
                outerArgumentName, innerArgumentName, leftDependencies, isCutHorizontal, isCutVertical, overlap, isSmall);

            System.Diagnostics.Process.Start(@"Dependency graphs\" + listBoxLog.SelectedItem.ToString() + ".png");
        }

        private void buttonGenerateAllGraphs_Click(object sender, EventArgs e)
        {
            int innerIterationCount = 0;
            int outerIterationCount = 0;
            String outerArgumentName = "";
            String innerArgumentName = "";
            bool isCutHorizontal = false;
            bool isCutVertical = false;
            List<KeyValuePair<String, String>> dependencies = new List<KeyValuePair<String, String>>();
            List<KeyValuePair<String, String>> leftDependencies = new List<KeyValuePair<String, String>>();

            bool overlap = true;

            if (checkBoxNoOverlapping.Checked)
                overlap = false;

            bool isSmall = false;

            if (checkBoxSmallNodes.Checked)
                isSmall = true;

            for (var i = 0; i < roslynWorker.ForLoops.Count(); i++)
            {
                innerIterationCount = roslynWorker.GetInnerIterationCount(i, "For");
                if (innerIterationCount > 10)
                {
                    innerIterationCount = 10;
                    isCutHorizontal = true;
                }
                outerIterationCount = roslynWorker.GetOuterIterationCount(i, "For");
                if (outerIterationCount > 10)
                {
                    outerIterationCount = 10;
                    isCutVertical = true;
                }
                outerArgumentName = roslynWorker.GetOuterArgumentName(i, "For");
                innerArgumentName = roslynWorker.GetInnerArgumentName(i, "For");
                roslynWorker.SetArrayExpressions(i, "For");

                for (var j = 0; j < roslynWorker.arrayExpressions.Count(); j++)
                {
                    dependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetOuterIterationDependency(j),
                        roslynWorker.GetInnerIterationDependency(j)));
                    leftDependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetLeftOuterDependency(j),
                        roslynWorker.GetLeftInnerDependency(j)));
                }
                GraphVizWorker.GenerateGraph(outerIterationCount, innerIterationCount, "ForLoop" + i.ToString(), 
                    dependencies, outerArgumentName, innerArgumentName, leftDependencies, isCutHorizontal, isCutVertical, overlap, isSmall);
                dependencies.Clear();
                leftDependencies.Clear();
                isCutHorizontal = false;
                isCutVertical = false;
            }

            for (var i = 0; i < roslynWorker.WhileLoops.Count(); i++)
            {
                innerIterationCount = roslynWorker.GetInnerIterationCount(i, "While");
                if (innerIterationCount > 10)
                {
                    innerIterationCount = 10;
                    isCutHorizontal = true;
                }
                outerIterationCount = roslynWorker.GetOuterIterationCount(i, "While");
                if (outerIterationCount > 10)
                {
                    outerIterationCount = 10;
                    isCutVertical = true;
                }
                outerArgumentName = roslynWorker.GetOuterArgumentName(i, "While");
                innerArgumentName = roslynWorker.GetInnerArgumentName(i, "While");
                roslynWorker.SetArrayExpressions(i, "While");

                for (var j = 0; j < roslynWorker.arrayExpressions.Count(); j++)
                {
                    dependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetOuterIterationDependency(j),
                        roslynWorker.GetInnerIterationDependency(j)));
                    leftDependencies.Add(new KeyValuePair<String, String>(roslynWorker.GetLeftOuterDependency(j),
                        roslynWorker.GetLeftInnerDependency(j)));
                }

                GraphVizWorker.GenerateGraph(outerIterationCount, innerIterationCount, "WhileLoop" + i.ToString(), 
                    dependencies, outerArgumentName, innerArgumentName, leftDependencies, isCutHorizontal, isCutVertical, overlap, isSmall);
                dependencies.Clear();
                leftDependencies.Clear();
                isCutHorizontal = false;
                isCutVertical = false;
            }

            string resultMessage = "";

            if (roslynWorker.ForLoops.Count() > 0)
                resultMessage += "Generated " + roslynWorker.ForLoops.Count() + " \"for\" loop graphs\n";
            if (roslynWorker.WhileLoops.Count() > 0)
                resultMessage += "Generated " + roslynWorker.WhileLoops.Count() + " \"while\" loop graphs";

            MessageBox.Show(resultMessage);
        }

        private void buttonShowCode_Click(object sender, EventArgs e)
        {
            var index = 0;
            index = Int32.Parse(Regex.Match(listBoxLog.SelectedItem.ToString(), @"\d+").Value);

            using (TextWriter writer = File.CreateText(@"LoopCode.txt"))
            {
                if (listBoxLog.SelectedItem.ToString().StartsWith("For"))
                {
                    roslynWorker.ForLoops.ElementAt(index).Key.WriteTo(writer);
                }
                if (listBoxLog.SelectedItem.ToString().StartsWith("While"))
                {
                    roslynWorker.WhileLoops.ElementAt(index).Key.WriteTo(writer);
                }
            }

            System.Diagnostics.Process.Start(@"LoopCode.txt");
        }

        private void buttonTextRepresentation_Click(object sender, EventArgs e)
        {
            var index = Int32.Parse(Regex.Match(listBoxLog.SelectedItem.ToString(), @"\d+").Value);

            if (listBoxLog.SelectedItem.ToString().StartsWith("For"))
            {
                roslynWorker.SaveTextRepresentation(index, "For", listBoxLog.SelectedItem.ToString());
            }

            if (listBoxLog.SelectedItem.ToString().StartsWith("While"))
            {
                roslynWorker.SaveTextRepresentation(index, "While", listBoxLog.SelectedItem.ToString());
            }                   
           
            System.Diagnostics.Process.Start(@"Dependency text\" + listBoxLog.SelectedItem.ToString() + ".txt");
        }

        private void buttonAllTextRepresentation_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < roslynWorker.ForLoops.Count(); i++)
            {
                roslynWorker.SaveTextRepresentation(i, "For", "ForLoop" + i.ToString());
            }

            for (var i = 0; i < roslynWorker.WhileLoops.Count(); i++)
            {
                roslynWorker.SaveTextRepresentation(i, "While", "WhileLoop" + i.ToString());
            }

            string resultMessage = "";

            if (roslynWorker.ForLoops.Count() > 0)
                resultMessage += "Saved " + roslynWorker.ForLoops.Count() + " \"for\" loop text representations\n";
            if (roslynWorker.WhileLoops.Count() > 0)
                resultMessage += "Saved " + roslynWorker.WhileLoops.Count() + " \"while\" loop text representations";

            MessageBox.Show(resultMessage);
        }

        private void buttonGraphDir_Click(object sender, EventArgs e)
        {
            string path = @"Dependency graphs";
            System.Diagnostics.Process.Start(path);
        }

        private void buttonRepresentationDir_Click(object sender, EventArgs e)
        {
            string path = @"Dependency text";
            System.Diagnostics.Process.Start(path);
        }
    }
}
