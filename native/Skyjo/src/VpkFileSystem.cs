using SteamDatabase.ValvePak;
using Ultralight.FNA;

namespace Skyjo;

public sealed class VpkFileSystem : UltralightFileSystem
{
    private readonly Package _package;

    public VpkFileSystem(string path)
    {
        _package = new Package();
        _package.OptimizeEntriesForBinarySearch(StringComparison.OrdinalIgnoreCase);
        _package.Read(path);
    }

    protected override bool FileExists(string path)
    {
        return _package.FindEntry(path) != null;
    }

    protected override byte[]? OpenFile(string path)
    {
        var entry = _package.FindEntry(path);
        if (entry == null)
            return null;

        _package.ReadEntry(entry, out var data);
        return data;
    }

    public override void Dispose()
    {
        _package.Dispose();
        base.Dispose();
    }
}