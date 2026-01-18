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
        }

        [Header("TMP Targets")]
        [SerializeField] private TMP_Text eventNameText;
        [SerializeField] private TMP_Text eventDateText;

        [Header("Image Target")]
        [SerializeField] private Image globalImage;

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

            if (events.Count > 0)
                StartCoroutine(RunSlideshow());
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
                if (currentIndex >= events.Count)
                    currentIndex = 0;

                UpdateEventContent(currentIndex);
                yield return Fade(0f, 1f);
            }
        }

        private void UpdateEventContent(int index)
        {
            var entry = events[index];

            if (eventNameText != null)
                eventNameText.text = entry.EventName;

            if (eventDateText != null)
                eventDateText.text = GetEventDate(entry);

            ApplyEventSprite(index, entry);
        }

        private void ApplyEventSprite(int index, EventEntry entry)
        {
            if (globalImage == null)
                return;

            bool hasA = entry.SpriteA != null;
            bool hasB = entry.SpriteB != null;

            if (hasA && hasB)
            {
                bool useA;

                if (!eventSpriteToggle.TryGetValue(index, out useA))
                    useA = true;
                else
                    useA = !useA;

                eventSpriteToggle[index] = useA;
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
