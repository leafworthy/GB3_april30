using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerInteractable))]
public class InteractionTitle : MonoBehaviour
{
    [Tooltip("The title text to display when this interaction is selected")]
    [SerializeField] private string title = "Interact";
    
    [Tooltip("Reference to a TextMeshProUGUI component to display the title (optional)")]
    [SerializeField] private TextMeshProUGUI titleText;
    
    // Reference to the PlayerInteractable
    private PlayerInteractable interactable;
    
    private void Awake()
    {
        interactable = GetComponent<PlayerInteractable>();
    }
    
    private void Start()
    {
        // Subscribe to events
        interactable.OnSelected += HandleSelected;
        interactable.OnDeselected += HandleDeselected;
        
        // Attempt to find the title text component
        if (titleText == null)
        {
            // Look for a TextMeshProUGUI in child objects
            titleText = GetComponentInChildren<TextMeshProUGUI>(true);
            
            if (titleText == null)
            {
                // Try to find it in the interaction indicator
                var indicator = GetComponentInChildren<InteractionIndicator>(true);
                if (indicator != null)
                {
                    titleText = indicator.GetComponentInChildren<TextMeshProUGUI>(true);
                }
            }
        }
    }
    
    // Called when the interaction is selected
    private void HandleSelected(Player player)
    {
        // Update the UI text if available
        if (titleText != null)
        {
            titleText.text = title;
        }
    }
    
    // Called when the interaction is deselected
    private void HandleDeselected(Player player)
    {
        // Clear the UI text if available
        if (titleText != null)
        {
            titleText.text = "";
        }
    }
    
    // Public method to update the title
    public void SetTitle(string newTitle)
    {
        title = newTitle;
        
        // Update the UI if it's currently selected
        if (interactable.isSelected && titleText != null)
        {
            titleText.text = title;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (interactable != null)
        {
            interactable.OnSelected -= HandleSelected;
            interactable.OnDeselected -= HandleDeselected;
        }
    }
}