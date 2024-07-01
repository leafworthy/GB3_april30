using __SCRIPTS._PLAYER;
using UnityEngine;

namespace __SCRIPTS
{
    public class PlayerIndicator : MonoBehaviour
    {
        public GameObject P1;
        public GameObject P2;
        public GameObject P3;
        public GameObject P4;

        private void Start()
        {
            Hide(P1);
            Hide(P2);
            Hide(P3);
            Hide(P4);
        }

        public void HideIndicator(Player player)
        {
            Debug.Log("HIDING");
            switch (player.playerIndex +1)
            {
                case 1:
                    Hide(P1);
                    break;
                case 2:
                    Hide(P2);
                    break;
                case 3:
                    Hide(P3);
                    break;
                case 4:
                    Hide(P4);
                    break;
                default:
                    break;
            }
        }

        private void Hide(GameObject go)
        {
            go.SetActive(false);
        }


        public void ShowIndicator(Player player)
        {
            gameObject.SetActive(true);
            Debug.Log("HERE Player" + (player.playerIndex + 1));
            switch (player.playerIndex+1)
            {
                case 1:
                    Debug.Log("HERE setactive");
                    P1.SetActive(true);
                    break;
                case 2:
                    P2.SetActive(true);
                    break;
                case 3:
                    P3.SetActive(true);
                    break;
                case 4:
                    P4.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void SetColor(Player player, Color newColor)
        {
            switch (player.playerIndex+1)
            {
                case 1:
                    SetColor(P1, newColor);
                    break;
                case 2:
                    P2.SetActive(true);
                    SetColor(P2, newColor);
                    break;
                case 3:
                    P3.SetActive(true);
                    SetColor(P3, newColor);
                    break;
                case 4:
                    P4.SetActive(true);
                    SetColor(P4, newColor);
                    break;
                default:
                    break;
            }
        }
        private void SetColor(GameObject go, Color newColor)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            sr.color = newColor;
        }
    }
}
