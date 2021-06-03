using System.Collections;
using System.Collections.Generic;
using _SCRIPTS;
using UnityEngine;

public class Testing : MonoBehaviour {

    [SerializeField] private HitFX hitFX;
    private DefenceHandler defence;
    private bool clicking;

    private void Start()
    {
        defence = hitFX.gameObject.GetComponent<DefenceHandler>();
    }

    private void Update() {
        if (Input.GetMouseButton(0))
        {
            if (!clicking)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                clicking = true;
                defence.TakeDamage( transform.position - mousePos, 10, mousePos);
            }
        }
        else
        {
            clicking = false;
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            hitFX.SetTintColor(new Color(1, 0, 0, 1f));
            HITSTUN.StartStun(1);
        }
        if (Input.GetKeyDown(KeyCode.U)) {
            hitFX.SetTintColor(new Color(0, 0, 1, 1f));
            HITSTUN.StartStun(1);
        }
    }

}
