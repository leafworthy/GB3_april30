using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GangstaBean.Console
{
    public class ConsolePanelManager : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private RectTransform mainPanel;
        [SerializeField] private RectTransform sidePanel;
        [SerializeField] private RectTransform splitterHandle;
        
        [Header("Layout Settings")]
        [SerializeField] private float minMainPanelWidth = 400f;
        [SerializeField] private float minSidePanelWidth = 200f;
        [SerializeField] private float splitterWidth = 4f;
        [SerializeField] private bool rememberLayout = true;
        
        private Canvas parentCanvas;
        private bool isDragging = false;
        private Vector2 lastMousePosition;
        private float totalWidth;
        
        private const string MAIN_PANEL_WIDTH_KEY = "Console_MainPanelWidth";
        private const string SIDE_PANEL_WIDTH_KEY = "Console_SidePanelWidth";
        
        private void Start()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            SetupSplitter();
            LoadLayout();
        }
        
        private void SetupSplitter()
        {
            if (splitterHandle == null) return;
            
            var eventTrigger = splitterHandle.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = splitterHandle.gameObject.AddComponent<EventTrigger>();
            
            var beginDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDragEntry.callback.AddListener(OnBeginDrag);
            eventTrigger.triggers.Add(beginDragEntry);
            
            var dragEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            dragEntry.callback.AddListener(OnDrag);
            eventTrigger.triggers.Add(dragEntry);
            
            var endDragEntry = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDragEntry.callback.AddListener(OnEndDrag);
            eventTrigger.triggers.Add(endDragEntry);
        }
        
        private void OnBeginDrag(BaseEventData eventData)
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
            totalWidth = GetComponent<RectTransform>().rect.width;
        }
        
        private void OnDrag(BaseEventData eventData)
        {
            if (!isDragging) return;
            
            Vector2 currentMousePosition = Input.mousePosition;
            float deltaX = currentMousePosition.x - lastMousePosition.x;
            
            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Camera cam = parentCanvas.worldCamera;
                if (cam != null)
                {
                    deltaX /= cam.pixelWidth / Screen.width;
                }
            }
            
            ResizePanels(deltaX);
            lastMousePosition = currentMousePosition;
        }
        
        private void OnEndDrag(BaseEventData eventData)
        {
            isDragging = false;
            if (rememberLayout)
                SaveLayout();
        }
        
        private void ResizePanels(float deltaX)
        {
            if (mainPanel == null || sidePanel == null) return;
            
            float currentMainWidth = mainPanel.rect.width;
            float currentSideWidth = sidePanel.rect.width;
            
            float newMainWidth = Mathf.Max(minMainPanelWidth, currentMainWidth + deltaX);
            float newSideWidth = Mathf.Max(minSidePanelWidth, currentSideWidth - deltaX);
            
            float totalAvailableWidth = totalWidth - splitterWidth;
            if (newMainWidth + newSideWidth > totalAvailableWidth)
            {
                if (deltaX > 0)
                    newSideWidth = totalAvailableWidth - newMainWidth;
                else
                    newMainWidth = totalAvailableWidth - newSideWidth;
            }
            
            mainPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newMainWidth);
            sidePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSideWidth);
            
            UpdatePanelPositions();
        }
        
        private void UpdatePanelPositions()
        {
            if (mainPanel == null || sidePanel == null || splitterHandle == null) return;
            
            float mainWidth = mainPanel.rect.width;
            
            mainPanel.anchoredPosition = new Vector2(0, mainPanel.anchoredPosition.y);
            
            splitterHandle.anchoredPosition = new Vector2(mainWidth, splitterHandle.anchoredPosition.y);
            
            sidePanel.anchoredPosition = new Vector2(mainWidth + splitterWidth, sidePanel.anchoredPosition.y);
        }
        
        private void SaveLayout()
        {
            if (mainPanel != null && sidePanel != null)
            {
                PlayerPrefs.SetFloat(MAIN_PANEL_WIDTH_KEY, mainPanel.rect.width);
                PlayerPrefs.SetFloat(SIDE_PANEL_WIDTH_KEY, sidePanel.rect.width);
                PlayerPrefs.Save();
            }
        }
        
        private void LoadLayout()
        {
            if (!rememberLayout) return;
            
            if (PlayerPrefs.HasKey(MAIN_PANEL_WIDTH_KEY) && PlayerPrefs.HasKey(SIDE_PANEL_WIDTH_KEY))
            {
                float savedMainWidth = PlayerPrefs.GetFloat(MAIN_PANEL_WIDTH_KEY);
                float savedSideWidth = PlayerPrefs.GetFloat(SIDE_PANEL_WIDTH_KEY);
                
                totalWidth = GetComponent<RectTransform>().rect.width;
                
                if (savedMainWidth >= minMainPanelWidth && savedSideWidth >= minSidePanelWidth)
                {
                    mainPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, savedMainWidth);
                    sidePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, savedSideWidth);
                    UpdatePanelPositions();
                }
            }
        }
        
        public void ResetLayout()
        {
            if (mainPanel == null || sidePanel == null) return;
            
            totalWidth = GetComponent<RectTransform>().rect.width;
            float availableWidth = totalWidth - splitterWidth;
            
            float defaultMainWidth = availableWidth * 0.75f;
            float defaultSideWidth = availableWidth * 0.25f;
            
            mainPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultMainWidth);
            sidePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultSideWidth);
            
            UpdatePanelPositions();
            
            if (rememberLayout)
                SaveLayout();
        }
        
        public void SetPanelSizes(float mainWidthPercent, float sideWidthPercent)
        {
            if (mainPanel == null || sidePanel == null) return;
            
            totalWidth = GetComponent<RectTransform>().rect.width;
            float availableWidth = totalWidth - splitterWidth;
            
            float newMainWidth = Mathf.Max(minMainPanelWidth, availableWidth * mainWidthPercent);
            float newSideWidth = Mathf.Max(minSidePanelWidth, availableWidth * sideWidthPercent);
            
            mainPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newMainWidth);
            sidePanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSideWidth);
            
            UpdatePanelPositions();
        }
    }
}