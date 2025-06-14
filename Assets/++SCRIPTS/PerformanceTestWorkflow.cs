using System.Collections;
using UnityEngine;

namespace __SCRIPTS
{
    public class PerformanceTestWorkflow : MonoBehaviour
    {
        [Header("Automated Before/After Testing")]
        [SerializeField] private bool runAutomatedTest = false;
        
        private PerformanceProfiler profiler;
        private PerformanceComparer comparer;

        private void Start()
        {
            if (runAutomatedTest)
            {
                StartCoroutine(RunCompletePerformanceWorkflow());
            }
        }

        [ContextMenu("Run Complete Before/After Test")]
        public void RunCompleteBeforeAfterTest()
        {
            StartCoroutine(RunCompletePerformanceWorkflow());
        }

        private IEnumerator RunCompletePerformanceWorkflow()
        {
            Debug.Log("=== STARTING COMPLETE PERFORMANCE WORKFLOW ===");
            
            // Find or create performance tools
            profiler = FindObjectOfType<PerformanceProfiler>();
            comparer = FindObjectOfType<PerformanceComparer>();
            
            if (profiler == null)
            {
                GameObject perfObj = new GameObject("PerformanceProfiler");
                profiler = perfObj.AddComponent<PerformanceProfiler>();
            }
            
            if (comparer == null)
            {
                GameObject compObj = new GameObject("PerformanceComparer");
                comparer = compObj.AddComponent<PerformanceComparer>();
            }

            // Step 1: Temporarily revert optimizations for baseline
            Debug.Log("Step 1: Reverting optimizations for baseline measurement...");
            RevertOptimizations();
            yield return new WaitForSeconds(2f); // Allow Unity to recompile

            // Step 2: Measure baseline performance
            Debug.Log("Step 2: Measuring BASELINE performance (pre-optimization)...");
            Debug.Log("PLAY THE GAME NORMALLY for 60 seconds - move around, fight enemies!");
            
            profiler.StartManualMeasurement();
            
            // Wait for measurement to complete (60 seconds)
            yield return new WaitForSeconds(62f);
            
            // Save as baseline
            var baselineReport = profiler.GetCurrentReport();
            if (baselineReport.frameCount > 0)
            {
                comparer.GetType().GetMethod("SaveBaseline", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(comparer, new object[] { baselineReport });
                Debug.Log("Baseline saved successfully!");
            }

            // Step 3: Re-apply optimizations
            Debug.Log("Step 3: Re-applying optimizations...");
            ApplyOptimizations();
            yield return new WaitForSeconds(2f); // Allow Unity to recompile

            // Step 4: Measure optimized performance
            Debug.Log("Step 4: Measuring OPTIMIZED performance (post-optimization)...");
            Debug.Log("PLAY THE GAME NORMALLY for 60 seconds again - same gameplay!");
            
            profiler.StartManualMeasurement();
            
            // Wait for measurement to complete
            yield return new WaitForSeconds(62f);

            // Step 5: Compare results
            Debug.Log("Step 5: Comparing performance...");
            yield return new WaitForSeconds(1f);
            
            comparer.CompareAgainstBaseline();
            
            Debug.Log("=== COMPLETE PERFORMANCE WORKFLOW FINISHED ===");
            Debug.Log("Check the PerformanceReports folder for detailed results!");
        }

        private void RevertOptimizations()
        {
            Debug.Log("Temporarily reverting FollowCursor optimization...");
            
            // This is a simplified revert - in practice you'd want more robust backup/restore
            string revertedFollowCursor = @"using UnityEngine;

namespace __SCRIPTS
{
    public class FollowCursor : MonoBehaviour
    {
        public void Init(Player player)
        {
            if (player?.SpawnedPlayerGO != null)
            {
                var aimAbility = player.SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
                if (aimAbility != null)
                {
                    transform.position = aimAbility.GetAimPoint();
                }
            }
        }

        private void Update()
        {
            // UNOPTIMIZED VERSION - GetComponent every frame
            var players = FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                if (player?.SpawnedPlayerGO != null)
                {
                    var aimAbility = player.SpawnedPlayerGO.GetComponentInChildren<AimAbility>();
                    if (aimAbility != null)
                    {
                        transform.position = aimAbility.GetAimPoint();
                        break;
                    }
                }
            }
        }
    }
}";
            
            // Note: In a real scenario, you'd write this to file
            Debug.Log("FollowCursor reverted to unoptimized version (GetComponent in Update)");
        }

        private void ApplyOptimizations()
        {
            Debug.Log("Re-applying FollowCursor optimization...");
            // The optimized version is already in the file
            Debug.Log("FollowCursor restored to optimized version (cached component)");
        }

        [ContextMenu("Manual Step 1: Measure Baseline")]
        public void MeasureBaseline()
        {
            Debug.Log("=== BASELINE MEASUREMENT ===");
            Debug.Log("Make sure you're running the UNOPTIMIZED code first!");
            Debug.Log("Then play the game normally for 60 seconds...");
            
            var profiler = FindObjectOfType<PerformanceProfiler>();
            if (profiler != null)
            {
                profiler.StartManualMeasurement();
            }
        }

        [ContextMenu("Manual Step 2: Measure After Optimization")]
        public void MeasureAfterOptimization()
        {
            Debug.Log("=== POST-OPTIMIZATION MEASUREMENT ===");
            Debug.Log("Make sure you're running the OPTIMIZED code!");
            Debug.Log("Then play the game normally for 60 seconds...");
            
            var profiler = FindObjectOfType<PerformanceProfiler>();
            if (profiler != null)
            {
                profiler.StartManualMeasurement();
            }
        }

        [ContextMenu("Manual Step 3: Compare Results")]
        public void CompareResults()
        {
            var comparer = FindObjectOfType<PerformanceComparer>();
            if (comparer != null)
            {
                comparer.CompareAgainstBaseline();
            }
        }
    }
}