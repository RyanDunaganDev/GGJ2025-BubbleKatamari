using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class BK_BubbleEnemy : MonoBehaviour
{
    private float bubbleVolume = 0f;
    private float halfRadius = 0f;
    private float cubedRadius = 0f;

    private SphereCollider sphereCollider;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        halfRadius = sphereCollider.radius / 2f;
        cubedRadius = sphereCollider.radius * sphereCollider.radius * sphereCollider.radius;
        bubbleVolume = (4f / 3f) * Mathf.PI * cubedRadius;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out BK_BubbleCharacter bubbleCharacter))
            {
                if (Vector3.Distance(transform.position, other.transform.position) - bubbleCharacter.HalfScaleFactor < halfRadius)
                {
                    float cumulativeVolume = bubbleVolume + bubbleCharacter.CurrentTargetVolume;
                    float newRadius = Mathf.Pow(3f * cumulativeVolume / 4f * Mathf.PI, 1f/3f);

                    bubbleCharacter.SetScaleFactor(newRadius);

                    BK_GameManager.Instance.AddScore(bubbleVolume);

                    // Pop this bubble
                    Destroy(gameObject);
                }
            }
        }
    }
}