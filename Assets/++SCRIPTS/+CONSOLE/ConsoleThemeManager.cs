using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GangstaBean.Console
{
    [CreateAssetMenu(fileName = "ConsoleTheme", menuName = "Console/Theme")]
    public class ConsoleTheme : ScriptableObject
    {
        [Header("Panel Colors")]
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        public Color headerColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        public Color sidebarColor = new Color(0.08f, 0.08f, 0.08f, 1f);
        public Color splitterColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        [Header("Text Colors")]
        public Color primaryTextColor = Color.white;
        public Color secondaryTextColor = Color.gray;
        public Color accentTextColor = new Color(0.2f, 0.8f, 1f, 1f);
        public Color titleTextColor = Color.white;
        
        [Header("Message Colors")]
        public Color infoColor = Color.white;
        public Color successColor = Color.green;
        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;
        public Color inputColor = Color.cyan;
        
        [Header("Button Colors")]
        public Color buttonNormalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        public Color buttonHoverColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color buttonActiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        public Color buttonTextColor = Color.white;
        
        [Header("Input Field Colors")]
        public Color inputFieldColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        public Color inputTextColor = Color.white;
        public Color placeholderColor = Color.gray;
    }
    
    public class ConsoleThemeManager : MonoBehaviour
    {
        [Header("Theme Settings")]
        [SerializeField] private ConsoleTheme currentTheme;
        [SerializeField] private ConsoleTheme[] availableThemes;
        
        [Header("UI References")]
        [SerializeField] private Image[] backgroundPanels;
        [SerializeField] private Image[] headerPanels;
        [SerializeField] private Image[] sidebarPanels;
        [SerializeField] private Image[] splitterHandles;
        [SerializeField] private TextMeshProUGUI[] primaryTexts;
        [SerializeField] private TextMeshProUGUI[] secondaryTexts;
        [SerializeField] private TextMeshProUGUI[] titleTexts;
        [SerializeField] private Button[] buttons;
        [SerializeField] private TMP_InputField[] inputFields;
        
        private void Start()
        {
            if (currentTheme == null && availableThemes.Length > 0)
                currentTheme = availableThemes[0];
            
            ApplyTheme();
        }
        
        public void ApplyTheme()
        {
            if (currentTheme == null) return;
            
            ApplyPanelColors();
            ApplyTextColors();
            ApplyButtonColors();
            ApplyInputFieldColors();
        }
        
        private void ApplyPanelColors()
        {
            foreach (var panel in backgroundPanels)
            {
                if (panel != null)
                    panel.color = currentTheme.backgroundColor;
            }
            
            foreach (var panel in headerPanels)
            {
                if (panel != null)
                    panel.color = currentTheme.headerColor;
            }
            
            foreach (var panel in sidebarPanels)
            {
                if (panel != null)
                    panel.color = currentTheme.sidebarColor;
            }
            
            foreach (var handle in splitterHandles)
            {
                if (handle != null)
                    handle.color = currentTheme.splitterColor;
            }
        }
        
        private void ApplyTextColors()
        {
            foreach (var text in primaryTexts)
            {
                if (text != null)
                    text.color = currentTheme.primaryTextColor;
            }
            
            foreach (var text in secondaryTexts)
            {
                if (text != null)
                    text.color = currentTheme.secondaryTextColor;
            }
            
            foreach (var text in titleTexts)
            {
                if (text != null)
                    text.color = currentTheme.titleTextColor;
            }
        }
        
        private void ApplyButtonColors()
        {
            foreach (var button in buttons)
            {
                if (button == null) continue;
                
                var colorBlock = button.colors;
                colorBlock.normalColor = currentTheme.buttonNormalColor;
                colorBlock.highlightedColor = currentTheme.buttonHoverColor;
                colorBlock.pressedColor = currentTheme.buttonActiveColor;
                colorBlock.selectedColor = currentTheme.buttonActiveColor;
                button.colors = colorBlock;
                
                var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.color = currentTheme.buttonTextColor;
            }
        }
        
        private void ApplyInputFieldColors()
        {
            foreach (var inputField in inputFields)
            {
                if (inputField == null) continue;
                
                var image = inputField.GetComponent<Image>();
                if (image != null)
                    image.color = currentTheme.inputFieldColor;
                
                inputField.textComponent.color = currentTheme.inputTextColor;
                
                if (inputField.placeholder is TextMeshProUGUI placeholder)
                    placeholder.color = currentTheme.placeholderColor;
            }
        }
        
        public void SetTheme(int themeIndex)
        {
            if (themeIndex >= 0 && themeIndex < availableThemes.Length)
            {
                currentTheme = availableThemes[themeIndex];
                ApplyTheme();
            }
        }
        
        public void SetTheme(ConsoleTheme theme)
        {
            if (theme != null)
            {
                currentTheme = theme;
                ApplyTheme();
            }
        }
        
        public ConsoleTheme GetCurrentTheme()
        {
            return currentTheme;
        }
        
        public Color GetMessageColor(ConsoleMessageType messageType)
        {
            if (currentTheme == null) return Color.white;
            
            return messageType switch
            {
                ConsoleMessageType.Success => currentTheme.successColor,
                ConsoleMessageType.Warning => currentTheme.warningColor,
                ConsoleMessageType.Error => currentTheme.errorColor,
                ConsoleMessageType.Input => currentTheme.inputColor,
                _ => currentTheme.infoColor
            };
        }
        
        [ContextMenu("Apply Current Theme")]
        public void ForceApplyTheme()
        {
            ApplyTheme();
        }
        
        [ContextMenu("Auto-Assign UI References")]
        public void AutoAssignReferences()
        {
            backgroundPanels = FindObjectsOfType<Image>();
            primaryTexts = FindObjectsOfType<TextMeshProUGUI>();
            buttons = FindObjectsOfType<Button>();
            inputFields = FindObjectsOfType<TMP_InputField>();
        }
    }
}