using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using static UnityEditor.PlayerSettings;

public class BK_BubbleSpline : MonoBehaviour
{
    private SplineContainer splineContainer;
    [SerializeField] private GameObject bubblePrefab;

    [SerializeField] private int numBubbles = 10;
    [SerializeField] private AnimationCurve bubbleDistribution;

#if UNITY_EDITOR
    private void OnValidate()
    {
        PlaceBubbles();
    }
#endif

    public void PlaceBubbles()
    {
        if (splineContainer == null) { splineContainer = GetComponent<SplineContainer>(); }
        if (bubblePrefab == null) { return; }
        RemoveBubbles();

        float3 splineStart = splineContainer.Spline.EvaluatePosition(0f);

        for (int i = 0; i < numBubbles; i++)
        {
            float pos = (float)i / (numBubbles - 1);
            float3 splinePos = splineContainer.Spline.EvaluatePosition(pos);
            //splinePos += splineStart;

            Debug.Log($"Spawn: {i}, {pos}, {splinePos}");

            GameObject newBubble = Instantiate(bubblePrefab, transform);
            newBubble.transform.localPosition = splinePos;
        }
    }

    public void RemoveBubbles()
    {
        if (transform.childCount == 0) { return; }

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
