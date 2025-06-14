using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace __SCRIPTS
{
    public class GetComponentAnalyzer : MonoBehaviour
    {
        [Header("Analysis Settings")]
        [SerializeField] private string scriptsPath = "Assets/++SCRIPTS";
        [SerializeField] private bool analyzeOnStart = false;
        [SerializeField] private bool generateReport = true;
        
        private List<GetComponentUsage> findings = new List<GetComponentUsage>();
        
        private void Start()
        {
            if (analyzeOnStart)
            {
                AnalyzeGetComponentUsage();
            }
        }
        
        [ContextMenu("Analyze GetComponent Usage")]
        public void AnalyzeGetComponentUsage()
        {
            findings.Clear();
            string fullScriptsPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), scriptsPath);
            
            if (!Directory.Exists(fullScriptsPath))
            {
                Debug.LogError($"Scripts directory not found: {fullScriptsPath}");
                return;
            }
            
            Debug.Log("Starting GetComponent analysis...");
            AnalyzeDirectory(fullScriptsPath);
            
            if (generateReport)
            {
                GenerateAnalysisReport();
            }
            
            Debug.Log($"Analysis complete. Found {findings.Count} GetComponent usage instances.");
        }
        
        private void AnalyzeDirectory(string directoryPath)
        {
            string[] csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
            
            foreach (string filePath in csFiles)
            {
                AnalyzeFile(filePath);
            }
        }
        
        private void AnalyzeFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                string fileName = Path.GetFileName(filePath);
                string relativePath = GetRelativePath(filePath);
                
                for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                {
                    string line = lines[lineNumber].Trim();
                    AnalyzeLine(line, fileName, relativePath, lineNumber + 1, lines);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error analyzing file {filePath}: {e.Message}");
            }
        }
        
        private void AnalyzeLine(string line, string fileName, string relativePath, int lineNumber, string[] allLines)
        {
            // Patterns to detect GetComponent calls
            var patterns = new[]
            {
                @"\.GetComponent<(\w+)>\(\)",
                @"\.GetComponent\(typeof\((\w+)\)\)",
                @"\.GetComponentInChildren<(\w+)>\(\)",
                @"\.GetComponentInParent<(\w+)>\(\)",
                @"\.GetComponents<(\w+)>\(\)",
                @"GetComponent<(\w+)>\(\)",
                @"GetComponentInChildren<(\w+)>\(\)",
                @"GetComponentInParent<(\w+)>\(\)",
                @"GetComponents<(\w+)>\(\)"
            };
            
            foreach (string pattern in patterns)
            {
                var matches = Regex.Matches(line, pattern);
                foreach (Match match in matches)
                {
                    var usage = new GetComponentUsage
                    {
                        fileName = fileName,
                        relativePath = relativePath,
                        lineNumber = lineNumber,
                        lineContent = line,
                        methodCall = match.Value,
                        componentType = match.Groups.Count > 1 ? match.Groups[1].Value : "Unknown",
                        context = DetermineContext(lineNumber - 1, allLines),
                        severity = CalculateSeverity(line, match.Value, allLines, lineNumber - 1)
                    };
                    
                    findings.Add(usage);
                }
            }
        }
        
        private string GetRelativePath(string fullPath)
        {
            string assetsPath = Application.dataPath.Replace("/Assets", "");
            return fullPath.Replace(assetsPath + "/", "").Replace("\\", "/");
        }
        
        private UsageContext DetermineContext(int lineIndex, string[] allLines)
        {
            // Look for method signatures around the line
            for (int i = Math.Max(0, lineIndex - 10); i <= Math.Min(allLines.Length - 1, lineIndex + 5); i++)
            {
                string contextLine = allLines[i].Trim();
                
                if (contextLine.Contains("void Update(") || contextLine.Contains("void Update ()"))
                    return UsageContext.Update;
                if (contextLine.Contains("void FixedUpdate(") || contextLine.Contains("void FixedUpdate ()"))
                    return UsageContext.FixedUpdate;
                if (contextLine.Contains("void LateUpdate(") || contextLine.Contains("void LateUpdate ()"))
                    return UsageContext.LateUpdate;
                if (contextLine.Contains("void Awake(") || contextLine.Contains("void Awake ()"))
                    return UsageContext.Awake;
                if (contextLine.Contains("void Start(") || contextLine.Contains("void Start ()"))
                    return UsageContext.Start;
                if (contextLine.Contains("void OnEnable(") || contextLine.Contains("void OnEnable ()"))
                    return UsageContext.OnEnable;
            }
            
            return UsageContext.Other;
        }
        
        private Severity CalculateSeverity(string line, string methodCall, string[] allLines, int lineIndex)
        {
            var context = DetermineContext(lineIndex, allLines);
            
            // Critical: GetComponent in Update/FixedUpdate loops
            if (context == UsageContext.Update || context == UsageContext.FixedUpdate)
                return Severity.Critical;
            
            // High: GetComponent in LateUpdate or within loops
            if (context == UsageContext.LateUpdate)
                return Severity.High;
            
            // Check if it's in a loop
            if (IsInLoop(lineIndex, allLines))
                return Severity.High;
            
            // Medium: GetComponentInChildren or GetComponents (more expensive)
            if (methodCall.Contains("InChildren") || methodCall.Contains("GetComponents"))
                return Severity.Medium;
            
            // Low: GetComponent in initialization methods
            if (context == UsageContext.Awake || context == UsageContext.Start || context == UsageContext.OnEnable)
                return Severity.Low;
            
            return Severity.Medium;
        }
        
        private bool IsInLoop(int lineIndex, string[] allLines)
        {
            // Look backwards for loop keywords
            for (int i = lineIndex; i >= Math.Max(0, lineIndex - 20); i--)
            {
                string line = allLines[i].Trim();
                if (line.Contains("for (") || line.Contains("foreach (") || line.Contains("while ("))
                    return true;
                if (line.Contains("}"))
                    break; // Exited a block, stop searching
            }
            return false;
        }
        
        private void GenerateAnalysisReport()
        {
            var report = GenerateReportContent();
            SaveReportToFile(report);
            LogSummaryToConsole();
        }
        
        private string GenerateReportContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== GETCOMPONENT USAGE ANALYSIS REPORT ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total findings: {findings.Count}");
            sb.AppendLine();
            
            // Summary by severity
            var criticalCount = 0;
            var highCount = 0;
            var mediumCount = 0;
            var lowCount = 0;
            
            foreach (var finding in findings)
            {
                switch (finding.severity)
                {
                    case Severity.Critical: criticalCount++; break;
                    case Severity.High: highCount++; break;
                    case Severity.Medium: mediumCount++; break;
                    case Severity.Low: lowCount++; break;
                }
            }
            
            sb.AppendLine("SEVERITY SUMMARY:");
            sb.AppendLine($"  Critical (Update/FixedUpdate): {criticalCount}");
            sb.AppendLine($"  High (Loops/LateUpdate): {highCount}");
            sb.AppendLine($"  Medium (Expensive calls): {mediumCount}");
            sb.AppendLine($"  Low (Initialization): {lowCount}");
            sb.AppendLine();
            
            // Group by severity for detailed listing
            var sortedFindings = new List<GetComponentUsage>(findings);
            sortedFindings.Sort((a, b) => b.severity.CompareTo(a.severity));
            
            sb.AppendLine("DETAILED FINDINGS:");
            sb.AppendLine("==================");
            
            foreach (var finding in sortedFindings)
            {
                sb.AppendLine($"[{finding.severity}] {finding.fileName}:{finding.lineNumber}");
                sb.AppendLine($"  Context: {finding.context}");
                sb.AppendLine($"  Component: {finding.componentType}");
                sb.AppendLine($"  Call: {finding.methodCall}");
                sb.AppendLine($"  Line: {finding.lineContent.Trim()}");
                sb.AppendLine($"  Path: {finding.relativePath}");
                sb.AppendLine();
            }
            
            // Optimization recommendations
            sb.AppendLine("OPTIMIZATION RECOMMENDATIONS:");
            sb.AppendLine("=============================");
            
            if (criticalCount > 0)
            {
                sb.AppendLine($"1. IMMEDIATE ACTION REQUIRED: {criticalCount} critical GetComponent calls in Update/FixedUpdate");
                sb.AppendLine("   - Cache these components in Awake() or Start()");
                sb.AppendLine("   - Expected performance impact: HIGH");
            }
            
            if (highCount > 0)
            {
                sb.AppendLine($"2. HIGH PRIORITY: {highCount} expensive GetComponent calls");
                sb.AppendLine("   - Consider caching or reducing call frequency");
                sb.AppendLine("   - Expected performance impact: MEDIUM");
            }
            
            if (mediumCount > 0)
            {
                sb.AppendLine($"3. MEDIUM PRIORITY: {mediumCount} potentially expensive calls");
                sb.AppendLine("   - Evaluate if caching would be beneficial");
                sb.AppendLine("   - Expected performance impact: LOW-MEDIUM");
            }
            
            return sb.ToString();
        }
        
        private void SaveReportToFile(string reportContent)
        {
            string directoryPath = Path.Combine(Application.dataPath, "../PerformanceReports");
            Directory.CreateDirectory(directoryPath);
            
            string fileName = $"getcomponent_analysis_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            
            File.WriteAllText(filePath, reportContent);
            Debug.Log($"GetComponent analysis report saved to: {Path.GetFullPath(filePath)}");
        }
        
        private void LogSummaryToConsole()
        {
            var critical = findings.FindAll(f => f.severity == Severity.Critical);
            var high = findings.FindAll(f => f.severity == Severity.High);
            
            Debug.Log($"GetComponent Analysis Summary:");
            Debug.Log($"  Critical issues: {critical.Count} (immediate optimization needed)");
            Debug.Log($"  High priority: {high.Count} (optimization recommended)");
            Debug.Log($"  Total findings: {findings.Count}");
            
            if (critical.Count > 0)
            {
                Debug.LogWarning("Critical GetComponent calls found in Update loops!");
                foreach (var item in critical)
                {
                    Debug.LogWarning($"  {item.fileName}:{item.lineNumber} - {item.componentType}");
                }
            }
        }
        
        public List<GetComponentUsage> GetFindings()
        {
            return new List<GetComponentUsage>(findings);
        }
        
        public List<GetComponentUsage> GetCriticalFindings()
        {
            return findings.FindAll(f => f.severity == Severity.Critical);
        }
        
        [Serializable]
        public class GetComponentUsage
        {
            public string fileName;
            public string relativePath;
            public int lineNumber;
            public string lineContent;
            public string methodCall;
            public string componentType;
            public UsageContext context;
            public Severity severity;
        }
        
        public enum UsageContext
        {
            Update,
            FixedUpdate,
            LateUpdate,
            Awake,
            Start,
            OnEnable,
            Other
        }
        
        public enum Severity
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Critical = 3
        }
    }
}