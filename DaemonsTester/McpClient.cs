using DaemonsMCP;
using DaemonsMCP.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaemonsMCPTester
{
    public class MCPClient : IDisposable
    {
    // Important: UpdateClassItem this path to point to your MCP project for testing
    // This should be the path to the DaemonsMCP.csproj file in your MCP project
    // Ensure this path is correct and accessible from your test environment
    private const string MCPExecutablePath = @"C:\MCPSandbox\DaemonsMCPDev\DaemonsMCP\DaemonsMCP.csproj";
        private Process? _mcpProcess;
        private StreamWriter? _stdin;
        private StreamReader? _stdout;
        private StreamReader? _stderr;
        private bool _isInitialized = false;
        private int _requestId = 1;
        private readonly Form1 _parentForm;
        private readonly SemaphoreSlim _requestSemaphore = new SemaphoreSlim(1, 1); // Only one request at a time


        public MCPClient(Form1 parentForm)
        {
            _parentForm = parentForm ?? throw new ArgumentNullException(nameof(parentForm));
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            try
            {
                _mcpProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --project \"{MCPExecutablePath}\" --verbosity quiet",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(MCPExecutablePath)
                    }
                };

                _mcpProcess.Start();
                _stdin = _mcpProcess.StandardInput;
                _stdout = _mcpProcess.StandardOutput;
                _stderr = _mcpProcess.StandardError;

                // Start background tasks to monitor stdout/stderr
                if (_stdout == null || _stderr == null || _stdin == null)
                {
                    throw new Exception("[Form1][MCPClient] Failed to initialize MCP process streams");
                }
                _ = Task.Run(async () =>
                {
                    try
                    {
                        string? line;
                        while (_stderr != null && (line = await _stderr.ReadLineAsync().ConfigureAwait(false)) != null)
                        {
                            
                            _parentForm?.AppendOutput($"[Form1][MCPClient] Received from STDERR: {line}");
                            System.Diagnostics.Debug.WriteLine($"[MCP-STDERR] {line}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _parentForm?.AppendOutput($"[Form1][MCPClient] monitor error: {ex.Message}");
                    }
                });

                // Wait a moment for the process to start
                await Task.Delay(1000);

                // Send MCP initialize message
                await SendInitializeMessage().ConfigureAwait(false);
                
                // Send initialized notification
                await SendInitializedNotification().ConfigureAwait(false);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[Form1][MCPClient]Failed to initialize MCP client: {ex.Message}", ex);
            }
        }

        private async Task SendInitializeMessage()
        {
            var initRequest = new JsonRpcRequest
            {
                Method = Cx.ToolInitializeCmd,
                Params = new
                {
                    protocolVersion = "2025-06-18",
                    capabilities = new
                    {
                        roots = new { }
                    },
                    clientInfo = new
                    {
                        name = "winforms-mcp-client",
                        version = "1.0.0"
                    }
                },
                Id = _requestId++
            };

            var response = await SendRawRequestAsync(initRequest).ConfigureAwait(false);
            _parentForm?.AppendOutput($"[Form1][MCPClient] Initialize Response: {response}");

        }

        private async Task SendInitializedNotification()
        {
            if (_stdin == null)
                throw new InvalidOperationException("[Form1][MCPClient] MCP Client stdin not initialized");
            var notification = new
            {
                jsonrpc = "2.0",
                method = Cx.ToolInitializedNotificationCmd
            };

            var json = JsonSerializer.Serialize(notification);
            await _stdin.WriteLineAsync(json).ConfigureAwait(false);
            await _stdin.FlushAsync().ConfigureAwait(false);
            _parentForm?.AppendOutput($"[Form1][MCPClient] Sent Initialized Notification:");
        }

        public async Task<string> SendRequestAsync(JsonRpcRequest request)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("[Form1][MCPClient] MCP Client not initialized");

            // Ensure only one request at a time to prevent stdin/stdout mixing
            await _requestSemaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return await SendRawRequestAsync(request).ConfigureAwait(false);
            }
            finally
            {
                _requestSemaphore.Release();
            }
        }

        private async Task<string> SendRawRequestAsync(JsonRpcRequest request)
        {
            try
            {
                if (_stdin == null)
                    throw new InvalidOperationException("[Form1][MCPClient] MCP Client stdin not initialized");
                if (_stdout == null)
                  throw new InvalidOperationException("[Form1][MCPClient] MCP Client stdout not initialized");

                var json = JsonSerializer.Serialize(request);
                _parentForm?.AppendOutput($"[Form1][MCPClient] Sending: {json}");
                System.Diagnostics.Debug.WriteLine($"Sending: {json}");
                
                await _stdin.WriteLineAsync(json).ConfigureAwait(false);
                await _stdin.FlushAsync().ConfigureAwait(false);

                  // Keep reading until we get the response with matching ID
                string? response = null;
                int attempts = 0;
                int maxAttempts = 10;
                
                do
                {
                    var responseTask = _stdout.ReadLineAsync();
                    var timeoutTask = Task.Delay(15000); // 15 second timeout
                    
                    var completedTask = await Task.WhenAny(responseTask, timeoutTask).ConfigureAwait(false);
                    
                    if (completedTask == timeoutTask)
                    {
                        _parentForm?.AppendOutput("[Form1][MCPClient]Timeout waiting for response");
                        throw new TimeoutException("[Form1][MCPClient]Timeout waiting for response from MCP server");
                    }
                    if (completedTask == responseTask) { 
                        response = responseTask.Result;
                        _parentForm?.AppendOutput($"[Form1][MCPClient] Received: {response ?? "NULL"}");
                    }  
                    
                    if (string.IsNullOrEmpty(response))
                    {
                        throw new Exception("[Form1][MCPClient] No response received from MCP server");
                    }

                    // Check if this response matches our request ID
                    try
                    {
                        var responseObj = JsonSerializer.Deserialize<JsonRpcResponse>(response);
                        if (responseObj?.Id == request.Id)
                        {
                            // This is our response
                            break;
                        }
                        else
                        {
                            _parentForm?.AppendOutput($"[Form1][MCPClient] Response ID mismatch. Expected: {request.Id}, Got: {responseObj?.Id}. Continuing to read...");
                        }
                    }
                    catch (JsonException)
                    {
                        _parentForm?.AppendOutput($"[Form1][MCPClient] Failed to parse response as JSON: {response}");
                        // Continue reading - might be a partial response
                    }
                    
                    attempts++;
                } while (attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    throw new Exception($"[Form1][MCPClient] Too many attempts to find matching response for request ID {request.Id}");
                }
                
                if (string.IsNullOrEmpty(response))
                {
                    throw new Exception("[Form1][MCPClient] No response received from MCP server");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"[Form1][MCPClient] Error communicating with MCP server: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _requestSemaphore?.Dispose();
                _stdin?.Close();
                _stdout?.Close();
                _stderr?.Close();

                if (_mcpProcess != null && !_mcpProcess.HasExited)
                {
                    _mcpProcess.Kill();
                    _mcpProcess.WaitForExit(5000);
                }

                _mcpProcess?.Dispose();
            }
            catch (Exception)
            {
                // Ignore disposal errors
            }
            GC.SuppressFinalize(this);
        }
    }
}
