using UnityEngine;

public class PushBackObject : MonoBehaviour
{
    public float pushForce;

    private void OnCollisionEnter(Collision collision)
    {
        var otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        if (otherRigidbody == null) return;

        Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
        pushDirection.y = 0f;

        otherRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}