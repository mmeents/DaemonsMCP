using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonsMCP.xUnit.Tests.Git {
  public class ExistingRepositoryTests {
    // Point this at an actual local repo you have
    private const string ExistingRepoPath = @"C:\MCPSandbox\DaemonsMCP";

    [Fact]
    public void CanOpenExistingRepository() {
      // Test opening a repo that's already cloned
      Assert.True(Repository.IsValid(ExistingRepoPath));

      using var repo = new Repository(ExistingRepoPath);

      Assert.NotNull(repo);
      Assert.NotNull(repo.Head);
    }

    [Fact]
    public void CanGetRemoteInformation() {
      using var repo = new Repository(ExistingRepoPath);

      var remotes = repo.Network.Remotes.ToList();

      Assert.NotEmpty(remotes);

      foreach (var remote in remotes) {
        Console.WriteLine($"Remote: {remote.Name}");
        Console.WriteLine($"  URL: {remote.Url}");
        Console.WriteLine($"  Push URL: {remote.PushUrl}");
      }
    }

    [Fact]
    public void CanListLocalAndRemoteBranches() {
      using var repo = new Repository(ExistingRepoPath);

      var localBranches = repo.Branches
          .Where(b => !b.IsRemote)
          .ToList();

      var remoteBranches = repo.Branches
          .Where(b => b.IsRemote)
          .ToList();

      Console.WriteLine($"Local Branches: {localBranches.Count}");
      foreach (var b in localBranches) {
        Console.WriteLine($"  - {b.FriendlyName}");
      }

      Console.WriteLine($"Remote Branches: {remoteBranches.Count}");
      foreach (var b in remoteBranches.Take(10)) // Just first 10
      {
        Console.WriteLine($"  - {b.FriendlyName}");
      }

      Assert.NotEmpty(localBranches);
      Assert.NotEmpty(remoteBranches);
    }

    [Fact]
    public void CanDetectDirtyWorkingDirectory() {
      using var repo = new Repository(ExistingRepoPath);      
      var status = repo.RetrieveStatus();
      
      Console.WriteLine($"Is Clean: {!status.IsDirty}");
      Console.WriteLine($"Modified: {status.Modified.Count()}");
      Console.WriteLine($"Added: {status.Added.Count()}");
      Console.WriteLine($"Removed: {status.Removed.Count()}");
      Console.WriteLine($"Untracked: {status.Untracked.Count()}");

      // This will vary based on your repo state, so just verify we can call it
      Assert.NotNull(status);
    }
  }
}
