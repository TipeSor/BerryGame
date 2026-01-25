using Raylib_cs;

namespace BerryGame
{
    internal interface IResourceHandle : IDisposable
    { }

    internal sealed class SharedResource<TNative>(TNative value, Action<TNative> release)
    {
        public readonly TNative Value = value;

        private int _refCount = 0;
        private bool _disposed;
        private readonly Action<TNative> _release = release;

        public void AddRef()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SharedResource<>));

            Interlocked.Increment(ref _refCount);
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                Dispose();
            }
        }

        private void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _release(Value);
        }
    }

    internal sealed class ResourceCache<TKey, TNative>(
        Func<TKey, TNative> loader,
        Action<TNative> releaser)
        where TKey : notnull
    {
        private readonly Dictionary<TKey, SharedResource<TNative>> _cache = [];
        private readonly object _lock = new();

        private readonly Func<TKey, TNative> _loader = loader;
        private readonly Action<TNative> _releaser = releaser;

        public SharedResource<TNative> Acquire(TKey key)
        {
            lock (_lock)
            {
                if (!_cache.TryGetValue(key, out SharedResource<TNative>? resource))
                {
                    TNative? native = _loader(key);
                    resource = new SharedResource<TNative>(native, _releaser);
                    _cache[key] = resource;
                }

                resource.AddRef();
                return resource;
            }
        }
    }

    internal sealed class Resource<TNative> : IDisposable
    {
        private SharedResource<TNative> _shared;
        private bool _disposed;

        internal Resource(SharedResource<TNative> shared)
        {
            _shared = shared;
        }

        public TNative Value => _shared.Value;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _shared.Release();
            _shared = null!;
        }
    }

    internal static class Textures
    {
        private static readonly ResourceCache<string, Texture2D> _cache =
            new(
                Raylib.LoadTexture,
                Raylib.UnloadTexture
            );

        public static Resource<Texture2D> Load(string path)
            => new(_cache.Acquire(path));
    }

    internal static class Sounds
    {
        private static readonly ResourceCache<string, Sound> _cache =
            new(
                Raylib.LoadSound,
                Raylib.UnloadSound
            );

        public static Resource<Sound> Load(string path)
            => new(_cache.Acquire(path));
    }

    internal static class FShaders
    {
        private static readonly ResourceCache<string, Shader> _cache =
            new(
                static s => Raylib.LoadShader(string.Empty, s),
                Raylib.UnloadShader
            );

        public static Resource<Shader> Load(string path)
            => new(_cache.Acquire(path));
    }

    internal static class VShaders
    {
        private static readonly ResourceCache<string, Shader> _cache =
            new(
                static s => Raylib.LoadShader(s, string.Empty),
                Raylib.UnloadShader
            );

        public static Resource<Shader> Load(string path)
            => new(_cache.Acquire(path));
    }
}
