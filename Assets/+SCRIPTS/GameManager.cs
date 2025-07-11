using UnityEngine;

namespace __SCRIPTS
{
    public class GameManager : Singleton<GameManager>
    {
        public static bool gameManagerLoaded;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void OnEnable()
        {
	        base.OnEnable();

            gameManagerLoaded = true;
        }


        private void OnDisable()
        {
            gameManagerLoaded = false;

        }

        private void Start()
        {
            // Initialize the game manager
            Debug.Log("GameManager started");
            DontDestroyOnLoad(gameObject);
        }


    }
}
