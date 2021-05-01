using System;
using System.Collections;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;

public class ColorSpritesPlayerColor : MonoBehaviour
{
 private ColorSprites sprites;
 private PlayerController player;

 private void Start()
 {
  sprites = GetComponent<ColorSprites>();
  player = GetComponent<PlayerController>();

  if (player != null)
  {
   SetColor();
  }
 }

 private void SetColor()
 {
  sprites.SetColor(player.GetPlayerColor());
 }
}
