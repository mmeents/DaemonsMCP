namespace PackedTableTabs.PropEditors {
  partial class BasePropertyEditor {
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
      lbValue = new Label();
      SuspendLayout();
      // 
      // lbName
      // 
      lbName.AutoSize = true;
      lbName.Location = new Point(14, 7);
      lbName.Margin = new Padding(3, 0, 1, 0);
      lbName.Name = "lbName";
      lbName.Size = new Size(67, 15);
      lbName.TabIndex = 0;
      lbName.Text = "This is sme:";
      lbName.TextAlign = ContentAlignment.TopRight;
      // 
      // lbValue
      // 
      lbValue.AutoSize = true;
      lbValue.Location = new Point(86, 6);
      lbValue.Name = "lbValue";
      lbValue.Size = new Size(38, 15);
      lbValue.TabIndex = 1;
      lbValue.Text = "label1";
      // 
      // BasePropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(lbValue);
      Controls.Add(lbName);
      Name = "BasePropertyEditor";
      Size = new Size(393, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    protected Label lbName;
    private Label lbValue;
  }
}
