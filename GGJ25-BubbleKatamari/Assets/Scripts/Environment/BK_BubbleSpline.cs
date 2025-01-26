using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using static UnityEditor.PlayerSettings;
using UnityEditor;

public class BK_BubbleSpline : MonoBehaviour
{
    private SplineContainer splineContainer;
    [SerializeField] private GameObject bubblePrefab;

    [SerializeField] private int numBubbles = 10;
    [SerializeField] private AnimationCurve bubblePositionDistribution;
    [SerializeField] private AnimationCurve bubbleSizeDistribution;
    [SerializeField] private AnimationCurve bubbleAnimOffsetDistribution;
    
    [SerializeField] private bool animated = false;
    [SerializeField] private float animSpeed = 1f;

    public void PlaceBubbles()
    {
        if (splineContainer == null) { splineContainer = GetComponent<SplineContainer>(); }
        if (bubblePrefab == null) { return; }
        RemoveBubbles();

        float3 splineStart = splineContainer.Spline.EvaluatePosition(0f);

        for (int i = 0; i < numBubbles; i++)
        {
            float pos = (float)i / (numBubbles - (splineContainer.Spline.Closed ? 0 : 1));
            float3 splinePos = splineContainer.Spline.EvaluatePosition(bubblePositionDistribution.Evaluate(pos));

            Debug.Log($"Spawn: {i}, {pos}, {splinePos}");

            //GameObject newBubble = Instantiate(bubblePrefab, transform);
            GameObject newBubble = PrefabUtility.InstantiatePrefab(bubblePrefab, transform) as GameObject;
            newBubble.transform.localPosition = splinePos;
            BK_BubbleEnemy bubbleEnemy = newBubble.GetComponent<BK_BubbleEnemy>();
            bubbleEnemy.SetScaleFactor(bubbleSizeDistribution.Evaluate(pos));

            if (animated)
            {
                SplineAnimate splineAnim = newBubble.AddComponent<SplineAnimate>();

                splineAnim.Container = splineContainer;
                splineAnim.Alignment = SplineAnimate.AlignmentMode.None;
                splineAnim.StartOffset = bubbleAnimOffsetDistribution.Evaluate(pos);
                splineAnim.AnimationMethod = SplineAnimate.Method.Speed;
                splineAnim.MaxSpeed = animSpeed;
            }
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
