namespace DaemonsMCP.Tests {
  [TestClass]
  public sealed class Test1 {
    [TestMethod]
    public void FolderPathTests() {

        // Arrange
        string[] paths = {
            "C:\\Users\\User\\Documents\\Project",
            "/home/user/documents/project",
            "C:/Users/User//Documents/Project",
            "C:\\Users\\User\\\\Documents/Project",
            "/home/user/documents\\project"
        };
    
        // Act & Assert
        foreach (var path in paths) {
            var fullPath = Path.GetFullPath(path);
            Console.WriteLine($"Full path: {fullPath}");
       
        }
        Assert.IsTrue(true);
    }
  }
}
