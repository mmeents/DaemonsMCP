namespace PackedTableTabs.PropEditors {
  partial class ComboBoxPropertyEditor {
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
      comboBox1 = new ComboBox();
      lbName = new Label();
      SuspendLayout();
      // 
      // comboBox1
      // 
      comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      comboBox1.FormattingEnabled = true;
      comboBox1.Location = new Point(155, 3);
      comboBox1.Name = "comboBox1";
      comboBox1.Size = new Size(277, 23);
      comboBox1.TabIndex = 0;
      comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(3, 6);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // ComboBoxPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(lbName);
      Controls.Add(comboBox1);
      Name = "ComboBoxPropertyEditor";
      Size = new Size(435, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ComboBox comboBox1;
    private Label lbName;
  }
}
