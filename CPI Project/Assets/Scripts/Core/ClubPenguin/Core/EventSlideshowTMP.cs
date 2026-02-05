using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Core
{
    public class EventSlideshowTMP : MonoBehaviour
    {
        [System.Serializable]
        private struct DateOptions
        {
            public bool UseYear;
            public bool UseMonth;
            public bool UseDay;
            public bool SubtractDay;
        }

        [System.Serializable]
        private struct EventEntry
        {
            public string EventName;
            public ScheduledEventDateDefinitionKey DateDefinitionKey;
            public DateType DateType;

            public Sprite SpriteA;
            public Sprite SpriteB;

            public bool OverrideGradientColor;
            public Color GradientColor;
        }

        [Header("TMP Targets")]
        [SerializeField] private TMP_Text eventNameText;
        [SerializeField] private TMP_Text eventDateText;

        [Header("Image Target")]
        [SerializeField] private Image globalImage;

        [Header("Gradient Target")]
        [SerializeField] private Graphic globalGradientGraphic;

        [Header("Events")]
        [SerializeField] private List<EventEntry> events = new List<EventEntry>();
        [SerializeField] private DateOptions startDateOptions;
        [SerializeField] private DateOptions endDateOptions;

        [Header("Timing")]
        [SerializeField] private float changeTimerSeconds = 3f;
        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private CanvasGroup dateCanvasGroup;
        private int currentIndex;

        private Dictionary<int, bool> eventSpriteToggle = new Dictionary<int, bool>();

        private List<EventEntry> runtimeEvents = new List<EventEntry>();

        private void Awake()
        {
            if (eventDateText == null)
                eventDateText = GetComponent<TMP_Text>();

            dateCanvasGroup = eventDateText.GetComponent<CanvasGroup>();
            if (dateCanvasGroup == null)
                dateCanvasGroup = eventDateText.gameObject.AddComponent<CanvasGroup>();

            dateCanvasGroup.alpha = 0f;
        }

        private void Start()
        {
            StartCoroutine(StartSlideshowWhenDataReady());
        }

        private IEnumerator StartSlideshowWhenDataReady()
        {
            while (Service.Get<IGameData>() == null)
                yield return null;

            Dictionary<int, ScheduledEventDateDefinition> dict = null;

            while (dict == null)
            {
                var gameData = Service.Get<IGameData>();
                if (gameData != null)
                    dict = gameData.Get<Dictionary<int, ScheduledEventDateDefinition>>();

                if (dict == null)
                    yield return null;
            }

            BuildRuntimeOrderedEvents(dict);

            if (runtimeEvents.Count > 0)
                StartCoroutine(RunSlideshow());
        }

        private void BuildRuntimeOrderedEvents(Dictionary<int, ScheduledEventDateDefinition> dict)
        {
            runtimeEvents.Clear();

            if (events == null || events.Count == 0)
                return;

            List<DatedEvent> dated = new List<DatedEvent>();
            List<EventEntryWithIndex> undated = new List<EventEntryWithIndex>();

            for (int i = 0; i < events.Count; i++)
            {
                var entry = events[i];

                if (TryGetSortDate(entry, dict, out System.DateTime date))
                {
                    dated.Add(new DatedEvent
                    {
                        Entry = entry,
                        Date = date.Date,
                        OriginalIndex = i
                    });
                }
                else
                {
                    undated.Add(new EventEntryWithIndex
                    {
                        Entry = entry,
                        OriginalIndex = i
                    });
                }
            }

            dated.Sort((a, b) =>
            {
                int cmp = a.Date.CompareTo(b.Date);
                if (cmp != 0) return cmp;
                return a.OriginalIndex.CompareTo(b.OriginalIndex);
            });

            int rotateIndex = 0;

            if (dated.Count > 0)
            {
                System.DateTime today = System.DateTime.Now.Date;

                double bestAbsDays = double.MaxValue;
                System.DateTime bestDate = System.DateTime.MaxValue;
                int bestOriginalIndex = int.MaxValue;

                for (int i = 0; i < dated.Count; i++)
                {
                    var d = dated[i];
                    double absDays = System.Math.Abs((d.Date - today).TotalDays);

                    if (absDays < bestAbsDays)
                    {
                        bestAbsDays = absDays;
                        bestDate = d.Date;
                        bestOriginalIndex = d.OriginalIndex;
                        rotateIndex = i;
                    }
                    else if (absDays == bestAbsDays)
                    {
                        if (d.Date < bestDate)
                        {
                            bestDate = d.Date;
                            bestOriginalIndex = d.OriginalIndex;
                            rotateIndex = i;
                        }
                        else if (d.Date == bestDate && d.OriginalIndex < bestOriginalIndex)
                        {
                            bestOriginalIndex = d.OriginalIndex;
                            rotateIndex = i;
                        }
                    }
                }
            }

            for (int i = rotateIndex; i < dated.Count; i++)
                runtimeEvents.Add(dated[i].Entry);

            for (int i = 0; i < rotateIndex; i++)
                runtimeEvents.Add(dated[i].Entry);

            undated.Sort((a, b) => a.OriginalIndex.CompareTo(b.OriginalIndex));

            for (int i = 0; i < undated.Count; i++)
                runtimeEvents.Add(undated[i].Entry);
        }

        private struct DatedEvent
        {
            public EventEntry Entry;
            public System.DateTime Date;
            public int OriginalIndex;
        }

        private struct EventEntryWithIndex
        {
            public EventEntry Entry;
            public int OriginalIndex;
        }

        private bool TryGetSortDate(EventEntry entry, Dictionary<int, ScheduledEventDateDefinition> dict, out System.DateTime date)
        {
            date = default;

            if (dict == null)
                return false;

            if (!dict.TryGetValue(entry.DateDefinitionKey.Id, out var definition))
                return false;

            if (entry.DateType == DateType.StartDate)
            {
                date = definition.Dates.StartDate.Date;
                if (startDateOptions.SubtractDay)
                    date = date.AddDays(-1);
                return true;
            }

            if (entry.DateType == DateType.EndDate)
            {
                date = definition.Dates.EndDate.Date;
                if (endDateOptions.SubtractDay)
                    date = date.AddDays(-1);
                return true;
            }

            if (entry.DateType == DateType.Both)
            {
                date = definition.Dates.StartDate.Date;
                if (startDateOptions.SubtractDay)
                    date = date.AddDays(-1);
                return true;
            }

            return false;
        }

        private IEnumerator RunSlideshow()
        {
            currentIndex = 0;

            UpdateEventContent(currentIndex);
            yield return Fade(0f, 1f);

            while (true)
            {
                yield return new WaitForSeconds(changeTimerSeconds);

                yield return Fade(1f, 0f);

                currentIndex++;
                if (currentIndex >= runtimeEvents.Count)
                    currentIndex = 0;

                UpdateEventContent(currentIndex);
                yield return Fade(0f, 1f);
            }
        }

        private void UpdateEventContent(int index)
        {
            var entry = runtimeEvents[index];

            if (eventNameText != null)
                eventNameText.text = entry.EventName;

            if (eventDateText != null)
                eventDateText.text = GetEventDate(entry);

            ApplyEventSprite(entry);
            ApplyEventGradientColor(entry);
        }

        private void ApplyEventSprite(EventEntry entry)
        {
            if (globalImage == null)
                return;

            bool hasA = entry.SpriteA != null;
            bool hasB = entry.SpriteB != null;

            int key = entry.DateDefinitionKey.Id;

            if (hasA && hasB)
            {
                bool useA;

                if (!eventSpriteToggle.TryGetValue(key, out useA))
                    useA = true;
                else
                    useA = !useA;

                eventSpriteToggle[key] = useA;
                globalImage.sprite = useA ? entry.SpriteA : entry.SpriteB;
            }
            else if (hasA)
            {
                globalImage.sprite = entry.SpriteA;
            }
            else if (hasB)
            {
                globalImage.sprite = entry.SpriteB;
            }
        }

        private void ApplyEventGradientColor(EventEntry entry)
        {
            if (globalGradientGraphic == null)
                return;

            if (entry.OverrideGradientColor)
                globalGradientGraphic.color = entry.GradientColor;
        }

        private string GetEventDate(EventEntry entry)
        {
            var dict = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
            if (!dict.TryGetValue(entry.DateDefinitionKey.Id, out var definition))
                return "";

            if (entry.DateType == DateType.StartDate)
                return BuildSingleDate(definition.Dates.StartDate, startDateOptions);

            if (entry.DateType == DateType.EndDate)
                return BuildSingleDate(definition.Dates.EndDate, endDateOptions);

            if (entry.DateType == DateType.Both)
            {
                string start = BuildSingleDate(definition.Dates.StartDate, startDateOptions);
                string end = BuildSingleDate(definition.Dates.EndDate, endDateOptions);
                return $"{start} - {end}";
            }

            return "";
        }

        private string BuildSingleDate(DateUnityWrapper wrapper, DateOptions options)
        {
            System.DateTime date = wrapper.Date;

            if (options.SubtractDay)
                date = date.AddDays(-1);

            List<string> parts = new List<string>();

            if (options.UseMonth) parts.Add(date.GetLocalizedMonth());
            if (options.UseDay) parts.Add(date.Day.ToString());
            if (options.UseYear) parts.Add(date.Year.ToString());

            return string.Join(" ", parts);
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                dateCanvasGroup.alpha = Mathf.Lerp(from, to, fadeCurve.Evaluate(Mathf.Clamp01(t)));
                yield return null;
            }

            dateCanvasGroup.alpha = to;
        }
    }
}
