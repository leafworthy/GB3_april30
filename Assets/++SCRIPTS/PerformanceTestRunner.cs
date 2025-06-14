using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __SCRIPTS
{
    public class PerformanceTestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private List<TestScenario> testScenarios = new List<TestScenario>();
        [SerializeField] private float testDuration = 30f;
        [SerializeField] private int warmupFrames = 60;
        [SerializeField] private bool runOnStart = false;
        [SerializeField] private bool generateComparisonReport = true;
        
        [Header("Auto-Actions on Completion")]
        [SerializeField] private bool pauseOnComplete = true;
        [SerializeField] private bool quitOnComplete = false;
        
        [Header("Test Environment")]
        [SerializeField] private GameObject testPrefab;
        [SerializeField] private int baseEnemyCount = 10;
        [SerializeField] private int baseInteractableCount = 5;
        [SerializeField] private Vector2 spawnArea = new Vector2(50f, 50f);
        
        private PerformanceProfiler profiler;
        private List<TestResult> testResults = new List<TestResult>();
        private TestScenario currentScenario;
        private bool isRunningTests = false;
        
        private void Awake()
        {
            profiler = FindObjectOfType<PerformanceProfiler>();
            if (profiler == null)
            {
                GameObject profilerObj = new GameObject("PerformanceProfiler");
                profiler = profilerObj.AddComponent<PerformanceProfiler>();
                DontDestroyOnLoad(profilerObj);
            }
            
            // Always ensure scenarios are initialized
            InitializeDefaultScenarios();
        }
        
        private void Start()
        {
            if (runOnStart)
            {
                StartCoroutine(RunAllTests());
            }
        }
        
        private void InitializeDefaultScenarios()
        {
            if (testScenarios == null)
            {
                testScenarios = new List<TestScenario>();
            }
            
            if (testScenarios.Count == 0)
            {
                testScenarios.Add(new TestScenario
                {
                    name = "Baseline",
                    description = "Current game state without modifications",
                    enemyMultiplier = 1f,
                    interactableMultiplier = 1f,
                    enableAI = true,
                    enableInteractions = true
                });
                
                testScenarios.Add(new TestScenario
                {
                    name = "Heavy Load",
                    description = "Stress test with increased enemy and interactable count",
                    enemyMultiplier = 3f,
                    interactableMultiplier = 4f,
                    enableAI = true,
                    enableInteractions = true
                });
                
                testScenarios.Add(new TestScenario
                {
                    name = "AI Disabled",
                    description = "Test without AI processing to isolate rendering performance",
                    enemyMultiplier = 2f,
                    interactableMultiplier = 2f,
                    enableAI = false,
                    enableInteractions = true
                });
                
                testScenarios.Add(new TestScenario
                {
                    name = "Interactions Disabled",
                    description = "Test without interaction system to isolate update loop performance",
                    enemyMultiplier = 2f,
                    interactableMultiplier = 2f,
                    enableAI = true,
                    enableInteractions = false
                });
            }
        }
        
        [ContextMenu("Run Performance Tests")]
        public void RunPerformanceTests()
        {
            if (isRunningTests)
            {
                Debug.LogWarning("Performance tests already running!");
                return;
            }
            
            // Ensure scenarios are initialized before running tests
            if (testScenarios == null || testScenarios.Count == 0)
            {
                InitializeDefaultScenarios();
            }
            
            StartCoroutine(RunAllTests());
        }
        
        private IEnumerator RunAllTests()
        {
            isRunningTests = true;
            testResults.Clear();
            
            Debug.Log($"Starting performance test suite with {testScenarios.Count} scenarios");
            
            for (int i = 0; i < testScenarios.Count; i++)
            {
                var scenario = testScenarios[i];
                Debug.Log($"Running test scenario {i + 1}/{testScenarios.Count}: {scenario.name}");
                
                yield return StartCoroutine(RunSingleTest(scenario));
                
                // Brief pause between tests
                yield return new WaitForSeconds(2f);
            }
            
            if (generateComparisonReport)
            {
                GenerateComparisonReport();
            }
            
            isRunningTests = false;
            Debug.Log("Performance test suite completed");
            
            // Auto-actions on completion
            if (pauseOnComplete)
            {
                Debug.Log("PerformanceTestRunner: Pausing game. Press Play to continue or check PerformanceReports folder for results.");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
                #else
                Time.timeScale = 0f;
                #endif
            }
            
            if (quitOnComplete)
            {
                Debug.Log("PerformanceTestRunner: Quitting application.");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
        
        private IEnumerator RunSingleTest(TestScenario scenario)
        {
            currentScenario = scenario;
            
            // Setup test environment
            SetupTestEnvironment(scenario);
            
            // Wait for setup to complete
            yield return new WaitForSeconds(1f);
            
            // Warmup period
            yield return StartCoroutine(WaitForFrames(warmupFrames));
            
            // Start profiler measurement
            profiler.StartManualMeasurement();
            
            // Run test for specified duration
            float startTime = Time.time;
            while (Time.time - startTime < testDuration)
            {
                // Simulate player activity during test
                SimulatePlayerActivity();
                yield return null;
            }
            
            // Get results from profiler
            var performanceReport = profiler.GetCurrentReport();
            
            // Create test result
            var testResult = new TestResult
            {
                scenario = scenario,
                performanceReport = performanceReport,
                timestamp = DateTime.Now
            };
            
            testResults.Add(testResult);
            
            // Cleanup test environment
            CleanupTestEnvironment();
            
            Debug.Log($"Test '{scenario.name}' completed. Avg Frame Time: {performanceReport.avgFrameTime:F2}ms");
        }
        
        private void SetupTestEnvironment(TestScenario scenario)
        {
            // Find or create test objects based on scenario
            SpawnTestEnemies(Mathf.RoundToInt(baseEnemyCount * scenario.enemyMultiplier));
            SpawnTestInteractables(Mathf.RoundToInt(baseInteractableCount * scenario.interactableMultiplier));
            
            // Configure AI system
            ConfigureAISystem(scenario.enableAI);
            
            // Configure interaction system
            ConfigureInteractionSystem(scenario.enableInteractions);
        }
        
        private void SpawnTestEnemies(int count)
        {
            var enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                // Use existing enemy spawning system if available
                for (int i = 0; i < count; i++)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition();
                    // This would need to be adapted based on your enemy spawning system
                    // enemyManager.SpawnEnemyAt(spawnPos);
                }
            }
            else
            {
                // Fallback: spawn test objects
                for (int i = 0; i < count; i++)
                {
                    Vector3 spawnPos = GetRandomSpawnPosition();
                    GameObject testEnemy = new GameObject($"TestEnemy_{i}");
                    testEnemy.transform.position = spawnPos;
                    // Use name instead of tag for identification
                    testEnemy.name = "TestObject_Enemy";
                }
            }
        }
        
        private void SpawnTestInteractables(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject testInteractable = new GameObject($"TestInteractable_{i}");
                testInteractable.transform.position = spawnPos;
                // Use name instead of tag for identification
                testInteractable.name = "TestObject_Interactable";
                
                // Add a basic interactable component if available
                var interactable = testInteractable.AddComponent<TestInteractable>();
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            return new Vector3(
                UnityEngine.Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                UnityEngine.Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
                0f
            );
        }
        
        private void ConfigureAISystem(bool enabled)
        {
            // Find and configure AI components
            var aiComponents = FindObjectsOfType<MonoBehaviour>();
            foreach (var ai in aiComponents)
            {
                if (ai.GetType().Name == "EnemyAI")
                {
                    ai.enabled = enabled;
                }
            }
        }
        
        private void ConfigureInteractionSystem(bool enabled)
        {
            // Find and configure interaction components
            var players = FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                // This would need to be adapted based on your interaction system
                // player.SetInteractionsEnabled(enabled);
            }
        }
        
        private void SimulatePlayerActivity()
        {
            // Simulate some player movement and interaction to stress test systems
            var players = FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                // Random movement simulation
                if (UnityEngine.Random.value < 0.1f) // 10% chance per frame
                {
                    Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                    // player.SimulateMovement(randomDirection);
                }
            }
        }
        
        private void CleanupTestEnvironment()
        {
            // Remove all test objects by name
            var allObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name.StartsWith("TestObject_"))
                {
                    DestroyImmediate(obj);
                }
            }
            
            // Reset AI and interaction systems
            ConfigureAISystem(true);
            ConfigureInteractionSystem(true);
        }
        
        private void GenerateComparisonReport()
        {
            if (testResults.Count == 0) return;
            
            string report = FormatComparisonReport();
            SaveComparisonReport(report);
            LogComparisonSummary();
        }
        
        private string FormatComparisonReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== PERFORMANCE TEST COMPARISON REPORT ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Test Duration: {testDuration}s per scenario");
            sb.AppendLine($"Scenarios Tested: {testResults.Count}");
            sb.AppendLine();
            
            // Summary table
            sb.AppendLine("PERFORMANCE SUMMARY:");
            sb.AppendLine("==================");
            sb.AppendFormat("{0,-20} {1,-12} {2,-12} {3,-12} {4,-12}\n", 
                "Scenario", "Avg Frame", "Min Frame", "Max Frame", "Avg FPS");
            sb.AppendLine(new string('-', 70));
            
            foreach (var result in testResults)
            {
                sb.AppendFormat("{0,-20} {1,-12:F2} {2,-12:F2} {3,-12:F2} {4,-12:F1}\n",
                    result.scenario.name,
                    result.performanceReport.avgFrameTime,
                    result.performanceReport.minFrameTime,
                    result.performanceReport.maxFrameTime,
                    1000f / result.performanceReport.avgFrameTime);
            }
            
            sb.AppendLine();
            
            // Detailed analysis
            sb.AppendLine("DETAILED ANALYSIS:");
            sb.AppendLine("================");
            
            if (testResults.Count > 1)
            {
                var baseline = testResults[0];
                for (int i = 1; i < testResults.Count; i++)
                {
                    var current = testResults[i];
                    float frameTimeChange = ((current.performanceReport.avgFrameTime - baseline.performanceReport.avgFrameTime) / baseline.performanceReport.avgFrameTime) * 100f;
                    
                    sb.AppendLine($"{current.scenario.name} vs {baseline.scenario.name}:");
                    sb.AppendLine($"  Frame Time Change: {frameTimeChange:+0.0;-0.0}%");
                    sb.AppendLine($"  Description: {current.scenario.description}");
                    sb.AppendLine();
                }
            }
            
            return sb.ToString();
        }
        
        private void SaveComparisonReport(string report)
        {
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            Directory.CreateDirectory(directoryPath);
            
            string fileName = $"performance_test_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            
            File.WriteAllText(filePath, report);
            Debug.Log($"Performance test report saved to: {Path.GetFullPath(filePath)}");
        }
        
        private void LogComparisonSummary()
        {
            if (testResults.Count == 0) return;
            
            Debug.Log("=== Performance Test Results ===");
            foreach (var result in testResults)
            {
                float fps = 1000f / result.performanceReport.avgFrameTime;
                Debug.Log($"{result.scenario.name}: {result.performanceReport.avgFrameTime:F2}ms ({fps:F1} FPS)");
            }
        }
        
        public List<TestResult> GetTestResults()
        {
            return new List<TestResult>(testResults);
        }
        
        private IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }
        
        [Serializable]
        public class TestScenario
        {
            public string name;
            public string description;
            public float enemyMultiplier = 1f;
            public float interactableMultiplier = 1f;
            public bool enableAI = true;
            public bool enableInteractions = true;
        }
        
        [Serializable]
        public class TestResult
        {
            public TestScenario scenario;
            public PerformanceProfiler.PerformanceReport performanceReport;
            public DateTime timestamp;
        }
        
        // Simple test interactable for spawning during tests
        private class TestInteractable : MonoBehaviour
        {
            private void Awake()
            {
                // Add any necessary components for interaction testing
            }
        }
    }
}