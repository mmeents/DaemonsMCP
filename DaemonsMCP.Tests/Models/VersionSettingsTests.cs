using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaemonsMCP.Core.Extensions;
using DaemonsMCP.Core.Models;

namespace DaemonsMCP.Tests.Models {

  [TestClass]
  public class VersionSettingsTests {

    public VersionSettingsTests() {
    }

    [TestMethod]
    public void VersionSettings_Defaults_AreSet() {
      var vs = new VersionSettings();
      Assert.IsNotNull(vs);
      Assert.AreEqual(Cx.AppName, vs.Name);
      Assert.AreEqual(Cx.AppVersion, vs.Version);
      Assert.AreEqual("Information", vs.LogLevel);      
      Assert.IsFalse(string.IsNullOrEmpty(vs.NodesFilePath));
    }

    [TestMethod]
    public void VersionSettings_CanBeSerializedAndDeserialized() {
      var vs = new VersionSettings();
      var json = System.Text.Json.JsonSerializer.Serialize(vs, Sx.DefaultJsonOptions);
      Assert.IsFalse(string.IsNullOrEmpty(json));
      Console.WriteLine("Serialized JSON: " + json);
      var deserialized = System.Text.Json.JsonSerializer.Deserialize<VersionSettings>(json, Sx.DefaultJsonOptions);
      Assert.IsNotNull(deserialized);
      Assert.AreEqual(vs.Name, deserialized!.Name);
      Assert.AreEqual(vs.Version, deserialized.Version);
      Assert.AreEqual(vs.LogLevel, deserialized.LogLevel);      
      Assert.AreEqual(vs.NodesFilePath, deserialized.NodesFilePath);
    }

  }
}
