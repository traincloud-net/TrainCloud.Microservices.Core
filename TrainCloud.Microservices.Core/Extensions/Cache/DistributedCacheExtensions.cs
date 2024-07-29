using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Extensions.Cache;

/// <summary>
/// Wraps IDistributedCache with de/serialization
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Options for the cache de/serialization
    /// </summary>
    private static JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Sets the object with the given key.
    /// </summary>
    /// <param name="cache">Represents a distributed cache of serialized objects.</param>
    /// <param name="key">A string identifying the requested object.</param>
    /// <param name="object">The object to be stored in the cache</param>
    /// <param name="entryOptions">Provides the cache options for an entry in IDistributedCache.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that may be used to cancel the read operation.</param>
    /// <typeparam name="TObject">The type of the requested object</typeparam>
    public async static Task SetObjectAsync<TObject>(this IDistributedCache cache,
                                                     string key,
                                                     TObject objectToCache,
                                                     DistributedCacheEntryOptions? entryOptions = null,
                                                     CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("key is null or empty");
        }

        using (var msObjectToCache = new MemoryStream())
        {
            await JsonSerializer.SerializeAsync(msObjectToCache, objectToCache, Options, cancellationToken);
            byte[] objectBytes = msObjectToCache.ToArray();

            if (entryOptions is not null)
            {
                await cache.SetAsync(key, objectBytes, entryOptions, cancellationToken);
            }
            else
            {
                await cache.SetAsync(key, objectBytes, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Gets a object with the given key.
    /// </summary>
    /// <param name="cache">Represents a distributed cache of serialized objects.</param>
    /// <param name="key">A string identifying the requested object.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that may be used to cancel the read operation.</param>
    /// <typeparam name="TObject">The type of the requested object</typeparam>
    /// <returns>The Task that represents the asynchronous operation, containing the located object or null.</returns>
    public async static Task<TObject?> GetObjectAsync<TObject>(this IDistributedCache cache,
                                                               string key,
                                                               CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("key is null or empty");
        }

        byte[]? objectBytes = await cache.GetAsync(key, cancellationToken);
        if (objectBytes is null)
        {
            return default;
        }

        using (var msObjectFromCache = new MemoryStream(objectBytes))
        {
            TObject? objectFromCache = await JsonSerializer.DeserializeAsync<TObject?>(msObjectFromCache, Options, cancellationToken);
            return objectFromCache;
        }
    }

    /// <summary>
    /// Refreshes a object in the cache based on its key, resetting its sliding expiration timeout (if any).
    /// </summary>
    /// <param name="cache">Represents a distributed cache of serialized objects.</param>
    /// <param name="key">A string identifying the requested object.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that may be used to cancel the read operation.</param>
    public async static Task RefreshObjectAsync(this IDistributedCache cache,
                                                string key,
                                                CancellationToken cancellationToken = default) =>
        await cache.RefreshAsync(key, cancellationToken);

    /// <summary>
    /// Removes a object with the given key.
    /// </summary>
    /// <param name="cache">Represents a distributed cache of serialized objects.</param>
    /// <param name="key">A string identifying the requested object.</param>
    /// <param name="cancellationToken">Optional. A cancellation token that may be used to cancel the read operation.</param>
    public async static Task RemoveObjectAsync(this IDistributedCache cache,
                                               string key,
                                               CancellationToken cancellationToken = default) =>
        await cache.RemoveAsync(key, cancellationToken);
}