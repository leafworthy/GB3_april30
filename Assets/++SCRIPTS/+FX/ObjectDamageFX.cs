using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace _SCRIPTS
{
	public class ObjectDamageFX : MonoBehaviour
	{
		private DefenceHandler defenceHandler;
		private HideRevealObjects damageStates;

		private int debreeMin = 2;
		private int debreeMax = 6;


		private int totalDamageStates;
		private int currentDamageState = 0;
		private bool broken;
		public bool DontDestroyColliderOnBroken;
		[SerializeField] private FXAssets.DebreeType debreeType;
		public event Action OnBreak;

		private void Awake()
		{
			defenceHandler = GetComponent<DefenceHandler>();
			defenceHandler.OnDamaged += Object_OnDamaged;

			damageStates = GetComponent<HideRevealObjects>();
			if (damageStates == null)
			{
				damageStates = GetComponentInChildren<HideRevealObjects>();
			}

			if (damageStates == null)
			{
				totalDamageStates = 0;
			}
			else
			{
				totalDamageStates = damageStates.objectsToReveal.Count;
			}
		}

		private void Object_OnDamaged(Vector3 damageSource, float damage, Vector3 position, bool isPoison)
		{
			if (!broken || DontDestroyColliderOnBroken)
			{
				int randomDebreeAmount = Random.Range(debreeMin, debreeMax);
				Vector3 bloodDir = (defenceHandler.GetPosition() - damageSource).normalized;

				SprayDebree(randomDebreeAmount, position, bloodDir);
				SprayDebree(randomDebreeAmount, position, -bloodDir);
				currentDamageState++;
				var bulletHits = ASSETS.FX.GetBulletHits(debreeType);
				if (bulletHits.Count > 0)
				{
					var newBulletHit = MAKER.Make(bulletHits.GetRandom(), position);
					MAKER.Unmake(newBulletHit, 2);
					var sortingGroup = GetComponent<SortingGroup>();
					var sortingLayerName = "Default";
					if (sortingGroup != null)
					{
						sortingLayerName = sortingGroup.sortingLayerName;
						var bulletHitSprite = newBulletHit.GetComponent<SpriteRenderer>();
						if (bulletHitSprite != null)
						{
							bulletHitSprite.sortingLayerName = sortingLayerName;
						}
					}
				}

				if (currentDamageState >= totalDamageStates)
				{
					if (!defenceHandler.IsInvincible())
					{
						OnBreak?.Invoke();
						Break(position, randomDebreeAmount, bloodDir);
					}
				}
				else
				{
					if (damageStates != null)
						damageStates.SetActiveObject(currentDamageState);
				}
			}

			//
		}

		private void Break(Vector3 position, int randomDebreeAmount, Vector3 bloodDir)
		{
			SprayDebree(randomDebreeAmount, position, bloodDir);
			SprayDebree(randomDebreeAmount, position, bloodDir);
			SprayDebree(randomDebreeAmount, position, bloodDir);

			if (!DontDestroyColliderOnBroken)
			{
				var boo = gameObject.GetComponent<Collider2D>();
				if (boo != null)
				{
					boo.enabled = false;
				}
			}


			broken = true;

			//MAKER.Unmake(gameObject);

		}

		private void SprayDebree(int quantity, Vector3 position, Vector3 sprayDirection)
		{
			var sortingGroup = GetComponent<SortingGroup>();
			var sortingLayerName = "Default";
			if (sortingGroup != null)
			{
				sortingLayerName = sortingGroup.sortingLayerName;
			}

			for (int j = 0; j < quantity; j++)
			{
				var newDebree = MAKER.Make(ASSETS.FX.wood_debree.GetRandom().gameObject, position);
				newDebree.GetComponent<FallToFloor>().Fire((sprayDirection.normalized), 2, defenceHandler.GetAimHeight(), sortingLayerName);
			}
		}
	}
}
