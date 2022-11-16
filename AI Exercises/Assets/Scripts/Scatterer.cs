using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scatterer : MonoBehaviour
{
    public float scatterRange;

    private void Update()
    {
        OverlapBoid();
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
