using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Animations : MonoBehaviour
{
	public AnimationEvents animEvents;
	[SerializeField] public Animator animator;
	public static readonly int ShootSpeed = Animator.StringToHash("ShootSpeed");
	
	public static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
	public static readonly int FallTrigger = Animator.StringToHash("FallTrigger");
	public static readonly int LandTrigger = Animator.StringToHash("LandTrigger");
	
	public static readonly int AggroTrigger = Animator.StringToHash("AggroTrigger");
	public static readonly int HitTrigger = Animator.StringToHash("HitTrigger");
	public static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
	public static readonly int Attack1Trigger = Animator.StringToHash("Attack1Trigger");
	public static readonly int Attack2Trigger = Animator.StringToHash("Attack2Trigger");
	public static readonly int Attack3Trigger = Animator.StringToHash("Attack3Trigger");
	
	public static readonly int ChargeStartTrigger = Animator.StringToHash("ChargeStartTrigger");
	public static readonly int ChargeAttackTrigger = Animator.StringToHash("ChargeAttackTrigger");
	public static readonly int JumpAttackTrigger = Animator.StringToHash("JumpAttackTrigger");
	public static readonly int FlyingTrigger = Animator.StringToHash("FlyingTrigger");
	public static readonly int KnifeTrigger = Animator.StringToHash("KnifeTrigger");
	public static readonly int ThrowTrigger = Animator.StringToHash("ThrowTrigger");
	public static readonly int DashTrigger = Animator.StringToHash("DashTrigger");

	public static readonly int IsFallingFromSky = Animator.StringToHash("FallFromSky");
	public static readonly int IsBobbing = Animator.StringToHash("IsBobbing");
	public static readonly int IsFalling = Animator.StringToHash("IsFalling");
	public static readonly int IsGlocking = Animator.StringToHash("IsGlocking");
	public static readonly int IsDead = Animator.StringToHash("IsDead");
	public static readonly int IsShooting = Animator.StringToHash("IsShooting");
	public static readonly int IsCharging = Animator.StringToHash("IsCharging");
	public static readonly int IsMoving = Animator.StringToHash("IsMoving");

	public static readonly int GlockTrigger = Animator.StringToHash("GlockTrigger");

	private HashSet<int> parameterHashes;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		CacheParameterHashes();
	}

	private void CacheParameterHashes()
	{
		parameterHashes = new HashSet<int>();
		foreach (var param in animator.parameters)
		{
			parameterHashes.Add(param.nameHash);
		}
	}
	private void OnValidate()
	{
		if (animator == null) animator = GetComponentInChildren<Animator>();
		animEvents = GetComponentInChildren<AnimationEvents>();
	}

	public void Play(string animationClipName, int layer, float startingPlace)
	{
		if(animator == null) animator = GetComponentInChildren<Animator>();
		animator.Play(animationClipName, layer, startingPlace);
	}

	public void SetFloat(int trigger, float amount)
	{
		if (animator == null) animator = GetComponentInChildren<Animator>();
		if (HasParameter(trigger)) animator.SetFloat(trigger, amount);
	}

	public void ResetTrigger(int trigger)
	{
		if (animator == null) animator = GetComponentInChildren<Animator>();
		if (HasParameter(trigger)) animator.ResetTrigger(trigger);
	}

	public void SetTrigger(int trigger)
	{
		if (animator == null) animator = GetComponentInChildren<Animator>();
		if (HasParameter(trigger))
		{
			animator.SetTrigger(trigger);
		}
	}

	public void SetBool(int parameterHash, bool value)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator not found.");
            return;
        }

        if (HasParameter(parameterHash))
        {
            if (animator.GetBool(parameterHash) != value)
            {
                animator.SetBool(parameterHash, value);
            }
        }
        else
        {
            Debug.LogWarning($"Parameter with hash {parameterHash} does not exist.", this);
        }
    }

	private  bool HasParameter(int parameterHash)
	{
		return animator != null && parameterHashes.Contains(parameterHash);
	}
}