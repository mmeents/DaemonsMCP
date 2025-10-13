using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs {
  public class MessageLogTab : TabPage {
    private TextBox textBox;
    private ToolStrip toolStrip;
    private ToolStripLabel toolStripLabel;
    private MenuStrip menuStrip;
    private SplitContainer topMenuSplitContainer;
    private ToolStripMenuItem hideMenuItem;
    public MessageLogTab(string tabName) : base(tabName) {

      textBox = new TextBox();
      topMenuSplitContainer = new SplitContainer();
      toolStrip = new ToolStrip();
      toolStripLabel = new ToolStripLabel();
      menuStrip = new MenuStrip();
      hideMenuItem = new ToolStripMenuItem();


      ((System.ComponentModel.ISupportInitialize)topMenuSplitContainer).BeginInit();
      topMenuSplitContainer.Panel1.SuspendLayout();
      topMenuSplitContainer.Panel2.SuspendLayout();
      topMenuSplitContainer.SuspendLayout();
      toolStrip.SuspendLayout();
      menuStrip.SuspendLayout();
      this.SuspendLayout();

      this.Controls.Add(topMenuSplitContainer);
      topMenuSplitContainer.BackColor = SystemColors.Control;
      topMenuSplitContainer.Dock = DockStyle.Top;
      topMenuSplitContainer.Location = new System.Drawing.Point(0, 0);
      topMenuSplitContainer.Margin = new Padding(0);
      topMenuSplitContainer.Name = "topMenuSplitContainer";
      topMenuSplitContainer.Panel1.Controls.Add(toolStrip);
      topMenuSplitContainer.Panel2.Controls.Add(menuStrip);
      topMenuSplitContainer.Size = new Size(251, 24);
      topMenuSplitContainer.SplitterDistance = 123;
      topMenuSplitContainer.IsSplitterFixed = true;
      topMenuSplitContainer.SplitterWidth = 1;
      topMenuSplitContainer.TabIndex = 0;
      topMenuSplitContainer.Margin = new Padding(0);
      topMenuSplitContainer.Resize += topMenuSplitContainer_Resize;

      toolStrip.Items.AddRange(new ToolStripItem[] { toolStripLabel });
      toolStrip.Location = new Point(0, 0);
      toolStrip.Name = "toolStrip1";
      toolStrip.Size = new Size(123, 24);
      toolStrip.TabIndex = 0;
      toolStrip.Text = "toolStrip";
      toolStripLabel.Name = "toolStripLabel";
      toolStripLabel.Size = new Size(80, 20);
      toolStripLabel.Text = tabName;  // Set the label text to the tab name

      menuStrip.Items.AddRange(new ToolStripItem[] { hideMenuItem });
      menuStrip.Location = new Point(0, 0);
      menuStrip.Name = "menuStrip2";
      menuStrip.RightToLeft = RightToLeft.Yes;
      menuStrip.Size = new Size(124, 24);
      menuStrip.TabIndex = 0;
      menuStrip.Text = "menuStrip2";

      hideMenuItem.Name = "hideMenuItem";
      hideMenuItem.Size = new Size(44, 20);
      hideMenuItem.Text = "Hide";
      hideMenuItem.Click += hideMenuItem_Click;

      this.Controls.Add(textBox);
      textBox.Margin = new Padding(0);
      textBox.Dock = DockStyle.Bottom;
      textBox.Multiline = true;
      textBox.ScrollBars = ScrollBars.Both;
      textBox.AcceptsTab = true;
      textBox.WordWrap = false;
      textBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      textBox.Text = "";


      topMenuSplitContainer.Panel1.ResumeLayout(false);
      topMenuSplitContainer.Panel1.PerformLayout();
      topMenuSplitContainer.Panel2.ResumeLayout(false);
      topMenuSplitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)topMenuSplitContainer).EndInit();
      topMenuSplitContainer.ResumeLayout(false);
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      toolStrip.ResumeLayout(false);
      toolStrip.PerformLayout();
      this.ResumeLayout(false);
      IsHideable = false;
      this.Margin = new Padding(0);
      this.Padding = new Padding(0);
    }

    delegate void SetLogMsgCallback(string msg);
    public void LogMsg(string msg) {
      if (this.textBox.InvokeRequired) {
        SetLogMsgCallback d = new SetLogMsgCallback(LogMsg);
        this.BeginInvoke(d, new object[] { msg });
      } else {
        if (!textBox.Visible) textBox.Visible = true;
        this.textBox.Text = msg + Environment.NewLine + textBox.Text;
        DoOnShowEvent();
      }
    }

    protected override void OnResize(EventArgs eventargs) {
      base.OnResize(eventargs);
      topMenuSplitContainer_Resize(this, eventargs);
    }

    private bool hideable = false;
    public bool IsHideable {
      get { return hideable; }
      set {
        hideable = value;
        if (hideable) {
          hideMenuItem.Visible = true;
        } else {
          hideMenuItem.Visible = false;
        }
      }
    }

    public event Action OnHideEvent = delegate { };
    public event Action OnShowEvent = delegate { };
    private void DoOnHideEvent() {
      if (OnHideEvent != null) {
        OnHideEvent();
      }
    }
    private void hideMenuItem_Click(object? sender, EventArgs e) {
      DoOnHideEvent();
    }
    private void DoOnShowEvent() {
      if (OnShowEvent != null) {
        OnShowEvent();
      }
    }

    private void topMenuSplitContainer_Resize(object? sender, EventArgs e) {
      topMenuSplitContainer.SplitterDistance = topMenuSplitContainer.Width / 2;
      textBox.Height = this.Height - topMenuSplitContainer.Height - 2; // Adjust height to fit below the menu
    }

    public void ClearLog() {
      textBox.Text = string.Empty;
    }
  }
}
  

