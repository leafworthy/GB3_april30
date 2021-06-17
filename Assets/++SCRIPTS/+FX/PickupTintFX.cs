using System;
using System.Security.Cryptography.X509Certificates;
using _SCRIPTS;
using UnityEngine;


public class PickupTintFX : MonoBehaviour
{
	private Material material;
	private Color materialTintColor;
	private DefenceHandler defence;
	public Renderer renderer;

	private float tintFadeSpeed = 6f;
	[SerializeField] private Color tintColor = Color.red;
	private float duration = .025f;
	private float shakeDamageMultiplier = .5f;
	[SerializeField] private SHAKER.ShakeIntensityType intensity;
	[SerializeField] private HITSTUN.StunLength stunLength;
	private Pickup pickup;
	private void Start()
	{
		pickup = GetComponent<Pickup>();
		pickup.OnPickup += StartTint;
	}

	public void StartTint()
	{
		materialTintColor = new Color();
		SetMaterial(renderer.material);
		SetTintColor(tintColor);
		SHAKER.ShakeCamera(transform.position, intensity);
		HITSTUN.StartStun(stunLength);
	}


	private void Update()
	{
		if (!(materialTintColor.a > 0)) return;
		materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
		material.SetColor("_Tint", materialTintColor);
	}

	private void SetMaterial(Material material)
	{
		this.material = material;
	}

	private void SetTintColor(Color color)
	{
		materialTintColor = color;
		material.SetColor("_Tint", materialTintColor);
	}


}
