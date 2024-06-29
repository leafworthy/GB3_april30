using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSayer : MonoBehaviour
{
	[SerializeField]private TextMeshPro text;
	[SerializeField]private Animator anim;
	private static readonly int SayTrigger = Animator.StringToHash("SayTrigger");
	private static readonly int StopTrigger = Animator.StringToHash("StopTrigger");
	private bool isSaying;
	private int counter;
	private float sayTime;
	private bool isHeld;
	

	public void Say(string message, float _sayTime = 3)
	{
		sayTime = _sayTime;
		isHeld = sayTime == 0;
		isSaying = true;
		text.text = message;
		anim.ResetTrigger(StopTrigger);
		anim.SetTrigger(SayTrigger);
	}

	private void FixedUpdate()
	{
		if (!isSaying) return;
		if (sayTime > 0)
		{
			sayTime -= Time.fixedDeltaTime;
		}
		else
		{
			if (isHeld) return;
			StopSaying();
		}
	}

	public void StopSaying()
	{
		if (!isSaying) return;
		isSaying = false;
		isHeld = false;
		sayTime = 0;
		anim.ResetTrigger(SayTrigger);
		anim.SetTrigger(StopTrigger);
	}
}