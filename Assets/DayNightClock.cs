using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(DayNightCycle))]
public class DayNightClock : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Reference to the Text UI element that displays the time")]
    public TMP_Text timeText;
    
    [Tooltip("Reference to the Image used for daytime radial progress")]
    public Image daytimeRadialImage;
    
    [Tooltip("Reference to the Image used for nighttime radial progress")]
    public Image nighttimeRadialImage;
    
    [Tooltip("Reference to the UI outline element")]
    public Image outlineImage;
    
    [Tooltip("Reference to the background image")]
    public Image backgroundImage;
    
    [Tooltip("Reference to the day icon GameObject")]
    public GameObject dayIcon;
    
    [Tooltip("Reference to the night icon GameObject")]
    public GameObject nightIcon;

    [Header("Time Format Settings")]
    [Tooltip("Use 24-hour format instead of 12-hour with AM/PM")]
    public bool use24HourFormat = false;
    
    [Tooltip("Show AM/PM indicator (12-hour format only)")]
    public bool showAmPm = true;
    
    [Tooltip("Show current day number")]
    public bool showDay = true;

    [Header("Visual Settings")]
    [Tooltip("Color of the time text during daytime")]
    public Color daytimeTextColor = Color.black;
    
    [Tooltip("Color of the time text during nighttime")]
    public Color nighttimeTextColor = Color.white;
    
    [Tooltip("Color of the outline UI element during daytime")]
    public Color daytimeOutlineColor = new Color(0.8f, 0.8f, 0.2f);
    
    [Tooltip("Color of the outline UI element during nighttime")]
    public Color nighttimeOutlineColor = new Color(0.2f, 0.3f, 0.8f);
    
    [Tooltip("Color of the background during daytime")]
    public Color daytimeBackgroundColor = new Color(0.9f, 0.9f, 0.7f);
    
    [Tooltip("Color of the background during nighttime")]
    public Color nighttimeBackgroundColor = new Color(0.1f, 0.1f, 0.3f);
    
    [Tooltip("Time of day considered as the start of daytime (0-1 range)")]
    [Range(0, 1)] public float daytimeStart = 0.25f; // 6 AM
    
    [Tooltip("Time of day considered as the start of nighttime (0-1 range)")]
    [Range(0, 1)] public float nighttimeStart = 0.75f; // 6 PM

    // References
    private DayNightCycle dayNightCycle;
    public int currentDay = 1;
    private float previousTimeOfDay;

    private void Start()
    {
        // Get reference to DayNightCycle component
        dayNightCycle = GetComponent<DayNightCycle>();
        if (dayNightCycle == null)
        {
            Debug.LogError("DayNightClock: No DayNightCycle component found!");
            enabled = false;
            return;
        }

        // Validate UI references
        if (timeText == null)
        {
            Debug.LogWarning("DayNightClock: Time text reference is missing!");
        }
        
        // Initial update
        UpdateClock(dayNightCycle.timeOfDay);
    }

    private void Update()
    {
        float timeOfDay = dayNightCycle.timeOfDay;
        
        // Check if a new day has started
        if (timeOfDay < previousTimeOfDay)
        {
            currentDay++;
        }
        
        // Store previous time of day for day change detection
        previousTimeOfDay = timeOfDay;
        
        // Update clock visuals
        UpdateClock(timeOfDay);
    }

    private void UpdateClock(float timeOfDay)
    {
        // Determine if it's daytime or nighttime
        bool isDaytime = IsDaytime(timeOfDay);
        
        // Update time text
        if (timeText != null)
        {
            // Format time string
            string timeString = FormatTimeString(timeOfDay);
            
            // Add day counter if enabled
            if (showDay)
            {
                if(isDaytime)
                    timeString += $"\nDay {currentDay}";
                else
                timeString += $"\nNight {currentDay}";
            }
            
            // Set text and color
            timeText.text = timeString;
            timeText.color = isDaytime ? daytimeTextColor : nighttimeTextColor;
        }
        
        // Update radial progress for day/night segments
        if (daytimeRadialImage != null)
        {
            daytimeRadialImage.gameObject.SetActive(isDaytime);
            if (isDaytime)
            {
                // Calculate day progress (0-1) based on time within day period
                float dayDuration = GetDayDuration();
                float dayProgress = (timeOfDay - daytimeStart + (timeOfDay < daytimeStart ? 1 : 0)) / dayDuration;
                dayProgress = Mathf.Clamp01(dayProgress);
                daytimeRadialImage.fillAmount = dayProgress;
            }
        }
        
        if (nighttimeRadialImage != null)
        {
            nighttimeRadialImage.gameObject.SetActive(!isDaytime);
            if (!isDaytime)
            {
                // Calculate night progress (0-1) based on time within night period
                float nightDuration = GetNightDuration();
                float nightProgress = (timeOfDay - nighttimeStart + (timeOfDay < nighttimeStart ? 1 : 0)) / nightDuration;
                nightProgress = Mathf.Clamp01(nightProgress);
                nighttimeRadialImage.fillAmount = nightProgress;
            }
        }
        
        // Update outline color
        if (outlineImage != null)
        {
            outlineImage.color = isDaytime ? daytimeOutlineColor : nighttimeOutlineColor;
        }
        
        // Update background color
        if (backgroundImage != null)
        {
            backgroundImage.color = isDaytime ? daytimeBackgroundColor : nighttimeBackgroundColor;
        }
        
        // Update day/night icons
        if (dayIcon != null)
        {
            dayIcon.SetActive(isDaytime);
        }
        
        if (nightIcon != null)
        {
            nightIcon.SetActive(!isDaytime);
        }
    }

    private bool IsDaytime(float timeOfDay)
    {
        if (daytimeStart < nighttimeStart)
        {
            // Normal case (daytime starts before nighttime)
            return timeOfDay >= daytimeStart && timeOfDay < nighttimeStart;
        }
        else
        {
            // Edge case (daytime crosses midnight)
            return timeOfDay >= daytimeStart || timeOfDay < nighttimeStart;
        }
    }
    
    private float GetDayDuration()
    {
        if (daytimeStart < nighttimeStart)
        {
            // Normal case
            return nighttimeStart - daytimeStart;
        }
        else
        {
            // Day crosses midnight
            return (1 - daytimeStart) + nighttimeStart;
        }
    }
    
    private float GetNightDuration()
    {
        if (nighttimeStart < daytimeStart)
        {
            // Normal case
            return daytimeStart - nighttimeStart;
        }
        else
        {
            // Night crosses midnight
            return (1 - nighttimeStart) + daytimeStart;
        }
    }

    private string FormatTimeString(float timeOfDay)
    {
        // Convert time of day (0-1) to hours and minutes
        float hours = timeOfDay * 24f;
        int hoursInt = Mathf.FloorToInt(hours);
        int minutes = Mathf.FloorToInt((hours - hoursInt) * 60f);
        
        // Format based on settings
        if (use24HourFormat)
        {
            return string.Format("{0:D2}:{1:D2}", hoursInt, minutes);
        }
        else
        {
            string period = (hoursInt >= 12) ? "PM" : "AM";
            int displayHours = (hoursInt % 12 == 0) ? 12 : hoursInt % 12;
            
            if (showAmPm)
                return string.Format("{0:D2}:{1:D2} {2}", displayHours, minutes, period);
            else
                return string.Format("{0:D2}:{1:D2}", displayHours, minutes);
        }
    }
}

