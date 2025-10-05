namespace PackedTableTabs.PropEditors {
  partial class NumericPropertyEditor {
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
      lbName = new Label();
      textBox1 = new TextBox();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(18, 8);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 0;
      lbName.Text = "label1";
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(95, 3);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(336, 23);
      textBox1.TabIndex = 1;
      textBox1.TextChanged += TextBox1_TextChanged;
      textBox1.KeyPress += TextBox1_KeyPress;
      textBox1.Leave += TextBox1_Leave;
      // 
      // NumericPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(textBox1);
      Controls.Add(lbName);
      Name = "NumericPropertyEditor";
      Size = new Size(435, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbName;
    private TextBox textBox1;
  }
}
