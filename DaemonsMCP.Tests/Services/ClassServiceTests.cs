using DaemonsMCP.Core.Config;
using DaemonsMCP.Core.Models;
using DaemonsMCP.Core.Repositories;
using DaemonsMCP.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace DaemonsMCP.Tests.Services {

  [TestClass]
  public class ClassServiceTests {
  
    private readonly IClassService? _classService;    
    private readonly Mock<ILoggerFactory>? _mockLoggerFactory = new();
    private readonly Mock<IIndexRepository>? _mockIndexRepository = new();
    private readonly Mock<IProjectRepository>? _mockProjectRepository = new();
    private readonly Mock<ISettingsRepository>? _mockSettingsRepository = new();

    private readonly IClassService? _classService2;
    private readonly IIndexRepository? _indexRepository;
    private readonly ISecurityService _securityService;
    private readonly Mock<IValidationService>? _mockValidationService = new();
    private readonly IValidationService _validationService ;
    private readonly Mock<ILogger<AppConfig>> _appConfigLogger = new();

    public ClassServiceTests() { 
      _mockLoggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(_appConfigLogger.Object);
      
      _classService = new ClassService( _mockLoggerFactory.Object, _mockIndexRepository.Object, _mockValidationService.Object );
      var appConfig = new App2Config( _mockLoggerFactory.Object, _mockProjectRepository.Object, _mockSettingsRepository.Object );
      _securityService = new SecurityService(_mockLoggerFactory.Object, appConfig);
      _indexRepository = new IndexRepository(_mockLoggerFactory.Object, appConfig, _mockValidationService.Object, _securityService);
      _validationService = new ValidationService(appConfig, _securityService);
      _classService2 = new ClassService( _mockLoggerFactory.Object, _indexRepository, _validationService);
    }

    [TestMethod]
    public async Task GetClassesAsync_ShouldReturnClasses() {
      // Arrange
      var projectName = "TestProject";
      var classes = new List<ClassListing> {
        new ClassListing { ClassName = "Class1", Namespace = "Namespace1", FileNamePath = "Path1" },
        new ClassListing { ClassName = "Class2", Namespace = "Namespace2", FileNamePath = "Path2" }
      };
      _mockIndexRepository.Setup(repo => repo.GetClassListingsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>() ))
                          .ReturnsAsync(classes);
      // Act
      var result = await _classService.GetClassesAsync(projectName,1, 20, classNameFilter:"Class2").ConfigureAwait(false);
      // Assert
      Assert.IsNotNull(result);
      List<ClassListing> resTest = (List<ClassListing>)(result.Data); 
      Assert.AreEqual(2, resTest.Count());
      Assert.AreEqual("Class1", resTest[0].ClassName);
      Assert.AreEqual("Class2", resTest[1].ClassName);

    }

    [TestMethod]
    public async Task GetClassesAsync_ShouldReturnClassesFromConfig() { 

      var result = await _classService2.GetClassesAsync("DaemonsMCP", 1, 20).ConfigureAwait(false);
     

      // Assert  testing inspected 30 total classes in DaemonsMCP1 project
      Assert.IsNotNull(result);
      List<ClassListing> resTest = (List<ClassListing>)(result.Data); 
      Assert.AreEqual(20, resTest.Count());         

    }

    [TestMethod]
    public async Task GetClassesAsync_ShouldReturnClassesFromConfig_TestPaging() {

      var result = await _classService2.GetClassesAsync("DaemonsMCP", 2, 25).ConfigureAwait(false);
      
      // Assert  testing inspected 30 total classes in DaemonsMCP1 project
      Assert.IsNotNull(result);
      List<ClassListing> resTest = (List<ClassListing>)(result.Data);
      Assert.AreEqual(25, resTest.Count());     

    }

    [TestMethod]
    public async Task GetClassesAsync_ShouldReturnClassesFromConfig_TestFilter() {

      var result = await _classService2.GetClassesAsync("PackedTables.NET", 1, 100, namespaceFilter: "PackedTables.NET").ConfigureAwait(false);

      var result2 = await _classService2.GetClassesAsync("DaemonsMCP", 1, 100, classNameFilter: "ClassServiceTests").ConfigureAwait(false);

      // Assert  testing inspected 30 total classes in DaemonsMCP1 project
      Assert.IsNotNull(result);
      List<ClassListing> resTest = (List<ClassListing>)(result.Data);
      List<ClassListing> res2Test = (List<ClassListing>)(result2.Data);


    }

    // ClassServiceTests

    [TestMethod]
    public async Task GetClassContentAsync_ShouldReturnClassContent() { 

      var result = await _classService2.GetClassContentAsync("DaemonsMCP",156);
      var resultJson = JsonSerializer.Serialize(result);
      Console.Write(result);

      Assert.IsTrue(result.Success);

    }

  }
}
