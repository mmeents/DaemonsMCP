namespace PackedTableTabs.PropEditors {
  partial class BooleanPropertyEditor {
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
      checkBox1 = new CheckBox();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(3, 6);
      lbName.Name = "lbName";
      lbName.Size = new Size(52, 15);
      lbName.TabIndex = 1;
      lbName.Text = "Property";
      lbName.TextAlign = ContentAlignment.TopRight;
      // 
      // checkBox1
      // 
      checkBox1.AutoSize = true;
      checkBox1.Location = new Point(67, 6);
      checkBox1.Name = "checkBox1";
      checkBox1.Size = new Size(15, 14);
      checkBox1.TabIndex = 0;
      checkBox1.UseVisualStyleBackColor = true;
      checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
      // 
      // BooleanPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(checkBox1);
      Controls.Add(lbName);
      Name = "BooleanPropertyEditor";
      Size = new Size(417, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbName;
    private CheckBox checkBox1;
  }
}
