namespace PracaInzynierska
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonAddSolution = new System.Windows.Forms.Button();
            this.labelSolutionName = new System.Windows.Forms.Label();
            this.buttonFindLoops = new System.Windows.Forms.Button();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.buttonShowGraph = new System.Windows.Forms.Button();
            this.buttonShowCode = new System.Windows.Forms.Button();
            this.buttonGenerateAllGraphs = new System.Windows.Forms.Button();
            this.buttonTextRepresentation = new System.Windows.Forms.Button();
            this.buttonAllTextRepresentation = new System.Windows.Forms.Button();
            this.buttonGraphDir = new System.Windows.Forms.Button();
            this.buttonRepresentationDir = new System.Windows.Forms.Button();
            this.checkBoxNoOverlapping = new System.Windows.Forms.CheckBox();
            this.checkBoxSmallNodes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonAddSolution
            // 
            this.buttonAddSolution.Location = new System.Drawing.Point(12, 88);
            this.buttonAddSolution.Name = "buttonAddSolution";
            this.buttonAddSolution.Size = new System.Drawing.Size(120, 50);
            this.buttonAddSolution.TabIndex = 0;
            this.buttonAddSolution.Text = "Add solution";
            this.buttonAddSolution.UseVisualStyleBackColor = true;
            this.buttonAddSolution.Click += new System.EventHandler(this.buttonAddSolution_Click);
            // 
            // labelSolutionName
            // 
            this.labelSolutionName.Location = new System.Drawing.Point(140, 38);
            this.labelSolutionName.Name = "labelSolutionName";
            this.labelSolutionName.Size = new System.Drawing.Size(237, 37);
            this.labelSolutionName.TabIndex = 1;
            this.labelSolutionName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonFindLoops
            // 
            this.buttonFindLoops.Location = new System.Drawing.Point(12, 144);
            this.buttonFindLoops.Name = "buttonFindLoops";
            this.buttonFindLoops.Size = new System.Drawing.Size(120, 50);
            this.buttonFindLoops.TabIndex = 2;
            this.buttonFindLoops.Text = "Find loops";
            this.buttonFindLoops.UseVisualStyleBackColor = true;
            this.buttonFindLoops.Visible = false;
            this.buttonFindLoops.Click += new System.EventHandler(this.buttonFindLoops_Click);
            // 
            // listBoxLog
            // 
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.ItemHeight = 16;
            this.listBoxLog.Location = new System.Drawing.Point(143, 88);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(234, 340);
            this.listBoxLog.TabIndex = 4;
            this.listBoxLog.SelectedIndexChanged += new System.EventHandler(this.listBoxLog_SelectedIndexChanged);
            // 
            // buttonShowGraph
            // 
            this.buttonShowGraph.Location = new System.Drawing.Point(383, 144);
            this.buttonShowGraph.Name = "buttonShowGraph";
            this.buttonShowGraph.Size = new System.Drawing.Size(120, 50);
            this.buttonShowGraph.TabIndex = 5;
            this.buttonShowGraph.Text = "Show graph";
            this.buttonShowGraph.UseVisualStyleBackColor = true;
            this.buttonShowGraph.Visible = false;
            this.buttonShowGraph.Click += new System.EventHandler(this.buttonShowGraph_Click);
            // 
            // buttonShowCode
            // 
            this.buttonShowCode.Location = new System.Drawing.Point(12, 312);
            this.buttonShowCode.Name = "buttonShowCode";
            this.buttonShowCode.Size = new System.Drawing.Size(120, 50);
            this.buttonShowCode.TabIndex = 6;
            this.buttonShowCode.Text = "Show code";
            this.buttonShowCode.UseVisualStyleBackColor = true;
            this.buttonShowCode.Visible = false;
            this.buttonShowCode.Click += new System.EventHandler(this.buttonShowCode_Click);
            // 
            // buttonGenerateAllGraphs
            // 
            this.buttonGenerateAllGraphs.Location = new System.Drawing.Point(383, 88);
            this.buttonGenerateAllGraphs.Name = "buttonGenerateAllGraphs";
            this.buttonGenerateAllGraphs.Size = new System.Drawing.Size(120, 50);
            this.buttonGenerateAllGraphs.TabIndex = 7;
            this.buttonGenerateAllGraphs.Text = "Generate all graphs";
            this.buttonGenerateAllGraphs.UseVisualStyleBackColor = true;
            this.buttonGenerateAllGraphs.Visible = false;
            this.buttonGenerateAllGraphs.Click += new System.EventHandler(this.buttonGenerateAllGraphs_Click);
            // 
            // buttonTextRepresentation
            // 
            this.buttonTextRepresentation.Location = new System.Drawing.Point(12, 256);
            this.buttonTextRepresentation.Name = "buttonTextRepresentation";
            this.buttonTextRepresentation.Size = new System.Drawing.Size(120, 50);
            this.buttonTextRepresentation.TabIndex = 8;
            this.buttonTextRepresentation.Text = "Show text representation";
            this.buttonTextRepresentation.UseVisualStyleBackColor = true;
            this.buttonTextRepresentation.Visible = false;
            this.buttonTextRepresentation.Click += new System.EventHandler(this.buttonTextRepresentation_Click);
            // 
            // buttonAllTextRepresentation
            // 
            this.buttonAllTextRepresentation.Location = new System.Drawing.Point(12, 200);
            this.buttonAllTextRepresentation.Name = "buttonAllTextRepresentation";
            this.buttonAllTextRepresentation.Size = new System.Drawing.Size(120, 50);
            this.buttonAllTextRepresentation.TabIndex = 9;
            this.buttonAllTextRepresentation.Text = "Save all text representations";
            this.buttonAllTextRepresentation.UseVisualStyleBackColor = true;
            this.buttonAllTextRepresentation.Visible = false;
            this.buttonAllTextRepresentation.Click += new System.EventHandler(this.buttonAllTextRepresentation_Click);
            // 
            // buttonGraphDir
            // 
            this.buttonGraphDir.Location = new System.Drawing.Point(382, 322);
            this.buttonGraphDir.Name = "buttonGraphDir";
            this.buttonGraphDir.Size = new System.Drawing.Size(120, 30);
            this.buttonGraphDir.TabIndex = 10;
            this.buttonGraphDir.Text = "Open graph dir";
            this.buttonGraphDir.UseVisualStyleBackColor = true;
            this.buttonGraphDir.Click += new System.EventHandler(this.buttonGraphDir_Click);
            // 
            // buttonRepresentationDir
            // 
            this.buttonRepresentationDir.Location = new System.Drawing.Point(382, 358);
            this.buttonRepresentationDir.Name = "buttonRepresentationDir";
            this.buttonRepresentationDir.Size = new System.Drawing.Size(120, 70);
            this.buttonRepresentationDir.TabIndex = 11;
            this.buttonRepresentationDir.Text = "Open text representation dir";
            this.buttonRepresentationDir.UseVisualStyleBackColor = true;
            this.buttonRepresentationDir.Click += new System.EventHandler(this.buttonRepresentationDir_Click);
            // 
            // checkBoxNoOverlapping
            // 
            this.checkBoxNoOverlapping.AutoSize = true;
            this.checkBoxNoOverlapping.Location = new System.Drawing.Point(383, 229);
            this.checkBoxNoOverlapping.Name = "checkBoxNoOverlapping";
            this.checkBoxNoOverlapping.Size = new System.Drawing.Size(126, 21);
            this.checkBoxNoOverlapping.TabIndex = 12;
            this.checkBoxNoOverlapping.Text = "No overlapping";
            this.checkBoxNoOverlapping.UseVisualStyleBackColor = true;
            // 
            // checkBoxSmallNodes
            // 
            this.checkBoxSmallNodes.AutoSize = true;
            this.checkBoxSmallNodes.Location = new System.Drawing.Point(383, 256);
            this.checkBoxSmallNodes.Name = "checkBoxSmallNodes";
            this.checkBoxSmallNodes.Size = new System.Drawing.Size(107, 21);
            this.checkBoxSmallNodes.TabIndex = 13;
            this.checkBoxSmallNodes.Text = "Small nodes";
            this.checkBoxSmallNodes.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 442);
            this.Controls.Add(this.checkBoxSmallNodes);
            this.Controls.Add(this.checkBoxNoOverlapping);
            this.Controls.Add(this.buttonRepresentationDir);
            this.Controls.Add(this.buttonGraphDir);
            this.Controls.Add(this.buttonAllTextRepresentation);
            this.Controls.Add(this.buttonTextRepresentation);
            this.Controls.Add(this.buttonGenerateAllGraphs);
            this.Controls.Add(this.buttonShowCode);
            this.Controls.Add(this.buttonShowGraph);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.buttonFindLoops);
            this.Controls.Add(this.labelSolutionName);
            this.Controls.Add(this.buttonAddSolution);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Dependencies visualizer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddSolution;
        private System.Windows.Forms.Label labelSolutionName;
        private System.Windows.Forms.Button buttonFindLoops;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Button buttonShowGraph;
        private System.Windows.Forms.Button buttonShowCode;
        private System.Windows.Forms.Button buttonGenerateAllGraphs;
        private System.Windows.Forms.Button buttonTextRepresentation;
        private System.Windows.Forms.Button buttonAllTextRepresentation;
        private System.Windows.Forms.Button buttonGraphDir;
        private System.Windows.Forms.Button buttonRepresentationDir;
        private System.Windows.Forms.CheckBox checkBoxNoOverlapping;
        private System.Windows.Forms.CheckBox checkBoxSmallNodes;
    }
}

