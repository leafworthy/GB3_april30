using UnityEngine;

public class ColorSpritesPlayerColor : MonoBehaviour
{
 private ColorSprites sprites;
 private IPlayerController playerRemote;

 private void Start()
 {
  sprites = GetComponent<ColorSprites>();
  playerRemote = GetComponent<IPlayerController>();

  if (playerRemote != null)
  {
   SetColor();
  }
 }

 private void SetColor()
 {
  sprites.SetColor(playerRemote.GetPlayerColor());
 }
}
