using Castle.Core.Logging;
using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Core;
using System.IO;
using System.Text;

namespace DaemonsMCP.Tests.Services
{
    [TestClass]
    public class FileContentTests
    {
        private Mock<IAppConfig> _mockConfig;
        private Mock<IValidationService> _mockValidationService;
        private Mock<ISecurityService> _mockSecurityService;
        private ProjectFileService _fileService;
        private string _testProjectPath;
        private string _testProjectName;
        private Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;
        private ProjectModel _testProject;

    [TestInitialize]
        public void TestInitialize()
        {
            var logsPath = Sx.LogsAppPath;
            Directory.CreateDirectory(logsPath);
            Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
                      .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
                      .Enrich.FromLogContext()
                      .WriteTo.File(
                          path: Path.Combine(logsPath, "test-indexservice-.log"),
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                      )
                      .WriteTo.Debug() // Shows in Visual Studio Debug Output window
                      .CreateLogger();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
            _mockConfig = new Mock<IAppConfig>();
            _mockValidationService = new Mock<IValidationService>();
            _mockSecurityService = new Mock<ISecurityService>();
            
            _testProjectName = "TestProject";
            _testProjectPath = Path.Combine(Path.GetTempPath(), "DaemonsMCPFileContentTests", Guid.NewGuid().ToString());
            _testProject = new ProjectModel(_testProjectName, "Test Description", _testProjectPath);

            Directory.CreateDirectory(_testProjectPath);
            _fileService = new ProjectFileService(_mockConfig.Object, _loggerFactory, _mockValidationService.Object, _mockSecurityService.Object );
            
            _mockSecurityService
                .Setup(s => s.IsWriteAllowed(It.IsAny<string>()))
                .Returns(true);

            _mockSecurityService
                .Setup(s => s.IsWriteContentSizeAllowed(It.IsAny<int>()))
                .Returns(true);

            

            


    }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_testProjectPath))
            {
                Directory.Delete(_testProjectPath, true);
            }
        }

        [TestMethod]
        public async Task GetFileAsync_WithTextFile_ShouldReturnCorrectContent()
        {
            // Arrange
            var testContent = "using System;\n\nnamespace TestProject\n{\n    public class TestClass\n    {\n        public void TestMethod()\n        {\n            Console.WriteLine(\"Hello World\");\n        }\n    }\n}";
            var fileName = "TestClass.cs";
            var fullPath = Path.Combine(_testProjectPath, fileName);
            
            await File.WriteAllTextAsync(fullPath, testContent, Encoding.UTF8);

            _mockValidationService
              .Setup(v => v.ValidateAndPrepare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .Returns(() => new ValidationContext {
                Project = _testProject,
                RelativePath = "",
                FullPath = Path.Combine(_testProjectPath, "TestClass.cs")
              });

      // Act
            var result = await _fileService.GetFileAsync(_testProjectName, fileName);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(fileName);
            result.Path.Should().Be(fileName);            
            result.ContentType.Should().Be("text/x-cs");
            result.Encoding.Should().Be("UTF-8");
            result.IsBinary.Should().BeFalse();
        }

        [TestMethod]
        public async Task GetFileAsync_WithBinaryFile_ShouldReturnEmptyContentWithCorrectMetadata()
        {
            // Arrange
            var binaryData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header
            var fileName = "test.png";
            var fullPath = Path.Combine(_testProjectPath, fileName);
            
            await File.WriteAllBytesAsync(fullPath, binaryData);

            _mockValidationService
              .Setup(v => v.ValidateAndPrepare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .Returns(() => new ValidationContext {
                Project = _testProject,
                RelativePath = "",
                FullPath = Path.Combine(_testProjectPath, fileName)
              });

            // Act
            var result = await _fileService.GetFileAsync(_testProjectName, fileName);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(fileName);
            result.Path.Should().Be(fileName);
            result.Content.Should().BeEmpty();
            result.ContentType.Should().Be("image/png");
            result.Size.Should().Be(binaryData.Length);
            result.IsBinary.Should().BeTrue();
        }

        [TestMethod]
        public async Task CreateFileAsync_WithValidContent_ShouldCreateFileWithCorrectContent()
        {
            // Arrange
            var testContent = "namespace TestProject\n{\n    public class NewClass\n    {\n        public string Property { get; set; } = string.Empty;\n    }\n}";
            var fileName = "NewClass.cs";
            var fullPath = Path.Combine(_testProjectPath, fileName);

            // Setup validation mocks
            _mockValidationService
                .Setup(v => v.ValidatePath(It.IsAny<string>()))
                .Callback<string>(path => { /* Path validation logic */ });

            _mockValidationService
                .Setup(v => v.ValidateContent(It.IsAny<string>()))
                .Callback<string>(content => { /* Content validation logic */ });

            _mockValidationService
              .Setup(v => v.ValidateAndPrepare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .Returns(() => new ValidationContext {
                Project = _testProject,
                RelativePath = "",
                FullPath = Path.Combine(_testProjectPath, fileName)
              });

            // Act
            var result = await _fileService.CreateFileAsync(_testProjectName, fileName, testContent);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Operation.Should().Be("create-project-file");
            result.Message.Should().Contain("File created successfully");

            // Verify file was actually created with correct content
            File.Exists(fullPath).Should().BeTrue();
            var actualContent = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
            actualContent.Should().Be(testContent);

            // Verify result data
            var resultData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(
                System.Text.Json.JsonSerializer.Serialize(result.Data));
            
            var fileName_prop = resultData?.GetProperty("fileName").GetString();
            var path_prop = resultData?.GetProperty("path").GetString();
            var size_prop = resultData?.GetProperty("size").GetInt64();
           
        }

        [TestMethod]
        public async Task UpdateFileAsync_WithNewContent_ShouldUpdateFileContentCorrectly()
        {
            // Arrange
            var originalContent = "// Original content\nnamespace TestProject\n{\n    public class TestClass { }\n}";
            var updatedContent = "// Updated content\nnamespace TestProject\n{\n    public class TestClass\n    {\n        public string NewProperty { get; set; } = \"Updated\";\n    }\n}";
            var fileName = "TestClass.cs";
            var fullPath = Path.Combine(_testProjectPath, fileName);

            // Create the original file
            await File.WriteAllTextAsync(fullPath, originalContent, Encoding.UTF8);

            // Setup validation mocks
            _mockValidationService
                .Setup(v => v.ValidatePath(It.IsAny<string>()))
                .Callback<string>(path => { /* Path validation logic */ });

            _mockValidationService
                .Setup(v => v.ValidateContent(It.IsAny<string>()))
                .Callback<string>(content => { /* Content validation logic */ });

            _mockSecurityService
                .Setup( v => v.IsWriteContentSizeAllowed(It.IsAny<long>())).Returns(true);

            _mockValidationService
              .Setup(v => v.ValidateAndPrepare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .Returns(() => new ValidationContext {
                Project = _testProject,
                RelativePath = "",
                FullPath = Path.Combine(_testProjectPath, fileName)
              });

            // Act
            var result = await _fileService.UpdateFileAsync(_testProjectName, fileName, updatedContent);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Operation.Should().Be("update-project-file");
            result.Message.Should().Contain("File updated successfully");

            // Verify file content was actually updated
            var actualContent = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
            actualContent.Should().Be(updatedContent);

            // Verify backup was created
            var backupFiles = Directory.GetFiles(_testProjectPath, "*.backup.*");
            backupFiles.Should().HaveCount(1);
            
            var backupContent = await File.ReadAllTextAsync(backupFiles[0], Encoding.UTF8);
            backupContent.Should().Be(originalContent);

            // Verify result data
            var resultData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(
                System.Text.Json.JsonSerializer.Serialize(result.Data));
            
            var fileName_prop = resultData?.GetProperty("fileName").GetString();
            var path_prop = resultData?.GetProperty("path").GetString();
            var size_prop = resultData?.GetProperty("size").GetInt64();
            var backupCreated_prop = resultData?.GetProperty("backupCreated").GetBoolean();
      
        }
    }
}
