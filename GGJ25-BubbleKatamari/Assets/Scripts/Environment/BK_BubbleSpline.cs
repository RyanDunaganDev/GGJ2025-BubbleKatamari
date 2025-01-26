using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEditor;

public class BK_BubbleSpline : MonoBehaviour
{
    private SplineContainer splineContainer;
    [SerializeField] private GameObject bubblePrefab;

    [SerializeField] private int numBubbles = 10;
    [SerializeField] private AnimationCurve bubblePositionDistribution;
    [SerializeField] private AnimationCurve bubbleSizeDistribution;
    
    [SerializeField] private bool animated = false;
    [SerializeField] private float animSpeed = 1f;

    //[SerializeField] private float splineGroundOffset = 0f;
    //[SerializeField] private LayerMask splineGroundLayers = 1;

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
                splineAnim.StartOffset = bubblePositionDistribution.Evaluate(pos);
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

    //List<Vector3> startPoints = new List<Vector3>();
    //List<Vector3> hitPoints = new List<Vector3>();

    //public void AlignSplineToGround()
    //{
    //    if (splineContainer == null) { splineContainer = GetComponent<SplineContainer>(); }
    //    if (splineContainer.Spline == null || splineContainer.Spline.Count == 0) { return; }

    //    startPoints.Clear();
    //    hitPoints.Clear();

    //    foreach (BezierKnot knot in splineContainer.Spline.Knots)
    //    {
    //        //Debug.Log($"Pos: {transform.InverseTransformPoint(knot.Position)}");

    //        startPoints.Add(transform.position + (Vector3)knot.Position);

    //        bool didHit = Physics.Raycast(transform.position + (Vector3)knot.Position, Vector3.down, out RaycastHit hit, 500f, splineGroundLayers);
    //        if (!didHit)
    //        {
    //            hitPoints.Add(transform.position + (Vector3)knot.Position);
    //            continue;
    //        }
    //        hitPoints.Add(hit.point);
    //        //Debug.Log($"Final Pos: {transform.TransformPoint(hit.point)}");
    //    }

    //    for (int i = 0; i < splineContainer.Spline.Count; i++)
    //    {
    //        splineContainer.Spline.SetKnot(i, new BezierKnot(hitPoints[i]));
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < startPoints.Count; i++)
    //    {
    //        if (startPoints[i] == null) { continue; }
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawLine(startPoints[i], startPoints[i] + (Vector3.down * 500f));
    //    }

    //    for (int i = 0; i < hitPoints.Count; i++)
    //    {
    //        if (hitPoints[i] == null) { continue; }
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(hitPoints[i], 1f);
    //    }
    //}
}
