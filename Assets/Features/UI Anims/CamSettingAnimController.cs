using UnityEngine;

public class CamSettingAnimController : MonoBehaviour
{
    [SerializeField] private Animator anim = null;
    [SerializeField] private string openAnimName = null;
    [SerializeField] private string closeAnimName = null;
    [SerializeField] private string openTriggerName = null;
    [SerializeField] private string closeTriggerName = null;
    
    
    public void OpenClosePanel()
    {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == closeAnimName)
        {
            anim.SetTrigger(openTriggerName);
            return;
        }
        
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == openAnimName)
        {
            anim.SetTrigger(closeTriggerName);
        }
    }
}
