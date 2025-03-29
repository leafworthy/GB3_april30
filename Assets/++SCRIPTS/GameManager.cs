using UnityEngine;

namespace __SCRIPTS
{
    public class GameManager : Singleton<GameManager>
    {
        public bool gameManagerLoaded;
        public Camera gameCamera;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            gameManagerLoaded = true;
        }

        private void Start()
        {
            // Initialize the game manager
            Debug.Log("GameManager started");
        }
        
        public void DisableGameCamera()
        {
            if (gameCamera != null)
            {
                gameCamera.gameObject.SetActive(false);
                Debug.Log("Game camera disabled");
            }
            else
            {
                Debug.LogWarning("Game camera is not assigned");
            }
        }
    }
}
