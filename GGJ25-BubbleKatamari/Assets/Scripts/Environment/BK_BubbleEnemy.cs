using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class BK_BubbleEnemy : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private Transform bubbleMesh;

    [Header("Enemy - Scale Factor")]
    private float currentScaleFactor = 1f;
    public float TotalScaleFactor { get { return currentScaleFactor; } }
    public float HalfScaleFactor { get { return currentScaleFactor / 2f; } }
    public float CurrentVolume { get { return (4f / 3f * Mathf.PI * (sphereCollider.radius * sphereCollider.radius * sphereCollider.radius)); } }

    private void Awake()
    {
        if (sphereCollider == null) { sphereCollider = GetComponent<SphereCollider>(); }
        if (bubbleMesh == null) { bubbleMesh = transform.GetChild(0); }

        SetScaleFactor(sphereCollider.radius * 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out BK_BubbleCharacter bubbleCharacter))
            {
                Debug.Log($"Size comp: {bubbleCharacter.CurrentVolume}, {CurrentVolume}");

                // Player is bigger
                if (bubbleCharacter.CurrentVolume >= CurrentVolume)
                {
                    float cumulativeVolume = CurrentVolume + bubbleCharacter.CurrentTargetVolume;
                    float newRadius = Mathf.Pow((3f * cumulativeVolume) / (4f * Mathf.PI), 1f / 3f);

                    bubbleCharacter.SetScaleFactor(newRadius * 2f);

                    BK_GameManager.Instance.AddScore(CurrentVolume);

                    // Pop this bubble
                    Destroy(gameObject);
                }
                else // Bubble is bigger
                {
                    // Player loses
                    Destroy(bubbleCharacter.gameObject);

                    // Player lost
                    BK_GameManager.Instance.PlayerLost();
                }
            }
        }
    }

    public void SetScaleFactor(float targetAmount)
    {
        currentScaleFactor = targetAmount;

        sphereCollider.radius = HalfScaleFactor;
        bubbleMesh.transform.localScale = Vector3.one * TotalScaleFactor;
    }
}