using Docker.Registry.DotNet.Application.Authentication;

using FluentAssertions;

using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests.Authentication;

[TestFixture]
public class AuthenticateParserTests
{
    [TestCase("realm=test realm,service=test service,scope=test scope", "test realm", "test service", "test scope")]
    [TestCase(
        "realm=\"test realm\",service=\"test service\",scope=\"test scope\"",
        "test realm",
        "test service",
        "test scope")]
    [TestCase(
        "realm=test realm,service=test service,scope=\"scope1,scope2\"",
        "test realm",
        "test service",
        "scope1,scope2")]
    [TestCase(
        "realm=\"test realm\",service=\"test service\",scope=\"scope1,scope2\"",
        "test realm",
        "test service",
        "scope1,scope2")]
    public void GivenACommaDelimitedChallengeHeader_WhenIParseItAsTyped_ThenItShouldReturnTheCorrectSegments(
        string header,
        string expectedRealm,
        string expectedService,
        string expectedScope)
    {
        var actual = AuthenticateParser.ParseTyped(header);

        expectedRealm.Should().Be(actual.Realm);
        expectedService.Should().Be(actual.Service);
        expectedScope.Should().Be(actual.Scope);
    }
}