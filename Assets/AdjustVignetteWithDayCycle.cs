using UnityEngine;
using UnityEngine.UI;

public class AdjustVignetteWithDayCycle : MonoBehaviour
{
    private DayNightCycle dayNightCycle;
    private Image vignette;
    public AnimationCurve vignetteCurve;
    [Range(0,1)]
    public float amount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();
        vignette = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(dayNightCycle == null) return;
        vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, vignetteCurve.Evaluate(dayNightCycle.GetCurrentDayFraction())); 
    }
}
