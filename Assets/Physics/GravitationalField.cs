using UnityEngine;

public class GravitationalField : MonoBehaviour
{
    public float gravitationalForce = 5f;
    public float gravitationalRadius = 10f;

    private void Update()
    {
        ApplyGravitationalForce();
    }

    private void ApplyGravitationalForce()
    {
        var colliders = Physics.OverlapSphere(transform.position, gravitationalRadius);

        foreach (Collider rbcollider in colliders)
        {
            var targetRigidbody = rbcollider.GetComponent<Rigidbody>();

            if (targetRigidbody == null) continue;
            Vector3 pullDirection = (transform.position - rbcollider.transform.position).normalized;
            targetRigidbody.AddForce(pullDirection * (gravitationalForce * Time.deltaTime));
        }
    }
}