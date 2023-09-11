
using Testing.CryptoBank.WebAPI.Integrations.Fixtures;

namespace Testing.CryptoBank.WebAPI.Integrations.Features;

[CollectionDefinition(Name)]
public class UsersTestsCollection : ICollectionFixture<TestFixture>
{
    public const string Name = nameof(UsersTestsCollection);

    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

