using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Services;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog;

namespace DaemonsMCP.Tests.Services
{
    [TestClass]
    public class ProjectServiceTests
    {
        private IAppConfig _config;
        private Mock<IAppConfig> _mockConfig = new Mock<IAppConfig>();
        private readonly Mock<IItemRepository> _mockItemRepository = new();
        private IItemRepository _itemRepository;
        private ProjectService _projectService;        
        private Dictionary<string, ProjectModel> _testProjects;
        private readonly ILoggerFactory _loggerFactory;
        private ProjectService _service;
        private readonly IValidationService _validationService;
        private readonly ISecurityService _securityService;


        public ProjectServiceTests()
        {
          //var testLogsPath = Path.Combine(Path.GetTempPath(), "DaemonsMCP-Tests", "logs");
          //Directory.CreateDirectory(testLogsPath);

          var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
          Directory.CreateDirectory(logsPath);

          Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning() // Adjust as needed
            .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning)) // Reduce Microsoft logging noise
            .MinimumLevel.Override("System", new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Warning))
            .Enrich.FromLogContext()
            .WriteTo.File(
               path: Path.Combine(logsPath, "daemons-.log"),
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 7,
               outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File(
               path: Path.Combine(logsPath, "daemons-errors-.log"),
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 30,
               restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, // Only warnings and errors
               outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

          _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));      // Constructor logic if needed
            _config = new AppConfig(_loggerFactory);
            _securityService = new SecurityService(_loggerFactory, _config);
            _validationService = new ValidationService(_config, _securityService);
            _itemRepository =  new ItemRepository( _config, _loggerFactory);
            _service = new ProjectService(_config, _itemRepository);

            _testProjects = new Dictionary<string, ProjectModel> {
              ["Project1"] = new ProjectModel("Project1", "First test project", "C:\\TestProjects\\Project1"),
              ["Project2"] = new ProjectModel("Project2", "Second test project", "C:\\TestProjects\\Project2"),
              ["DisabledProject"] = new ProjectModel("DisabledProject", "Disabled project", "C:\\TestProjects\\Disabled")
            };
            _mockConfig.Setup(c => c.Projects).Returns(_testProjects);
            _projectService = new ProjectService(_mockConfig.Object, _mockItemRepository.Object);

        }

    [TestInitialize]
        public void TestInitialize()
        {

        }

    #region GetProjectsAsync Tests

        [TestMethod]
        public async Task GetProjectsAsync_WithConfiguredProjects_ShouldReturnAllProjects()
        {
            // TODO: Implement test for retrieving all configured projects
        }

        [TestMethod]
        public async Task GetProjectsAsync_WithEmptyConfiguration_ShouldReturnEmptyCollection()
        {
            // TODO: Implement test for empty project configuration
        }

        [TestMethod]
        public async Task GetProjectsAsync_WithNullConfiguration_ShouldReturnEmptyCollection()
        {
            // TODO: Implement test for null project configuration
        }

        [TestMethod]
        public async Task GetProjectsAsync_ShouldReturnProjectsWithCorrectProperties()
        {
            // TODO: Implement test for project property validation
        }

        [TestMethod]
        public async Task GetProjectsAsync_ShouldNotReturnDisabledProjects()
        {
            // TODO: Implement test for filtering disabled projects
        }

        [TestMethod]
        public void GetProjectByName_WithValidName_ShouldReturnProject()
        {
            // TODO: Implement test for valid project name lookup
        }

        [TestMethod]
        public void GetProjectByName_WithInvalidName_ShouldReturnNull()
        {
            // TODO: Implement test for invalid project name lookup
        }

        [TestMethod]
        public void GetProjectByName_WithNullName_ShouldReturnNull()
        {
            // TODO: Implement test for null project name
        }

        [TestMethod]
        public void GetProjectByName_WithEmptyName_ShouldReturnNull()
        {
            // TODO: Implement test for empty project name
        }

        [TestMethod]
        public void GetProjectByName_WithCaseSensitiveName_ShouldHandleCorrectly()
        {
            // TODO: Implement test for case sensitivity in project names
        }

        [TestMethod]
        public void IsProjectEnabled_WithEnabledProject_ShouldReturnTrue()
        {
            // TODO: Implement test for enabled project check
        }

        [TestMethod]
        public void IsProjectEnabled_WithDisabledProject_ShouldReturnFalse()
        {
            // TODO: Implement test for disabled project check
        }

        [TestMethod]
        public void IsProjectEnabled_WithNonExistentProject_ShouldReturnFalse()
        {
            // TODO: Implement test for non-existent project check
        }

        [TestMethod]
        public void ValidateProjectExists_WithValidProject_ShouldNotThrow()
        {
            // TODO: Implement test for valid project existence validation
        }

        [TestMethod]
        public void ValidateProjectExists_WithInvalidProject_ShouldThrowArgumentException()
        {
            // TODO: Implement test for invalid project existence validation
        }

        [TestMethod]
        public void GetProjectCount_ShouldReturnCorrectCount()
        {
            // TODO: Implement test for project count retrieval
        }

        [TestMethod]
        public async Task GetProjectsAsync_ShouldReturnReadOnlyCollection()
        {
            // TODO: Implement test for read-only project collection
        }
    #endregion


        [TestMethod]
        public async Task GetItemTypes_WithValidProject_ShouldReturnItemTypes() {
          var projects = await _service.GetProjectsAsync().ConfigureAwait(false);
          foreach (var project in projects) {


            var results3 = await _service.GetNodes(project.Name, nodeId:null, maxDepth:2).ConfigureAwait(false);
            results3.Should().NotBeNull();


          }



        }


    }
}