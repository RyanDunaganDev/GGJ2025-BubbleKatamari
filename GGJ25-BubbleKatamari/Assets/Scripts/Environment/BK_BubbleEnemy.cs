using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

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

    [Header("Enemy - Death Shader")]
    private Material mat = null;
    [SerializeField] private float duration = 0.1f;

    private void Awake()
    {
        mat = GetComponentInChildren<Renderer>().material;

        if (sphereCollider == null) { sphereCollider = GetComponent<SphereCollider>(); }
        if (bubbleMesh == null) { bubbleMesh = transform.GetChild(0); }

        RefreshScaleFactor();
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
                    StartCoroutine(BubbleDeath());
                }
                else // Bubble is bigger
                {
                    // Player loses
                    bubbleCharacter.KillPlayer();
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

    public void RefreshScaleFactor()
    {
        SetScaleFactor(sphereCollider.radius * 2f);
    }

    public IEnumerator BubbleDeath()
    {
        BK_AudioManager.Instance.PlayBubblePopOneshot();
        
        float count = 0f;

        while (count < duration)
        {
            count += Time.deltaTime;
            float value = math.remap(0f, duration, -1f, 1, count); // -1f to 1f
            mat.SetFloat("_IsPop", value);
            yield return null;
        }

        yield return null;

        Destroy(gameObject);
    }
}