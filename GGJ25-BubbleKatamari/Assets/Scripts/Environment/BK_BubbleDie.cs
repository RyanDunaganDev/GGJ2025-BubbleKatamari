using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;

[RequireComponent(typeof(Material))]
public class BK_BubbleDie : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    private string paramName = "IsPop";
    private Material mat = null;

    float step = 0.1f;

    private void Awake()
    {
        mat = GetComponent<Material>();
    }

    public void RunDieAnimation()
    {
        StartCoroutine(CoDieAnimtation());
    }

    private IEnumerator CoDieAnimtation()
    {
        float duratedTime = 0f;

        while (duratedTime < duration)
        {
            duratedTime += step;
            mat.SetFloat(paramName, math.remap(0f, duratedTime, -1f, 1, duratedTime));
            yield return new WaitForSeconds(step);
        }
    }
}
