// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.ImageReferences;

public record ImageReference
{
    public ImageReference(ImageTag tag)
    {
        this.Tag = tag ?? throw new ArgumentNullException(nameof(tag), "Invalid Tag");
    }

    public ImageReference(ImageDigest digest)
    {
        this.Digest = digest ?? throw new ArgumentNullException(nameof(digest), "Invalid Digest");
    }

    public ImageTag? Tag { get; init; }

    public ImageDigest? Digest { get; init; }

    public bool IsDigest => this.Digest != null;

    public bool IsTag => this.Tag != null;

    public static ImageReference Create(ImageTag tag)
    {
        return new ImageReference(tag);
    }

    public static ImageReference Create(ImageDigest digest)
    {
        return new ImageReference(digest);
    }

    public static ImageReference Create(string reference)
    {
        return ImageDigest.TryCreate(reference, out var digest) ? Create(digest) : Create(ImageTag.Create(reference));
    }

    public static bool TryCreate(string reference, out ImageReference? imageReference)
    {
        if (ImageDigest.TryCreate(reference, out var digest))
        {
            imageReference = Create(digest);
            return true;
        }

        if (ImageTag.TryCreate(reference, out var tag))
        {
            imageReference = Create(tag);
            return true;
        }

        imageReference = null;
        return false;
    }

    public override string ToString()
    {
        if (this.IsTag) return this.Tag?.ToString()!;
        if (this.IsDigest) return this.Digest?.ToString()!;

        return base.ToString();
    }
}