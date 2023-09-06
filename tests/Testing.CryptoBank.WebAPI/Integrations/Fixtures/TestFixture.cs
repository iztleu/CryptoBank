﻿using CryptoBank.WebAPI.Database;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Testing.CryptoBank.WebAPI.Integrations.Common.Harnesses;
using Testing.CryptoBank.WebAPI.Integrations.Common.Harnesses.Base;
using Testing.CryptoBank.WebAPI.Integrations.Common.Helpers;

namespace Testing.CryptoBank.WebAPI.Integrations.Fixtures;

public class TestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestFixture()
    {
        Database = new();
        HttpClient = new();

        _factory = new WebApplicationFactory<Program>()
            .AddHarness(Database)
            .AddHarness(HttpClient);
    }

    public WebApplicationFactory<Program> Factory => _factory;
    public DatabaseHarness<Program, AppDbContext> Database { get; }
    public HttpClientHarness<Program> HttpClient { get; }

    public async Task InitializeAsync()
    {
        await Database.Start(_factory, Create.CancellationToken(60));
        await HttpClient.Start(_factory, Create.CancellationToken());

        _ = _factory.Server;

        AssertionOptions.AssertEquivalencyUsing(options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds()))
            .WhenTypeIs<DateTime>()
            .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds()))
            .WhenTypeIs<DateTimeOffset>());
    }

    public async Task DisposeAsync()
    {
        await HttpClient.Stop(Create.CancellationToken());
        await Database.Stop(Create.CancellationToken());
    }
}
