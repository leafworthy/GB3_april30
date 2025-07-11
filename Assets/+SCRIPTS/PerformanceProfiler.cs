using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Unity.Profiling;

namespace __SCRIPTS
{
    public class PerformanceProfiler : MonoBehaviour
    {
        [Header("Performance Measurement Settings")]
        [SerializeField] private bool enableProfiling = true;
        [SerializeField] private float measurementDuration = 60f;
        [SerializeField] private int warmupFrames = 120;
        [SerializeField] private string reportDirectory = "PerformanceReports";
        
        [Header("Auto-Actions on Completion")]
        [SerializeField] private bool pauseOnComplete = true;
        [SerializeField] private bool quitOnComplete = false;
        
        [Header("Measurement Categories")]
        [SerializeField] private bool measureFrameTime = true;
        [SerializeField] private bool measureMemoryUsage = true;
        [SerializeField] private bool measureGCAllocations = true;
        [SerializeField] private bool measureRenderingStats = true;
        
        private List<float> frameTimeData = new List<float>();
        private List<float> memoryUsageData = new List<float>();
        private List<float> gcAllocationData = new List<float>();
        private List<int> drawCallData = new List<int>();
        
        private float measurementStartTime;
        private bool isMeasuring = false;
        private int frameCount = 0;
        private int warmupFrameCount = 0;
        
        // Performance counters
        private ProfilerRecorder frameTimeRecorder;
        private ProfilerRecorder gcAllocRecorder;
        private ProfilerRecorder drawCallsRecorder;
        private ProfilerRecorder totalReservedMemoryRecorder;
        
        private void Awake()
        {
            if (enableProfiling)
            {
                DontDestroyOnLoad(gameObject);
                InitializeProfilerRecorders();
            }
        }
        
        private void InitializeProfilerRecorders()
        {
            frameTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            gcAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 15);
            drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 15);
            totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory", 15);
        }
        
        private void Update()
        {
            if (!enableProfiling) return;
            
            frameCount++;
            
            // Warmup period
            if (warmupFrameCount < warmupFrames)
            {
                warmupFrameCount++;
                return;
            }
            
            // Start measuring after warmup
            if (!isMeasuring)
            {
                StartMeasurement();
            }
            
            // Collect data during measurement period
            if (isMeasuring)
            {
                CollectFrameData();
                
                // Check if measurement period is complete
                if (Time.time - measurementStartTime >= measurementDuration)
                {
                    CompleteMeasurement();
                }
            }
        }
        
        private void StartMeasurement()
        {
            isMeasuring = true;
            measurementStartTime = Time.time;
            
            frameTimeData.Clear();
            memoryUsageData.Clear();
            gcAllocationData.Clear();
            drawCallData.Clear();
            
            Debug.Log($"PerformanceProfiler: Starting measurement for {measurementDuration} seconds");
        }
        
        private void CollectFrameData()
        {
            if (measureFrameTime && frameTimeRecorder.Valid)
            {
                float frameTime = frameTimeRecorder.LastValue / 1_000_000f; // Convert to milliseconds
                frameTimeData.Add(frameTime);
            }
            
            if (measureMemoryUsage && totalReservedMemoryRecorder.Valid)
            {
                float memoryMB = totalReservedMemoryRecorder.LastValue / (1024f * 1024f);
                memoryUsageData.Add(memoryMB);
            }
            
            if (measureGCAllocations && gcAllocRecorder.Valid)
            {
                float gcAllocKB = gcAllocRecorder.LastValue / 1024f;
                gcAllocationData.Add(gcAllocKB);
            }
            
            if (measureRenderingStats && drawCallsRecorder.Valid)
            {
                drawCallData.Add((int)drawCallsRecorder.LastValue);
            }
        }
        
        private void CompleteMeasurement()
        {
            isMeasuring = false;
            GeneratePerformanceReport();
            
            // Automatically save this measurement as baseline for easy comparison
            var comparer = FindObjectOfType<PerformanceComparer>();
            if (comparer != null)
            {
                var currentReport = GetCurrentReport();
                if (currentReport.frameCount > 0)
                {
                    // Use reflection to call SaveBaseline directly
                    var saveMethod = comparer.GetType().GetMethod("SaveBaseline", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    saveMethod?.Invoke(comparer, new object[] { currentReport });
                    Debug.Log("PerformanceProfiler: Measurement automatically saved as baseline for comparison.");
                }
            }
            
            Debug.Log("PerformanceProfiler: Measurement complete. Report generated.");
            Debug.Log("Use Tools → Performance Analysis → Compare Against Baseline to see performance improvements.");
            
            // Auto-actions on completion
            if (pauseOnComplete)
            {
                Debug.Log("PerformanceProfiler: Pausing game. Press Play to continue or check your results.");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
                #else
                Time.timeScale = 0f;
                #endif
            }
            
            if (quitOnComplete)
            {
                Debug.Log("PerformanceProfiler: Quitting application.");
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
        
        public void StartManualMeasurement()
        {
            if (!enableProfiling) return;
            
            warmupFrameCount = warmupFrames; // Skip warmup for manual measurements
            isMeasuring = false; // Will start on next Update
        }
        
        public PerformanceReport GetCurrentReport()
        {
            return new PerformanceReport
            {
                timestamp = DateTime.Now,
                measurementDuration = measurementDuration,
                frameCount = frameTimeData.Count,
                avgFrameTime = CalculateAverage(frameTimeData),
                minFrameTime = CalculateMin(frameTimeData),
                maxFrameTime = CalculateMax(frameTimeData),
                frameTimeStdDev = CalculateStandardDeviation(frameTimeData),
                avgMemoryUsage = CalculateAverage(memoryUsageData),
                avgGCAllocation = CalculateAverage(gcAllocationData),
                avgDrawCalls = CalculateAverage(drawCallData)
            };
        }
        
        private void GeneratePerformanceReport()
        {
            var report = GetCurrentReport();
            string reportContent = FormatReport(report);
            
            SaveReportToFile(reportContent);
        }
        
        private string FormatReport(PerformanceReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== PERFORMANCE MEASUREMENT REPORT ===");
            sb.AppendLine($"Timestamp: {report.timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Measurement Duration: {report.measurementDuration:F1}s");
            sb.AppendLine($"Frame Count: {report.frameCount}");
            sb.AppendLine();
            
            sb.AppendLine("FRAME TIME ANALYSIS:");
            sb.AppendLine($"  Average: {report.avgFrameTime:F2}ms ({1000f/report.avgFrameTime:F1} FPS)");
            sb.AppendLine($"  Minimum: {report.minFrameTime:F2}ms");
            sb.AppendLine($"  Maximum: {report.maxFrameTime:F2}ms");
            sb.AppendLine($"  Std Dev: {report.frameTimeStdDev:F2}ms");
            sb.AppendLine();
            
            sb.AppendLine("MEMORY ANALYSIS:");
            sb.AppendLine($"  Average Memory Usage: {report.avgMemoryUsage:F1} MB");
            sb.AppendLine($"  Average GC Allocation: {report.avgGCAllocation:F2} KB/frame");
            sb.AppendLine();
            
            sb.AppendLine("RENDERING ANALYSIS:");
            sb.AppendLine($"  Average Draw Calls: {report.avgDrawCalls:F0}");
            sb.AppendLine();
            
            sb.AppendLine("PERFORMANCE CLASSIFICATION:");
            sb.AppendLine($"  Frame Time: {ClassifyFrameTime(report.avgFrameTime)}");
            sb.AppendLine($"  Memory Usage: {ClassifyMemoryUsage(report.avgGCAllocation)}");
            sb.AppendLine($"  Consistency: {ClassifyConsistency(report.frameTimeStdDev)}");
            
            return sb.ToString();
        }
        
        private void SaveReportToFile(string reportContent)
        {
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            Directory.CreateDirectory(directoryPath);
            
            string fileName = $"performance_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            
            File.WriteAllText(filePath, reportContent);
            Debug.Log($"Performance report saved to: {Path.GetFullPath(filePath)}");
        }
        
        private float CalculateAverage(List<float> data)
        {
            if (data.Count == 0) return 0f;
            float sum = 0f;
            foreach (float value in data) sum += value;
            return sum / data.Count;
        }
        
        private float CalculateAverage(List<int> data)
        {
            if (data.Count == 0) return 0f;
            float sum = 0f;
            foreach (int value in data) sum += value;
            return sum / data.Count;
        }
        
        private float CalculateMin(List<float> data)
        {
            if (data.Count == 0) return 0f;
            float min = float.MaxValue;
            foreach (float value in data)
                if (value < min) min = value;
            return min;
        }
        
        private float CalculateMax(List<float> data)
        {
            if (data.Count == 0) return 0f;
            float max = float.MinValue;
            foreach (float value in data)
                if (value > max) max = value;
            return max;
        }
        
        private float CalculateStandardDeviation(List<float> data)
        {
            if (data.Count <= 1) return 0f;
            
            float average = CalculateAverage(data);
            float sumSquaredDifferences = 0f;
            
            foreach (float value in data)
            {
                float difference = value - average;
                sumSquaredDifferences += difference * difference;
            }
            
            return Mathf.Sqrt(sumSquaredDifferences / (data.Count - 1));
        }
        
        private string ClassifyFrameTime(float avgFrameTime)
        {
            if (avgFrameTime < 16.67f) return "EXCELLENT (60+ FPS)";
            if (avgFrameTime < 33.33f) return "GOOD (30-60 FPS)";
            if (avgFrameTime < 50f) return "ACCEPTABLE (20-30 FPS)";
            return "POOR (<20 FPS)";
        }
        
        private string ClassifyMemoryUsage(float avgGCAllocation)
        {
            if (avgGCAllocation < 1f) return "EXCELLENT (<1 KB/frame)";
            if (avgGCAllocation < 5f) return "GOOD (1-5 KB/frame)";
            if (avgGCAllocation < 20f) return "ACCEPTABLE (5-20 KB/frame)";
            return "POOR (>20 KB/frame)";
        }
        
        private string ClassifyConsistency(float stdDev)
        {
            if (stdDev < 2f) return "VERY CONSISTENT";
            if (stdDev < 5f) return "CONSISTENT";
            if (stdDev < 10f) return "SOMEWHAT INCONSISTENT";
            return "INCONSISTENT";
        }
        
        private void OnDestroy()
        {
            frameTimeRecorder.Dispose();
            gcAllocRecorder.Dispose();
            drawCallsRecorder.Dispose();
            totalReservedMemoryRecorder.Dispose();
        }
        
        [Serializable]
        public class PerformanceReport
        {
            public DateTime timestamp;
            public float measurementDuration;
            public int frameCount;
            public float avgFrameTime;
            public float minFrameTime;
            public float maxFrameTime;
            public float frameTimeStdDev;
            public float avgMemoryUsage;
            public float avgGCAllocation;
            public float avgDrawCalls;
        }
    }
}