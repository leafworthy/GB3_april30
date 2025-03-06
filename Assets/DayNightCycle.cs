using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
  
    [Header("References")]
    [Tooltip("The Light2D component to control")]
    public Light2D targetLight;
    [HideInInspector] public float previousTimeOfDay;
    [Header("Time Settings")]
    [Tooltip("Duration of a full day-night cycle in seconds")]
    public float cycleDuration = 240f;
    [Tooltip("Starting time of day (0-1 where 0 is midnight, 0.25 is sunrise, 0.5 is noon, 0.75 is sunset)")]
    [Range(0f, 1f)] public float timeOfDay = 0.25f;
    [Tooltip("How fast time passes")]
    public float timeMultiplier = 1.0f;
    [Tooltip("Pause the cycle")]
    public bool pauseCycle = false;

    [Header("Light Intensity")]
    [Tooltip("Curve to control light intensity throughout the day")]
    public AnimationCurve intensityCurve = AnimationCurve.Linear(0f, 0.1f, 1f, 1f);
    [Tooltip("Multiplier for the intensity value from the curve")]
    public float intensityMultiplier = 1.0f;

    [Header("Light Color")]
    [Tooltip("Gradient to control light color throughout the day")]
    public Gradient colorGradient;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private string currentTimeString;

    // Time labels for easier understanding
    private readonly string[] timeLabels = new string[] 
    {
        "Midnight", "Early Morning", "Sunrise", "Morning", 
        "Noon", "Afternoon", "Sunset", "Evening", "Night"
    };

    // Initialize default values
    private void Reset()
    {
        // Find light component if not assigned
        if (targetLight == null)
            targetLight = GetComponent<Light2D>();
        
        // Create default color gradient
        colorGradient = new Gradient();
        var colorKeys = new GradientColorKey[6];
        colorKeys[0] = new GradientColorKey(new Color(0.11f, 0.1f, 0.3f), 0.0f);  // Midnight (dark blue)
        colorKeys[1] = new GradientColorKey(new Color(0.7f, 0.3f, 0.2f), 0.2f);   // Dawn (orange)
        colorKeys[2] = new GradientColorKey(new Color(1f, 0.9f, 0.7f), 0.3f);     // Sunrise (warm yellow)
        colorKeys[3] = new GradientColorKey(new Color(1f, 1f, 1f), 0.5f);         // Noon (white)
        colorKeys[4] = new GradientColorKey(new Color(1f, 0.6f, 0.3f), 0.75f);    // Sunset (orange)
        colorKeys[5] = new GradientColorKey(new Color(0.1f, 0.1f, 0.25f), 0.9f);  // Night (dark blue)
        
        var alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
        
        colorGradient.SetKeys(colorKeys, alphaKeys);
        
        // Create default intensity curve
        intensityCurve = new AnimationCurve();
        intensityCurve.AddKey(new Keyframe(0.0f, 0.1f));    // Midnight (dim)
        intensityCurve.AddKey(new Keyframe(0.2f, 0.2f));    // Dawn (increasing)
        intensityCurve.AddKey(new Keyframe(0.3f, 0.8f));    // Sunrise (brightening)
        intensityCurve.AddKey(new Keyframe(0.5f, 1.0f));    // Noon (brightest)
        intensityCurve.AddKey(new Keyframe(0.75f, 0.8f));   // Sunset (dimming)
        intensityCurve.AddKey(new Keyframe(0.85f, 0.2f));   // Dusk (dimmer)
        intensityCurve.AddKey(new Keyframe(1.0f, 0.1f));    // Night (dim)
        
        // Smooth out the curve
        for (int i = 0; i < intensityCurve.keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(intensityCurve, i, AnimationUtility.TangentMode.Auto);
            AnimationUtility.SetKeyRightTangentMode(intensityCurve, i, AnimationUtility.TangentMode.Auto);
        }
    }

    private void Start()
    {
        // Validate light component
        if (targetLight == null)
        {
            Debug.LogError("DayNightCycle: No Light2D target assigned!");
            enabled = false;
            return;
        }
        
        // Apply initial light settings
        UpdateLight();
    }

    private void Update()
    {
        if (!pauseCycle)
        {
            // Update time of day
            timeOfDay += (Time.deltaTime / cycleDuration) * timeMultiplier;
            
            // Wrap time of day between 0 and 1
            if (timeOfDay >= 1.0f)
                timeOfDay -= 1.0f;
        }
        
        // Apply light settings based on time of day
        UpdateLight();
        
        // Update debug information
        if (showDebugInfo)
            UpdateDebugInfo();
    }

    public void UpdateLight()
    {
        if (targetLight == null) return;
        
        // Update color from gradient
        targetLight.color = colorGradient.Evaluate(timeOfDay);
        
        // Update intensity from animation curve
        targetLight.intensity = intensityCurve.Evaluate(timeOfDay) * intensityMultiplier;
    }
    
    public float GetLightIntensity() => intensityCurve.Evaluate(timeOfDay) * intensityMultiplier;

    private void UpdateDebugInfo()
    {
        float hours = timeOfDay * 24f;
        int hoursInt = Mathf.FloorToInt(hours);
        int minutes = Mathf.FloorToInt((hours - hoursInt) * 60f);
        
        // Convert to 12-hour format with AM/PM
        string period = (hoursInt >= 12) ? "PM" : "AM";
        int displayHours = (hoursInt % 12 == 0) ? 12 : hoursInt % 12;
        
        currentTimeString = string.Format("{0:D2}:{1:D2} {2}", displayHours, minutes, period);

        // Determine approximate time label
        int labelIndex = Mathf.FloorToInt(timeOfDay * timeLabels.Length);
        labelIndex = Mathf.Clamp(labelIndex, 0, timeLabels.Length - 1);
    }

    // Public methods for external control

    /// <summary>
    /// Set the time of day directly (0-1 range)
    /// </summary>
    public void SetTimeOfDay(float newTime)
    {
        timeOfDay = Mathf.Clamp01(newTime);
        UpdateLight();
    }

    /// <summary>
    /// Set the time multiplier (how fast time passes)
    /// </summary>
    public void SetTimeMultiplier(float multiplier)
    {
        timeMultiplier = multiplier;
    }

    /// <summary>
    /// Pause or unpause the cycle
    /// </summary>
    public void SetPaused(bool paused)
    {
        pauseCycle = paused;
    }

    /// <summary>
    /// Skip to a specific time of day
    /// </summary>
    public void SkipToTime(float targetTime)
    {
        timeOfDay = Mathf.Clamp01(targetTime);
        UpdateLight();
    }

    /// <summary>
    /// Skip to morning (6:00 AM)
    /// </summary>
    public void SkipToMorning()
    {
        SkipToTime(0.25f);
    }

    /// <summary>
    /// Skip to noon (12:00 PM)
    /// </summary>
    public void SkipToNoon()
    {
        SkipToTime(0.5f);
    }

    /// <summary>
    /// Skip to sunset (around 6:00 PM)
    /// </summary>
    public void SkipToSunset()
    {
        SkipToTime(0.75f);
    }

    /// <summary>
    /// Skip to midnight (12:00 AM)
    /// </summary>
    public void SkipToMidnight()
    {
        SkipToTime(0.0f);
    }

    public float GetCurrentDayFraction()
    {
        return timeOfDay;
    }
}
