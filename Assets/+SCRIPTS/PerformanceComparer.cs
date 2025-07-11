using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace __SCRIPTS
{
    public class PerformanceComparer : MonoBehaviour
    {
        [Header("Comparison Settings")]
        [SerializeField] private bool autoSaveBaseline = true;
        [SerializeField] private string baselineFileName = "performance_baseline.json";
        [SerializeField] private float significantChangeThreshold = 5f; // Percentage
        
        private PerformanceProfiler.PerformanceReport savedBaseline;
        private List<ComparisonResult> comparisonHistory = new List<ComparisonResult>();
        
        private void Start()
        {
            LoadBaseline();
        }
        
        [ContextMenu("Save Current Performance as Baseline")]
        public void SaveCurrentAsBaseline()
        {
            var profiler = FindObjectOfType<PerformanceProfiler>();
            if (profiler == null)
            {
                Debug.LogError("PerformanceProfiler not found! Cannot save baseline.");
                return;
            }
            
            var currentReport = profiler.GetCurrentReport();
            
            // Check if the current report has valid data
            if (currentReport.frameCount == 0)
            {
                Debug.LogWarning("No performance data available yet. Run a measurement first, then this will save that data as baseline.");
                Debug.LogWarning("Use Tools → Performance Analysis → Start Simple Performance Measurement to collect data first.");
                return;
            }
            
            SaveBaseline(currentReport);
            Debug.Log("Current performance data saved as baseline for future comparisons.");
        }
        
        [ContextMenu("Compare Against Baseline")]
        public void CompareAgainstBaseline()
        {
            // Check if baseline exists (either in memory or try to load from file)
            if (savedBaseline.timestamp == default)
            {
                LoadBaseline();
            }
            
            if (savedBaseline.timestamp == default)
            {
                Debug.LogError("No baseline found! Use 'Save Current Performance as Baseline' first, or run a performance measurement.");
                Debug.LogError("Go to Tools → Performance Analysis → Start Simple Performance Measurement first.");
                return;
            }
            
            var profiler = FindObjectOfType<PerformanceProfiler>();
            if (profiler == null)
            {
                Debug.LogError("PerformanceProfiler not found! Create one first using Tools → Performance Analysis → Create Performance Tools GameObject");
                return;
            }
            
            var currentReport = profiler.GetCurrentReport();
            
            // Check if the current report has valid data
            if (currentReport.frameCount == 0)
            {
                Debug.LogError("No current performance data available! Run a performance measurement first.");
                Debug.LogError("Use Tools → Performance Analysis → Start Simple Performance Measurement to collect data.");
                return;
            }
            
            var comparison = CompareReports(savedBaseline, currentReport);
            
            comparisonHistory.Add(comparison);
            GenerateComparisonReport(comparison);
            LogComparisonSummary(comparison);
        }
        
        public ComparisonResult CompareReports(PerformanceProfiler.PerformanceReport baseline, PerformanceProfiler.PerformanceReport current)
        {
            var result = new ComparisonResult
            {
                baseline = baseline,
                current = current,
                timestamp = DateTime.Now
            };
            
            // Calculate percentage changes
            result.frameTimeChange = CalculatePercentageChange(baseline.avgFrameTime, current.avgFrameTime);
            result.memoryUsageChange = CalculatePercentageChange(baseline.avgMemoryUsage, current.avgMemoryUsage);
            result.gcAllocationChange = CalculatePercentageChange(baseline.avgGCAllocation, current.avgGCAllocation);
            result.drawCallChange = CalculatePercentageChange(baseline.avgDrawCalls, current.avgDrawCalls);
            result.consistencyChange = CalculatePercentageChange(baseline.frameTimeStdDev, current.frameTimeStdDev);
            
            // Determine overall assessment
            result.overallAssessment = DetermineOverallAssessment(result);
            
            return result;
        }
        
        private float CalculatePercentageChange(float baseline, float current)
        {
            if (baseline == 0) return current == 0 ? 0 : 100;
            return ((current - baseline) / baseline) * 100f;
        }
        
        private PerformanceAssessment DetermineOverallAssessment(ComparisonResult result)
        {
            int improvements = 0;
            int regressions = 0;
            
            // Frame time improvement is positive (lower is better)
            if (result.frameTimeChange < -significantChangeThreshold) improvements++;
            else if (result.frameTimeChange > significantChangeThreshold) regressions++;
            
            // Memory usage improvement is positive (lower is better)
            if (result.memoryUsageChange < -significantChangeThreshold) improvements++;
            else if (result.memoryUsageChange > significantChangeThreshold) regressions++;
            
            // GC allocation improvement is positive (lower is better)
            if (result.gcAllocationChange < -significantChangeThreshold) improvements++;
            else if (result.gcAllocationChange > significantChangeThreshold) regressions++;
            
            // Consistency improvement is positive (lower std dev is better)
            if (result.consistencyChange < -significantChangeThreshold) improvements++;
            else if (result.consistencyChange > significantChangeThreshold) regressions++;
            
            if (improvements > regressions) return PerformanceAssessment.Improved;
            if (regressions > improvements) return PerformanceAssessment.Degraded;
            if (improvements == 0 && regressions == 0) return PerformanceAssessment.NoChange;
            return PerformanceAssessment.Mixed;
        }
        
        private void SaveBaseline(PerformanceProfiler.PerformanceReport report)
        {
            savedBaseline = report;
            
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            Directory.CreateDirectory(directoryPath);
            
            string filePath = Path.Combine(directoryPath, baselineFileName);
            string jsonData = JsonUtility.ToJson(report, true);
            
            File.WriteAllText(filePath, jsonData);
            Debug.Log($"Performance baseline saved to: {Path.GetFullPath(filePath)}");
        }
        
        private void LoadBaseline()
        {
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            string filePath = Path.Combine(directoryPath, baselineFileName);
            
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(filePath);
                    savedBaseline = JsonUtility.FromJson<PerformanceProfiler.PerformanceReport>(jsonData);
                    Debug.Log($"Performance baseline loaded from: {Path.GetFullPath(filePath)}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load baseline: {e.Message}");
                }
            }
            else
            {
                Debug.Log("No performance baseline found. Save a baseline first.");
            }
        }
        
        private void GenerateComparisonReport(ComparisonResult comparison)
        {
            string report = FormatComparisonReport(comparison);
            SaveComparisonReportToFile(report);
        }
        
        private string FormatComparisonReport(ComparisonResult comparison)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== PERFORMANCE COMPARISON REPORT ===");
            sb.AppendLine($"Generated: {comparison.timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Overall Assessment: {comparison.overallAssessment}");
            sb.AppendLine();
            
            sb.AppendLine("BASELINE PERFORMANCE:");
            sb.AppendLine($"  Date: {comparison.baseline.timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"  Frame Time: {comparison.baseline.avgFrameTime:F2}ms ({1000f/comparison.baseline.avgFrameTime:F1} FPS)");
            sb.AppendLine($"  Memory Usage: {comparison.baseline.avgMemoryUsage:F1} MB");
            sb.AppendLine($"  GC Allocation: {comparison.baseline.avgGCAllocation:F2} KB/frame");
            sb.AppendLine($"  Draw Calls: {comparison.baseline.avgDrawCalls:F0}");
            sb.AppendLine($"  Consistency: {comparison.baseline.frameTimeStdDev:F2}ms std dev");
            sb.AppendLine();
            
            sb.AppendLine("CURRENT PERFORMANCE:");
            sb.AppendLine($"  Date: {comparison.current.timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"  Frame Time: {comparison.current.avgFrameTime:F2}ms ({1000f/comparison.current.avgFrameTime:F1} FPS)");
            sb.AppendLine($"  Memory Usage: {comparison.current.avgMemoryUsage:F1} MB");
            sb.AppendLine($"  GC Allocation: {comparison.current.avgGCAllocation:F2} KB/frame");
            sb.AppendLine($"  Draw Calls: {comparison.current.avgDrawCalls:F0}");
            sb.AppendLine($"  Consistency: {comparison.current.frameTimeStdDev:F2}ms std dev");
            sb.AppendLine();
            
            sb.AppendLine("PERFORMANCE CHANGES:");
            sb.AppendLine($"  Frame Time: {FormatChange(comparison.frameTimeChange, true)}");
            sb.AppendLine($"  Memory Usage: {FormatChange(comparison.memoryUsageChange, true)}");
            sb.AppendLine($"  GC Allocation: {FormatChange(comparison.gcAllocationChange, true)}");
            sb.AppendLine($"  Draw Calls: {FormatChange(comparison.drawCallChange, true)}");
            sb.AppendLine($"  Consistency: {FormatChange(comparison.consistencyChange, true)}");
            sb.AppendLine();
            
            sb.AppendLine("ANALYSIS:");
            sb.AppendLine(GenerateAnalysis(comparison));
            
            sb.AppendLine();
            sb.AppendLine("RECOMMENDATIONS:");
            sb.AppendLine(GenerateRecommendations(comparison));
            
            return sb.ToString();
        }
        
        private string FormatChange(float percentageChange, bool lowerIsBetter)
        {
            string direction = "";
            string significance = "";
            
            if (Math.Abs(percentageChange) >= significantChangeThreshold)
            {
                if (lowerIsBetter)
                {
                    direction = percentageChange < 0 ? " (IMPROVED)" : " (DEGRADED)";
                }
                else
                {
                    direction = percentageChange > 0 ? " (IMPROVED)" : " (DEGRADED)";
                }
                significance = " ***";
            }
            else
            {
                direction = " (minor change)";
            }
            
            return $"{percentageChange:+0.0;-0.0}%{direction}{significance}";
        }
        
        private string GenerateAnalysis(ComparisonResult comparison)
        {
            var sb = new StringBuilder();
            
            switch (comparison.overallAssessment)
            {
                case PerformanceAssessment.Improved:
                    sb.AppendLine("Overall performance has IMPROVED compared to baseline.");
                    break;
                case PerformanceAssessment.Degraded:
                    sb.AppendLine("Overall performance has DEGRADED compared to baseline.");
                    break;
                case PerformanceAssessment.Mixed:
                    sb.AppendLine("Performance shows mixed results - some improvements, some degradations.");
                    break;
                case PerformanceAssessment.NoChange:
                    sb.AppendLine("Performance is essentially unchanged from baseline.");
                    break;
            }
            
            // Specific metric analysis
            if (Math.Abs(comparison.frameTimeChange) >= significantChangeThreshold)
            {
                if (comparison.frameTimeChange < 0)
                    sb.AppendLine($"Frame time improved by {Math.Abs(comparison.frameTimeChange):F1}% - optimization successful!");
                else
                    sb.AppendLine($"Frame time degraded by {comparison.frameTimeChange:F1}% - investigate performance regression.");
            }
            
            if (Math.Abs(comparison.gcAllocationChange) >= significantChangeThreshold)
            {
                if (comparison.gcAllocationChange < 0)
                    sb.AppendLine($"Memory allocations reduced by {Math.Abs(comparison.gcAllocationChange):F1}% - good memory optimization.");
                else
                    sb.AppendLine($"Memory allocations increased by {comparison.gcAllocationChange:F1}% - memory usage regression detected.");
            }
            
            return sb.ToString();
        }
        
        private string GenerateRecommendations(ComparisonResult comparison)
        {
            var sb = new StringBuilder();
            
            if (comparison.overallAssessment == PerformanceAssessment.Degraded)
            {
                sb.AppendLine("- Consider reverting recent changes that may have caused performance regression");
                sb.AppendLine("- Profile the application to identify new performance bottlenecks");
            }
            else if (comparison.overallAssessment == PerformanceAssessment.Improved)
            {
                sb.AppendLine("- Great work! Consider applying similar optimizations to other systems");
                sb.AppendLine("- Update baseline to reflect the improved performance");
            }
            
            if (comparison.frameTimeChange > significantChangeThreshold)
            {
                sb.AppendLine("- Frame time regression detected - check Update/FixedUpdate loops for new expensive operations");
            }
            
            if (comparison.gcAllocationChange > significantChangeThreshold)
            {
                sb.AppendLine("- Memory allocation increase detected - review recent code for new allocations in Update loops");
            }
            
            if (comparison.consistencyChange > significantChangeThreshold)
            {
                sb.AppendLine("- Frame time consistency degraded - look for intermittent expensive operations or GC spikes");
            }
            
            if (sb.Length == 0)
            {
                sb.AppendLine("- Continue monitoring performance with regular measurements");
                sb.AppendLine("- Consider further optimizations if performance targets are not yet met");
            }
            
            return sb.ToString();
        }
        
        private void SaveComparisonReportToFile(string report)
        {
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            Directory.CreateDirectory(directoryPath);
            
            string fileName = $"performance_comparison_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            
            File.WriteAllText(filePath, report);
            Debug.Log($"Performance comparison report saved to: {Path.GetFullPath(filePath)}");
        }
        
        private void LogComparisonSummary(ComparisonResult comparison)
        {
            Debug.Log($"Performance Comparison Summary:");
            Debug.Log($"  Overall: {comparison.overallAssessment}");
            Debug.Log($"  Frame Time: {comparison.frameTimeChange:+0.0;-0.0}%");
            Debug.Log($"  Memory: {comparison.memoryUsageChange:+0.0;-0.0}%");
            Debug.Log($"  GC Alloc: {comparison.gcAllocationChange:+0.0;-0.0}%");
            
            if (comparison.overallAssessment == PerformanceAssessment.Improved)
            {
                Debug.Log("<color=green>Performance improved!</color>");
            }
            else if (comparison.overallAssessment == PerformanceAssessment.Degraded)
            {
                Debug.LogWarning("Performance degraded - investigate regression!");
            }
        }
        
        public List<ComparisonResult> GetComparisonHistory()
        {
            return new List<ComparisonResult>(comparisonHistory);
        }
        
        [Serializable]
        public class ComparisonResult
        {
            public PerformanceProfiler.PerformanceReport baseline;
            public PerformanceProfiler.PerformanceReport current;
            public DateTime timestamp;
            
            public float frameTimeChange;
            public float memoryUsageChange;
            public float gcAllocationChange;
            public float drawCallChange;
            public float consistencyChange;
            
            public PerformanceAssessment overallAssessment;
        }
        
        public enum PerformanceAssessment
        {
            Improved,
            Degraded,
            Mixed,
            NoChange
        }
    }
}