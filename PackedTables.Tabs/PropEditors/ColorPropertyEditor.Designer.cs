namespace PackedTableTabs.PropEditors {
  partial class ColorPropertyEditor {
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
      colorPanel = new Panel();
      btnChoose = new Button();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(10, 10);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 0;
      lbName.Text = "label1";
      // 
      // colorPanel
      // 
      colorPanel.Location = new Point(68, 3);
      colorPanel.Name = "colorPanel";
      colorPanel.Size = new Size(46, 24);
      colorPanel.TabIndex = 1;
      // 
      // btnChoose
      // 
      btnChoose.Location = new Point(120, 3);
      btnChoose.Name = "btnChoose";
      btnChoose.Size = new Size(26, 23);
      btnChoose.TabIndex = 2;
      btnChoose.Text = "...";
      btnChoose.UseVisualStyleBackColor = true;
      // 
      // ColorPropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(btnChoose);
      Controls.Add(colorPanel);
      Controls.Add(lbName);
      Name = "ColorPropertyEditor";
      Size = new Size(409, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lbName;
    private Panel colorPanel;
    private Button btnChoose;
  }
}
