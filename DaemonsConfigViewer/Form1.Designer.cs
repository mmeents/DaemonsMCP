namespace DaemonsConfigViewer {
  partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        if (_appConfig != null) {
          _appConfig.OnProjectsLoadedEvent -= ProjectsReloaded;
        }
        if (_nodesRepo != null) {
          _nodesRepo.OnNodesLoadedEvent -= NodesChangedNeedsReload;
        }
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
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      ScRoot = new SplitContainer();
      tabControl1 = new TabControl();
      TpConfig = new TabPage();
      splitContainer1 = new SplitContainer();
      tvMain = new TreeView();
      contextMenuStrip1 = new ContextMenuStrip(components);
      addProjectToolStripMenuItem = new ToolStripMenuItem();
      RemoveProjectMenuItem = new ToolStripMenuItem();
      AddItemMenuItem = new ToolStripMenuItem();
      MoveItemUpMenuItem = new ToolStripMenuItem();
      RemoveItemMenuItem = new ToolStripMenuItem();
      imageList1 = new ImageList(components);
      tabControl2 = new TabControl();
      tpSettings = new TabPage();
      lbLogPath = new LinkLabel();
      lbPath = new LinkLabel();
      btnSettingsCancel = new Button();
      btnSettingsOk = new Button();
      label10 = new Label();
      edBlockedFolders = new TextBox();
      label9 = new Label();
      edBlockedFiles = new TextBox();
      label8 = new Label();
      edBlockedExt = new TextBox();
      label7 = new Label();
      edAllowedExt = new TextBox();
      label6 = new Label();
      label5 = new Label();
      label4 = new Label();
      edMaxWriteSize = new TextBox();
      label3 = new Label();
      lbAppName = new Label();
      cbAllowWrite = new CheckBox();
      edMaxFileSize = new TextBox();
      tpIndex = new TabPage();
      lbIndexLine2 = new Label();
      lbIndexLine1 = new Label();
      lbIndexHeader = new Label();
      panel1 = new Panel();
      lbProjectFolder = new LinkLabel();
      label1 = new Label();
      TpNodes = new TabPage();
      splitContainer2 = new SplitContainer();
      tvNodes = new TreeView();
      tabControl4 = new TabControl();
      panel2 = new Panel();
      tabControl3 = new TabControl();
      ((System.ComponentModel.ISupportInitialize)ScRoot).BeginInit();
      ScRoot.Panel1.SuspendLayout();
      ScRoot.Panel2.SuspendLayout();
      ScRoot.SuspendLayout();
      tabControl1.SuspendLayout();
      TpConfig.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      contextMenuStrip1.SuspendLayout();
      tabControl2.SuspendLayout();
      tpSettings.SuspendLayout();
      tpIndex.SuspendLayout();
      panel1.SuspendLayout();
      TpNodes.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      SuspendLayout();
      // 
      // ScRoot
      // 
      ScRoot.Dock = DockStyle.Fill;
      ScRoot.Location = new Point(0, 0);
      ScRoot.Name = "ScRoot";
      ScRoot.Orientation = Orientation.Horizontal;
      // 
      // ScRoot.Panel1
      // 
      ScRoot.Panel1.Controls.Add(tabControl1);
      // 
      // ScRoot.Panel2
      // 
      ScRoot.Panel2.Controls.Add(tabControl3);
      ScRoot.Size = new Size(706, 646);
      ScRoot.SplitterDistance = 522;
      ScRoot.TabIndex = 0;
      // 
      // tabControl1
      // 
      tabControl1.Alignment = TabAlignment.Bottom;
      tabControl1.Controls.Add(TpConfig);
      tabControl1.Controls.Add(TpNodes);
      tabControl1.Dock = DockStyle.Fill;
      tabControl1.Location = new Point(0, 0);
      tabControl1.Multiline = true;
      tabControl1.Name = "tabControl1";
      tabControl1.SelectedIndex = 0;
      tabControl1.Size = new Size(706, 522);
      tabControl1.TabIndex = 0;
      tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
      // 
      // TpConfig
      // 
      TpConfig.Controls.Add(splitContainer1);
      TpConfig.Controls.Add(panel1);
      TpConfig.Location = new Point(4, 4);
      TpConfig.Name = "TpConfig";
      TpConfig.Padding = new Padding(3);
      TpConfig.Size = new Size(698, 494);
      TpConfig.TabIndex = 0;
      TpConfig.Text = "Projects And Settings";
      TpConfig.UseVisualStyleBackColor = true;
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(3, 69);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(tvMain);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(tabControl2);
      splitContainer1.Size = new Size(692, 422);
      splitContainer1.SplitterDistance = 264;
      splitContainer1.TabIndex = 1;
      // 
      // tvMain
      // 
      tvMain.ContextMenuStrip = contextMenuStrip1;
      tvMain.Dock = DockStyle.Fill;
      tvMain.ImageIndex = 0;
      tvMain.ImageList = imageList1;
      tvMain.Location = new Point(0, 0);
      tvMain.Name = "tvMain";
      tvMain.SelectedImageIndex = 0;
      tvMain.Size = new Size(264, 422);
      tvMain.TabIndex = 0;
      tvMain.AfterSelect += tvMain_AfterSelect;
      // 
      // contextMenuStrip1
      // 
      contextMenuStrip1.Items.AddRange(new ToolStripItem[] { addProjectToolStripMenuItem, RemoveProjectMenuItem, AddItemMenuItem, MoveItemUpMenuItem, RemoveItemMenuItem });
      contextMenuStrip1.Name = "contextMenuStrip1";
      contextMenuStrip1.Size = new Size(158, 114);
      contextMenuStrip1.Opening += contextMenuStrip1_Opening;
      // 
      // addProjectToolStripMenuItem
      // 
      addProjectToolStripMenuItem.Name = "addProjectToolStripMenuItem";
      addProjectToolStripMenuItem.Size = new Size(157, 22);
      addProjectToolStripMenuItem.Text = "Add Project";
      addProjectToolStripMenuItem.Click += addProjectToolStripMenuItem_Click;
      // 
      // RemoveProjectMenuItem
      // 
      RemoveProjectMenuItem.Name = "RemoveProjectMenuItem";
      RemoveProjectMenuItem.Size = new Size(157, 22);
      RemoveProjectMenuItem.Text = "Remove Project";
      RemoveProjectMenuItem.Click += RemoveProjectMenuItem_Click;
      // 
      // AddItemMenuItem
      // 
      AddItemMenuItem.Name = "AddItemMenuItem";
      AddItemMenuItem.Size = new Size(157, 22);
      AddItemMenuItem.Text = "Add Item";
      AddItemMenuItem.Click += AddItemMenuItem_Click;
      // 
      // MoveItemUpMenuItem
      // 
      MoveItemUpMenuItem.Name = "MoveItemUpMenuItem";
      MoveItemUpMenuItem.Size = new Size(157, 22);
      MoveItemUpMenuItem.Text = "Move Item Up";
      MoveItemUpMenuItem.Click += MoveItemUpMenuItem_Click;
      // 
      // RemoveItemMenuItem
      // 
      RemoveItemMenuItem.Name = "RemoveItemMenuItem";
      RemoveItemMenuItem.Size = new Size(157, 22);
      RemoveItemMenuItem.Text = "Remove Item";
      RemoveItemMenuItem.Click += RemoveItemMenuItem_Click;
      // 
      // imageList1
      // 
      imageList1.ColorDepth = ColorDepth.Depth32Bit;
      imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
      imageList1.TransparentColor = Color.Transparent;
      imageList1.Images.SetKeyName(0, "doc-star-in.png");
      imageList1.Images.SetKeyName(1, "page.png");
      // 
      // tabControl2
      // 
      tabControl2.Alignment = TabAlignment.Bottom;
      tabControl2.Controls.Add(tpSettings);
      tabControl2.Controls.Add(tpIndex);
      tabControl2.Dock = DockStyle.Fill;
      tabControl2.Location = new Point(0, 0);
      tabControl2.Name = "tabControl2";
      tabControl2.SelectedIndex = 0;
      tabControl2.Size = new Size(424, 422);
      tabControl2.TabIndex = 0;
      tabControl2.SelectedIndexChanged += tabControl2_SelectedIndexChanged;
      // 
      // tpSettings
      // 
      tpSettings.BackColor = SystemColors.Control;
      tpSettings.Controls.Add(lbLogPath);
      tpSettings.Controls.Add(lbPath);
      tpSettings.Controls.Add(btnSettingsCancel);
      tpSettings.Controls.Add(btnSettingsOk);
      tpSettings.Controls.Add(label10);
      tpSettings.Controls.Add(edBlockedFolders);
      tpSettings.Controls.Add(label9);
      tpSettings.Controls.Add(edBlockedFiles);
      tpSettings.Controls.Add(label8);
      tpSettings.Controls.Add(edBlockedExt);
      tpSettings.Controls.Add(label7);
      tpSettings.Controls.Add(edAllowedExt);
      tpSettings.Controls.Add(label6);
      tpSettings.Controls.Add(label5);
      tpSettings.Controls.Add(label4);
      tpSettings.Controls.Add(edMaxWriteSize);
      tpSettings.Controls.Add(label3);
      tpSettings.Controls.Add(lbAppName);
      tpSettings.Controls.Add(cbAllowWrite);
      tpSettings.Controls.Add(edMaxFileSize);
      tpSettings.Location = new Point(4, 4);
      tpSettings.Name = "tpSettings";
      tpSettings.Padding = new Padding(3);
      tpSettings.Size = new Size(416, 394);
      tpSettings.TabIndex = 0;
      tpSettings.Text = "Settings";
      // 
      // lbLogPath
      // 
      lbLogPath.AutoSize = true;
      lbLogPath.Location = new Point(17, 51);
      lbLogPath.Name = "lbLogPath";
      lbLogPath.Size = new Size(230, 15);
      lbLogPath.TabIndex = 19;
      lbLogPath.TabStop = true;
      lbLogPath.Text = "LogFolder C:\\ProgramData\\DaemonsMCP";
      lbLogPath.Click += lbLogPath_Click;
      // 
      // lbPath
      // 
      lbPath.AutoSize = true;
      lbPath.Location = new Point(17, 31);
      lbPath.Name = "lbPath";
      lbPath.Size = new Size(232, 15);
      lbPath.TabIndex = 18;
      lbPath.TabStop = true;
      lbPath.Text = "AppFolder C:\\ProgramData\\DaemonsMCP";
      lbPath.Click += linkLabel1_Click;
      // 
      // btnSettingsCancel
      // 
      btnSettingsCancel.Location = new Point(185, 287);
      btnSettingsCancel.Name = "btnSettingsCancel";
      btnSettingsCancel.Size = new Size(75, 23);
      btnSettingsCancel.TabIndex = 17;
      btnSettingsCancel.Text = "Cancel";
      btnSettingsCancel.UseVisualStyleBackColor = true;
      btnSettingsCancel.Click += btnSettingsCancel_Click;
      // 
      // btnSettingsOk
      // 
      btnSettingsOk.Location = new Point(104, 287);
      btnSettingsOk.Name = "btnSettingsOk";
      btnSettingsOk.Size = new Size(75, 23);
      btnSettingsOk.TabIndex = 16;
      btnSettingsOk.Text = "Ok";
      btnSettingsOk.UseVisualStyleBackColor = true;
      btnSettingsOk.Click += btnSettingsOk_Click;
      // 
      // label10
      // 
      label10.AutoSize = true;
      label10.Location = new Point(8, 261);
      label10.Name = "label10";
      label10.Size = new Size(90, 15);
      label10.TabIndex = 15;
      label10.Text = "Blocked Folders";
      // 
      // edBlockedFolders
      // 
      edBlockedFolders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edBlockedFolders.Location = new Point(104, 258);
      edBlockedFolders.Name = "edBlockedFolders";
      edBlockedFolders.Size = new Size(306, 23);
      edBlockedFolders.TabIndex = 14;
      edBlockedFolders.TextChanged += edBlockedFolders_TextChanged;
      // 
      // label9
      // 
      label9.AutoSize = true;
      label9.Location = new Point(15, 232);
      label9.Name = "label9";
      label9.Size = new Size(75, 15);
      label9.TabIndex = 13;
      label9.Text = "Blocked Files";
      // 
      // edBlockedFiles
      // 
      edBlockedFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edBlockedFiles.Location = new Point(104, 229);
      edBlockedFiles.Name = "edBlockedFiles";
      edBlockedFiles.Size = new Size(306, 23);
      edBlockedFiles.TabIndex = 12;
      edBlockedFiles.TextChanged += edBlockedFiles_TextChanged;
      // 
      // label8
      // 
      label8.AutoSize = true;
      label8.Location = new Point(15, 203);
      label8.Name = "label8";
      label8.Size = new Size(68, 15);
      label8.TabIndex = 11;
      label8.Text = "Blocked Ext";
      // 
      // edBlockedExt
      // 
      edBlockedExt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edBlockedExt.Location = new Point(104, 200);
      edBlockedExt.Name = "edBlockedExt";
      edBlockedExt.Size = new Size(306, 23);
      edBlockedExt.TabIndex = 10;
      edBlockedExt.TextChanged += edBlockedExt_TextChanged;
      // 
      // label7
      // 
      label7.AutoSize = true;
      label7.Location = new Point(15, 174);
      label7.Name = "label7";
      label7.Size = new Size(69, 15);
      label7.TabIndex = 9;
      label7.Text = "Allowed Ext";
      // 
      // edAllowedExt
      // 
      edAllowedExt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      edAllowedExt.Location = new Point(104, 171);
      edAllowedExt.Name = "edAllowedExt";
      edAllowedExt.Size = new Size(306, 23);
      edAllowedExt.TabIndex = 8;
      edAllowedExt.TextChanged += edAllowedExt_TextChanged;
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.Location = new Point(210, 137);
      label6.Name = "label6";
      label6.Size = new Size(38, 15);
      label6.TabIndex = 7;
      label6.Text = "label6";
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(211, 81);
      label5.Name = "label5";
      label5.Size = new Size(38, 15);
      label5.TabIndex = 6;
      label5.Text = "label5";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(15, 136);
      label4.Name = "label4";
      label4.Size = new Size(84, 15);
      label4.TabIndex = 5;
      label4.Text = "Max Write Size";
      // 
      // edMaxWriteSize
      // 
      edMaxWriteSize.Location = new Point(104, 133);
      edMaxWriteSize.Name = "edMaxWriteSize";
      edMaxWriteSize.Size = new Size(100, 23);
      edMaxWriteSize.TabIndex = 4;
      edMaxWriteSize.TextChanged += edMaxWriteSize_TextChanged;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(15, 82);
      label3.Name = "label3";
      label3.Size = new Size(74, 15);
      label3.TabIndex = 3;
      label3.Text = "Max File Size";
      // 
      // lbAppName
      // 
      lbAppName.AutoSize = true;
      lbAppName.Location = new Point(11, 12);
      lbAppName.Name = "lbAppName";
      lbAppName.Size = new Size(162, 15);
      lbAppName.TabIndex = 2;
      lbAppName.Text = "Security Settings Read Access";
      // 
      // cbAllowWrite
      // 
      cbAllowWrite.AutoSize = true;
      cbAllowWrite.Location = new Point(15, 108);
      cbAllowWrite.Name = "cbAllowWrite";
      cbAllowWrite.Size = new Size(119, 19);
      cbAllowWrite.TabIndex = 1;
      cbAllowWrite.Text = "Allow File Writing";
      cbAllowWrite.UseVisualStyleBackColor = true;
      cbAllowWrite.CheckedChanged += cbAllowWrite_CheckedChanged;
      // 
      // edMaxFileSize
      // 
      edMaxFileSize.Location = new Point(104, 79);
      edMaxFileSize.Name = "edMaxFileSize";
      edMaxFileSize.Size = new Size(100, 23);
      edMaxFileSize.TabIndex = 0;
      edMaxFileSize.TextChanged += edMaxFileSize_TextChanged;
      // 
      // tpIndex
      // 
      tpIndex.Controls.Add(lbIndexLine2);
      tpIndex.Controls.Add(lbIndexLine1);
      tpIndex.Controls.Add(lbIndexHeader);
      tpIndex.Location = new Point(4, 4);
      tpIndex.Name = "tpIndex";
      tpIndex.Padding = new Padding(3);
      tpIndex.Size = new Size(416, 394);
      tpIndex.TabIndex = 1;
      tpIndex.Text = "Index";
      tpIndex.UseVisualStyleBackColor = true;
      // 
      // lbIndexLine2
      // 
      lbIndexLine2.AutoSize = true;
      lbIndexLine2.Location = new Point(36, 62);
      lbIndexLine2.Name = "lbIndexLine2";
      lbIndexLine2.Size = new Size(74, 15);
      lbIndexLine2.TabIndex = 2;
      lbIndexLine2.Text = "lbIndexLine2";
      // 
      // lbIndexLine1
      // 
      lbIndexLine1.AutoSize = true;
      lbIndexLine1.Location = new Point(36, 37);
      lbIndexLine1.Name = "lbIndexLine1";
      lbIndexLine1.Size = new Size(74, 15);
      lbIndexLine1.TabIndex = 1;
      lbIndexLine1.Text = "lbIndexLine1";
      // 
      // lbIndexHeader
      // 
      lbIndexHeader.AutoSize = true;
      lbIndexHeader.Font = new Font("Segoe UI", 12F);
      lbIndexHeader.Location = new Point(6, 3);
      lbIndexHeader.Name = "lbIndexHeader";
      lbIndexHeader.Size = new Size(52, 21);
      lbIndexHeader.TabIndex = 0;
      lbIndexHeader.Text = "label2";
      // 
      // panel1
      // 
      panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      panel1.Controls.Add(lbProjectFolder);
      panel1.Controls.Add(label1);
      panel1.Dock = DockStyle.Top;
      panel1.Location = new Point(3, 3);
      panel1.Name = "panel1";
      panel1.Size = new Size(692, 66);
      panel1.TabIndex = 0;
      // 
      // lbProjectFolder
      // 
      lbProjectFolder.AutoSize = true;
      lbProjectFolder.Location = new Point(22, 32);
      lbProjectFolder.Name = "lbProjectFolder";
      lbProjectFolder.Size = new Size(60, 15);
      lbProjectFolder.TabIndex = 4;
      lbProjectFolder.TabStop = true;
      lbProjectFolder.Text = "linkLabel1";
      lbProjectFolder.LinkClicked += lbProjectFolder_LinkClicked;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
      label1.Location = new Point(5, 2);
      label1.Name = "label1";
      label1.Size = new Size(443, 21);
      label1.TabIndex = 0;
      label1.Text = "Config file: C:\\ProgramData\\DaemonsMCP\\daemonsmcp.ptcfg";
      // 
      // TpNodes
      // 
      TpNodes.Controls.Add(splitContainer2);
      TpNodes.Controls.Add(panel2);
      TpNodes.Location = new Point(4, 4);
      TpNodes.Name = "TpNodes";
      TpNodes.Padding = new Padding(3);
      TpNodes.Size = new Size(698, 494);
      TpNodes.TabIndex = 1;
      TpNodes.Text = "Nodes Trees";
      TpNodes.UseVisualStyleBackColor = true;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = DockStyle.Fill;
      splitContainer2.Location = new Point(3, 54);
      splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(tvNodes);
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(tabControl4);
      splitContainer2.Size = new Size(692, 437);
      splitContainer2.SplitterDistance = 294;
      splitContainer2.TabIndex = 1;
      // 
      // tvNodes
      // 
      tvNodes.AllowDrop = true;
      tvNodes.ContextMenuStrip = contextMenuStrip1;
      tvNodes.Dock = DockStyle.Fill;
      tvNodes.Location = new Point(0, 0);
      tvNodes.Name = "tvNodes";
      tvNodes.Size = new Size(294, 437);
      tvNodes.TabIndex = 0;
      tvNodes.ItemDrag += tvNodes_ItemDrag;
      tvNodes.AfterSelect += tvNodes_AfterSelect;
      tvNodes.DragDrop += tvNodes_DragDrop;
      tvNodes.DragEnter += tvNodes_DragEnter;
      tvNodes.DragOver += tvNodes_DragOver;
      // 
      // tabControl4
      // 
      tabControl4.Alignment = TabAlignment.Bottom;
      tabControl4.Dock = DockStyle.Fill;
      tabControl4.Location = new Point(0, 0);
      tabControl4.Name = "tabControl4";
      tabControl4.SelectedIndex = 0;
      tabControl4.Size = new Size(394, 437);
      tabControl4.TabIndex = 0;
      // 
      // panel2
      // 
      panel2.Dock = DockStyle.Top;
      panel2.Location = new Point(3, 3);
      panel2.Name = "panel2";
      panel2.Size = new Size(692, 51);
      panel2.TabIndex = 0;
      // 
      // tabControl3
      // 
      tabControl3.Alignment = TabAlignment.Bottom;
      tabControl3.Dock = DockStyle.Fill;
      tabControl3.Location = new Point(0, 0);
      tabControl3.Name = "tabControl3";
      tabControl3.SelectedIndex = 0;
      tabControl3.Size = new Size(706, 120);
      tabControl3.TabIndex = 0;
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(706, 646);
      Controls.Add(ScRoot);
      Name = "Form1";
      Text = "DaemonsMCP Config";
      FormClosed += Form1_FormClosed;
      Shown += Form1_Shown;
      ScRoot.Panel1.ResumeLayout(false);
      ScRoot.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)ScRoot).EndInit();
      ScRoot.ResumeLayout(false);
      tabControl1.ResumeLayout(false);
      TpConfig.ResumeLayout(false);
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      contextMenuStrip1.ResumeLayout(false);
      tabControl2.ResumeLayout(false);
      tpSettings.ResumeLayout(false);
      tpSettings.PerformLayout();
      tpIndex.ResumeLayout(false);
      tpIndex.PerformLayout();
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      TpNodes.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
      splitContainer2.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private SplitContainer ScRoot;
    private TabControl tabControl1;
    private TabPage TpConfig;
    private Panel panel1;
    private Label label1;
    private SplitContainer splitContainer1;
    private TreeView tvMain;
    private TabControl tabControl2;
    private TabControl tabControl3;
    private ImageList imageList1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem addProjectToolStripMenuItem;
    private TabPage tpSettings;
    private TextBox edMaxFileSize;
    private Label lbAppName;
    private CheckBox cbAllowWrite;
    private Label label4;
    private TextBox edMaxWriteSize;
    private Label label3;
    private Label label10;
    private TextBox edBlockedFolders;
    private Label label9;
    private TextBox edBlockedFiles;
    private Label label8;
    private TextBox edBlockedExt;
    private Label label7;
    private TextBox edAllowedExt;
    private Label label6;
    private Label label5;
    private Button btnSettingsCancel;
    private Button btnSettingsOk;
    private TabPage TpNodes;
    private Panel panel2;
    private SplitContainer splitContainer2;
    private TreeView tvNodes;
    private TabControl tabControl4;
    private LinkLabel lbPath;
    private LinkLabel lbLogPath;
    private ToolStripMenuItem RemoveProjectMenuItem;
    private ToolStripMenuItem AddItemMenuItem;
    private ToolStripMenuItem MoveItemUpMenuItem;
    private ToolStripMenuItem RemoveItemMenuItem;
    private TabPage tpIndex;
    private Label lbIndexHeader;
    private Label lbIndexLine2;
    private Label lbIndexLine1;
    private LinkLabel lbProjectFolder;
  }
}
