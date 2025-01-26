using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(BK_BubbleEnemy))]
public class BK_BubbleEnemyEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh Scale Factor"))
        {
            (target as BK_BubbleEnemy).RefreshScaleFactor();
            EditorUtility.SetDirty(target);
        }
    }
}
