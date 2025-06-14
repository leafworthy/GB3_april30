using UnityEngine;
using UnityEditor;
using __SCRIPTS;

namespace __SCRIPTS.Editor
{
    public class PerformanceAnalysisMenu
    {
        [MenuItem("Tools/Performance Analysis/Analyze GetComponent Usage")]
        public static void AnalyzeGetComponentUsage()
        {
            // Find existing analyzer or create temporary one
            var analyzer = FindOrCreateAnalyzer();
            
            if (analyzer != null)
            {
                analyzer.AnalyzeGetComponentUsage();
                Debug.Log("GetComponent analysis completed. Check the Console and PerformanceReports folder for results.");
            }
            else
            {
                Debug.LogError("Failed to create GetComponentAnalyzer.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Create Performance Tools GameObject")]
        public static void CreatePerformanceToolsGameObject()
        {
            // Create a GameObject with all performance tools
            GameObject perfTools = new GameObject("PerformanceTools");
            
            // Add all performance components
            perfTools.AddComponent<GetComponentAnalyzer>();
            perfTools.AddComponent<PerformanceProfiler>();
            perfTools.AddComponent<PerformanceTestRunner>();
            perfTools.AddComponent<PerformanceComparer>();
            
            // Select the created object
            Selection.activeGameObject = perfTools;
            
            Debug.Log("PerformanceTools GameObject created with all analysis components.");
            Debug.Log("You can now right-click on components in the Inspector to access context menus.");
        }
        
        [MenuItem("Tools/Performance Analysis/Run Quick Performance Test")]
        public static void RunQuickPerformanceTest()
        {
            var testRunner = FindOrCreateTestRunner();
            
            if (testRunner != null)
            {
                testRunner.RunPerformanceTests();
                Debug.Log("Performance test started. Check the Console for progress updates.");
            }
            else
            {
                Debug.LogError("Failed to create PerformanceTestRunner.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Start Simple Performance Measurement")]
        public static void StartSimplePerformanceMeasurement()
        {
            var profiler = FindOrCreateProfiler();
            
            if (profiler != null)
            {
                profiler.StartManualMeasurement();
                Debug.Log("Performance measurement started. Will run for 60 seconds and generate a report.");
                Debug.Log("PLAY YOUR GAME NORMALLY during this time - move around, fight enemies, interact with objects.");
                Debug.Log("Game will PAUSE automatically when measurement completes.");
                Debug.Log("After completion, use 'Compare Against Baseline' to see performance improvements.");
            }
            else
            {
                Debug.LogError("Failed to create PerformanceProfiler.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Start Performance Test (Auto-Pause)")]
        public static void StartPerformanceTestWithAutoPause()
        {
            var testRunner = FindOrCreateTestRunner();
            
            if (testRunner != null)
            {
                // Configure for auto-pause
                var profiler = FindOrCreateProfiler();
                if (profiler != null)
                {
                    // Use reflection to set pauseOnComplete = true
                    var pauseField = profiler.GetType().GetField("pauseOnComplete", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    pauseField?.SetValue(profiler, true);
                }
                
                testRunner.RunPerformanceTests();
                Debug.Log("Performance test started. Game will PAUSE automatically when all tests complete.");
                Debug.Log("Check PerformanceReports folder for detailed results after completion.");
            }
            else
            {
                Debug.LogError("Failed to create PerformanceTestRunner.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Compare Against Baseline")]
        public static void CompareAgainstBaseline()
        {
            var comparer = FindOrCreateComparer();
            
            if (comparer != null)
            {
                comparer.CompareAgainstBaseline();
                Debug.Log("Performance comparison completed. Check the Console and PerformanceReports folder for results.");
            }
            else
            {
                Debug.LogError("Failed to create PerformanceComparer.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Save Current as Baseline")]
        public static void SaveCurrentAsBaseline()
        {
            var comparer = FindOrCreateComparer();
            
            if (comparer != null)
            {
                comparer.SaveCurrentAsBaseline();
                Debug.Log("Current performance saved as baseline for future comparisons.");
            }
            else
            {
                Debug.LogError("Failed to create PerformanceComparer.");
            }
        }
        
        [MenuItem("Tools/Performance Analysis/Testing Workflow Instructions")]
        public static void ShowTestingWorkflow()
        {
            Debug.Log("=== PROPER PERFORMANCE TESTING WORKFLOW ===");
            Debug.Log("STEP 1: First, replace FollowCursor with FollowCursor_Unoptimized in your scene");
            Debug.Log("STEP 2: Run 'Start Simple Performance Measurement' and play normally for 60 seconds");
            Debug.Log("STEP 3: This automatically saves as baseline");
            Debug.Log("STEP 4: Replace back with optimized FollowCursor");
            Debug.Log("STEP 5: Run 'Start Simple Performance Measurement' again for 60 seconds");
            Debug.Log("STEP 6: Run 'Compare Against Baseline' to see the improvement!");
            Debug.Log("");
            Debug.Log("The key is measuring BEFORE optimizations (baseline) vs AFTER optimizations (current)");
        }
        
        private static GetComponentAnalyzer FindOrCreateAnalyzer()
        {
            // Look for existing analyzer
            var analyzer = Object.FindObjectOfType<GetComponentAnalyzer>();
            
            if (analyzer == null)
            {
                // Create temporary GameObject with analyzer
                GameObject temp = new GameObject("TempAnalyzer");
                analyzer = temp.AddComponent<GetComponentAnalyzer>();
                
                // Set it to be destroyed after use
                temp.hideFlags = HideFlags.HideAndDontSave;
            }
            
            return analyzer;
        }
        
        private static PerformanceTestRunner FindOrCreateTestRunner()
        {
            // Look for existing test runner
            var testRunner = Object.FindObjectOfType<PerformanceTestRunner>();
            
            if (testRunner == null)
            {
                // Create temporary GameObject with test runner
                GameObject temp = new GameObject("TempTestRunner");
                testRunner = temp.AddComponent<PerformanceTestRunner>();
                
                // Set it to be destroyed after use
                temp.hideFlags = HideFlags.HideAndDontSave;
            }
            
            return testRunner;
        }
        
        private static PerformanceProfiler FindOrCreateProfiler()
        {
            // Look for existing profiler
            var profiler = Object.FindObjectOfType<PerformanceProfiler>();
            
            if (profiler == null)
            {
                // Create temporary GameObject with profiler
                GameObject temp = new GameObject("TempProfiler");
                profiler = temp.AddComponent<PerformanceProfiler>();
                
                // Set it to be destroyed after use
                temp.hideFlags = HideFlags.HideAndDontSave;
            }
            
            return profiler;
        }
        
        private static PerformanceComparer FindOrCreateComparer()
        {
            // Look for existing comparer
            var comparer = Object.FindObjectOfType<PerformanceComparer>();
            
            if (comparer == null)
            {
                // Create temporary GameObject with comparer
                GameObject temp = new GameObject("TempComparer");
                comparer = temp.AddComponent<PerformanceComparer>();
                
                // Set it to be destroyed after use
                temp.hideFlags = HideFlags.HideAndDontSave;
            }
            
            return comparer;
        }
    }
}