using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 direction { get; private set; }
    public float speed = 10;
    public float rotationSpeed = 100;
    public float neighbourRadius = 5f;
    public float seperationRadius = 2f;
    public float scatterDuration = 3f;
    [SerializeField] private Vector3 seperationScalar = Vector3.one;  // Scales the velocity vector of the Seperation rule, modify to 'scale' the shape of the flock

    [Header("Scores")]
    [SerializeField] private float neighbourCohesionScore;
    [SerializeField] private float generalCohesionScore;
    [SerializeField] private float seperationScore;
    [SerializeField] private float alignmentScore;


    private HashSet<Boid> neighbourBoids = new HashSet<Boid>();
    private HashSet<Boid> seperationBoids = new HashSet<Boid>();
    private BoidManager manager;
    private Vector3 startPos;
    private bool isScattering;

    private void Start()
    {
        GenerateStartpos();
    }

    private void GenerateStartpos()
    {
        float randomXDir = Random.Range(-1f, 1f);
        float randomYDir = Random.Range(-1f, 1f);
        float randomZDir = Random.Range(-1f, 1f);
        direction = new Vector3(randomXDir, randomYDir, randomZDir).normalized;

        float randomXPos = Random.Range(-20f, 20f);
        float randomYPos = Random.Range(-10f, 10f);
        float randomZPos = Random.Range(-20f, 20f);
        startPos = new Vector3(randomXPos, randomYPos, randomZPos);
        transform.position = startPos;
    }

    public void InjectManager(BoidManager _boidManager)
    {
        manager = _boidManager;
    }

    private void Update()
    {
        FindNearbyBoids();
        CalculateDirection();
        MoveToDirection();
    }

    public void Scatter()
    {
        if (isScattering) return;

        InverseCohesionScores();
        Invoke(nameof(InverseCohesionScores), scatterDuration);
    }

    private void InverseCohesionScores()
    {
        generalCohesionScore *= -1;
        neighbourCohesionScore *= -1;
        isScattering = !isScattering;
    }

    private void CalculateDirection()
    {
        Vector3 vNCohesion = ApplyNeighbouringCohesion();
        Vector3 vGCohesion = ApplyGeneralCohesion();
        Vector3 vSegregation = ApplySegregation();
        Vector3 vAllignment = ApplyAllignment();

        Vector3 totalDirection = (vNCohesion * neighbourCohesionScore
                                    + vGCohesion * generalCohesionScore
                                    + vSegregation * seperationScore
                                    + vAllignment * alignmentScore
                                    ).normalized;
        if (totalDirection.magnitude > .1f)
        {
            direction = totalDirection;
        }
    }

    private void MoveToDirection()
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

    // Rule 1: Cohesion
    // Neighbouring cohesion causes all boids to stick with all boids that are close to it
    private Vector3 ApplyNeighbouringCohesion()
    {
        Vector3 averagePosAll = CalculateAveragePosOfSet(neighbourBoids);

        Vector3 directionToAverage = (averagePosAll - transform.position);
        return directionToAverage;
    }

    // General cohesion ensures that all the boids don't grow apart too much
    private Vector3 ApplyGeneralCohesion()
    {
        Vector3 averagePosAll = manager.averagePos;
        Vector3 averagePosExcl = ((averagePosAll / manager.boidCount) - transform.position) * (manager.boidCount - 1) ;    // Exclude this boid from the average pos of the boids

        Vector3 directionToAverage = (averagePosAll - transform.position);
        return directionToAverage;
    }

    // Rule 2: Segregation
    private Vector3 ApplySegregation()
    {
        Vector3 avoidanceDirection = Vector3.zero;
        foreach (Boid boid in seperationBoids)
        {
            float distance = Vector3.Distance(boid.transform.position, transform.position);
            avoidanceDirection -= (boid.transform.position - transform.position).normalized/distance;
        }

        avoidanceDirection.x *= seperationScalar.x;
        avoidanceDirection.y *= seperationScalar.y;
        avoidanceDirection.z *= seperationScalar.z;
        return avoidanceDirection;
    }

    // Rule 3: Allignment
    private Vector3 ApplyAllignment()
    {
        Vector3 averageDirAll = CalculateAverageDirectionOfSet(neighbourBoids);
        //Vector3 averageDirExcl = ((averageDirAll / manager.boidCount) - direction) * (manager.boidCount - 1);    // Exclude this boid from the average direction of the boids

        return averageDirAll;
    }

    private Vector3 CalculateAveragePosOfSet(HashSet<Boid> _boidSet)
    {
        Vector3 totalPos = Vector3.zero;

        foreach (Boid b in _boidSet)
        {
            totalPos += b.transform.position;
        }

        return totalPos / _boidSet.Count;
    }

    private Vector3 CalculateAverageDirectionOfSet(HashSet<Boid> _boidSet)
    {
        Vector3 totalDirection = Vector3.zero;

        foreach (Boid b in _boidSet)
        {
            totalDirection += b.direction;
        }

        return totalDirection / _boidSet.Count;
    }

    private void FindNearbyBoids()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, neighbourRadius);

        foreach (Collider collider in colliders)
        {
            Boid boid = collider.GetComponent<Boid>();
            if (boid != null && boid != this)
            {
                neighbourBoids.Add(boid);
            }
        }

        seperationBoids = neighbourBoids.Where(b => (Vector3.Distance(b.transform.position, transform.position) < seperationRadius)).ToHashSet();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, neighbourRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seperationRadius);
    }
}
