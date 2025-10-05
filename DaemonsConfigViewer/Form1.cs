using DaemonsConfigViewer.Models;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackedTables.Net;
using PackedTableTabs;
using PackedTableTabs.Extensions;
using PackedTableTabs.PropEditors;
using Serilog;
using Serilog.Core;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.PropertyGridInternal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DaemonsConfigViewer {

  public partial class Form1 : Form {
    private MessageLogTab textTabPage;

    private ILoggerFactory _loggerFactory;
    private IProjectRepository _projectRepository;
    private ISettingsRepository _settingsRepository;
    private PackedTableTabs.PropertiesTab projectTab = new PackedTableTabs.PropertiesTab("Projects");
    private PackedTableTabs.PropertiesTab NodesRepoTab = new PackedTableTabs.PropertiesTab("Node Properties");
    private IAppConfig _appConfig;
    private INodesRepository _nodesRepo;

    private IStatusProvider _statusProvider;
    private ITypeProvider _typeProvider;
    private IParentItemProvider _parentItemProvider;
    private ISecurityService _securityService;
    private IValidationService _validationService;
    private IIndexRepository _indexRepository;
    private IIndexService _indexService;

    public Form1(ServiceProvider _serviceProvider) {
      InitializeComponent();
 //     LoadLogger();
      label1.Text = $"{Cx.AppName} select project below";
      lbProjectFolder.Text = $"Project Folder: {Sx.CommonAppPath}";
      
      _loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
      _projectRepository = _serviceProvider.GetRequiredService<IProjectRepository>();
      _settingsRepository = _serviceProvider.GetRequiredService<ISettingsRepository>();

      _appConfig = _serviceProvider.GetRequiredService<IAppConfig>();      
      _appConfig.OnProjectsLoadedEvent += ProjectsReloaded;

      _nodesRepo = _serviceProvider.GetRequiredService<INodesRepository>();
      _nodesRepo.OnNodesLoadedEvent += NodesChangedNeedsReload;
      
      _statusProvider = _serviceProvider.GetRequiredService<IStatusProvider>();
      _typeProvider = _serviceProvider.GetRequiredService<ITypeProvider>();
      _parentItemProvider = _serviceProvider.GetRequiredService<IParentItemProvider>();

      _securityService = _serviceProvider.GetRequiredService<ISecurityService>();
      _validationService = _serviceProvider.GetRequiredService<IValidationService>();
      _indexRepository = _serviceProvider.GetRequiredService<IIndexRepository>();
      _indexService = _serviceProvider.GetRequiredService<IIndexService>();


      tabControl2.TabPages.Add(this.projectTab);
      tabControl4.TabPages.Add(this.NodesRepoTab);
      // configure logging example
      this.textTabPage = new MessageLogTab("Errors");
      textTabPage.IsHideable = true;
      textTabPage.OnHideEvent += () => {
        ScRoot.Panel2Collapsed = true;
      };
      textTabPage.OnShowEvent += () => {
        ScRoot.Panel2Collapsed = false;
      };

      projectTab.LogTab = this.textTabPage;
      NodesRepoTab.LogTab = this.textTabPage;
      tabControl3.TabPages.Add(this.textTabPage);

    }

    delegate void RepoReloadedCallback();

    private void NodesChangedNeedsReload() {
      if (this.tvNodes.InvokeRequired) {
        var d = new RepoReloadedCallback(NodesChangedNeedsReload);
        this.Invoke(d, new object[] { });
      } else {
        LoadNodes();
      }
    }

   
    private void ProjectsReloaded() {
      if (this.tvMain.InvokeRequired) {
        var d = new RepoReloadedCallback(ProjectsReloaded);
        this.Invoke(d, new object[] { });
      } else {
        LoadProjects();
      }
    }

    private void Form1_Shown(object sender, EventArgs e) {
      LoadProjects();
      LoadNodes();
      var table = _projectRepository.Projects;
      var schema = table.GetUISchema();
      schema.ConfigureColumn(Cx.ProjectIdCol).AsReadOnly().WithTooltip("System generated ID");
      schema.ConfigureColumn(Cx.ProjectNameCol).WithOrder(1);
      schema.ConfigureColumn(Cx.ProjectPathCol).WithOrder(2).UseEditor(EditorType.FolderPicker);
      schema.ConfigureColumn(Cx.ProjectDescriptionCol).WithOrder(3).UseEditor(EditorType.Memo).WithTooltip("Project Description");
      table.MoveFirst();
      projectTab.TableRow = table.Current;
      projectTab.UISchema = schema;
      projectTab.SetLabelRight(76);
      projectTab.OnPostEvent += ProjectTab_OnPostEvent;

      var nodeTable = _nodesRepo.ItemsTable;
      var nodeSchema = nodeTable.GetUISchema();



      nodeSchema.ConfigureColumn(Cx.ItemIdCol)
        .WithOrder(1)
        .AsReadOnly()
        .WithTooltip("System generated ID");

      nodeSchema.ConfigureColumn(Cx.ItemParentCol)
        .UseEditor(EditorType.ComboBox)
        .WithLabelText("Parent")
        .AsReadOnly()
        .WithOrder(2)
        .WithLookupProvider(_parentItemProvider)
        .WithTooltip("Parent");

      nodeSchema.ConfigureColumn(Cx.ItemRankCol)
        .WithLabelText("Rank")
        .WithOrder(3)
        .WithTooltip("Node Rank - lower numbers are higher priority");

      nodeSchema.ConfigureColumn(Cx.ItemTypeIdCol)
        .UseEditor(EditorType.ComboBox)
        .WithLabelText("Type")
        .WithOrder(4)
        .WithLookupProvider(_typeProvider)
        .WithTooltip("Node Type");

      nodeSchema.ConfigureColumn(Cx.ItemStatusCol)
        .UseEditor(EditorType.ComboBox)
        .WithLabelText("Status")
        .WithOrder(5)
        .WithLookupProvider(_statusProvider)
        .WithTooltip("Node Status");

      nodeSchema.ConfigureColumn(Cx.ItemNameCol)
        .WithLabelText("Name")
        .WithOrder(6);

      nodeSchema.ConfigureColumn(Cx.ItemDetailsCol)
        .UseEditor(EditorType.Memo)
        .WithOrder(7)
        .WithTooltip("Node Description");

      nodeSchema.ConfigureColumn(Cx.ItemModifiedCol)
        .WithOrder(8)
        .WithTooltip("Last Modified");

      nodeSchema.ConfigureColumn(Cx.ItemCreatedCol)
        .WithOrder(9)
        .WithTooltip("Created On");

      nodeSchema.ConfigureColumn(Cx.ItemCompletedCol)
        .WithOrder(10)
        .WithTooltip("Completed On");




      nodeTable.MoveFirst();
      NodesRepoTab.TableRow = nodeTable.Current;
      NodesRepoTab.UISchema = nodeSchema;
      NodesRepoTab.SetLabelRight(76);
      NodesRepoTab.OnPostEvent += NodesRepoTab_OnPostEvent;

      setupSettingsTab();

    }

    #region Projects Tab

    private void ProjectTab_OnPostEvent() {
      if (_projectRepository.ProjectsTableSet.Modified) {
        _projectRepository.Save();
        LoadProjects();
      }
    }

    private void LoadProjects() {
      var projects = _projectRepository.GetAllProjects();
      string selectedNode = "";
      TreeNode? selected = null;
      if (tvMain.SelectedNode != null) {
        selectedNode = tvMain.SelectedNode.Text;
      }
      tvMain.Nodes.Clear();
      var rootNode = new TreeNode("Projects") {
        Tag = null,
        ImageIndex = 0,
        SelectedImageIndex = 0,
        NodeFont = new System.Drawing.Font(tvMain.Font, System.Drawing.FontStyle.Bold)
      };
      var projectsRoot = tvMain.Nodes.Add(rootNode);
      foreach (var project in projects) {
        var projectNode = new TreeNode(project.Name) {
          Tag = project,
          ImageIndex = 1,
          SelectedImageIndex = 1
        };
        if (selectedNode != "" && project.Name == selectedNode) {
          selected = projectNode;
        }
        rootNode.Nodes.Add(projectNode);
      }
      tvMain.ExpandAll();
      if (selected != null) {
        tvMain.SelectedNode = selected;
      } else if (tvMain.Nodes.Count > 0) {
        tvMain.SelectedNode = tvMain.Nodes[0];
      }
    }

    private void tvMain_AfterSelect(object sender, TreeViewEventArgs e) {
      var node = tvMain.SelectedNode;
      if (node != null && node.Tag is not null && node.Tag is DaemonsMCP.Core.Models.ProjectModel project) {
        if (_projectRepository.Projects.FindFirst(Cx.ProjectIdCol, project.Id)) {
          var row = _projectRepository.Projects.Current;
          if (row != null) {
            projectTab.TableRow = row;
            projectTab.SetLabelRight(76);
            projectTab.SetEditingMode(false);
            label1.Text = $"{Cx.AppName} selected {project.Name}";
            lbProjectFolder.Text = $"Project Folder: {project.Path}";
            lbIndexHeader.Text = $"Index Files for Project: {project.Name}";
            var indexCountsResult = Task.Run(async () => await _indexService.GetIndexStatus().ConfigureAwait(false)).GetAwaiter().GetResult();
            if (indexCountsResult == null) {
              lbIndexLine1.Text = $"No index information available.";
              lbIndexLine2.Text = $"";
              return;
            } else {
              var projectIndexCounts = indexCountsResult.Projects.FirstOrDefault(pi => string.Compare(pi.ProjectName, project.Name, true) == 0);
              if (projectIndexCounts == null) {
                lbIndexLine1.Text = $"No index information available.";
                lbIndexLine2.Text = $"";
                return;
              }
              lbIndexHeader.Text = $"Index Files for Project: {projectIndexCounts.ProjectName} found.";
              lbIndexLine1.Text = $"Indexed Files: {projectIndexCounts.FileCount} Classes: {projectIndexCounts.ClassCount} Methods {projectIndexCounts.MethodCount}";
              lbIndexLine2.Text = $"Index Queue: {projectIndexCounts.IndexQueuedCount} atm...";
            }
          }
        }
      } else {
        projectTab.TableRow = null;
      }
    }

    private void tabControl2_SelectedIndexChanged(object sender, EventArgs e) {
      TabPage currentTab = tabControl2.SelectedTab;
      if (currentTab != null && currentTab.Text == "Projects") {
        tvMain_AfterSelect(sender, null);
      }
    }

    private void addProjectToolStripMenuItem_Click(object sender, EventArgs e) {
      if (tabControl2.SelectedTab != projectTab) {
        tabControl2.SelectedTab = projectTab;
      }
      var newRow = _projectRepository.Projects.AddRow();
      projectTab.TableRow = newRow;
      projectTab.SetEditingMode(true);
      projectTab.SetLabelRight(76);
    }

    private void RemoveProjectMenuItem_Click(object sender, EventArgs e) {
      var current = tvMain.SelectedNode;
      if (current != null && current.Tag is DaemonsMCP.Core.Models.ProjectModel project) {
        var result = MessageBox.Show($"Are you sure you want to delete project '{project.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.Yes) {
          _projectRepository.DeleteProject(project.Id);
          _projectRepository.Save();
          LoadProjects();
        }
      }
    }

    #endregion

    #region Settings Tab

    private bool _settingsModified = false;
    private bool _settingsLoading = false;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SettingsModified {
      get {
        return _settingsModified;
      }
      set {
        _settingsModified = value;
        btnSettingsOk.Visible = value;
        btnSettingsCancel.Visible = value;
        if (value) {
          edMaxFileSize.BackColor = PropertiesTabColors.EditingBackground;
          edMaxWriteSize.BackColor = PropertiesTabColors.EditingBackground;
          edAllowedExt.BackColor = PropertiesTabColors.EditingBackground;
          edBlockedExt.BackColor = PropertiesTabColors.EditingBackground;
          edBlockedFiles.BackColor = PropertiesTabColors.EditingBackground;
          edBlockedFolders.BackColor = PropertiesTabColors.EditingBackground;
        } else {
          edMaxFileSize.BackColor = PropertiesTabColors.StandardEditorWhite;
          edMaxWriteSize.BackColor = PropertiesTabColors.StandardEditorWhite;
          edAllowedExt.BackColor = PropertiesTabColors.StandardEditorWhite;
          edBlockedExt.BackColor = PropertiesTabColors.StandardEditorWhite;
          edBlockedFiles.BackColor = PropertiesTabColors.StandardEditorWhite;
          edBlockedFolders.BackColor = PropertiesTabColors.StandardEditorWhite;
        }
      }
    }


    private void setupSettingsTab() {
      _settingsLoading = true;
      SettingsModified = false;
      lbAppName.Text = $"{Cx.AppName} {Cx.AppVersion} Settings";
      lbPath.Text = "AppFolder " + Sx.CommonAppPath;
      lbLogPath.Text = "LogFolder " + Sx.LogsAppPath;
      var security = _appConfig.Security;

      edMaxFileSize.Text = security.MaxFileSize;
      label5.Text = $"(Max: {_settingsRepository.MaxFileSizeBytes} bytes)";
      edMaxWriteSize.Text = security.MaxFileWriteSize;
      label6.Text = $"(Max: {_settingsRepository.MaxFileWriteSizeBytes} bytes)";
      cbAllowWrite.Checked = security.AllowWrite;
      var allowedList = security.AllowedExtensions;
      edAllowedExt.Text = string.Join(";", allowedList);
      var blockedList = security.BlockedExtensions;
      edBlockedExt.Text = string.Join(";", blockedList);
      var blockedNames = security.BlockedFileNames;
      edBlockedFiles.Text = string.Join(";", blockedNames);
      var blockedFolders = security.WriteProtectedPaths;
      edBlockedFolders.Text = string.Join(";", blockedFolders);
      _settingsLoading = false;
    }

    private void btnSettingsOk_Click(object sender, EventArgs e) {
      try {
        var size = edMaxFileSize.Text.Trim();
        var newSize = size.ParseFileSize();
        _settingsRepository.MaxFileSize = size;

        size = edMaxWriteSize.Text.Trim();
        newSize = size.ParseFileSize();
        _settingsRepository.MaxFileWriteSize = size;

        var allow = cbAllowWrite.Checked;
        _settingsRepository.AllowFileWrites = allow;

        var list = edAllowedExt.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        _settingsRepository.AllowedExtensions = list;

        list = edBlockedExt.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        _settingsRepository.BlockedExtensions = list;

        list = edBlockedFiles.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        _settingsRepository.BlockedFileNames = list;

        list = edBlockedFolders.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        _settingsRepository.BlockedFolders = list;

        _settingsRepository.Save();
        setupSettingsTab();
      } catch (Exception ex) {
        textTabPage.LogMsg($"Error saving settings: {ex.Message}");
        MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void btnSettingsCancel_Click(object sender, EventArgs e) {
      setupSettingsTab();
    }

    private void edMaxFileSize_TextChanged(object sender, EventArgs e) {
      try {
        if (!_settingsLoading) {
          var size = edMaxFileSize.Text.Trim();
          if (size != _settingsRepository.MaxFileSize) {
            SettingsModified = true;
            var newSize = size.ParseFileSize();
            label5.Text = $"(New Size: {newSize} bytes)";
            btnSettingsOk.Enabled = true;
          }
        }
      } catch {
        edMaxFileSize.BackColor = PropertiesTabColors.ValidationError;
        label5.Text = $"Error Parsing, for ex: 60KB";
        btnSettingsOk.Enabled = false;
      }
    }

    private void edMaxWriteSize_TextChanged(object sender, EventArgs e) {
      try {
        if (!_settingsLoading) {
          var size = edMaxWriteSize.Text.Trim();
          if (size != _settingsRepository.MaxFileWriteSize) {
            SettingsModified = true;
            var newSize = size.ParseFileSize();
            label6.Text = $"(New Size: {newSize} bytes)";
            btnSettingsOk.Enabled = true;
          }
        }
      } catch {
        edMaxWriteSize.BackColor = PropertiesTabColors.ValidationError;
        label6.Text = $"Error Parsing, for ex: 60KB";
        btnSettingsOk.Enabled = false;
      }
    }

    private void cbAllowWrite_CheckedChanged(object sender, EventArgs e) {
      if (!_settingsLoading) {
        var allow = cbAllowWrite.Checked;
        if (allow != _settingsRepository.AllowFileWrites) {
          SettingsModified = true;
        }
      }
    }

    private void edAllowedExt_TextChanged(object sender, EventArgs e) {
      if (!_settingsLoading) {
        var list = edAllowedExt.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        if (!list.SequenceEqual(_settingsRepository.AllowedExtensions)) {
          SettingsModified = true;
        }
      }
    }

    private void edBlockedExt_TextChanged(object sender, EventArgs e) {
      if (!_settingsLoading) {
        var list = edBlockedExt.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        if (!list.SequenceEqual(_settingsRepository.BlockedExtensions)) {
          SettingsModified = true;
        }
      }
    }

    private void edBlockedFiles_TextChanged(object sender, EventArgs e) {
      if (!_settingsLoading) {
        var list = edBlockedFiles.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        if (!list.SequenceEqual(_settingsRepository.BlockedFileNames)) {
          SettingsModified = true;
        }
      }
    }

    private void edBlockedFolders_TextChanged(object sender, EventArgs e) {
      if (!_settingsLoading) {
        var list = edBlockedFolders.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).Distinct().ToList();
        if (!list.SequenceEqual(_settingsRepository.BlockedFolders)) {
          SettingsModified = true;
        }
      }
    }

    private void linkLabel1_Click(object sender, EventArgs e) {
      var text = lbPath.Text.ParseLast(" ");
      if (text == null) { return; }
      try {
        System.Diagnostics.Process.Start("explorer.exe", text);
      } catch (Exception ex) {
        textTabPage.LogMsg($"Error opening folder: {ex.Message}");
        MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void lbLogPath_Click(object sender, EventArgs e) {
      var text = lbLogPath.Text.ParseLast(" ");
      if (text == null) { return; }
      try {
        System.Diagnostics.Process.Start("explorer.exe", text);
      } catch (Exception ex) {
        textTabPage.LogMsg($"Error opening folder: {ex.Message}");
        MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
      TabPage currentTab = tabControl1.SelectedTab;
      if (currentTab != null && currentTab.Name == "TpNodes") {
        LoadNodes();
      } else if (currentTab != null && currentTab.Name == "TpConfig") {
        tabControl2_SelectedIndexChanged(sender, e);
      }
    }


    private int _lookoutForNodeId = -1;
    private TreeNode? _lookoutFoundSelectedNode = null;
    private void LoadNodes() {

      if (tvNodes.SelectedNode != null) {
        _lookoutForNodeId = (tvNodes.SelectedNode.Tag as Nodes).Id;
      }
      tvNodes.Nodes.Clear();

      var nodes = _nodesRepo.GetNodes(nodeId: null, 20);
      if (nodes == null) return;
      nodes.ForEach(n => LoadNodeRecursive(null, n));

      tvNodes.ExpandAll();
      tvNodes.SelectedNode = _lookoutFoundSelectedNode;
      tvNodes.Focus();
    }

    private void LoadNodeRecursive(TreeNode? parent, Nodes nodeParam) {
      if (nodeParam == null) return;
      TreeNode tnNew = new TreeNode() {
        Text = nodeParam.Name,
        Tag = nodeParam,
        ImageIndex = 1,
        SelectedImageIndex = 1
      };
      if (nodeParam.Id == _lookoutForNodeId) {
        _lookoutFoundSelectedNode = tnNew;
      }
      if (parent == null) {
        tvNodes.Nodes.Add(tnNew);
      } else {
        parent.Nodes.Add(tnNew);
      }
      nodeParam.Subnodes.ForEach(n => LoadNodeRecursive(tnNew, n));

    }


    private void NodesRepoTab_OnPostEvent() {
      if (_nodesRepo.StorageTables.Modified) {
        _nodesRepo.WriteStorage();
        LoadNodes();
      }
    }

    private void tvNodes_AfterSelect(object sender, TreeViewEventArgs e) {
      if (_skipReset) return;
      var node = tvNodes.SelectedNode;

      if (node != null && node.Tag is not null && node.Tag is Nodes n) {
        if (_nodesRepo.ItemsTable.FindFirst(Cx.ItemIdCol, n.Id)) {
          var row = _nodesRepo.ItemsTable.Current;
          if (row != null) {
            NodesRepoTab.TableRow = row;
            NodesRepoTab.SetLabelRight(76);
            NodesRepoTab.SetEditingMode(_nodesRepo.ItemsTable.State != TableState.Browse);
          }
        }
      } else {
        NodesRepoTab.TableRow = null;
      }
    }

    private void tvNodes_ItemDrag(object sender, ItemDragEventArgs e) {
      if (e.Button == MouseButtons.Left && e.Item != null) {
        DoDragDrop(e.Item, DragDropEffects.Move);
      }
    }

    private void tvNodes_DragDrop(object sender, DragEventArgs e) {
      try {
        Point targetPt = tvNodes.PointToClient(new Point(e.X, e.Y));
        TreeNode? targetItem = tvNodes.GetNodeAt(targetPt);
        var targetNode = targetItem?.Tag as Nodes;
        TreeNode? dragNode = (TreeNode?)e?.Data?.GetData(typeof(TreeNode));
        var dragNodes = dragNode?.Tag as Nodes;

        if (dragNode == null || dragNodes == null) {
          return;
        }

        if (targetNode == null) {
          if (dragNode.Parent != null && dragNode.Parent.Nodes.Contains(dragNode)) {
            dragNode.Parent.Nodes.Remove(dragNode);
            tvNodes.Nodes.Add(dragNode);
            dragNodes.ParentId = 0;
            _nodesRepo.AddUpdateNode(dragNodes);
            _nodesRepo.WriteStorage();
            LoadNodes();
          }
          return;
        }

        if (targetItem != null && !targetItem.Nodes.Contains(dragNode)) {
          if (dragNode?.Parent?.Nodes.Contains(dragNode) ?? false) {
            dragNode.Parent.Nodes.Remove(dragNode);
          }
          targetItem.Nodes.Add(dragNode);
          dragNodes.ParentId = targetNode?.Id ?? 0;
          _nodesRepo.AddUpdateNode(dragNodes);
          _nodesRepo.WriteStorage();
          LoadNodes();
          return;
        }

      } catch (Exception edd) {
        textTabPage.LogMsg(edd.Message);
      } finally {
      }
    }

    private void tvNodes_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.Move;
    }

    private void tvNodes_DragOver(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(typeof(TreeNode))) {
        Point targetPt = tvNodes.PointToClient(new Point(e.X, e.Y));
        TreeNode? targetItem = tvNodes.GetNodeAt(targetPt);
        TreeNode? dragNode = (TreeNode?)e?.Data?.GetData(typeof(TreeNode));
        if (dragNode == null || dragNode == targetItem || targetItem != null && (dragNode.Parent == targetItem || targetItem.Parent == dragNode)) {
          e.Effect = DragDropEffects.None;
          return;
        }
      } else {
        e.Effect = DragDropEffects.None;
        return;
      }
      e.Effect = DragDropEffects.Move;
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {
      if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Name == "TpNodes") {
        addProjectToolStripMenuItem.Visible = false;
        RemoveProjectMenuItem.Visible = false;

        AddItemMenuItem.Visible = true;
        RemoveItemMenuItem.Visible = tvNodes.SelectedNode != null;
        MoveItemUpMenuItem.Visible = tvNodes.SelectedNode != null && (tvNodes.SelectedNode.CanSwitchUp());

      } else if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Name == "TpConfig") {
        addProjectToolStripMenuItem.Visible = true;
        RemoveProjectMenuItem.Visible = tvMain.SelectedNode != null;

        AddItemMenuItem.Visible = false;
        RemoveItemMenuItem.Visible = false;
        MoveItemUpMenuItem.Visible = false;
      }
    }


    private bool _skipReset = false;
    private void AddItemMenuItem_Click(object sender, EventArgs e) {
      int parentId = 0;
      int parentType = 0;
      int nextRank = 1;
      int statusId = _nodesRepo.notStartedStatusId;
      var selectedNode = tvNodes.SelectedNode;
      if (selectedNode != null && selectedNode.Tag != null && selectedNode.Tag is Nodes node) {
        parentId = node.Id;
        parentType = node.TypeId;
        nextRank = selectedNode.Nodes.Count + 1;
      }

      var newRow = _nodesRepo.ItemsTable.AddRow();
      newRow[Cx.ItemParentCol].Value = parentId;
      newRow[Cx.ItemTypeIdCol].Value = parentType;
      newRow[Cx.ItemStatusCol].Value = statusId;
      newRow[Cx.ItemRankCol].Value = nextRank;
      newRow[Cx.ItemNameCol].Value = "New item";
      newRow[Cx.ItemCreatedCol].Value = DateTime.Now;
      newRow[Cx.ItemModifiedCol].Value = DateTime.Now;
      NodesRepoTab.TableRow = newRow;
      NodesRepoTab.SetEditingMode(true);
      NodesRepoTab.SetLabelRight(76);

      if (selectedNode != null) {
        TreeNode newNode = new TreeNode() {
          Text = "New item",
          Tag = new Nodes() {
            Id = (int)newRow[Cx.ItemIdCol].Value,
            ParentId = parentId,
            TypeId = parentType,
            TypeName = _nodesRepo.GetTypeNameById(parentType) ?? "",
            StatusId = statusId,
            Status = _nodesRepo.GetTypeNameById(statusId) ?? "",
            Name = "New item",
            Created = (DateTime)newRow[Cx.ItemCreatedCol].Value,
            Modified = (DateTime)newRow[Cx.ItemModifiedCol].Value
          },
          ImageIndex = 1,
          SelectedImageIndex = 1
        };
        _skipReset = true;
        try {
          if (selectedNode.Nodes == null) {
            tvNodes.Nodes.Add(newNode);
          } else {
            selectedNode.Nodes.Add(newNode);
          }

          tvNodes.SelectedNode = newNode;
        } finally {
          _skipReset = false;
          NodesRepoTab.SetEditingMode(_nodesRepo.ItemsTable.State != TableState.Browse);
        }
      }
    }

    private void MoveItemUpMenuItem_Click(object sender, EventArgs e) {
      var selectedNode = tvNodes.SelectedNode;
      if (selectedNode != null && selectedNode.Tag != null && selectedNode.Tag is Nodes node) {
        selectedNode.SwitchRankUp();
        _nodesRepo.WriteStorage();
        LoadNodes();
      }
    }

    private void RemoveItemMenuItem_Click(object sender, EventArgs e) {
      var selectedNode = tvNodes.SelectedNode;
      if (selectedNode != null && selectedNode.Tag != null && selectedNode.Tag is Nodes node) {
        var result = MessageBox.Show($"Are you sure you want to delete node '{node.Name}' and all its subnodes?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result != DialogResult.Yes) return;
        var parent = selectedNode.Parent;
        if (parent != null && parent.Nodes.Contains(selectedNode)) {
          parent.Nodes.Remove(selectedNode);
        } else if (tvNodes.Nodes.Contains(selectedNode)) {
          tvNodes.Nodes.Remove(selectedNode);
        }
        tvNodes.SelectedNode = parent;
        _nodesRepo.RemoveNode(node.Id, RemoveStrategy.DeleteCascade);
        _nodesRepo.WriteStorage();
      }
    }

    private void lbProjectFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      var text = lbProjectFolder.Text.Substring(15).TrimStart().TrimEnd();
      if (text == null) { return; }
      try {
        System.Diagnostics.Process.Start("explorer.exe", text);
      } catch (Exception ex) {
        textTabPage.LogMsg($"Error opening folder: {ex.Message}");
        MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e) {

    }

    private void ImportLegacyConfigMenuItem_Click(object sender, EventArgs e) {
      var openFileDialog = new OpenFileDialog {
        Filter = "JSON Config|daemonsmcp.json|All Files|*.*",
        Title = "Import Legacy Configuration"
      };

      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          // Read old config
          var jsonContent = File.ReadAllText(openFileDialog.FileName);
          var oldConfig = JsonSerializer.Deserialize<DaemonsMcpConfiguration>(jsonContent, Sx.DefaultJsonOptions);          

          // Import projects
          foreach (var project in oldConfig.Projects.Where(p => p.Enabled)) {
            var newProject = new ProjectModel(project.Name, project.Description, project.Path);
            _projectRepository.AddProject(newProject);
          }
          _projectRepository.Save();

          MessageBox.Show($"Imported {oldConfig.Projects.Count} projects successfully!",
                          "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
          LoadProjects();
        } catch (Exception ex) {
          MessageBox.Show($"Error importing config: {ex.Message}",
                          "Import Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }


  }
}
