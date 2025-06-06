using System.Collections.Generic;
using __SCRIPTS;
using UnityEngine;
using VInspector.Libs;

[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
	public GameObject m_Prefab;

	private ParticleSystem m_ParticleSystem;
	private List<GameObject> m_Instances = new();
	private ParticleSystem.Particle[] m_Particles;
	public GameObject particleParent;
	public AnimationClip animationClip;

	// Start is called before the first frame update
	private void Start()
	{
		m_ParticleSystem = GetComponent<ParticleSystem>();
		var main = m_ParticleSystem.main;
		main.startLifetime = animationClip.length;
		m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
	}

	// Update is called once per frame
	private void LateUpdate()
	{
		var count = m_ParticleSystem.GetParticles(m_Particles);

		int attempts = 0;
		int maxAttempts = 100;
		
		while (m_Instances.Count < count && attempts < maxAttempts)
		{
			var prefab = ObjectMaker.I.Make(m_Prefab);
			if (prefab != null)
			{
				Debug.Log("new instance ");
				prefab.transform.SetParent(particleParent.transform);
				m_Instances.Add(prefab);
			}
			else
			{
				Debug.LogWarning("AttachGameObjectsToParticles: Failed to create prefab instance");
				break;
			}
			attempts++;
		}

		var instanceToRemove = new List<GameObject>();
		var worldSpace = m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World;
		for (var i = 0; i < m_Instances.Count; i++)
		{
			if (m_Instances[i].gameObject == null)
			{
				Debug.Log("instance added");
				instanceToRemove.Add(m_Instances[i]);
				continue;
			}
			if (i < count)
			{
				if (worldSpace)
					m_Instances[i].transform.position = m_Particles[i].position;
				else
					m_Instances[i].transform.localPosition = m_Particles[i].position;
				m_Instances[i].SetActive(true);
			}
			else
				m_Instances[i].SetActive(false);
		}

		foreach (var instance in instanceToRemove)
		{
			Debug.Log("removed instance");
			m_Instances.Remove(instance);
		}
	}
}