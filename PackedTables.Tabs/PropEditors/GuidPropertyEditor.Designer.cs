namespace PackedTableTabs.PropEditors {
  partial class GuidPropertyEditor {
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
      btnGenerate = new Button();
      textBox1 = new TextBox();
      lbName = new Label();
      SuspendLayout();
      // 
      // btnGenerate
      // 
      btnGenerate.AccessibleName = "";
      btnGenerate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnGenerate.Location = new Point(446, 3);
      btnGenerate.Name = "btnGenerate";
      btnGenerate.Size = new Size(55, 23);
      btnGenerate.TabIndex = 0;
      btnGenerate.Text = "🎲 New";
      btnGenerate.UseVisualStyleBackColor = true;
      btnGenerate.Click += BtnGenerate_Click;
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(235, 3);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(209, 23);
      textBox1.TabIndex = 1;
      textBox1.TextChanged += TextBox1_TextChanged;
      textBox1.Leave += TextBox1_Leave;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(9, 6);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 2;
      lbName.Text = "label1";
      // 
      // GuidPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(lbName);
      Controls.Add(textBox1);
      Controls.Add(btnGenerate);
      Name = "GuidPropertyEditor";
      Size = new Size(504, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnGenerate;
    private TextBox textBox1;
    private Label lbName;
  }
}
