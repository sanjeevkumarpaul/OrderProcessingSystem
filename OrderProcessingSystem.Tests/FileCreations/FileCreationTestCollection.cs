using Xunit;

namespace OrderProcessingSystem.Tests.FileCreations;

/// <summary>
/// Collection definition to prevent file creation tests from running in parallel
/// This prevents race conditions when multiple tests write to the same BlobStorageSimulation files
/// </summary>
[CollectionDefinition("FileCreation Tests")]
public class FileCreationTestCollection
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
