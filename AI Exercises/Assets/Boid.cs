using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10;

    public float overlapRadius = 2f;

    private HashSet<Boid> boids = new HashSet<Boid>();
    private BoidManager manager;

    private float cohesionScore;
    private float seperationScore;
    private float alignmentScore;
    private Vector2 startPos;

    private void Start()
    {
        float randomXDir = Random.Range(-1f, 1f);
        float randomYDir = Random.Range(-1f, 1f);
        direction = new Vector2(randomXDir, randomYDir).normalized;

        float randomXPos = Random.Range(-20f, 20f);
        float randomYPos = Random.Range(-10f, 10f);
        startPos = new Vector2(randomXPos, randomYPos);
        transform.position = startPos;
    }

    public void InjectManager(BoidManager _boidManager)
    {
        manager = _boidManager;
    }

    private void Update()
    {
        Vector2 movement = direction * speed * Time.deltaTime;
        transform.position += new Vector3(movement.x, movement.y, 0f);

        ApplyCohesion();

        FindNearbyBoids();
    }

    private void ApplyCohesion()
    {
        float distanceToAverage = Vector3.Distance(manager.averagePos, transform.position);
        Vector3 directionToAverage = manager.averagePos - transform.position;
        direction = directionToAverage;
    }

    private void FindNearbyBoids()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, overlapRadius);

        foreach (Collider collider in colliders)
        {
            Boid boid = collider.GetComponent<Boid>();
            boids.Add(boid);
        }
    }
}
