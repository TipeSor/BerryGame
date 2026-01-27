using Raylib_cs;

namespace BerryEngine
{
    public sealed class SharedResource<TNative>(TNative value, Action<TNative> release)
    {
        public readonly TNative Value = value;

        private int _refCount = 0;
        private bool _disposed;
        private readonly Action<TNative> _release = release;

        internal void AddRef()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SharedResource<TNative>));

            Interlocked.Increment(ref _refCount);
        }

        internal void Release()
        {
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                Dispose();
            }
        }

        public Resource<TNative> Acquire()
        {
            AddRef();
            return new(this);
        }

        private void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _release(Value);
        }
    }

    public sealed class ResourceCache<TKey, TNative>(
        Func<TKey, TNative> loader,
        Action<TNative> releaser)
        where TKey : notnull
    {
        private readonly Dictionary<TKey, SharedResource<TNative>> _cache = [];
        private readonly object _lock = new();

        private readonly Func<TKey, TNative> _loader = loader;
        private readonly Action<TNative> _releaser = releaser;

        public Resource<TNative> Acquire(TKey key)
        {
            lock (_lock)
            {
                if (!_cache.TryGetValue(key, out SharedResource<TNative>? shared))
                {
                    TNative? native = _loader(key);
                    shared = new SharedResource<TNative>(native, _releaser);
                    _cache[key] = shared;
                }

                return shared.Acquire();
            }
        }
    }

    public sealed class Resource<TNative> : IDisposable
    {
        private SharedResource<TNative> _shared;
        private bool _disposed;

        internal Resource(SharedResource<TNative> shared)
        {
            _shared = shared;
        }

        public TNative? Value => _shared.Value;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _shared.Release();
            _shared = null!;
        }
    }

    public static class Fonts
    {
        private static readonly ResourceCache<string, Font> _cache =
            new(Raylib.LoadFont, Raylib.UnloadFont);

        public static Resource<Font> Load(string path)
            => _cache.Acquire(path);
    }

    public static class Textures
    {
        private static readonly ResourceCache<string, Texture2D> _cache =
            new(Raylib.LoadTexture, Raylib.UnloadTexture);

        public static Resource<Texture2D> Load(string path)
            => _cache.Acquire(path);
    }

    public static class Images
    {
        private static readonly ResourceCache<string, Image> _cache =
            new(Raylib.LoadImage, Raylib.UnloadImage);

        public static Resource<Image> Load(string path)
            => _cache.Acquire(path);
    }

    public static class Sounds
    {
        private static readonly ResourceCache<string, Sound> _cache =
            new(Raylib.LoadSound, Raylib.UnloadSound);

        public static Resource<Sound> Load(string path)
            => _cache.Acquire(path);
    }

    public static class Music
    {
        private static readonly ResourceCache<string, Raylib_cs.Music> _cache =
            new(Raylib.LoadMusicStream, Raylib.UnloadMusicStream);

        public static Resource<Raylib_cs.Music> Load(string path)
            => _cache.Acquire(path);
    }

    public static class Waves
    {
        private static readonly ResourceCache<string, Wave> _cache =
            new(Raylib.LoadWave, Raylib.UnloadWave);

        public static Resource<Wave> Load(string path)
            => _cache.Acquire(path);
    }
}
