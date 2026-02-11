// FileName: /StreamingAssetBundleDevice.cs
// FileContents:

#if UNITY_ANDROID
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleDevice : Device
    {
        private string streamingAssetsPath;

        public const string DEVICE_TYPE = "sa-bundle";

        public override string DeviceType
        {
            get
            {
                return "sa-bundle";
            }
        }

        public StreamingAssetBundleDevice(DeviceManager deviceManager)
            : base(deviceManager)
        {
            // For Android, assets are typically accessed via JAR file paths or OBBs.
            // This path construction is specific to how Android handles streaming assets.
            streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";
            Debug.Log("Android StreamingAssets Path: " + streamingAssetsPath);
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
        {
            StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
            AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
            CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
            return result;
        }

        private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
        {
            string key = entry.Key;

            // Validate the entry key
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Asset entry key is null or empty.");
                yield break; // Exit the coroutine if the key is invalid
            }

            // Construct the URL for Android. Path.Combine handles platform-specific separators.
            // Ensure 'key' is correctly formatted for the asset bundle path.
            string url = Path.Combine(streamingAssetsPath, key + ".txt");
            Debug.Log("Loading asset from URL (Android): " + url);

            wrapper.LoadFromDownload(url);
            yield return wrapper.WebRequest;
            AssetBundle assetBundle = wrapper.AssetBundle;
            if (handler != null)
            {
                handler(key, (TAsset)(object)assetBundle);
            }
            yield return null;
        }

        public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
        {
            throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
        }
    }
}
#elif UNITY_IOS || UNITY_IPHONE
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleDevice : Device
    {
        private string streamingAssetsPath;

        public const string DEVICE_TYPE = "sa-bundle";

        public override string DeviceType
        {
            get
            {
                return "sa-bundle";
            }
        }

        public StreamingAssetBundleDevice(DeviceManager deviceManager)
            : base(deviceManager)
        {
            // For iOS, streaming assets are typically in the /raw folder within the app bundle.
            streamingAssetsPath = "file://" + Application.dataPath + "/raw";
            Debug.Log("iOS StreamingAssets Path: " + streamingAssetsPath);
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
        {
            StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
            AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
            CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
            return result;
        }

        private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
        {
            string key = entry.Key;

            // Validate the entry key
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Asset entry key is null or empty.");
                yield break; // Exit the coroutine if the key is invalid
            }

            // Construct the URL for iOS. Path.Combine handles platform-specific separators.
            // Ensure 'key' is correctly formatted for the asset bundle path.
            string url = Path.Combine(streamingAssetsPath, key + ".txt");
            Debug.Log("Loading asset from URL (iOS): " + url);

            wrapper.LoadFromDownload(url);
            yield return wrapper.WebRequest;
            AssetBundle assetBundle = wrapper.AssetBundle;
            if (handler != null)
            {
                handler(key, (TAsset)(object)assetBundle);
            }
            yield return null;
        }

        public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
        {
            throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
        }
    }
}
#else // This block is for Editor, Windows, Mac, Linux, WebGL, etc.
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
    public class StreamingAssetBundleDevice : Device
    {
        public const string DEVICE_TYPE = "sa-bundle";

        private string streamingAssetsPath;

        public override string DeviceType
        {
            get
            {
                return "sa-bundle";
            }
        }

        public StreamingAssetBundleDevice(DeviceManager deviceManager)
            : base(deviceManager)
        {
            // Get the current base URL dynamically instead of hardcoding
            streamingAssetsPath = GetCurrentBaseURL();
            Debug.Log("Dynamic StreamingAssets Base URL: " + streamingAssetsPath);
        }

        private string GetCurrentBaseURL()
        {
            string baseUrl = Application.absoluteURL;

            // If Application.absoluteURL is empty (e.g., in editor or standalone builds),
            // fall back to a reasonable default
            if (string.IsNullOrEmpty(baseUrl))
            {
#if UNITY_EDITOR
                // In editor, use localhost for testing
                return "http://localhost/StreamingAssets/";
#else
                // For standalone builds, try to construct from dataPath
                // This is a fallback and might not work for all scenarios
                string dataPath = Application.dataPath;
                if (dataPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // Already a web URL (WebGL)
                    return dataPath;
                }
                else
                {
                    // Local file path, convert to file:// URL
                    return "file://" + Path.Combine(dataPath, "StreamingAssets/");
                }
#endif
            }

            // Parse the current URL to get the base path
            try
            {
                Uri currentUri = new Uri(baseUrl);
                string pathAndQuery = currentUri.PathAndQuery;

                // Remove the filename if present (for index.html, etc.)
                if (!string.IsNullOrEmpty(pathAndQuery) && pathAndQuery.Contains("/"))
                {
                    int lastSlashIndex = pathAndQuery.LastIndexOf('/');
                    if (lastSlashIndex >= 0)
                    {
                        pathAndQuery = pathAndQuery.Substring(0, lastSlashIndex + 1);
                    }
                }

                // Construct the base URL with StreamingAssets path
                UriBuilder uriBuilder = new UriBuilder(currentUri.Scheme, currentUri.Host, currentUri.Port)
                {
                    Path = pathAndQuery + "StreamingAssets/"
                };

                return uriBuilder.Uri.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing current URL '{baseUrl}': {ex.Message}");

                // Fallback: use the URL directly with StreamingAssets appended
                if (!baseUrl.EndsWith("/"))
                {
                    baseUrl += "/";
                }
                return baseUrl + "StreamingAssets/";
            }
        }

        public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
        {
            StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
            AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
            CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
            return result;
        }

        private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
        {
            string key = entry.Key;

            // Validate the entry key
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Asset entry key is null or empty.");
                yield break; // Exit the coroutine if the key is invalid
            }

            // Construct the URL for HTTP loading.
            // The 'key' should contain the relative path to the asset bundle within StreamingAssets,
            // using forward slashes (e.g., "assetbundles/generated/webgl/screenpenguinhome.sa.unity3d").
            // We directly concatenate it with the base HTTP URL.
            string url = streamingAssetsPath + key + ".txt";

            // Ensure the URL is properly formatted (handle any double slashes)
            url = url.Replace("//StreamingAssets/", "/StreamingAssets/")
                    .Replace(":///", "://");

            Debug.Log("Loading asset from dynamically constructed URL: " + url);

            // Load the asset
            wrapper.LoadFromDownload(url);
            yield return wrapper.WebRequest;

            AssetBundle assetBundle = wrapper.AssetBundle;
            if (handler != null)
            {
                handler(key, (TAsset)(object)assetBundle);
            }
            yield return null;
        }

        public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
        {
            throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
        }
    }
}
#endif
