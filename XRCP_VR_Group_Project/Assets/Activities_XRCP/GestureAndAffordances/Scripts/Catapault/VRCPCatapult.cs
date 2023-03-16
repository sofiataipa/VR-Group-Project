using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCPCatapult : MonoBehaviour
{
    public GameObject handAnchor;
    public Transform projectileSpawnTransform;
    public GameObject projectilePrefab;
    public float interactThreshold = 0.5f;

    private bool activated;
    private GameObject currentProjectile = null;
    private Rigidbody projectileRigidbody = null;
    private LineRenderer line;

    private Vector3 vec;
    private Vector3 forceVector;
    private float counter;
    private float magnitude;
    
    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        counter = 100;
        line = transform.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
           vec = projectileSpawnTransform.position - handAnchor.transform.position;

           Vector3[] positions = { projectileSpawnTransform.position, handAnchor.transform.position };
           line.SetPositions(positions);
           currentProjectile.transform.position = handAnchor.transform.position;
           currentProjectile.transform.LookAt(handAnchor.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if(counter < 30)
        {
            counter++;
            float forceFade = 10f * (10-counter / 10);
            projectileRigidbody.AddForce(forceVector * (magnitude* forceFade));
        }
        else if(!activated)
        {
            currentProjectile = null;
            projectileRigidbody = null;

        }
 
    }

    public void Release()
    {
        if(currentProjectile != null)
        {
            forceVector = vec.normalized;
            magnitude = vec.magnitude;
           
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.useGravity = true;
            activated = false;
            line.enabled = false;
            counter = 0;
        }

    }

    public void Pull()
    {
        vec = projectileSpawnTransform.position - handAnchor.transform.position;
        float length = vec.magnitude;
        if (length < interactThreshold && currentProjectile == null && counter >=10)
        {
            currentProjectile = Instantiate(projectilePrefab, projectileSpawnTransform.position, transform.rotation);
            projectileRigidbody = currentProjectile.GetComponent<Rigidbody>();
            projectileRigidbody.isKinematic = true;
            projectileRigidbody.useGravity = false;
            line.enabled = true;
            activated = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(projectileSpawnTransform.position, interactThreshold);
    }

}
