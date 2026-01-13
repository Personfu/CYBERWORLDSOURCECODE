using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Disney.Kelowna.Common;

public class AnnualEventsController : MonoBehaviour
{
    public static AnnualEventsController Instance { get; private set; }

    [Header("CDN Control")]
    [Tooltip("When enabled, event dates are fetched from the CDN and override embedded data. This allows the party switcher to control events remotely.")]
    public bool useCDNDates = true;
    
    [Tooltip("Reference to the CDN Event Date Service. If null, will try to find it automatically.")]
    public CDNEventDateService cdnService;

    [Header("Legacy Server Control (Deprecated)")]
    [Tooltip("Legacy mode: reads timestamps from embedded assets. Use CDN Dates instead for remote control.")]
    public bool useServerDates = false;

    [Serializable]
    public class EventInfo
    {
        [Tooltip("Use the event ID from the ScheduledEventDate asset.")]
        public string eventID;

        [Tooltip("Use the ScheduledEventDate name that appears in the embedded_content_manifest.txt (example: date_22_halloween2018 results to halloween2018).")]
        public string eventName;

        [Tooltip("Start date of the event (set at midnight). Example: October 31 00:00.")]
        public int startMonth;
        public int startDay;

        [Tooltip("End date of the event (also set at midnight). To keep the event active through November 1st, set this to November 2 00:00.")]
        public int endMonth;
        public int endDay;

        [Tooltip("Map scene names to event specific audio keys.")]
        public SceneAudioMapping[] SceneAudioMappings;

        [Tooltip("Optional material swap prefab for this event.")]
        public GameObject SnowballPrefab;
        public Material OriginalMaterial;
        public Material EventMaterial;
    }

    [Serializable]
    public class SceneAudioMapping
    {
        [Tooltip("Scene name this mapping applies to.")]
        public string SceneName;

        [Tooltip("Audio key used when the event is not active.")]
        public string DefaultAudioKey;

        [Tooltip("Audio key used while the event is active.")]
        public string EventAudioKey;
    }

    [Header("Annual events (loops every year).")]
    public EventInfo[] events;

    [Header("Timing")]
    [Tooltip("Delay before first event check (seconds). Allows CDN data to load first.")]
    public float initialDelay = 10f;
    
    [Tooltip("How often to refresh event status (seconds).")]
    public float refreshInterval = 300f;

    private Dictionary<string, string> activeSceneKeys = new Dictionary<string, string>();
    private HashSet<string> seenScenes = new HashSet<string>();
    private float nextRefreshTime;
    private bool hasAppliedInitial = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (events.Length == 0)
        {
            Debug.LogError("No events configured.");
            return;
        }

        // Try to find CDN service if not assigned
        if (cdnService == null)
        {
            cdnService = FindObjectOfType<CDNEventDateService>();
        }

        // Subscribe to CDN data loaded event
        if (cdnService != null)
        {
            cdnService.OnCDNDataLoaded += OnCDNDataLoaded;
        }

        // Delay initial application to allow CDN data to load
        if (useCDNDates || useServerDates)
        {
            Debug.Log($"[AnnualEventsController] CDN dates mode enabled. Waiting {initialDelay}s for CDN data...");
            nextRefreshTime = Time.time + initialDelay;
        }
        else
        {
            ApplyEvents();
            hasAppliedInitial = true;
            nextRefreshTime = Time.time + refreshInterval;
        }
    }

    private void OnDestroy()
    {
        if (cdnService != null)
        {
            cdnService.OnCDNDataLoaded -= OnCDNDataLoaded;
        }
    }

    private void OnCDNDataLoaded()
    {
        Debug.Log("[AnnualEventsController] CDN data loaded, applying events...");
        ApplyEvents();
    }

    private void Update()
    {
        if (Time.time >= nextRefreshTime)
        {
            if (!hasAppliedInitial)
            {
                Debug.Log("[AnnualEventsController] Initial delay complete. Applying events with CDN data...");
                hasAppliedInitial = true;
            }
            ApplyEvents();
            nextRefreshTime = Time.time + refreshInterval;
        }
    }

    public void ForceRefresh()
    {
        Debug.Log("[AnnualEventsController] Force refresh requested.");
        ApplyEvents();
    }

    private void ApplyEvents()
    {
        activeSceneKeys.Clear();
        seenScenes.Clear();

        int currentYear = DateTime.UtcNow.Year;
        DateTimeOffset currentDateUtc = DateTimeOffset.UtcNow;
        long currentTimestamp = currentDateUtc.ToUnixTimeMilliseconds();

        // Check if CDN service is ready
        bool cdnReady = useCDNDates && cdnService != null && cdnService.IsLoaded;
        if (useCDNDates && !cdnReady)
        {
            Debug.Log("[AnnualEventsController] CDN mode enabled but data not yet loaded, will retry...");
        }

        foreach (var eventInfo in events)
        {
            // Parse event ID
            int eventId;
            if (!int.TryParse(eventInfo.eventID, out eventId))
            {
                Debug.LogWarning($"[AnnualEventsController] Could not parse event ID: {eventInfo.eventID}");
                continue;
            }

            bool isActive = false;
            long startTimestamp = 0;
            long endTimestamp = 0;
            string dateSource = "unknown";

            // Priority 1: CDN dates (if enabled and loaded)
            if (useCDNDates && cdnReady)
            {
                if (cdnService.GetEventTimestamps(eventId, out startTimestamp, out endTimestamp))
                {
                    isActive = currentTimestamp >= startTimestamp && currentTimestamp < endTimestamp;
                    dateSource = "CDN";
                }
                else
                {
                    // Event not in CDN data, fall back to embedded
                    dateSource = "CDN-missing";
                }
            }

            // Priority 2: Embedded asset timestamps (useServerDates legacy mode or CDN fallback)
            if (dateSource == "unknown" || dateSource == "CDN-missing")
            {
                string path = $"definitions/scheduledeventdates/date_{eventInfo.eventID}_{eventInfo.eventName}";
                var eventAsset = Resources.Load<ScriptableObject>(path);

                if (eventAsset != null)
                {
                    var type = eventAsset.GetType();
                    var datesField = type.GetField("Dates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var datesValue = datesField?.GetValue(eventAsset);

                    if (datesValue != null)
                    {
                        var startDateField = datesValue.GetType().GetField("StartDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        var endDateField = datesValue.GetType().GetField("EndDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (startDateField != null && endDateField != null)
                        {
                            var startDateObj = startDateField.GetValue(datesValue);
                            var endDateObj = endDateField.GetValue(datesValue);

                            if (startDateObj != null && endDateObj != null)
                            {
                                if (useServerDates || dateSource == "CDN-missing")
                                {
                                    // Use embedded timestamps
                                    startTimestamp = GetFieldOrPropertyValue<long>(startDateObj, "TimeStampInMilliseconds");
                                    endTimestamp = GetFieldOrPropertyValue<long>(endDateObj, "TimeStampInMilliseconds");
                                    isActive = currentTimestamp >= startTimestamp && currentTimestamp < endTimestamp;
                                    dateSource = dateSource == "CDN-missing" ? "embedded(CDN-fallback)" : "embedded";
                                }
                                else
                                {
                                    int startYear = currentYear;
                                    int endYear = (eventInfo.endMonth < eventInfo.startMonth) ? currentYear + 1 : currentYear;

                                    SetFieldOrPropertyValue(startDateObj, "day", eventInfo.startDay);
                                    SetFieldOrPropertyValue(startDateObj, "month", eventInfo.startMonth);
                                    SetFieldOrPropertyValue(startDateObj, "year", startYear);
                                    SetFieldOrPropertyValue(endDateObj, "day", eventInfo.endDay);
                                    SetFieldOrPropertyValue(endDateObj, "month", eventInfo.endMonth);
                                    SetFieldOrPropertyValue(endDateObj, "year", endYear);

                                    DateTimeOffset startDateTime = new DateTimeOffset(startYear, eventInfo.startMonth, eventInfo.startDay, 0, 0, 0, TimeSpan.Zero);
                                    DateTimeOffset endDateTime = new DateTimeOffset(endYear, eventInfo.endMonth, eventInfo.endDay, 0, 0, 0, TimeSpan.Zero);

                                    startTimestamp = startDateTime.ToUnixTimeMilliseconds();
                                    endTimestamp = endDateTime.ToUnixTimeMilliseconds();

                                    SetFieldOrPropertyValue(startDateObj, "TimeStampInMilliseconds", startTimestamp);
                                    SetFieldOrPropertyValue(endDateObj, "TimeStampInMilliseconds", endTimestamp);

                                    isActive = currentDateUtc >= startDateTime && currentDateUtc < endDateTime;
                                    dateSource = "hardcoded";
                                }

                                // If CDN mode is enabled, update the embedded asset with CDN timestamps
                                // This ensures other game systems that read the embedded assets also see CDN values
                                if (useCDNDates && cdnReady && dateSource == "CDN")
                                {
                                    SetFieldOrPropertyValue(startDateObj, "TimeStampInMilliseconds", startTimestamp);
                                    SetFieldOrPropertyValue(endDateObj, "TimeStampInMilliseconds", endTimestamp);
                                }
                            }
                        }
                    }
                }
            }

            // Also update the embedded asset if we got CDN data (for other systems to read)
            if (useCDNDates && cdnReady && dateSource == "CDN")
            {
                UpdateEmbeddedAsset(eventInfo, startTimestamp, endTimestamp);
            }

            Debug.Log($"[{dateSource}] Event {eventInfo.eventID} ({eventInfo.eventName}): Start={startTimestamp}, End={endTimestamp}, Current={currentTimestamp}, Active={isActive}");

            // Apply audio keys
            foreach (var mapping in eventInfo.SceneAudioMappings)
            {
                if (isActive)
                {
                    activeSceneKeys[mapping.SceneName] = mapping.EventAudioKey;
                }
                else
                {
                    if (!activeSceneKeys.ContainsKey(mapping.SceneName))
                    {
                        activeSceneKeys[mapping.SceneName] = mapping.DefaultAudioKey;
                    }
                }
                seenScenes.Add(mapping.SceneName);
            }

            if (eventInfo.SnowballPrefab != null)
            {
                var tr = eventInfo.SnowballPrefab.GetComponent<TrailRenderer>();
                if (tr != null)
                {
                    tr.material = isActive ? eventInfo.EventMaterial : eventInfo.OriginalMaterial;
                    Debug.Log($"Material set on prefab '{eventInfo.SnowballPrefab.name}' for eventID '{eventInfo.eventID}' ({eventInfo.eventName}) Active: {isActive}");
                }
            }

            Debug.Log($"Updated event {eventInfo.eventID} ({eventInfo.eventName}) dates. Active: {isActive}");
        }

        ApplySceneAudioKeys();
    }

    private void UpdateEmbeddedAsset(EventInfo eventInfo, long startTimestamp, long endTimestamp)
    {
        string path = $"definitions/scheduledeventdates/date_{eventInfo.eventID}_{eventInfo.eventName}";
        var eventAsset = Resources.Load<ScriptableObject>(path);

        if (eventAsset != null)
        {
            var type = eventAsset.GetType();
            var datesField = type.GetField("Dates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var datesValue = datesField?.GetValue(eventAsset);

            if (datesValue != null)
            {
                var startDateField = datesValue.GetType().GetField("StartDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var endDateField = datesValue.GetType().GetField("EndDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (startDateField != null && endDateField != null)
                {
                    var startDateObj = startDateField.GetValue(datesValue);
                    var endDateObj = endDateField.GetValue(datesValue);

                    if (startDateObj != null && endDateObj != null)
                    {
                        SetFieldOrPropertyValue(startDateObj, "TimeStampInMilliseconds", startTimestamp);
                        SetFieldOrPropertyValue(endDateObj, "TimeStampInMilliseconds", endTimestamp);
                        Debug.Log($"[CDN] Updated embedded asset for event {eventInfo.eventID} with CDN timestamps");
                    }
                }
            }
        }
    }

    private void ApplySceneAudioKeys()
    {
        foreach (var sceneName in seenScenes)
        {
            string scenePath = $"definitions/scene/scene_{sceneName}";
            var sceneAsset = Resources.Load<ScriptableObject>(scenePath);

            if (sceneAsset != null)
            {
                var type = sceneAsset.GetType();
                var audioKeyField = type.GetField("SceneAudioContentKey", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (audioKeyField != null)
                {
                    string keyToUse = activeSceneKeys.ContainsKey(sceneName) ? activeSceneKeys[sceneName] : null;
                    if (!string.IsNullOrEmpty(keyToUse))
                    {
                        audioKeyField.SetValue(sceneAsset, new PrefabContentKey(keyToUse));
                        Debug.Log($"Scene '{sceneName}' audio key set to '{keyToUse}'");
                    }
                }
                else
                {
                    Debug.LogError($"SceneAudioContentKey field not found in scene '{sceneName}'");
                }
            }
            else
            {
                Debug.LogError($"Scene asset not found at path: {scenePath}");
            }
        }
    }

    private void SetFieldOrPropertyValue(object target, string name, object newValue)
    {
        var type = target.GetType();

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, newValue);
            return;
        }

        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, newValue);
            return;
        }

        Debug.LogError($"Field or property {name} not found in {type.Name}!");
    }

    private T GetFieldOrPropertyValue<T>(object target, string name)
    {
        var type = target.GetType();

        var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return (T)field.GetValue(target);
        }

        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.CanRead)
        {
            return (T)property.GetValue(target);
        }

        Debug.LogError($"Field or property {name} not found in {type.Name}!");
        return default(T);
    }
}

/* Example:
   Halloween Event
   Start Date: October 31 at midnight (00:00 UTC)
   End Date: November 2 at midnight (00:00 UTC)
   This means the event is active for the full day of October 31 and November 1.
*/
