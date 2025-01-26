using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent (typeof(Collider))]
public class BK_AreaEffector : MonoBehaviour
{
    [SerializeField] private LayerMask colliderMask = 1;

    [Header("Force")]
    [SerializeField] private bool useGlobalAngle = false;
    [SerializeField] private Vector3 forceDirection = Vector3.up;
    [SerializeField] private float forceMagnitude = 10f;
    [SerializeField] private float forceVariation = 0f;

    public void OnTriggerStay(Collider other)
    {
        Debug.Log($"Other: {other.gameObject.name}, {LayerMask.LayerToName(other.gameObject.layer)}");
        if (colliderMask == (colliderMask | (1 << other.gameObject.layer)))
        {
            Debug.Log($"Valid Layer Other: {other.gameObject.name}, {LayerMask.LayerToName(other.gameObject.layer)}");
            if (other.TryGetComponent(out Rigidbody rb))
            {
                Debug.Log($"Applying force to: {other.gameObject}");
                if (useGlobalAngle)
                {
                    rb.AddForce(forceDirection * (forceMagnitude + (forceVariation * Random.Range(-1f, 1f))));
                }
                else
                {
                    // TODO: Why does this not work as expected?
                    rb.AddForce(transform.InverseTransformDirection(forceDirection) * (forceMagnitude + (forceVariation * Random.Range(-1f, 1f))));
                }
            }
        }
    }
}
