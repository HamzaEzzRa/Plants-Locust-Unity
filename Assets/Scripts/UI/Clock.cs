using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class Clock : MonoBehaviour
{
    private struct TimeObject
    {
        Mask mask;
        RectTransform[] props;

        public Mask Mask => mask;
        public RectTransform[] Props => props;

        public Image[] Images { get; private set; }

        public TimeObject(Mask mask, RectTransform[] props)
        {
            this.mask = mask;
            this.props = props;

            List<Image> tmp = new List<Image>(props.Length);
            tmp.Add(mask.GetComponent<Image>());
            foreach (RectTransform rectTransform in props)
            {
                Image[] propImages = rectTransform.GetComponentsInChildren<Image>();
                foreach (Image image in propImages)
                {
                    tmp.Add(image);
                }
            }

            Images = tmp.ToArray();
        }
    }

    [SerializeField] private Mask dayMask;
    [SerializeField] private RectTransform sun;
    [SerializeField] private RectTransform clouds;
    [SerializeField, Range(1f, 2f)] private float sunOffsetFactor = 1f;
    
    [SerializeField] private Mask nightMask;
    [SerializeField] private RectTransform moon;
    [SerializeField] private RectTransform stars;
    [SerializeField, Range(1f, 2f)] private float moonOffsetFactor = 1f;

    [SerializeField, Range(0.001f, 1f)] private float fadeSpeed = 0.01f;
    [SerializeField, Range(1f, 5f)] private float cloudSpeed = 2f;

    [SerializeField] private bool isActive;

    private TimeObject dayObject, nightObject;

    private void Start()
    {
        dayObject = new TimeObject(dayMask, new RectTransform[] { sun, clouds });
        nightObject = new TimeObject(nightMask, new RectTransform[] { moon, stars });
    }

    private void Update()
    {
        if (isActive)
        {
            TimeManager timeManager = TimeManager.Instance;
            if (!timeManager)
            {
                timeManager = GameObject.Find(FindObjectOfType(typeof(TimeManager)).name)
                    .transform.GetComponent<TimeManager>();
            }

            float timeOfDay = timeManager.TimeOfDay;
            float factor = (timeOfDay / 24f) * 2 * Mathf.PI;

            if (!timeManager.AtNight)
            {
                if (!dayMask.gameObject.activeInHierarchy)
                {
                    maskSwitch(nightObject, dayObject);
                }

                float width = dayMask.rectTransform.rect.width - sun.rect.width * sunOffsetFactor;
                float height = dayMask.rectTransform.rect.height - sun.rect.height * sunOffsetFactor;
                sun.transform.localPosition = new Vector2(
                    -Mathf.Sin(2 * factor - Mathf.PI) * width,
                    -Mathf.Cos(2 * factor - Mathf.PI) * height
                );

                float xLimit = dayMask.rectTransform.rect.width + clouds.rect.width / 2f;
                clouds.localPosition += cloudSpeed * Time.deltaTime * 10f * Vector3.right;
                if (clouds.localPosition.x > xLimit)
                    clouds.localPosition = -xLimit * Vector3.right;
            }
            else
            {
                if (!nightMask.gameObject.activeInHierarchy)
                {
                    maskSwitch(dayObject, nightObject);
                }

                float width = nightMask.rectTransform.rect.width - moon.rect.width * moonOffsetFactor;
                float height = nightMask.rectTransform.rect.height - moon.rect.height * moonOffsetFactor;
                moon.transform.localPosition = new Vector2(
                    -Mathf.Sin(2 * factor - Mathf.PI) * width,
                    -Mathf.Cos(2 * factor - Mathf.PI) * height
                );
            }
        }
    }

    private void maskSwitch(TimeObject toFade, TimeObject toShow)
    {
        toShow.Mask.gameObject.SetActive(true);

        StartCoroutine(
            Extra.FadeSwitch(toFade.Images, toShow.Images, fadeSpeed,
            () => {
                toFade.Mask.gameObject.SetActive(false);
            })
        );
    }

    public string GetTime()
    {
        TimeManager timeManager = TimeManager.Instance;
        if (!timeManager)
        {
            timeManager = GameObject.Find(FindObjectOfType(typeof(TimeManager)).name).transform.GetComponent<TimeManager>();
        }

        string timeIndicator = timeManager.TimeOfDay >= 12 ? "PM" : "AM";

        int hours = (int)timeManager.TimeOfDay % 12;
        int minutes = (int)(timeManager.TimeOfDay * 60) % 60;
        int seconds = (int)(timeManager.TimeOfDay * 3600) % 60;

        string timeString = (hours < 10 ? "0" : "") + hours
            + ":" + (minutes < 10 ? "0" : "") + minutes
            + ":" + (seconds < 10 ? "0" : "") + seconds
            + " " + timeIndicator;
        
        return timeString;
    }
}
