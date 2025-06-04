using TMPro;
using UnityEngine;

namespace GangstaBean.Effects
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDripEffect : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject dripParticlePrefab;
        [SerializeField, Range(0.1f, 5f)] private float emissionRate = 1f;
        [SerializeField] private bool useRandomEmission = true;
        [SerializeField, Range(0f, 2f)] private float randomEmissionVariance = 0.5f;
    
        [Header("Particle Appearance")]
        [SerializeField, Range(0.1f, 3f)] private float baseScale = 1f;
        [SerializeField] private bool useRandomScale = true;
        [SerializeField, Range(0f, 1f)] private float randomScaleVariance = 0.3f;
        [SerializeField] private float destroyAfterSeconds = 3f;
        [SerializeField] private float verticalOffset = 10f;
    
        // Private variables
        private TextMeshProUGUI textMesh;
        private RectTransform rectTransform;
        private float nextEmitTime;
        private Canvas parentCanvas;
        [SerializeField] private GameObject dripParent;
    
        private void Awake()
        {
            dripParent.transform.SetParent(transform.parent, false);
            textMesh = GetComponent<TextMeshProUGUI>();
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
        
            if (parentCanvas == null)
            {
                Debug.LogError("TextDripEffect requires a parent Canvas component");
                enabled = false;
            }
        
            if (dripParticlePrefab == null)
            {
                Debug.LogError("TextDripEffect requires a dripParticlePrefab to be assigned");
                enabled = false;
            }
        }
    
        private void Start()
        {
            nextEmitTime = Time.time + GetNextEmissionDelay();
        }
    
        private void Update()
        {
            // Check if it's time to emit a new particle
            if (Time.time >= nextEmitTime)
            {
                SpawnParticle();
                nextEmitTime = Time.time + GetNextEmissionDelay();
            }
        }
    
        private float GetNextEmissionDelay()
        {
            if (useRandomEmission)
            {
                return emissionRate * (1f + Random.Range(-randomEmissionVariance, randomEmissionVariance));
            }
            return emissionRate;
        }
    
        private void SpawnParticle()
        {
            StartCoroutine(SpawnParticleAtEndOfFrame());
        }
    
        private System.Collections.IEnumerator SpawnParticleAtEndOfFrame()
        {
            // Wait until end of frame to avoid transient artifact errors
            yield return new WaitForEndOfFrame();
        
            // Get the actual width of the text content (not just the rect transform)
            float textWidth = textMesh.preferredWidth;
            if (textWidth <= 0) yield break; // No text to drip from
        
            // Calculate spawn position along the baseline of the text
            float xPos = Random.Range(-textWidth / 2, textWidth / 2);
            Vector2 localSpawnPos = new Vector2(xPos, -textMesh.preferredHeight / 2 + verticalOffset); // Baseline of text with offset
        
            // Create the particle
            GameObject particleObj = Instantiate(dripParticlePrefab);
            particleObj.transform.SetParent(dripParent.transform, false); // Parent to canvas, set worldPositionStays to false
        
            // Position the particle at the correct spawn point
            RectTransform particleRect = particleObj.GetComponent<RectTransform>();
            particleRect.anchoredPosition = rectTransform.anchoredPosition + localSpawnPos;
        
            // Set the scale
            float scale = baseScale;
            if (useRandomScale)
            {
                scale *= (1f - randomScaleVariance/2) + Random.value * randomScaleVariance;
            }
            particleRect.localScale = new Vector3(scale, scale, 1);
        
            // Destroy after animation completes
            Destroy(particleObj, destroyAfterSeconds);
        }
    
        private void OnDisable()
        {
            // Stop spawning when disabled
        }
    }
}