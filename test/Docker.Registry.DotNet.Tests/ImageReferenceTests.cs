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

using Docker.Registry.DotNet.Domain.ImageReferences;

using FluentAssertions;

using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests;

[TestFixture]
public class ImageReferenceTests
{
    [TestCase("   sha256:0d1c30c6bf461513951e4875fe7846f9e2f25fdfec09f4be6b39dbe639d362ca    ")]
    [TestCase("pipelines@sha256:2ef9a59041a7c4f36001abaec4fe7c10c26c1ead4da11515ba2af346fe60ddac")]
    public void ImageReferenceWithDigestShouldWork(string digest)
    {
        var imageRef = ImageReference.Create(digest);

        imageRef.Should().NotBeNull();

        imageRef.Digest?.ToString().Should().Be(digest.Trim().ToLower());

        imageRef.IsDigest.Should().BeTrue();
    }

    [Test]
    public void ImageReferenceWithInvalidDigestShouldFail()
    {
        var digest =
            "   sha256:0d1c30c6bf461513951e4_75fe7846f9e2f25fdfec09f4be6b.9dbe639d362ca    ";

        var action = () => ImageReference.Create(digest);

        action.Should().Throw<ArgumentException>().WithMessage("*is invalid*");
    }

    [Test]
    public void ImageReferenceWithTagShouldWork()
    {
        var tag = "   1.2.34.5-master    ";

        var imageRef = ImageReference.Create(tag);

        imageRef.Should().NotBeNull();

        imageRef.Tag?.ToString().Should().Be(tag.Trim());

        imageRef.IsTag.Should().BeTrue();
    }

    [Test]
    public void TagEmptyShouldFail()
    {
        var action = () => ImageReference.Create("");

        action.Should().Throw<ArgumentException>().WithMessage("*not be null or empty*");
    }

    [Test]
    public void TagTooLargeShouldFail()
    {
        var action = () => ImageReference.Create(new string(Enumerable.Repeat('b', 129).ToArray()));

        action.Should().Throw<ArgumentException>().WithMessage("*value is too large*");
    }

    [Test]
    public void TagWithInvalidCharactersShouldFail()
    {
        var action = () => ImageReference.Create("fdsklfkdjsl dfsakldfskljdfs");

        action.Should().Throw<ArgumentException>().WithMessage("*is invalid*");
    }

    [Test]
    public void TagWithInvalidStartShouldFail()
    {
        var action = () => ImageReference.Create(".blahblah");

        action.Should().Throw<ArgumentException>().WithMessage("*can't start*");
    }
}