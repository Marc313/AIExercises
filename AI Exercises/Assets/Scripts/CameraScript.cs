using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float speed;
    /*    private Vector3 offset;
        private BoidManager boidManager;

        private void Start()
        {
            boidManager = FindObjectOfType<BoidManager>();
            offset = transform.position - boidManager.averagePos;
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {
            Vector3 newPos = transform.position;
            newPos.y = boidManager.averagePos.y + offset.y;
            transform.position = new Vector3(transform.position.x, newPos.y, newPos.z);
        }*/

    private void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        float forward = 0;
        if (Input.GetKey(KeyCode.E)) forward++;
        if (Input.GetKey(KeyCode.Q)) forward--;

        transform.position += new Vector3(hor, vert, forward) * Time.deltaTime * speed;
    }
}
