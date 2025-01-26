using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;

[RequireComponent(typeof(Material))]
public class BK_BubbleDie : MonoBehaviour
{
    [SerializeField] private float durtion = 0.5f;
    private string paramName = "IsPop";
    private Material mat = null;


    public void RunDieAnimation()
    {
        StartCoroutine(CoDieAnimtation());
    }

    private IEnumerator CoDieAnimtation()
    {
        float duratedTime = 0f;
        float isPop = 0f;
        while (duratedTime < durtion)
        {
            duratedTime += Time.deltaTime;
            mat.SetFloat(paramName, math.remap(0f, duratedTime, -1f, 1, duratedTime));
            yield return false;
        }
        yield return true;
    }

    void Activate()
    {
        mat = GetComponent<Material>();
    }
}
