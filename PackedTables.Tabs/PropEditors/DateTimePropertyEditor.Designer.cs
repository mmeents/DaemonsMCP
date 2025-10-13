namespace PackedTableTabs.PropEditors {
  partial class DateTimePropertyEditor {
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
      dateTimePicker1 = new DateTimePicker();
      lbName = new Label();
      checkBoxHasValue = new CheckBox();
      SuspendLayout();
      // 
      // dateTimePicker1
      // 
      dateTimePicker1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      dateTimePicker1.Format = DateTimePickerFormat.Short;
      dateTimePicker1.Location = new Point(296, 3);
      dateTimePicker1.Name = "dateTimePicker1";
      dateTimePicker1.Size = new Size(109, 23);
      dateTimePicker1.TabIndex = 0;
      dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
      // 
      // lbName
      // 
      lbName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      lbName.AutoSize = true;
      lbName.Location = new Point(12, 7);
      lbName.Name = "lbName";
      lbName.Size = new Size(38, 15);
      lbName.TabIndex = 1;
      lbName.Text = "label1";
      // 
      // checkBoxHasValue
      // 
      checkBoxHasValue.AutoSize = true;
      checkBoxHasValue.Location = new Point(157, 6);
      checkBoxHasValue.Name = "checkBoxHasValue";
      checkBoxHasValue.Size = new Size(77, 19);
      checkBoxHasValue.TabIndex = 2;
      checkBoxHasValue.Text = "Has Value";
      checkBoxHasValue.UseVisualStyleBackColor = true;
      checkBoxHasValue.CheckedChanged += CheckBoxHasValue_CheckedChanged;
      // 
      // DateTimePropertyEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(checkBoxHasValue);
      Controls.Add(lbName);
      Controls.Add(dateTimePicker1);
      Name = "DateTimePropertyEditor";
      Size = new Size(408, 30);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private DateTimePicker dateTimePicker1;
    private Label lbName;
    private CheckBox checkBoxHasValue;
  }
}
