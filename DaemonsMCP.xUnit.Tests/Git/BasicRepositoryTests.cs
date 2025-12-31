using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DaemonsMCP.xUnit.Tests.Git {
  // DaemonsMCP.xUnit.Tests/Git/BasicRepositoryTests.cs
  public class BasicRepositoryTests : IDisposable {
    private readonly string _testRepoPath;
    private readonly string _testRepoUrl = "https://github.com/libgit2/TestGitRepository.git"; // tiny test repo

    public BasicRepositoryTests() {
      _testRepoPath = Path.Combine(Path.GetTempPath(), $"git-test-{Guid.NewGuid()}");
    }

    [Fact]
    public void CanCloneRepository() {
      
      var clonePath = Repository.Clone(_testRepoUrl, _testRepoPath);

      Assert.StartsWith(_testRepoPath, clonePath, StringComparison.OrdinalIgnoreCase);
      Assert.True(Directory.Exists(_testRepoPath));
      Assert.True(Repository.IsValid(_testRepoPath));
    }

    [Fact]
    public void CanListBranches() {

      Repository.Clone(_testRepoUrl, _testRepoPath);

      var repo = new Repository(_testRepoPath);
      try {

        // Get all branches
        var branches = repo.Branches.ToList();

        // Get current branch
        var currentBranch = repo.Head;

        Assert.NotEmpty(branches);
        Assert.NotNull(currentBranch);

        // Output for manual verification
        foreach (var branch in branches) {
          Console.WriteLine($"Branch: {branch.FriendlyName}, " +
                          $"IsRemote: {branch.IsRemote}, " +
                          $"IsTracking: {branch.IsTracking}, " +
                          $"IsCurrentRepositoryHead: {branch.IsCurrentRepositoryHead}");
        }


      } finally { 
        repo.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
      }
      
    }

    [Fact]
    public void CanIdentifyCurrentBranch() {
      Repository.Clone(_testRepoUrl, _testRepoPath);

      using var repo = new Repository(_testRepoPath);
      var currentBranch = repo.Head;

      Assert.NotNull(currentBranch.FriendlyName);
      Assert.True(currentBranch.IsCurrentRepositoryHead);
    }

    public void Dispose() {
      if (!Directory.Exists(_testRepoPath)) {
        return;
      }

      // Help ensure native handles are released
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      if (Directory.Exists(_testRepoPath)) {
        DeleteDirectory(_testRepoPath);
      }

    }

    private static void DeleteDirectory(string path) {
      const int maxRetries = 10;
      const int delayMs = 100;

      for (int i = 0; i < maxRetries; i++) {
        try {
          // Remove read-only attributes that Git sets
          var directory = new DirectoryInfo(path);
          SetAttributesNormal(directory);

          Directory.Delete(path, recursive: true);
          return; // Success!
        } catch (IOException) when (i < maxRetries - 1) {
          // File handles still open, wait and retry
          Thread.Sleep(delayMs * (i + 1)); // Exponential backoff
        } catch (UnauthorizedAccessException) {
          // Try to fix permissions
          var directory = new DirectoryInfo(path);
          SetAttributesNormal(directory);
          Thread.Sleep(delayMs);
        }
      }
    }

    private static void SetAttributesNormal(DirectoryInfo dir) {
      foreach (var subDir in dir.GetDirectories()) {
        SetAttributesNormal(subDir);
      }

      foreach (var file in dir.GetFiles()) {
        file.Attributes = FileAttributes.Normal;
      }

      dir.Attributes = FileAttributes.Normal;
    }
  }
}
