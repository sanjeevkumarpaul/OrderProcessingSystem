using Microsoft.Extensions.Logging;
using Moq;
using OrderProcessingSystem.Utilities.Helpers;
using Xunit;

namespace OrderProcessingSystem.Tests.Utilities;

public class FileHelperTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileHelperTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        FileHelper.EnsureDirectoryExists(_tempDirectory);
    }

    [Fact]
    public async Task WriteFileAsync_Should_CreateFileWithContent()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "test.txt");
        var content = "Test content";
        var mockLogger = new Mock<ILogger>();

        // Act
        await FileHelper.WriteFileAsync(filePath, content, mockLogger.Object);

        // Assert
        Assert.True(File.Exists(filePath));
        var actualContent = await File.ReadAllTextAsync(filePath);
        Assert.Equal(content, actualContent);
    }

    [Fact]
    public async Task WriteFileAsync_Should_CreateDirectoryIfNotExists()
    {
        // Arrange
        var subdirectory = Path.Combine(_tempDirectory, "subdir");
        var filePath = Path.Combine(subdirectory, "test.txt");
        var content = "Test content";

        // Act
        await FileHelper.WriteFileAsync(filePath, content);

        // Assert
        Assert.True(Directory.Exists(subdirectory));
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task WriteJsonFileAsync_Should_WriteToCorrectPath()
    {
        // Arrange
        var fileName = "test.json";
        var jsonContent = "{\"test\": \"value\"}";
        var mockLogger = new Mock<ILogger>();

        // Act
        await FileHelper.WriteJsonFileAsync(_tempDirectory, fileName, jsonContent, mockLogger.Object);

        // Assert
        var expectedPath = Path.Combine(_tempDirectory, fileName);
        Assert.True(File.Exists(expectedPath));
        var actualContent = await File.ReadAllTextAsync(expectedPath);
        Assert.Equal(jsonContent, actualContent);
    }

    [Fact]
    public async Task ReadFileAsync_Should_ReturnFileContent()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "read-test.txt");
        var expectedContent = "Content to read";
        await File.WriteAllTextAsync(filePath, expectedContent);

        // Act
        var actualContent = await FileHelper.ReadFileAsync(filePath);

        // Assert
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public async Task ReadFileAsync_WithNonExistentFile_Should_ThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_tempDirectory, "does-not-exist.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => FileHelper.ReadFileAsync(nonExistentPath));
    }

    [Fact]
    public void FileExists_Should_ReturnTrueForExistingFile()
    {
        // Arrange
        var filePath = Path.Combine(_tempDirectory, "exists-test.txt");
        File.WriteAllText(filePath, "content");

        // Act
        var result = FileHelper.FileExists(filePath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FileExists_Should_ReturnFalseForNonExistentFile()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_tempDirectory, "does-not-exist.txt");

        // Act
        var result = FileHelper.FileExists(nonExistentPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FindSolutionRoot_Should_FindSolutionFile()
    {
        // Arrange
        var solutionDir = Path.Combine(_tempDirectory, "solution");
        FileHelper.EnsureDirectoryExists(solutionDir);
        var solutionFile = Path.Combine(solutionDir, "TestSolution.sln");
        File.WriteAllText(solutionFile, "solution content");

        var projectDir = Path.Combine(solutionDir, "Project");
        FileHelper.EnsureDirectoryExists(projectDir);

        // Act
        var result = FileHelper.FindSolutionRoot(projectDir, "TestSolution.sln");

        // Assert
        Assert.Equal(solutionDir, result);
    }

    [Fact]
    public void FindSolutionRoot_WithNoSolutionFile_Should_ThrowDirectoryNotFoundException()
    {
        // Arrange
        var projectDir = Path.Combine(_tempDirectory, "no-solution");
        FileHelper.EnsureDirectoryExists(projectDir);

        // Act & Assert
        Assert.Throws<DirectoryNotFoundException>(() => FileHelper.FindSolutionRoot(projectDir, "NonExistent.sln"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
