using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;



[ExecuteAlways, System.Serializable]
public class SpriteDepth : SerializedMonoBehaviour
{
	public int order = 0;
	[OdinSerialize] private SpriteRenderer spriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();
	[SerializeField] private SpriteRenderer _spriteRenderer;

	private void OnValidate()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetSortingOrder(int i)
	{
		if (spriteRenderer == null) return;
		spriteRenderer.sortingOrder = i;
		Debug.Log( "Set sorting order of " + gameObject.name + " to " + i, this);
	}
}
