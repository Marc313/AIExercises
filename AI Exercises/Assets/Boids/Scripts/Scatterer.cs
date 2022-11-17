using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterer : MonoBehaviour
{
    public float scatterRange;
    private BoidManager bm;
    private Vector3 worldPos;

    private void Awake()
    {
        bm = FindObjectOfType<BoidManager>();
    }

    private void Update()
    {
        OverlapBoid();
        FollowMouse();
    }

    private void FollowMouse()
    {
        Plane cameraPlane = new Plane(Vector3.back, bm.averagePos.z);

        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (cameraPlane.Raycast(ray, out float distance))
        {
            worldPos = ray.GetPoint(distance);
        }

        transform.position = worldPos;
    }

    private void OverlapBoid()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, scatterRange);

        foreach(Collider collider in colliders)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != null)
            {
                boid.Scatter();
            }
        }
    }
}
