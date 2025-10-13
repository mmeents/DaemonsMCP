namespace PackedTableTabs.PropEditors {
  partial class FilePickerPropertyEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      textBox1 = new TextBox();
      lbName = new Label();
      btnBrowse = new Button();
      SuspendLayout();
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(124, 3);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(223, 23);
      textBox1.TabIndex = 0;
      textBox1.TextChanged += TextBox1_TextChanged;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(6, 6);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // btnBrowse
      // 
      btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnBrowse.Location = new Point(348, 2);
      btnBrowse.Name = "btnBrowse";
      btnBrowse.Size = new Size(24, 24);
      btnBrowse.TabIndex = 2;
      btnBrowse.Text = "📁";
      btnBrowse.UseVisualStyleBackColor = true;
      btnBrowse.Click += BtnBrowse_Click;
      // 
      // FilePickerPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(btnBrowse);
      Controls.Add(lbName);
      Controls.Add(textBox1);
      Name = "FilePickerPropertyEditor";
      Size = new Size(375, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private Label lbName;
    private Button btnBrowse;
  }
}
