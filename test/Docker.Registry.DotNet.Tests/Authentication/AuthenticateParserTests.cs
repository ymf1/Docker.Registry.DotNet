// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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