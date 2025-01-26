using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

public class BK_MasterUI : MonoBehaviour
{
    protected void PlayBubblePopSFX(ClickEvent evt) { PlayBubblePopSFX(); }
    protected void PlayBubblePopSFX(MouseOverEvent evt) { PlayBubblePopSFX(); }
    protected void PlayBubblePopSFX()
    {
        BK_AudioManager.Instance.PlayBubblePopOneshot();
    }

    protected void RegisterButtonSFX(Button btn)
    {
        btn.RegisterCallback<ClickEvent>(PlayBubblePopSFX);
        btn.RegisterCallback<MouseOverEvent>(PlayBubblePopSFX);
    }
    protected void UnregisterButtonSFX(Button btn)
    {
        btn.UnregisterCallback<ClickEvent>(PlayBubblePopSFX);
        btn.UnregisterCallback<MouseOverEvent>(PlayBubblePopSFX);
    }

    protected Vector2 V2Wobble(float offsetX, float offsetY, float totalScale = 0.05f)
    {
        float xScl = Mathf.Sin((Time.unscaledTime + offsetX) * 2f);
        float yScl = Mathf.Cos((Time.unscaledTime + offsetY));

        return new Vector2(math.remap(-1f, 1f, 1f - totalScale, 1f, xScl), math.remap(-1f, 1f, 1f - totalScale, 1f, yScl));
    }

    protected Translate V2Translate(float offsetX, float offsetY, float xDist, float yDist, float offScale = 1f)
    {
        return new Translate(new Length((Mathf.PerlinNoise1D((Time.unscaledTime + offsetX) * 0.3f) * 2f - 1f) * xDist * offScale), new Length((Mathf.PerlinNoise1D((Time.unscaledTime + offsetY) * 0.15f) * 2f - 1f) * yDist * offScale));
    }
}
