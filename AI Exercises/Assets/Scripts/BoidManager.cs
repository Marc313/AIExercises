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
    public int boidCount = 100;

    private void Awake()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Boid newBoid = Instantiate(boidPrefab).GetComponent<Boid>();
            newBoid.InjectManager(this);
            boidList.Add(newBoid);
        }

        boidObjList = boidList.Select(b => b.gameObject).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            foreach(Boid boid in boidList)
            {
                boid.Scatter();
            }
        }
    }

    private Vector3 CalculateAveragePos()
    {
        Vector3 totalPos = Vector3.zero;

        foreach (GameObject obj in boidObjList)
        {
            totalPos += obj.transform.position;
        }

        return totalPos / boidCount;
    }

    private Vector3 CalculateAverageDirection()
    {
        Vector3 totalDirection = Vector3.zero;

        
        foreach (Boid b in boidList)
        {
            totalDirection += b.direction;
        }

        return totalDirection / boidCount;
    }
}
