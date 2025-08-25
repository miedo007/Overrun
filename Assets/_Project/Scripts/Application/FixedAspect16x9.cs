using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspect16x9 : MonoBehaviour
{
    [Tooltip("Run only on phones/tablets. Desktop stays full-rect.")]
    public bool onlyOnMobile = true;

    public float targetAspect = 16f / 9f;
    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); Apply(); }
    void OnRectTransformDimensionsChange() { Apply(); }
    void OnPreCull() { Apply(); } // handles iOS UI bars changing size

    bool IsMobile() =>
        Application.isMobilePlatform || SystemInfo.deviceType == DeviceType.Handheld;

    void Apply()
    {
        if (onlyOnMobile && !IsMobile()) { cam.rect = new Rect(0,0,1,1); return; }

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
            cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);          // letterbox
        else
        {
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);            // pillarbox
        }
    }
}
