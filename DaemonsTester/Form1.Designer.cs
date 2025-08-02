namespace DaemonsMCPTester
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      _initButton = new Button();
      _listProjectsButton = new Button();
      _projectNameTextBox = new TextBox();
      _pathTextBox = new TextBox();
      _listDirsButton = new Button();
      _listFilesButton = new Button();
      _getFileButton = new Button();
      splitContainer1 = new SplitContainer();
      _outputTextBox = new TextBox();
      lbFilter = new Label();
      _filterTextBox = new TextBox();
      pathLabel = new Label();
      projectLabel = new Label();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      SuspendLayout();
      // 
      // _initButton
      // 
      _initButton.Location = new Point(23, 19);
      _initButton.Name = "_initButton";
      _initButton.Size = new Size(106, 38);
      _initButton.TabIndex = 1;
      _initButton.Text = "Start";
      _initButton.Click += InitButton_Click;
      // 
      // _listProjectsButton
      // 
      _listProjectsButton.Location = new Point(300, 18);
      _listProjectsButton.Name = "_listProjectsButton";
      _listProjectsButton.Size = new Size(153, 41);
      _listProjectsButton.TabIndex = 2;
      _listProjectsButton.Text = "List Projects";
      _listProjectsButton.Click += ListProjectsButton_Click;
      // 
      // _projectNameTextBox
      // 
      _projectNameTextBox.Location = new Point(300, 73);
      _projectNameTextBox.Name = "_projectNameTextBox";
      _projectNameTextBox.Size = new Size(256, 23);
      _projectNameTextBox.TabIndex = 4;
      // 
      // _pathTextBox
      // 
      _pathTextBox.Location = new Point(300, 151);
      _pathTextBox.Name = "_pathTextBox";
      _pathTextBox.Size = new Size(655, 23);
      _pathTextBox.TabIndex = 6;
      // 
      // _listDirsButton
      // 
      _listDirsButton.Location = new Point(459, 19);
      _listDirsButton.Name = "_listDirsButton";
      _listDirsButton.Size = new Size(105, 38);
      _listDirsButton.TabIndex = 7;
      _listDirsButton.Text = "List Dirs";
      _listDirsButton.Click += ListDirsButton_Click;
      // 
      // _listFilesButton
      // 
      _listFilesButton.Location = new Point(570, 19);
      _listFilesButton.Name = "_listFilesButton";
      _listFilesButton.Size = new Size(106, 38);
      _listFilesButton.TabIndex = 8;
      _listFilesButton.Text = "List Files";
      _listFilesButton.Click += ListFilesButton_Click;
      // 
      // _getFileButton
      // 
      _getFileButton.Location = new Point(975, 142);
      _getFileButton.Name = "_getFileButton";
      _getFileButton.Size = new Size(92, 43);
      _getFileButton.TabIndex = 9;
      _getFileButton.Text = "Get File";
      _getFileButton.Click += GetFileButton_Click;
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(0, 0);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(_outputTextBox);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(lbFilter);
      splitContainer1.Panel2.Controls.Add(_filterTextBox);
      splitContainer1.Panel2.Controls.Add(pathLabel);
      splitContainer1.Panel2.Controls.Add(projectLabel);
      splitContainer1.Panel2.Controls.Add(_initButton);
      splitContainer1.Panel2.Controls.Add(_listProjectsButton);
      splitContainer1.Panel2.Controls.Add(_projectNameTextBox);
      splitContainer1.Panel2.Controls.Add(_pathTextBox);
      splitContainer1.Panel2.Controls.Add(_listDirsButton);
      splitContainer1.Panel2.Controls.Add(_listFilesButton);
      splitContainer1.Panel2.Controls.Add(_getFileButton);
      splitContainer1.Size = new Size(1244, 664);
      splitContainer1.SplitterDistance = 355;
      splitContainer1.TabIndex = 10;
      // 
      // _outputTextBox
      // 
      _outputTextBox.Dock = DockStyle.Fill;
      _outputTextBox.Location = new Point(0, 0);
      _outputTextBox.Multiline = true;
      _outputTextBox.Name = "_outputTextBox";
      _outputTextBox.ScrollBars = ScrollBars.Both;
      _outputTextBox.Size = new Size(1244, 355);
      _outputTextBox.TabIndex = 1;
      _outputTextBox.WordWrap = false;
      // 
      // lbFilter
      // 
      lbFilter.Location = new Point(221, 113);
      lbFilter.Name = "lbFilter";
      lbFilter.Size = new Size(73, 28);
      lbFilter.TabIndex = 13;
      lbFilter.Text = "Filter";
      // 
      // _filterTextBox
      // 
      _filterTextBox.Location = new Point(300, 110);
      _filterTextBox.Name = "_filterTextBox";
      _filterTextBox.Size = new Size(256, 23);
      _filterTextBox.TabIndex = 12;
      // 
      // pathLabel
      // 
      pathLabel.Location = new Point(230, 151);
      pathLabel.Name = "pathLabel";
      pathLabel.Size = new Size(55, 31);
      pathLabel.TabIndex = 11;
      pathLabel.Text = "Path";
      // 
      // projectLabel
      // 
      projectLabel.Location = new Point(221, 76);
      projectLabel.Name = "projectLabel";
      projectLabel.Size = new Size(73, 28);
      projectLabel.TabIndex = 10;
      projectLabel.Text = "Project";
      // 
      // Form1
      // 
      ClientSize = new Size(1244, 664);
      Controls.Add(splitContainer1);
      Name = "Form1";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "MCP Client Test";
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel1.PerformLayout();
      splitContainer1.Panel2.ResumeLayout(false);
      splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private MCPClient _mcpClient;
        private Button _initButton;
        private Button _listProjectsButton;
        private Button _listDirsButton;
        private Button _listFilesButton;
        private Button _getFileButton;
        private TextBox _projectNameTextBox;
        private TextBox _pathTextBox;
        private SplitContainer splitContainer1;
        private TextBox _outputTextBox;
        private Label pathLabel;
        private Label projectLabel;
        private Label lbFilter;
        private TextBox _filterTextBox;
    }
}
