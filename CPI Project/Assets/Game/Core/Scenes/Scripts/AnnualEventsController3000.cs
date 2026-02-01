using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Disney.Kelowna.Common;

public class AnnualEventsController : MonoBehaviour
{
    public static AnnualEventsController Instance { get; private set; }

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

    private Dictionary<string, string> activeSceneKeys = new Dictionary<string, string>();
    private HashSet<string> seenScenes = new HashSet<string>();
    private float refreshInterval = 300f; // 5 minutes
    private float nextRefreshTime;

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

        ApplyEvents();
        nextRefreshTime = Time.time + refreshInterval;
    }

    private void Update()
    {
        if (Time.time >= nextRefreshTime)
        {
            ApplyEvents();
            nextRefreshTime = Time.time + refreshInterval;
        }
    }

    private void ApplyEvents()
    {
        activeSceneKeys.Clear();
        seenScenes.Clear();

        int currentYear = DateTime.UtcNow.Year;
        DateTimeOffset currentDateUtc = DateTimeOffset.UtcNow;

        foreach (var eventInfo in events)
        {
            string path = $"definitions/scheduledeventdates/date_{eventInfo.eventID}_{eventInfo.eventName}";
            var eventAsset = Resources.Load<ScriptableObject>(path);

            if (eventAsset != null)
            {
                var type = eventAsset.GetType();
                var datesField = type.GetField("Dates", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                var datesValue = datesField.GetValue(eventAsset);
                if (datesValue != null)
                {
                    var startDateField = datesValue.GetType().GetField("StartDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var endDateField = datesValue.GetType().GetField("EndDate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (startDateField != null && endDateField != null)
                    {
                        var startDate = startDateField.GetValue(datesValue);
                        var endDate = endDateField.GetValue(datesValue);

                        if (startDate != null && endDate != null)
                        {
                            int startYear = currentYear;
                            int endYear = (eventInfo.endMonth < eventInfo.startMonth) ? currentYear + 1 : currentYear;

                            // Update StartDate / EndDate fields
                            SetFieldOrPropertyValue(startDate, "day", eventInfo.startDay);
                            SetFieldOrPropertyValue(startDate, "month", eventInfo.startMonth);
                            SetFieldOrPropertyValue(startDate, "year", startYear);

                            SetFieldOrPropertyValue(endDate, "day", eventInfo.endDay);
                            SetFieldOrPropertyValue(endDate, "month", eventInfo.endMonth);
                            SetFieldOrPropertyValue(endDate, "year", endYear);

                            // Use UTC for safety
                            DateTimeOffset startDateTime = new DateTimeOffset(startYear, eventInfo.startMonth, eventInfo.startDay, 0, 0, 0, TimeSpan.Zero);
                            DateTimeOffset endDateTime = new DateTimeOffset(endYear, eventInfo.endMonth, eventInfo.endDay, 0, 0, 0, TimeSpan.Zero);

                            long startTimestamp = startDateTime.ToUnixTimeMilliseconds();
                            long endTimestamp = endDateTime.ToUnixTimeMilliseconds();

                            SetFieldOrPropertyValue(startDate, "TimeStampInMilliseconds", startTimestamp);
                            SetFieldOrPropertyValue(endDate, "TimeStampInMilliseconds", endTimestamp);

                            // Check if active in UTC range
                            bool isActive = currentDateUtc >= startDateTime && currentDateUtc < endDateTime;

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
                    }
                }
            }
        }

        ApplySceneAudioKeys();
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
}

/* Example:
   Halloween Event
   Start Date: October 31 at midnight (00:00 UTC)
   End Date: November 2 at midnight (00:00 UTC)
   This means the event is active for the full day of October 31 and November 1.
*/
