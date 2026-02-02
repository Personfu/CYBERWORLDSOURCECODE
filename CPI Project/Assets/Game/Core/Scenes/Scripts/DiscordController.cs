using ClubPenguin;
using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.PartyGames;
using ClubPenguin.Progression;
using Discord.Sdk;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
    private const ulong applicationId = 1235329861980127343UL;

    private Discord.Sdk.Client client;
    private bool initialized;

    private static ulong gameStartTimeMs;
    private static MethodInfo runCallbacksStatic;

    private EventChannel eventChannel;
    private bool questHooked;
    private bool tubeHooked;

    private static string currentBaseRoomName = "";
    private static string currentAdditiveRoomName = "";
    private static string questOnlyDetailsOverride = "";
    private static string areaNameOverride = "";

    private static bool tubeLobbyActive = false;
    private static bool tubeRaceActive = false;
    private static PartyGameDefinition.GameTypes tubeRaceType = PartyGameDefinition.GameTypes.TUBE_RACE_RED;

    private static bool tubeScoreValid = false;
    private static float tubeScoreValue = 0f;
    private static float tubeScoreUntilUnscaled = 0f;

    [SerializeField] private float statsSwapSeconds = 30f;

    private bool statsMode;
    private float nextStatsSwapUnscaled;

    public static DiscordController Instance { get; private set; }
    public static string CurrentRoomName { get; private set; }
    public static string LastLoadedSceneName { get; private set; }
    public static string LastLoadedAdditiveSceneName { get; private set; }

    public static void SetRoomGlobal(string roomNameOrQuestOverride)
    {
        if (Instance != null)
            Instance.SetRoom(roomNameOrQuestOverride);
    }

    public static void SetRoomFromSceneNameGlobal(string sceneName)
    {
        if (Instance != null)
            Instance.SetBaseRoom(Instance.GetCustomSceneName(sceneName));
    }

    public static void RefreshRoomFromLastLoadedSceneGlobal()
    {
        if (Instance != null)
            Instance.RefreshFromSceneState();
    }

    public static void SetAreaNameOverrideGlobal(string areaName)
    {
        if (Instance != null)
            Instance.SetAreaNameOverride(areaName);
    }

    public static void ClearAreaNameOverrideGlobal()
    {
        if (Instance != null)
            Instance.SetAreaNameOverride("");
    }

    [Serializable]
    public class SceneNameMapping
    {
        public string sceneName;
        public string displayName;
    }

    public SceneNameMapping[] customSceneNames;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!IsDiscordRunning())
        {
            UnityEngine.Debug.LogWarning("Discord is not running or installed. Skipping Discord integration.");
            return;
        }

        try
        {
            client = new Discord.Sdk.Client();
            client.AddLogCallback(OnDiscordLog, LoggingSeverity.Info);
            client.SetStatusChangedCallback(OnStatusChanged);
            client.SetApplicationId(applicationId);

            try
            {
                int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                client.SetGameWindowPid(pid);
            }
            catch { }

            if (gameStartTimeMs == 0UL)
                gameStartTimeMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            CacheOptionalRunCallbacks();

            CurrentRoomName = "Loading...";
            currentBaseRoomName = CurrentRoomName;
            currentAdditiveRoomName = "";
            questOnlyDetailsOverride = "";
            areaNameOverride = "";

            tubeLobbyActive = false;
            tubeRaceActive = false;
            tubeScoreValid = false;
            tubeScoreValue = 0f;
            tubeScoreUntilUnscaled = 0f;

            statsMode = false;
            nextStatsSwapUnscaled = Time.unscaledTime + Mathf.Max(5f, statsSwapSeconds);

            UpdatePresence(BuildStateText(), "default_icon", BuildDetailsText(), gameStartTimeMs);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            initialized = true;

            TryHookQuestEvents();
            TryHookTubeRaceEvents();
            RefreshPresenceOnly();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Failed to initialize Discord Social SDK: " + e.Message);
            client = null;
            initialized = false;
        }
    }

    private void Update()
    {
        if (!initialized || client == null)
            return;

        TryRunCallbacksIfExposed();

        if (!questHooked)
            TryHookQuestEvents();

        if (!tubeHooked)
            TryHookTubeRaceEvents();

        if (ShouldSwapStatsModeNow())
        {
            statsMode = !statsMode;
            nextStatsSwapUnscaled = Time.unscaledTime + Mathf.Max(5f, statsSwapSeconds);
            RefreshPresenceOnly();
        }

        if (tubeScoreValid && Time.unscaledTime > tubeScoreUntilUnscaled)
        {
            tubeScoreValid = false;
            tubeScoreValue = 0f;
            RefreshPresenceOnly();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        if (eventChannel != null)
        {
            eventChannel.RemoveAllListeners();
            eventChannel = null;
        }

        if (Instance == this)
            Instance = null;

        SafeShutdown();
    }

    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        if (eventChannel != null)
        {
            eventChannel.RemoveAllListeners();
            eventChannel = null;
        }

        SafeShutdown();
    }

    private void SafeShutdown()
    {
        if (client != null)
        {
            tryInvoke(client, "Disconnect");
            tryInvoke(client, "Shutdown");
            tryInvoke(client, "Dispose");
            client = null;
        }

        initialized = false;
    }

    private void OnDiscordLog(string message, LoggingSeverity severity)
    {
        UnityEngine.Debug.Log($"[Discord Social SDK] {severity}: {message}");
    }

    private void OnStatusChanged(Discord.Sdk.Client.Status status, Discord.Sdk.Client.Error error, int errorCode)
    {
        UnityEngine.Debug.Log($"[Discord Social SDK] Status changed: {status}");
        if (error != Discord.Sdk.Client.Error.None)
            UnityEngine.Debug.LogError($"[Discord Social SDK] Error: {error} (code {errorCode})");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            LastLoadedAdditiveSceneName = scene.name;
            currentAdditiveRoomName = NormalizeName(GetCustomSceneName(scene.name));
            RefreshPresenceOnly();
            return;
        }

        LastLoadedSceneName = scene.name;
        currentBaseRoomName = NormalizeName(GetCustomSceneName(scene.name));
        currentAdditiveRoomName = "";
        questOnlyDetailsOverride = "";
        RefreshPresenceOnly();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (!string.IsNullOrEmpty(LastLoadedAdditiveSceneName) && scene.name == LastLoadedAdditiveSceneName)
        {
            LastLoadedAdditiveSceneName = "";
            currentAdditiveRoomName = "";
        }

        if (tubeLobbyActive || tubeRaceActive)
        {
            tubeLobbyActive = false;
            tubeRaceActive = false;
        }

        RefreshFromSceneState();
    }

    private string GetCustomSceneName(string sceneName)
    {
        if (customSceneNames != null)
        {
            foreach (var mapping in customSceneNames)
            {
                if (mapping != null && mapping.sceneName == sceneName)
                    return mapping.displayName;
            }
        }

        return sceneName;
    }

    private bool IsQuestActiveNow()
    {
        try
        {
            QuestService qs = Service.Get<QuestService>();
            return qs != null && qs.ActiveQuest != null;
        }
        catch
        {
            return false;
        }
    }

    private bool TryGetActiveQuestDefinition(out QuestDefinition def)
    {
        def = null;
        try
        {
            QuestService qs = Service.Get<QuestService>();
            if (qs == null || qs.ActiveQuest == null)
                return false;
            def = qs.ActiveQuest.Definition;
            return def != null;
        }
        catch
        {
            return false;
        }
    }

    private void TryHookQuestEvents()
    {
        try
        {
            EventDispatcher dispatcher = Service.Get<EventDispatcher>();
            if (dispatcher == null)
                return;

            if (eventChannel == null)
                eventChannel = new EventChannel(dispatcher);

            eventChannel.AddListener<QuestEvents.QuestStarted>(OnQuestStarted);
            eventChannel.AddListener<QuestEvents.QuestCompleted>(OnQuestCompleted);
            eventChannel.AddListener<QuestEvents.QuestSyncCompleted>(OnQuestSyncCompleted);

            questHooked = true;
        }
        catch
        {
            questHooked = false;
        }
    }

    private void TryHookTubeRaceEvents()
    {
        try
        {
            EventDispatcher dispatcher = Service.Get<EventDispatcher>();
            if (dispatcher == null)
                return;

            if (eventChannel == null)
                eventChannel = new EventChannel(dispatcher);

            eventChannel.AddListener<TubeRaceEvents.LocalPlayerJoinedLobby>(OnTubeLobbyJoin);
            eventChannel.AddListener<TubeRaceEvents.LocalPlayerLeftLobby>(OnTubeLobbyLeave);
            eventChannel.AddListener<TubeRaceEvents.CloseLobby>(OnTubeLobbyClose);
            eventChannel.AddListener<TubeRaceEvents.RaceStart>(OnTubeRaceStart);
            eventChannel.AddListener<TubeRaceEvents.RaceEnd>(OnTubeRaceEnd);
            eventChannel.AddListener<TubeRaceEvents.EndGameResultsReceived>(OnTubeEndGameResults);

            tubeHooked = true;
        }
        catch
        {
            tubeHooked = false;
        }
    }

    private bool OnQuestStarted(QuestEvents.QuestStarted evt)
    {
        RefreshPresenceOnly();
        return false;
    }

    private bool OnQuestCompleted(QuestEvents.QuestCompleted evt)
    {
        questOnlyDetailsOverride = "";
        RefreshPresenceOnly();
        return false;
    }

    private bool OnQuestSyncCompleted(QuestEvents.QuestSyncCompleted evt)
    {
        if (!IsQuestActiveNow())
            questOnlyDetailsOverride = "";
        RefreshPresenceOnly();
        return false;
    }

    private void ClearTubeRaceState()
    {
        tubeLobbyActive = false;
        tubeRaceActive = false;
    }

    private bool OnTubeLobbyJoin(TubeRaceEvents.LocalPlayerJoinedLobby evt)
    {
        tubeLobbyActive = true;
        tubeRaceActive = false;
        RefreshPresenceOnly();
        return false;
    }

    private bool OnTubeLobbyLeave(TubeRaceEvents.LocalPlayerLeftLobby evt)
    {
        ClearTubeRaceState();
        RefreshFromSceneState();
        return false;
    }

    private bool OnTubeLobbyClose(TubeRaceEvents.CloseLobby evt)
    {
        ClearTubeRaceState();
        RefreshFromSceneState();
        return false;
    }

    private bool OnTubeRaceStart(TubeRaceEvents.RaceStart evt)
    {
        tubeLobbyActive = false;
        tubeRaceActive = true;
        tubeRaceType = evt.RaceType;
        tubeScoreValid = false;
        tubeScoreValue = 0f;
        RefreshPresenceOnly();
        return false;
    }

    private bool OnTubeRaceEnd(TubeRaceEvents.RaceEnd evt)
    {
        ClearTubeRaceState();
        RefreshFromSceneState();
        return false;
    }

    private bool OnTubeEndGameResults(TubeRaceEvents.EndGameResultsReceived evt)
    {
        try
        {
            long localId = 0L;
            try
            {
                CPDataEntityCollection c = Service.Get<CPDataEntityCollection>();
                if (c != null)
                    localId = c.LocalPlayerSessionId;
            }
            catch { }

            if (evt.PlayerResults != null && evt.PlayerResults.Count > 0 && localId > 0L)
            {
                var r = evt.PlayerResults.FirstOrDefault(x => x != null && x.PlayerId == localId);
                if (r != null)
                {
                    tubeScoreValue = r.OverallScore;
                    tubeScoreValid = true;
                    tubeScoreUntilUnscaled = Time.unscaledTime + 12f;
                }
            }
        }
        catch { }

        ClearTubeRaceState();
        RefreshFromSceneState();
        return false;
    }

    private void RefreshPresenceOnly()
    {
        if (string.IsNullOrEmpty(currentBaseRoomName))
            return;

        if (!IsQuestActiveNow())
            questOnlyDetailsOverride = "";

        string detailsText = BuildDetailsText();
        string stateText = BuildStateText();

        string roomImage = "default_icon";
        string iconMatchName = BuildIconMatchName();

        bool iconFound = false;

        if (IsQuestActiveNow())
        {
            string questMascotImage = GetQuestMascotImageKey();
            if (!string.IsNullOrEmpty(questMascotImage))
            {
                roomImage = questMascotImage;
                iconFound = true;
            }
        }

        if (!iconFound)
        {
            if (iconMatchName.IndexOf("Halloween Construction", StringComparison.OrdinalIgnoreCase) >= 0 ||
                iconMatchName.IndexOf("Holiday Construction", StringComparison.OrdinalIgnoreCase) >= 0 ||
                iconMatchName.IndexOf("Summer Splash Construction", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                roomImage = "jackhammer";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Beacon Boardwalk | The Migrator", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "rh";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Island Central | DJ Cadence's Studio", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "dj";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Island Central | Halloween", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "town_halloween";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Island Central | Rainbow", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "rainbow_town";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Mt. Blizzard | Blizzard Beach", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "blizzardbeach";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Halloween", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "halloween";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Frozen", StringComparison.OrdinalIgnoreCase))
            {
                string[] holidayIcons = { "holiday", "cfc", "olaf" };
                roomImage = holidayIcons[new System.Random().Next(holidayIcons.Length)];
                iconFound = true;
            }
            else if (iconMatchName.Contains("Holiday", StringComparison.OrdinalIgnoreCase))
            {
                string[] holidayIcons = { "holiday", "cfc", "olaf" };
                roomImage = holidayIcons[new System.Random().Next(holidayIcons.Length)];
                iconFound = true;
            }
            else if (iconMatchName.Contains("Rainbow Migration", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "rainbow";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Anniversary", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "anniversary";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Valentines", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "valentines";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Arcade", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Bean", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Fishing", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Jetpack", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Roundup", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Pizzatron", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Smoothie", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "arcade";
                iconFound = true;
            }
            else if (iconMatchName.Contains("April", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("???", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "box_dimension";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Waddle", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "sunset";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Splash", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "summersplash";
                iconFound = true;
            }
            else if (iconMatchName.Contains("wpd", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "wpd";
                iconFound = true;
            }
            else if (iconMatchName.Contains("World", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "wpd";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Medieval", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "medieval";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Credits", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "credits";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Penglantian", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Pirate", StringComparison.OrdinalIgnoreCase) ||
                     iconMatchName.Contains("Expedition", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "penglantian";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Igloo", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "igloo";
                iconFound = true;
            }
            else if (iconMatchName.Contains("Summit", StringComparison.OrdinalIgnoreCase))
            {
                roomImage = "summit";
                iconFound = true;
            }

            if (!iconFound)
            {
                int currentMonth = DateTime.Now.Month;
                switch (currentMonth)
                {
                    case 2:
                        roomImage = "valentines";
                        break;
                    case 10:
                        roomImage = "halloween";
                        break;
                    case 12:
                        string[] holidayIcons = { "holiday", "cfc", "olaf" };
                        roomImage = holidayIcons[new System.Random().Next(holidayIcons.Length)];
                        break;
                    default:
                        roomImage = iconMatchName.Contains("Island Central", StringComparison.OrdinalIgnoreCase) ? "island_central" : "default_icon";
                        break;
                }
            }
        }

        CurrentRoomName = detailsText;

        if (initialized && client != null)
            UpdatePresence(stateText, roomImage, detailsText, gameStartTimeMs);
    }

    private string GetQuestMascotImageKey()
    {
        QuestDefinition def;
        if (!TryGetActiveQuestDefinition(out def))
            return null;

        string mascotName = "";
        if (def.Mascot != null)
            mascotName = def.Mascot.name ?? "";

        if (mascotName.Equals("AuntArctic", StringComparison.OrdinalIgnoreCase)) return "aa";
        if (mascotName.Equals("Rockhopper", StringComparison.OrdinalIgnoreCase)) return "rh";
        if (mascotName.Equals("Rookie", StringComparison.OrdinalIgnoreCase)) return "rk";
        if (mascotName.Equals("Scorn", StringComparison.OrdinalIgnoreCase)) return "sc";
        if (mascotName.Equals("DJCadence", StringComparison.OrdinalIgnoreCase)) return "dj";
        if (mascotName.Equals("DJCadence2", StringComparison.OrdinalIgnoreCase)) return "dj";
        if (mascotName.Equals("Dot", StringComparison.OrdinalIgnoreCase)) return "dt";
        if (mascotName.Equals("Dot2", StringComparison.OrdinalIgnoreCase)) return "dt";
        if (mascotName.Equals("Gary", StringComparison.OrdinalIgnoreCase)) return "gy";
        if (mascotName.Equals("Herbert", StringComparison.OrdinalIgnoreCase)) return "hb";
        if (mascotName.Equals("JetpackGuy", StringComparison.OrdinalIgnoreCase)) return "jg";
        if (mascotName.Equals("Rory", StringComparison.OrdinalIgnoreCase)) return "ry";
        if (mascotName.Equals("Rory2", StringComparison.OrdinalIgnoreCase)) return "ry";
        if (mascotName.Equals("Shellbeard", StringComparison.OrdinalIgnoreCase)) return "sb";

        return null;
    }

    private string NormalizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        int idx;

        idx = name.IndexOf("| On a Quest:", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) name = name.Substring(0, idx);

        idx = name.IndexOf("| On a Quest:", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) name = name.Substring(0, idx);

        idx = name.IndexOf("| On a Quest", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) name = name.Substring(0, idx);

        idx = name.IndexOf("| On a Quest", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0) name = name.Substring(0, idx);

        return name.Trim().TrimEnd('|').Trim();
    }

    private string BuildTubeStateText()
    {
        if (tubeLobbyActive)
            return "Tube Race | Lobby";

        if (tubeRaceActive)
        {
            if (tubeRaceType == PartyGameDefinition.GameTypes.TUBE_RACE_RED)
                return "Tube Race | Red";
            if (tubeRaceType == PartyGameDefinition.GameTypes.TUBE_RACE_BLUE)
                return "Tube Race | Blue";
            return "Tube Race";
        }

        if (tubeScoreValid)
        {
            int s = Mathf.RoundToInt(tubeScoreValue);
            return "Tube Race | Score " + s;
        }

        return "";
    }

    private bool TryGetPlayerStats(out int ageDays, out int level, out int coins)
    {
        ageDays = 0;
        level = 0;
        coins = 0;

        try
        {
            CPDataEntityCollection collection = Service.Get<CPDataEntityCollection>();
            if (collection == null)
                return false;

            DataEntityHandle h = collection.LocalPlayerHandle;
            if (h.IsNull)
                return false;

            ProfileData pd;
            if (collection.TryGetComponent<ProfileData>(h, out pd))
                ageDays = pd.PenguinAgeInDays;

            try
            {
                ProgressionService ps = Service.Get<ProgressionService>();
                if (ps != null)
                    level = ps.Level;
            }
            catch { }

            CoinsData cd;
            if (collection.TryGetComponent<CoinsData>(h, out cd))
                coins = cd.Coins;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private string BuildRotatingNonQuestNonTubeStateText()
    {
        if (!statsMode)
            return $"Unity {Application.unityVersion} | Version {Application.version}";

        int age, lvl, c;
        if (TryGetPlayerStats(out age, out lvl, out c))
            return "Age " + age + "d | Level " + lvl + " | Coins " + c;

        return $"Unity {Application.unityVersion} | Version {Application.version}";
    }

    private bool ShouldSwapStatsModeNow()
    {
        if (Time.unscaledTime < nextStatsSwapUnscaled)
            return false;

        if (IsQuestActiveNow())
            return false;

        if (tubeLobbyActive || tubeRaceActive || tubeScoreValid)
            return false;

        return true;
    }

    private string BuildStateText()
    {
        if (IsQuestActiveNow())
        {
            QuestDefinition def;
            if (TryGetActiveQuestDefinition(out def))
            {
                string mascotName = (def.Mascot != null) ? (def.Mascot.name ?? "Unknown") : "Unknown";
                int chapter = def.ChapterNumber;
                int episode = def.QuestNumber;
                return "On a Quest | " + mascotName + " | C" + chapter + "E" + episode;
            }

            return "On a Quest";
        }

        string tube = BuildTubeStateText();
        if (!string.IsNullOrEmpty(tube))
            return tube;

        return BuildRotatingNonQuestNonTubeStateText();
    }

    private string BuildDetailsText()
    {
        bool questActive = IsQuestActiveNow();

        string baseRoom = NormalizeName(currentBaseRoomName);
        string additive = NormalizeName(currentAdditiveRoomName);

        string roomForDetails = baseRoom;

        if (!string.IsNullOrEmpty(areaNameOverride))
            roomForDetails = areaNameOverride;

        if (questActive && !string.IsNullOrEmpty(questOnlyDetailsOverride))
            return questOnlyDetailsOverride;

        if (questActive)
            return roomForDetails;

        if (!string.IsNullOrEmpty(areaNameOverride))
            return roomForDetails;

        return !string.IsNullOrEmpty(additive) ? additive : baseRoom;
    }

    private string BuildIconMatchName()
    {
        string baseRoom = NormalizeName(currentBaseRoomName);
        if (!string.IsNullOrEmpty(areaNameOverride))
            return areaNameOverride;

        if (IsQuestActiveNow())
            return baseRoom;

        string additive = NormalizeName(currentAdditiveRoomName);
        return !string.IsNullOrEmpty(additive) ? additive : baseRoom;
    }

    private void RefreshFromSceneState()
    {
        if (!string.IsNullOrEmpty(LastLoadedSceneName))
            currentBaseRoomName = NormalizeName(GetCustomSceneName(LastLoadedSceneName));

        if (!string.IsNullOrEmpty(LastLoadedAdditiveSceneName))
            currentAdditiveRoomName = NormalizeName(GetCustomSceneName(LastLoadedAdditiveSceneName));
        else
            currentAdditiveRoomName = "";

        RefreshPresenceOnly();
    }

    private void SetBaseRoom(string roomName)
    {
        currentBaseRoomName = NormalizeName(roomName);
        currentAdditiveRoomName = "";
        questOnlyDetailsOverride = "";
        RefreshPresenceOnly();
    }

    private void SetAreaNameOverride(string areaName)
    {
        areaNameOverride = (areaName ?? "").Trim();
        RefreshPresenceOnly();
    }

    public void SetRoom(string roomNameOrQuestOverride)
    {
        if (IsQuestActiveNow())
        {
            questOnlyDetailsOverride = (roomNameOrQuestOverride ?? "").Trim();
            RefreshPresenceOnly();
            return;
        }

        currentBaseRoomName = NormalizeName(roomNameOrQuestOverride);
        currentAdditiveRoomName = "";
        questOnlyDetailsOverride = "";
        RefreshPresenceOnly();
    }

    private void UpdatePresence(string state, string imageKey, string details, ulong startTimestampMs)
    {
        if (client == null)
            return;

        if (startTimestampMs == 0UL)
        {
            startTimestampMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            gameStartTimeMs = startTimestampMs;
        }

        Activity activity = new Activity();
        activity.SetType(ActivityTypes.Playing);
        activity.SetState(state);
        activity.SetDetails(details);

        var assets = new ActivityAssets();
        assets.SetLargeImage(imageKey);
        assets.SetLargeText(state);
        activity.SetAssets(assets);

        var timestamps = new ActivityTimestamps();
        timestamps.SetStart(startTimestampMs);
        activity.SetTimestamps(timestamps);

        client.UpdateRichPresence(activity, OnUpdateRichPresence);
    }

    private void OnUpdateRichPresence(ClientResult result)
    {
        if (result.Successful())
            UnityEngine.Debug.Log("[Discord Social SDK] Rich presence updated.");
        else
            UnityEngine.Debug.LogError($"[Discord Social SDK] Failed to update rich presence: {result.Error()}");
    }

    private bool IsDiscordRunning()
    {
        try
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("Discord");
            return processes.Any();
        }
        catch
        {
            return false;
        }
    }

    private static void CacheOptionalRunCallbacks()
    {
        if (runCallbacksStatic != null)
            return;

        string[] candidateTypeNames =
        {
            "discordpp",
            "discordpp.discordpp",
            "Discord.Sdk.discordpp",
            "Discord.Sdk.NativeMethods"
        };

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var tn in candidateTypeNames)
            {
                try
                {
                    var t = asm.GetType(tn, false);
                    if (t == null) continue;

                    var mi = t.GetMethod("RunCallbacks", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);
                    if (mi != null)
                    {
                        runCallbacksStatic = mi;
                        return;
                    }
                }
                catch { }
            }
        }
    }

    private static void TryRunCallbacksIfExposed()
    {
        if (runCallbacksStatic == null)
            return;

        try
        {
            runCallbacksStatic.Invoke(null, null);
        }
        catch { }
    }

    private static void tryInvoke(object obj, string methodName)
    {
        if (obj == null) return;

        try
        {
            var mi = obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (mi != null) mi.Invoke(obj, null);
        }
        catch { }
    }
}
