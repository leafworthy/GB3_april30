using __SCRIPTS._INTERACTION;
using __SCRIPTS._PLAYER;
using __SCRIPTS._UNITS;
using UnityEngine;

namespace __SCRIPTS
{
  public class GarageDoorInteraction : PlayerInteractable
  {
    private bool isOpen;
    private Animator animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    public Life life;

    protected  void Start()
    {
      animator = GetComponentInChildren<Animator>();
      OnActionPress += ActionPress;
    }

    private void ActionPress(Player obj)
    {
      isOpen = !isOpen;
      animator.SetBool(IsOpen, isOpen);
      life.gameObject.SetActive(!isOpen);
    }
  }
}
