using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(BK_BubbleSpline))]
public class BK_BubbleSplineEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Place Bubbles"))
        {
            (target as BK_BubbleSpline).PlaceBubbles();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Clear Bubbles"))
        {
            (target as BK_BubbleSpline).RemoveBubbles();
            EditorUtility.SetDirty(target);
        }

        //if (GUILayout.Button("Align Spline to Ground"))
        //{
        //    (target as BK_BubbleSpline).AlignSplineToGround();
        //    EditorUtility.SetDirty(target);
        //}

        base.OnInspectorGUI();
    }
}
