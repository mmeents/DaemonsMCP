using DaemonsMCP.Core.Models;
using FluentAssertions;
using System.Text.Json;

namespace DaemonsMCP.Tests.Models
{
    [TestClass]
    public class OperationResultTests
    {
      
        [TestMethod]
        public void CreateSuccess_WithValidParameters_ShouldReturnSuccessResult() {
          // Arrange
          var operation = "test-operation";
          var message = "Operation completed successfully";
          var data = new { id = 1, name = "test" };

          // Act
          var result = OperationResult.CreateSuccess(operation, message, data);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeTrue();
          result.Operation.Should().Be(operation);
          result.Message.Should().Be(message);
          result.Data.Should().Be(data);
          result.ErrorMessage.Should().BeNull();
          result.Exception.Should().BeNull();
        }

        [TestMethod]
        public void CreateSuccess_WithNullOperation_ShouldThrowArgumentException() {
          // Arrange
          string nullOperation = null;
          var message = "Test message";

          // Act & Assert
          var act = () => OperationResult.CreateSuccess(nullOperation, message);
          act.Should().Throw<ArgumentException>()
             .WithMessage("*operation*");
        }

        [TestMethod]
        public void CreateSuccess_WithEmptyOperation_ShouldThrowArgumentException() {
          // Arrange
          var emptyOperation = string.Empty;
          var message = "Test message";

          // Act & Assert
          var act = () => OperationResult.CreateSuccess(emptyOperation, message);
          act.Should().Throw<ArgumentException>()
             .WithMessage("*operation*");
        }

        [TestMethod]
        public void CreateSuccess_WithData_ShouldIncludeDataInResult() {
          // Arrange
          var operation = "test-operation";
          var message = "Operation successful";
          var testData = new {
            fileName = "test.txt",
            size = 1024,
            created = DateTime.Now
          };

          // Act
          var result = OperationResult.CreateSuccess(operation, message, testData);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeTrue();
          result.Data.Should().Be(testData);
          result.Data.Should().BeEquivalentTo(testData);
        }

        [TestMethod]
        public void CreateSuccess_WithoutData_ShouldHaveNullData() {
          // Arrange
          var operation = "test-operation";
          var message = "Operation completed without data";

          // Act
          var result = OperationResult.CreateSuccess(operation, message);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeTrue();
          result.Operation.Should().Be(operation);
          result.Message.Should().Be(message);
          result.Data.Should().BeNull();
          result.ErrorMessage.Should().BeNull();
          result.Exception.Should().BeNull();
        }

        [TestMethod]
        public void CreateFailure_WithValidParameters_ShouldReturnFailureResult() {
          // Arrange
          var operation = "test-operation";
          var errorMessage = "Operation failed due to invalid input";
          var exception = new InvalidOperationException("Test exception");

          // Act
          var result = OperationResult.CreateFailure(operation, errorMessage, exception);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeFalse();
          result.Operation.Should().Be(operation);
          result.ErrorMessage.Should().Be(errorMessage);
          result.Exception.Should().Be(exception);
          result.Message.Should().BeEmpty();
          result.Data.Should().BeNull();
        }

        [TestMethod]
        public void CreateFailure_WithException_ShouldIncludeException() {
          // Arrange
          var operation = "file-read";
          var errorMessage = "Failed to read file";
          var testException = new FileNotFoundException("The specified file was not found", "test.txt");

          // Act
          var result = OperationResult.CreateFailure(operation, errorMessage, testException);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeFalse();
          result.Exception.Should().Be(testException);
          result.Exception.Should().BeOfType<FileNotFoundException>();
          result.Exception.Message.Should().Be("The specified file was not found");
          result.Operation.Should().Be(operation);
          result.ErrorMessage.Should().Be(errorMessage);
        }

        [TestMethod]
        public void CreateFailure_WithoutException_ShouldHaveNullException() {
          // Arrange
          var operation = "validation-check";
          var errorMessage = "Validation failed - missing required field";

          // Act
          var result = OperationResult.CreateFailure(operation, errorMessage);

          // Assert
          result.Should().NotBeNull();
          result.Success.Should().BeFalse();
          result.Operation.Should().Be(operation);
          result.ErrorMessage.Should().Be(errorMessage);
          result.Exception.Should().BeNull();
          result.Message.Should().BeEmpty();
          result.Data.Should().BeNull();
        }

        [TestMethod]
        public void CreateFailure_WithNullOperation_ShouldThrowArgumentException() {
          // Arrange
          string nullOperation = null;
          var errorMessage = "Operation failed";

          // Act & Assert
          var act = () => OperationResult.CreateFailure(nullOperation, errorMessage);
          act.Should().Throw<ArgumentException>()
             .WithMessage("*operation*");
        }

        [TestMethod]
        public void CreateFailure_WithNullErrorMessage_ShouldThrowArgumentException() {
          // Arrange
          var operation = "test-operation";
          string nullErrorMessage = null;

          // Act & Assert
          var act = () => OperationResult.CreateFailure(operation, nullErrorMessage);
          act.Should().Throw<ArgumentException>()
             .WithMessage("*errorMessage*");
        }

        [TestMethod]
        public void Success_WithSuccessResult_ShouldReturnTrue() {
          // Arrange
          var operation = "file-create";
          var message = "File created successfully";

          // Act
          var result = OperationResult.CreateSuccess(operation, message);

          // Assert
          result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Success_WithFailureResult_ShouldReturnFalse() {
          // Arrange
          var operation = "file-delete";
          var errorMessage = "File not found";

          // Act
          var result = OperationResult.CreateFailure(operation, errorMessage);

          // Assert
          result.Success.Should().BeFalse();
        }

        [TestMethod]
        public void Operation_ShouldReturnCorrectValue() {
          // Arrange
          var expectedOperation = "directory-create";

          // Act - Test with success result
          var successResult = OperationResult.CreateSuccess(expectedOperation, "Directory created");
          var failureResult = OperationResult.CreateFailure(expectedOperation, "Directory creation failed");

          // Assert
          successResult.Operation.Should().Be(expectedOperation);
          failureResult.Operation.Should().Be(expectedOperation);
        }

        [TestMethod]
        public void Message_ShouldReturnCorrectValue() {
          // Arrange
          var operation = "file-update";
          var expectedMessage = "File updated successfully with new content";

          // Act
          var result = OperationResult.CreateSuccess(operation, expectedMessage);

          // Assert
          result.Message.Should().Be(expectedMessage);
          result.Message.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void ErrorMessage_OnSuccessResult_ShouldBeNull() {
          // Arrange
          var operation = "backup-create";
          var message = "Backup created successfully";

          // Act
          var result = OperationResult.CreateSuccess(operation, message);

          // Assert
          result.ErrorMessage.Should().BeNull();
          result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void ErrorMessage_OnFailureResult_ShouldReturnErrorMessage() {
          // Arrange
          var operation = "config-load";
          var expectedErrorMessage = "Configuration file not found or invalid format";
          var exception = new FileNotFoundException("Config file missing");

          // Act
          var result = OperationResult.CreateFailure(operation, expectedErrorMessage, exception);

          // Assert
          result.ErrorMessage.Should().Be(expectedErrorMessage);
          result.ErrorMessage.Should().NotBeNullOrEmpty();
          result.Success.Should().BeFalse();
        }

        [TestMethod]
        public void Exception_OnSuccessResult_ShouldBeNull() {
          // Arrange
          var operation = "data-export";
          var message = "Data exported successfully to CSV";

          // Act
          var result = OperationResult.CreateSuccess(operation, message);

          // Assert
          result.Exception.Should().BeNull();
          result.Success.Should().BeTrue();
        }

        [TestMethod]
        public void Exception_OnFailureResult_ShouldReturnException() {
          // Arrange
          var operation = "network-connect";
          var errorMessage = "Failed to establish network connection";
          var expectedException = new TimeoutException("Connection timed out after 30 seconds");

          // Act
          var result = OperationResult.CreateFailure(operation, errorMessage, expectedException);

          // Assert
          result.Exception.Should().Be(expectedException);
          result.Exception.Should().BeOfType<TimeoutException>();
          result.Exception.Message.Should().Be("Connection timed out after 30 seconds");
          result.Success.Should().BeFalse();
        }

        [TestMethod]
        public void Data_ShouldReturnCorrectValue() {
          // Arrange
          var operation = "database-query";
          var message = "Query executed successfully";
          var expectedData = new {
            recordCount = 42,
            executionTime = TimeSpan.FromMilliseconds(150),
            tableName = "Users",
            queryType = "SELECT"
          };

          // Act
          var result = OperationResult.CreateSuccess(operation, message, expectedData);

          // Assert
          result.Data.Should().Be(expectedData);
          result.Data.Should().BeEquivalentTo(expectedData);
          result.Data.Should().NotBeNull();
        }

        [TestMethod]
        public void OperationResult_ShouldBeSerializableToJson() {
          // Arrange
          var operation = "file-backup";
          var message = "Backup completed successfully";
          var data = new { fileCount = 25, totalSize = "1.2GB" };
          var result = OperationResult.CreateSuccess(operation, message, data);

          // Act
          var json = JsonSerializer.Serialize(result);

          // Assert
          json.Should().NotBeNullOrEmpty();
          json.Should().Contain("file-backup");
          json.Should().Contain("Backup completed successfully");
          json.Should().Contain("fileCount");
          json.Should().Contain("25");
          json.Should().Contain("1.2GB");
        }

        [TestMethod]
        public void OperationResult_ShouldBeDeserializableFromJson() {
          // Arrange
          var originalOperation = "config-validate";
          var originalErrorMessage = "Invalid configuration detected";
          var originalResult = OperationResult.CreateFailure(originalOperation, originalErrorMessage);
          var json = JsonSerializer.Serialize(originalResult);

          // Act
          var deserializedResult = JsonSerializer.Deserialize<OperationResult>(json);

          // Assert
          deserializedResult.Should().NotBeNull();
          deserializedResult.Success.Should().BeFalse();
          deserializedResult.Operation.Should().Be(originalOperation);
          deserializedResult.ErrorMessage.Should().Be(originalErrorMessage);          
          deserializedResult.Data.Should().BeNull();
        }

        [TestMethod]
        public void ToString_ShouldReturnMeaningfulString()
        {
            // Arrange
            var successResult = OperationResult.CreateSuccess("file-compress", "Compression completed");
            var failureResult = OperationResult.CreateFailure("network-sync", "Connection timeout");

            // Act
            var successString = successResult.ToString();
            var failureString = failureResult.ToString();

            // Assert
            successString.Should().NotBeNullOrEmpty();
            successString.Should().Contain("file-compress");
            successString.Should().Contain("true");
            
            failureString.Should().NotBeNullOrEmpty();
            failureString.Should().Contain("network-sync");
            failureString.Should().Contain("false");
        }
       

        [TestMethod]
        public void Equals_WithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var result1 = OperationResult.CreateSuccess("process-data", "Processing complete");
            var result2 = OperationResult.CreateSuccess("process-data", "Processing failed");
            var result3 = OperationResult.CreateFailure("different-operation", "Error occurred");
            var result4 = OperationResult.CreateSuccess("different-operation", "Processing complete");

            // Act & Assert
            result1.Equals(result2).Should().BeFalse();
            result1.Equals(result3).Should().BeFalse();
            result1.Equals(result4).Should().BeFalse();
            (result1 == result2).Should().BeFalse();
            result1.Should().NotBe(result2);
            result1.Equals(null).Should().BeFalse();
        }

    }
}