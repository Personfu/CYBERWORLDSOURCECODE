using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class CDNEventDateService : MonoBehaviour
{
    public static CDNEventDateService Instance { get; private set; }

    [Header("CDN Configuration")]
    [Tooltip("Base URL for the CDN server")]
    public string cdnBaseUrl = "http://localhost:3001";
    
    [Tooltip("Path to the date definitions JSON file on the CDN")]
    public string dateDefinitionsPath = "/1541098597/ServerData/dfbdb4ce33ce290e8379405737ad1e4beb351db8.json";
    
    [Tooltip("How often to refresh CDN data (seconds). Set to 0 to only fetch once.")]
    public float refreshInterval = 60f;
    
    [Tooltip("Timeout for CDN requests (seconds)")]
    public float requestTimeout = 10f;

    public event Action OnCDNDataLoaded;

    public Dictionary<int, CDNDateDefinition> DateDefinitions { get; private set; } = new Dictionary<int, CDNDateDefinition>();

    public bool IsLoaded { get; private set; } = false;

    public DateTime LastFetchTime { get; private set; }

    public string LastError { get; private set; }

    private float nextRefreshTime;
    private bool isFetching = false;

    [Serializable]
    public class CDNDateDefinition
    {
        public int Id;
        public long StartTimestamp;
        public long EndTimestamp;

        public bool IsActive(long currentTimestamp)
        {
            return currentTimestamp >= StartTimestamp && currentTimestamp < EndTimestamp;
        }
    }

    [Serializable]
    private class CDNDateDefinitionJson
    {
        public int Id;
        public CDNDateRangeJson Dates;
    }

    [Serializable]
    private class CDNDateRangeJson
    {
        public CDNTimestampJson StartDate;
        public CDNTimestampJson EndDate;
    }

    [Serializable]
    private class CDNTimestampJson
    {
        public long TimeStampInMilliseconds;
    }

    [Serializable]
    private class CDNDateDefinitionArrayWrapper
    {
        public CDNDateDefinitionJson[] items;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Start fetching immediately
        StartCoroutine(FetchCDNDataCoroutine());
    }

    private void Update()
    {
        // Periodic refresh if enabled
        if (refreshInterval > 0 && Time.time >= nextRefreshTime && !isFetching)
        {
            StartCoroutine(FetchCDNDataCoroutine());
        }
    }

    public void ForceRefresh()
    {
        if (!isFetching)
        {
            StartCoroutine(FetchCDNDataCoroutine());
        }
    }

    public bool TryGetDateDefinition(int eventId, out CDNDateDefinition definition)
    {
        return DateDefinitions.TryGetValue(eventId, out definition);
    }

    public bool IsEventActive(int eventId)
    {
        if (TryGetDateDefinition(eventId, out CDNDateDefinition def))
        {
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return def.IsActive(currentTimestamp);
        }
        return false;
    }

    public bool GetEventTimestamps(int eventId, out long startTimestamp, out long endTimestamp)
    {
        if (TryGetDateDefinition(eventId, out CDNDateDefinition def))
        {
            startTimestamp = def.StartTimestamp;
            endTimestamp = def.EndTimestamp;
            return true;
        }
        startTimestamp = 0;
        endTimestamp = 0;
        return false;
    }

    private IEnumerator FetchCDNDataCoroutine()
    {
        isFetching = true;
        string url = cdnBaseUrl + dateDefinitionsPath;
        
        Debug.Log($"[CDNEventDateService] Fetching date definitions from: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = (int)requestTimeout;
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    ParseCDNData(json);
                    IsLoaded = true;
                    LastFetchTime = DateTime.UtcNow;
                    LastError = null;
                    Debug.Log($"[CDNEventDateService] Successfully loaded {DateDefinitions.Count} date definitions from CDN");
                    
                    // Fire event
                    OnCDNDataLoaded?.Invoke();
                }
                catch (Exception ex)
                {
                    LastError = $"Parse error: {ex.Message}";
                    Debug.LogError($"[CDNEventDateService] Failed to parse CDN data: {ex.Message}");
                }
            }
            else
            {
                LastError = $"Request failed: {request.error}";
                Debug.LogWarning($"[CDNEventDateService] Failed to fetch CDN data: {request.error}");
            }
        }

        nextRefreshTime = Time.time + refreshInterval;
        isFetching = false;
    }

    private void ParseCDNData(string json)
    {
        
        DateDefinitions.Clear();

        
        try
        {
            string wrappedJson = "{\"items\":" + json + "}";
            CDNDateDefinitionArrayWrapper wrapper = JsonUtility.FromJson<CDNDateDefinitionArrayWrapper>(wrappedJson);
            
            if (wrapper != null && wrapper.items != null)
            {
                foreach (var item in wrapper.items)
                {
                    if (item != null && item.Dates != null)
                    {
                        CDNDateDefinition def = new CDNDateDefinition
                        {
                            Id = item.Id,
                            StartTimestamp = item.Dates.StartDate?.TimeStampInMilliseconds ?? 0,
                            EndTimestamp = item.Dates.EndDate?.TimeStampInMilliseconds ?? 0
                        };
                        DateDefinitions[item.Id] = def;
                        
                        long currentTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        bool isActive = def.IsActive(currentTs);
                        Debug.Log($"[CDNEventDateService] Event {def.Id}: Start={def.StartTimestamp}, End={def.EndTimestamp}, Active={isActive}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CDNEventDateService] JSON parse error: {ex.Message}");
            throw;
        }
    }
}
