using __SCRIPTS._SCENES;
using FunkyCode;
using FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS
{
    public class LightManagerResetter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            LevelScene.OnStart += OnLevelStart;
        }

        private void OnLevelStart()
        {
            var manager = LightingManager2D.Get();
            UnityEngine.Debug.Log("Lighting Manager 2D: reinitialized");

            if (manager.version > 0 && manager.version < Lighting2D.VERSION)
            {
                UnityEngine.Debug.Log("Lighting Manager 2D: version update from " + manager.version_string + " to " +
                                      Lighting2D.VERSION_STRING);
            }

            foreach (Transform transform in manager.transform)
            {
                DestroyImmediate(transform.gameObject);
            }

            manager.version_string = Lighting2D.VERSION_STRING;
            manager.version = Lighting2D.VERSION;

            Light2D.ForceUpdateAll();

            LightingManager2D.ForceUpdate();
        }
    }
}
