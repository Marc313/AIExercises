using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;

    public List<GameObject> boidObjList { get; private set; } = new List<GameObject>();
    public List<Boid> boidList { get; private set; } = new List<Boid>();

    public Vector3 averagePos => CalculateAveragePos();
    public Vector3 averageDirection => CalculateAverageDirection();
    public int boidCount => boidList.Count;

    private void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            Boid newBoid = Instantiate(boidPrefab).GetComponent<Boid>();
            newBoid.InjectManager(this);
            boidList.Add(newBoid);
        }

        boidObjList = boidList.Select(b => b.gameObject).ToList();
    }

    private Vector3 CalculateAveragePos()
    {
        Vector3 totalPos = Vector3.zero;

        foreach (GameObject obj in boidObjList)
        {
            totalPos += obj.transform.position;
        }

        return totalPos / boidObjList.Count;
    }

    private Vector3 CalculateAverageDirection()
    {
        Vector3 totalDirection = Vector3.zero;

        
        foreach (Boid b in boidList)
        {
            totalDirection += b.direction;
        }

        return totalDirection / boidObjList.Count;
    }
}
