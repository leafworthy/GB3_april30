using System.Collections.Generic;
using System.Linq;
using __SCRIPTS;
using Sirenix.OdinInspector;
using UnityEngine;

public class NPCShoeColorSwitcher : MonoBehaviour
{
	public SpriteRenderer leftLegSpriteRenderer;
	 public SpriteRenderer rightLegSpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [Button]
    public void SwitchShoeColor()
    {
var _materials = Resources.LoadAll<Material>("NPCMaterials").ToList();
	    var material = _materials.GetRandom();
		leftLegSpriteRenderer.sharedMaterial = material;
		rightLegSpriteRenderer.sharedMaterial = material;
	}
}
