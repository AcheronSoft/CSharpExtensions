namespace CSharpExtensions.Tests.Base;

[CollectionDefinition(nameof(SharedFixture))]
public class SharedFixtureCollection : ICollectionFixture<SharedFixture>
{ }