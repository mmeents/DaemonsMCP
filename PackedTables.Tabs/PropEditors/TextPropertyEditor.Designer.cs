namespace PackedTableTabs.PropEditors {
  partial class TextPropertyEditor  {
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
      SuspendLayout();
      // 
      // textBox1
      // 
      textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      textBox1.Location = new Point(85, 3);
      textBox1.Name = "textBox1";
      textBox1.Size = new Size(326, 23);
      textBox1.TabIndex = 0;
      textBox1.TextChanged += textBox1_TextChanged;
      textBox1.Enter += textBox1_Enter;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(9, 8);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // TextPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(lbName);
      Controls.Add(textBox1);
      Name = "TextPropertyEditor";
      Size = new Size(414, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private Label lbName;
  }
}
