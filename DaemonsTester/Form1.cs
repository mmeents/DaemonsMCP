using DaemonsMCP;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DaemonsMCPTester
{
    public partial class Form1 : Form
    {
        private int _requestId = 1;
        public Form1()
        {
            InitializeComponent();
            _mcpClient = new MCPClient(this);            
        }

        private async void InitButton_Click(object sender, EventArgs e)
        {
            try
            {
                _initButton.Enabled = false;
                AppendOutput("[Form1] Initializing MCP Client...");
                
                await _mcpClient.InitializeAsync().ConfigureAwait(false);
                
                AppendOutput("[Form1] MCP Client initialized successfully!");
                
                _listProjectsButton.Enabled = true;
                _listDirsButton.Enabled = true;
                _listFilesButton.Enabled = true;
                _getFileButton.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendOutput($"[Form1] Error initializing: {ex.Message}");                
            }
        }

        private async void ListProjectsButton_Click(object sender, EventArgs e)
        {
            try
            {
                AppendOutput("[Form1] Requesting project list...");
                
                var request = new JsonRpcRequest
                {
                    Method = Tx.CallMethod,
                    Params = new
                    {
                        name = Px.listProjects,
                        arguments = new { }
                    },
                    Id = _requestId++
                };

                var response = await _mcpClient.SendRequestAsync(request).ConfigureAwait(false);
                AppendOutput($"[Form1] Projects Response: {response}");
            }
            catch (Exception ex)
            {
                AppendOutput($"[Form1] Error listing projects: {ex.Message}");
            }
        }

        private async void ListDirsButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_projectNameTextBox.Text))
                {
                    AppendOutput("[Form1] Please enter a project name first");
                    return;
                }

                AppendOutput($"[Form1] Requesting directories for project: {_projectNameTextBox.Text}");
                
                var request = new JsonRpcRequest
                {
                    Method = Tx.CallMethod,
                    Params = new
                    {
                        name = Px.listProjectDirectory,
                        arguments = new
                        {
                            projectName = _projectNameTextBox.Text,
                            path = _pathTextBox.Text ?? "",
                            filter = _filterTextBox.Text ?? ""
                        }
                    },
                    Id = _requestId++
                };

                var response = await _mcpClient.SendRequestAsync(request).ConfigureAwait(false);
                AppendOutput($"[Form1] Directories Response: {response}");
            }
            catch (Exception ex)
            {
                AppendOutput($"[Form1] Error listing directories: {ex.Message}");
            }
        }

        private async void ListFilesButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_projectNameTextBox.Text))
                {
                    AppendOutput("[Form1] Please enter a project name first");
                    return;
                }

                AppendOutput($"[Form1] Requesting files for project: {_projectNameTextBox.Text}");
                
                var request = new JsonRpcRequest
                {
                    Method = Tx.CallMethod,
                    Params = new
                    {
                        name = Px.listProjectFiles,
                        arguments = new
                        {
                            projectName = _projectNameTextBox.Text,
                            path = _pathTextBox.Text ?? "",
                            filter = _filterTextBox.Text ?? ""
                        }
                    },
                    Id = _requestId++
                };

                var response = await _mcpClient.SendRequestAsync(request).ConfigureAwait(false);
                AppendOutput($"[Form1] Files Response: {response}");
            }
            catch (Exception ex)
            {
                AppendOutput($"[Form1] Error listing files: {ex.Message}");
            }
        }

        private async void GetFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_projectNameTextBox.Text) || string.IsNullOrEmpty(_pathTextBox.Text))
                {
                    AppendOutput("[Form] Please enter both project name and file path");
                    return;
                }

                AppendOutput($"[Form] Requesting file content: {_pathTextBox.Text}");
                
                var request = new JsonRpcRequest
                {
                    Method = Tx.CallMethod,
                    Params = new
                    {
                        name = Px.getProjectFile,
                        arguments = new
                        {
                            projectName = _projectNameTextBox.Text,
                            path = _pathTextBox.Text
                        }
                    },
                    Id = _requestId++
                };

                var response = await _mcpClient.SendRequestAsync(request).ConfigureAwait(false);
                AppendOutput($"[Form] File Content Response: {response}");
            }
            catch (Exception ex)
            {
                AppendOutput($"[Form] Error getting file: {ex.Message}");
            }
        }

        public void AppendOutput(string message)
        {
            if (_outputTextBox.InvokeRequired)
            {
                _outputTextBox.Invoke(new Action<string>(AppendOutput), message);
                return;
            }
            
            _outputTextBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");
            _outputTextBox.SelectionStart = _outputTextBox.Text.Length;
            _outputTextBox.ScrollToCaret();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _mcpClient?.Dispose();
            base.OnFormClosed(e);
        }
          

    }


      // JSON-RPC Data Models (matching your server)
    public class JsonRpcRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("params")]
        public object Params { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class JsonRpcResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
        [JsonPropertyName("result")]
        public JsonElement? Result { get; set; }
        [JsonPropertyName("error")]
        public JsonElement? Error { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public interface IMCPClient : IDisposable
    {
        Task InitializeAsync();
        Task<string> SendRequestAsync(JsonRpcRequest request);        
    }

    

}
