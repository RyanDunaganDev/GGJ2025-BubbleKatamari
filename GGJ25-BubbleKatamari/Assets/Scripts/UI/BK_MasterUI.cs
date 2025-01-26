using UnityEngine;
using UnityEngine.UIElements;

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
}
