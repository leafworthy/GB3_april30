using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour
{
	public List<GameObject> prefab = new();
	public GameObject origin;
	public GameObject container;
	public float rate;
	public float currentTime = 0;
	public float lifeTime = 2;
	public float minScale = 2;
	public float maxScale = 8;
	public float yRange = 2;
	public float max;
	private Vector3 lastPos;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		lastPos = origin.transform.position;
	}

	// Update is called once per frame
	private void Update()
	{
		var originDelta = origin.transform.position - lastPos;
		var speed = originDelta.magnitude / Time.deltaTime;
		var rateSpeed = rate + speed;
		rateSpeed = Mathf.Min( rateSpeed, max );
		currentTime += Time.deltaTime * (rateSpeed);
		if (currentTime >= lifeTime)
		{
			currentTime = 0;
			var newObj = ObjectMaker.I.Make(prefab.GetRandom());
			newObj.transform.position = origin.transform.position + new Vector3(Random.Range(-yRange/3, yRange / 3), Random.Range(-yRange, yRange), 0);
			newObj.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
			lastPos = origin.transform.position;
		}
	}
}