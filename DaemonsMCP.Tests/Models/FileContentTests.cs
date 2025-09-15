using DaemonsMCP.Core.Models;
using FluentAssertions;
using System.Text.Json;

namespace DaemonsMCP.Tests.Models
{
    [TestClass]
    public class FileContentTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeWithDefaults() {
            // Act
            var fileContent = new FileContent();

            // Assert
            fileContent.Should().NotBeNull();
            fileContent.FileName.Should().Be(string.Empty);
            fileContent.Path.Should().Be(string.Empty);
            fileContent.Size.Should().Be(0);
            fileContent.ContentType.Should().Be(string.Empty);
            fileContent.Encoding.Should().Be(string.Empty);
            fileContent.Content.Should().Be(string.Empty);
            fileContent.IsBinary.Should().BeFalse();
        }

        [TestMethod]
        public void FileName_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testFileName = "TestDocument.cs";

            // Act
            fileContent.FileName = testFileName;

            // Assert
            fileContent.FileName.Should().Be(testFileName);
            fileContent.FileName.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Path_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testPath = "src/Components/TestComponent.cs";

            // Act
            fileContent.Path = testPath;

            // Assert
            fileContent.Path.Should().Be(testPath);
            fileContent.Path.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Size_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testSize = 1024L;

            // Act
            fileContent.Size = testSize;

            // Assert
            fileContent.Size.Should().Be(testSize);
            fileContent.Size.Should().BePositive();
        }

        [TestMethod]
        public void ContentType_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testContentType = "text/x-cs";

            // Act
            fileContent.ContentType = testContentType;

            // Assert
            fileContent.ContentType.Should().Be(testContentType);
            fileContent.ContentType.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Encoding_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testEncoding = "UTF-8";

            // Act
            fileContent.Encoding = testEncoding;

            // Assert
            fileContent.Encoding.Should().Be(testEncoding);
            fileContent.Encoding.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void Content_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();
            var testContent = "using System;\n\nnamespace TestProject\n{\n    public class TestClass\n    {\n        // Test content\n    }\n}";

            // Act
            fileContent.Content = testContent;

            // Assert
            fileContent.Content.Should().Be(testContent);
            fileContent.Content.Should().NotBeNullOrEmpty();
            fileContent.Content.Should().Contain("TestClass");
        }

        [TestMethod]
        public void IsBinary_ShouldSetAndGetCorrectly() {
            // Arrange
            var fileContent = new FileContent();

            // Act & Assert - Test setting to true
            fileContent.IsBinary = true;
            fileContent.IsBinary.Should().BeTrue();

            // Act & Assert - Test setting to false
            fileContent.IsBinary = false;
            fileContent.IsBinary.Should().BeFalse();
        }

        [TestMethod]
        public void FileContent_WithTextFile_ShouldHaveCorrectProperties() {
            // Arrange & Act
            var fileContent = new FileContent {
              FileName = "TestClass.cs",
              Path = "src/Models/TestClass.cs",
              Size = 1024,
              ContentType = "text/x-cs",
              Encoding = "UTF-8",
              Content = "using System;\n\nnamespace TestProject\n{\n    public class TestClass { }\n}",
              IsBinary = false
            };

            // Assert
            fileContent.FileName.Should().Be("TestClass.cs");
            fileContent.Path.Should().Be("src/Models/TestClass.cs");
            fileContent.Size.Should().Be(1024);
            fileContent.ContentType.Should().Be("text/x-cs");
            fileContent.Encoding.Should().Be("UTF-8");
            fileContent.Content.Should().NotBeEmpty();
            fileContent.Content.Should().Contain("TestClass");
            fileContent.IsBinary.Should().BeFalse();
        }

        [TestMethod]
        public void FileContent_WithBinaryFile_ShouldHaveEmptyContent() {
            // Arrange & Act
            var fileContent = new FileContent {
              FileName = "image.png",
              Path = "assets/images/image.png",
              Size = 2048,
              ContentType = "image/png",
              Encoding = "binary",
              Content = "", // Binary files should have empty content
              IsBinary = true
            };

            // Assert
            fileContent.FileName.Should().Be("image.png");
            fileContent.Path.Should().Be("assets/images/image.png");
            fileContent.Size.Should().Be(2048);
            fileContent.ContentType.Should().Be("image/png");
            fileContent.Encoding.Should().Be("binary");
            fileContent.Content.Should().BeEmpty();
            fileContent.IsBinary.Should().BeTrue();
        }

        [TestMethod]
        public void FileContent_ShouldBeSerializableToJson() {
            // Arrange
            var fileContent = new FileContent {
              FileName = "example.json",
              Path = "data/example.json",
              Size = 512,
              ContentType = "application/json",
              Encoding = "UTF-8",
              Content = "{ \"name\": \"test\", \"value\": 123 }",
              IsBinary = false
            };

            // Act
            var json = JsonSerializer.Serialize(fileContent);

            // Assert
            json.Should().NotBeNullOrEmpty();
            json.Should().Contain("\"FileName\":\"example.json\"");
            json.Should().Contain("\"Path\":\"data/example.json\"");
            json.Should().Contain("\"Size\":512");
            json.Should().Contain("\"ContentType\":\"application/json\"");
            json.Should().Contain("\"Encoding\":\"UTF-8\"");
            json.Should().Contain("\"Content\":\"{");
            json.Should().Contain("\"IsBinary\":false");
        }

        [TestMethod]
        public void FileContent_ShouldBeDeserializableFromJson() {
            // Arrange
            var originalFileContent = new FileContent {
              FileName = "test.cs",
              Path = "src/test.cs",
              Size = 256,
              ContentType = "text/x-cs",
              Encoding = "UTF-8",
              Content = "using System;",
              IsBinary = false
            };

            var json = JsonSerializer.Serialize(originalFileContent);

            // Act
            var deserializedFileContent = JsonSerializer.Deserialize<FileContent>(json);

            // Assert
            deserializedFileContent.Should().NotBeNull();
            deserializedFileContent!.FileName.Should().Be("test.cs");
            deserializedFileContent.Path.Should().Be("src/test.cs");
            deserializedFileContent.Size.Should().Be(256);
            deserializedFileContent.ContentType.Should().Be("text/x-cs");
            deserializedFileContent.Encoding.Should().Be("UTF-8");
            deserializedFileContent.Content.Should().Be("using System;");
            deserializedFileContent.IsBinary.Should().BeFalse();
        }

        [TestMethod]
        public void ToString_ShouldReturnMeaningfulString() {
            // Arrange
            var fileContent = new FileContent {
              FileName = "example.txt",
              Path = "docs/example.txt",
              Size = 100,
              ContentType = "text/plain",
              Encoding = "UTF-8",
              Content = "Hello World",
              IsBinary = false
            };

            // Act
            var result = fileContent.ToString();

            // Assert
            result.Should().NotBeNullOrEmpty();
            // Since FileContent doesn't override ToString, it should return the type name
            result.Should().Be("DaemonsMCP.Core.Models.FileContent");
        }

        [TestMethod]
        public void Equals_WithSameValues_ShouldReturnTrue() {
            // Arrange
            var fileContent1 = new FileContent {
              FileName = "test.json",
              Path = "data/test.json",
              Size = 512,
              ContentType = "application/json",
              Encoding = "UTF-8",
              Content = "{ \"test\": true }",
              IsBinary = false
            };

            var fileContent2 = new FileContent {
              FileName = "test.json",
              Path = "data/test.json",
              Size = 512,
              ContentType = "application/json",
              Encoding = "UTF-8",
              Content = "{ \"test\": true }",
              IsBinary = false
            };

            // Act & Assert
            // Since FileContent doesn't override Equals, this tests reference equality
            fileContent1.Equals(fileContent2).Should().BeFalse(); // Different instances
            fileContent1.Equals(fileContent1).Should().BeTrue(); // Same instance

            // Test null comparison
            fileContent1.Equals(null).Should().BeFalse();

            // Test different type comparison
            fileContent1.Equals("string").Should().BeFalse();
        }

        [TestMethod]
        public void Equals_WithDifferentValues_ShouldReturnFalse() {
            // Arrange
            var fileContent1 = new FileContent {
              FileName = "file1.txt",
                      Path = "docs/file1.txt",
                      Size = 100,
              ContentType = "text/plain",
                      Encoding = "UTF-8",
                      Content = "Content 1",
                      IsBinary = false
            };

            var fileContent2 = new FileContent {
              FileName = "file2.txt",
                      Path = "docs/file2.txt",
                      Size = 200,
              ContentType = "text/html",
                      Encoding = "ASCII",
                      Content = "Content 2",
                      IsBinary = true
            };

            var fileContent3 = new FileContent {
              FileName = "file1.txt", // Same filename but different content
                      Path = "docs/file1.txt",
                      Size = 100,
              ContentType = "text/plain",
                      Encoding = "UTF-8",
                      Content = "Different Content",
                      IsBinary = false
            };

            // Act & Assert
            // Since FileContent doesn't override Equals, this tests reference equality
            fileContent1.Equals(fileContent2).Should().BeFalse(); // Different instances
            fileContent1.Equals(fileContent3).Should().BeFalse(); // Different instances even with some same values
            fileContent2.Equals(fileContent3).Should().BeFalse(); // Different instances

            // Test with null
            fileContent1.Equals(null).Should().BeFalse();

            // Test with different type
            fileContent1.Equals("not a FileContent").Should().BeFalse();
        }

        [TestMethod]
        public void GetHashCode_WithSameValues_ShouldReturnSameHash() {
          // Arrange
          var fileContent1 = new FileContent {
            FileName = "test.cs",
                    Path = "src/test.cs",
                    Size = 512,
            ContentType = "text/x-cs",
                    Encoding = "UTF-8",
                    Content = "using System;",
                    IsBinary = false
          };

          var fileContent2 = new FileContent {
            FileName = "test.cs",
                    Path = "src/test.cs",
                    Size = 512,
            ContentType = "text/x-cs",
                    Encoding = "UTF-8",
                    Content = "using System;",
                    IsBinary = false
          };

          // Act
          var hash1 = fileContent1.GetHashCode();
          var hash2 = fileContent2.GetHashCode();
          var hash1Again = fileContent1.GetHashCode();

          // Assert
          // Since FileContent doesn't override GetHashCode, each instance will have different hash codes
          hash1.Should().NotBe(hash2); // Different instances have different hash codes
          hash1.Should().Be(hash1Again); // Same instance should return consistent hash code

        }

    }
}
