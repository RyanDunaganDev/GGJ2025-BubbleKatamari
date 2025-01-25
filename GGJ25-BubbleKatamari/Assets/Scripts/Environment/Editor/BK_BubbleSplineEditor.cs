using UnityEngine;

[UnityEditor.CustomEditor(typeof(BK_BubbleSpline))]
public class BK_BubbleSplineEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Place Bubbles"))
            (target as BK_BubbleSpline).PlaceBubbles();

        if (GUILayout.Button("Clear Bubbles"))
            (target as BK_BubbleSpline).RemoveBubbles();

        base.OnInspectorGUI();
    }
}
