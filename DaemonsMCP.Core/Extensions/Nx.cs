using DaemonsMCP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaemonsMCP.Core.Extensions {
  public static class Nx {

    public static bool CanSwitchDown(this TreeNode item) {
      if (item.Parent == null) return false;
      var ImChildNo = item.Parent.Nodes.IndexOf(item);
      if (ImChildNo < 0) return false;
      return ImChildNo < item.Parent.Nodes.Count - 1;
    }

    public static TreeNode GetSwitchDownItem(this TreeNode item) {
      if (item.Parent == null) return null;
      var ImChildNo = item.Parent.Nodes.IndexOf(item);
      if (ImChildNo < 0) return null;
      if (ImChildNo + 1 <= item.Parent.Nodes.Count - 1) {
        return ((TreeNode)item.Parent.Nodes[ImChildNo + 1]);
      }
      return null;
    }

    public static bool SwitchRankDown(this TreeNode item) {
      if (item.Parent == null) return false;
      var ImChildNo = item.Parent.Nodes.IndexOf(item);
      if (ImChildNo < 0) return false;
      if (ImChildNo + 1 <= item.Parent.Nodes.Count - 1) {
        var nodeItem = item.Tag as Nodes;
        var switchSibling = item.Parent.Nodes[ImChildNo + 1].Tag as Nodes;
        if (nodeItem == null || switchSibling == null) return false;
        var rank = nodeItem.Rank;
        nodeItem.Rank = switchSibling.Rank;
        switchSibling.Rank = rank;
        return true;
      }
      return false;
    }

    public static bool CanSwitchUp(this TreeNode item) {
      if (item.Parent == null) return false;
      return item.Parent.Nodes.IndexOf(item) >= 1;
    }

    public static TreeNode GetSwitchUpItem(this TreeNode item) {
      if (item.Parent == null) return null;
      var ImChildNo = item.Parent.Nodes.IndexOf(item);
      if (ImChildNo < 1) return null;
      return ((TreeNode)item.Parent.Nodes[ImChildNo - 1]);
    }

    public static bool SwitchRankUp(this TreeNode item) {
      if (item.Parent == null) return false;
      var ImChildNo = item.Parent.Nodes.IndexOf(item);
      if (ImChildNo < 1) return false;
      var itemNode = item.Tag as Nodes;
      var switchSibling = item.Parent.Nodes[ImChildNo - 1].Tag as Nodes;
      if (itemNode == null || switchSibling == null) return false;
      var rank = itemNode.Rank;
      itemNode.Rank = switchSibling.Rank;
      switchSibling.Rank = rank;
      return false;
    }  


  }
}
