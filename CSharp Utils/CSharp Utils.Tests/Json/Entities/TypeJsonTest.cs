using System;

namespace CSharp_Utils.Tests.Json.Entities;

internal class TypeJsonTest : IEquatable<TypeJsonTest>
{
    public string Key { get; set; }

    public bool Equals(TypeJsonTest other)
    {
        if (other == null) return false;
        return Key == other.Key;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as TypeJsonTest);
    }

    public override int GetHashCode()
    {
        return Key?.GetHashCode() ?? 0;
    }
}
