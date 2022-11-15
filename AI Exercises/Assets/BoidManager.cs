using System;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;

    public List<GameObject> boidList { get; private set; } = new List<GameObject>();
    public Vector3 averagePos { get; private set; }

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            Boid newBoid = Instantiate(boidPrefab).GetComponent<Boid>();
            newBoid.InjectManager(this);
            boidList.Add(newBoid.gameObject);
        }
    }

    private void Update()
    {
        averagePos = CalculateAveragePos();
    }

    private Vector3 CalculateAveragePos()
    {
        Vector3 totalPos = Vector3.zero;

        foreach (GameObject obj in boidList)
        {
            totalPos += obj.transform.position;
        }

        return totalPos / boidList.Count;
    }
}
