using System.Collections.Generic;
using GangstaBean.Core;
using Sirenix.OdinInspector;
using UnityEngine;


[ExecuteAlways]
public class PalletteSwapper : MonoBehaviour, IPoolable
{
	public List<Material> pallettes = new();
	public SpriteRenderer spriteRenderer;

	private void Start()
	{
		Swap();
	}

	public void OnPoolSpawn()
	{
		Swap();
	}

	[Button]
	public void Swap()
	{
		Debug.Log("swap");
		spriteRenderer.material = pallettes[Random.Range(0, pallettes.Count)];
	}

	public void OnPoolDespawn()
	{
	}
}
