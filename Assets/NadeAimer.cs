using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NadeAimer : MonoBehaviour
{
    private GameObject currentArrowHead;
    private DirectionHandler directionHandler;
    private UnitStats stats;
    private BeanAttackHandler attack;
    private Vector3 aimDirection;
    private Vector3 hitpoint;
    public float throwTime = 10;
    private static List<GameObject> Circles = new List<GameObject>();

    public GameObject aimCenter;
    public GameObject circleSprite;
    public GameObject arrowHeadPrefab;
    public GameObject objectToThrow;

    private Vector2 pointA; // starting point
    private Vector2 pointB; // end point
    private Vector3 velocity; // initial velocity vector
    private Vector3 currentAngle; // current angle
    private float dTime = 0;
    public NadeLaunchHandler nadeLaunchHandler;

    void Start()
    {
        nadeLaunchHandler = objectToThrow.GetComponent<NadeLaunchHandler>();
        pointA = aimCenter.transform.position;
       pointB = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        currentArrowHead = MAKER.Make(arrowHeadPrefab, pointB);
        currentArrowHead.SetActive(true);
        objectToThrow.transform.position = pointA;
        dTime = 0;
    }


    void FixedUpdate()
    {
        pointA = aimCenter.transform.position;
        pointB = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentArrowHead.transform.position = pointB;

     //CreateParabola(pointA, pointB, throwTime);

        if (Input.GetMouseButton(0))
        {
            if (!clicking)
            {
                clicking = true;

            }
        }
        else
        {
            clicking = false;
        }
    }


    private Vector3 force;
    private float baseHeight = 1f;
    private float maxAlt = 1f;


    private bool launched = false;
    private float closeEnoughDistance = 1f;
    private float radius = .75f;
    public int throwHeight = 3;
    private bool clicking;
    private float distanceFactor = .1f;


    private void ProjectileHitTarget()
    {
        Debug.Log("hit");
    }






}
