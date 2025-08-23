using UnityEngine.UI;

namespace Mtl.UI
{
    public class ReverseContentSizeFitter : ContentSizeFitter
    {
        // Safety boolean to prevent two reverse fitters from
        // crawling up the hierarchy at the same time
        private static bool _applyingLayout;

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            if (_applyingLayout)
            {
                return;
            }

            _applyingLayout = true;

            ContentSizeFitter parent = this;
            while ((parent = parent.transform.parent.GetComponent<ContentSizeFitter>()) != null)
            {
                parent.SetLayoutHorizontal();
            }

            _applyingLayout = false;
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            if (_applyingLayout)
            {
                return;
            }

            _applyingLayout = true;

            ContentSizeFitter parent = this;
            while ((parent = parent.transform.parent.GetComponent<ContentSizeFitter>()) != null)
            {
                parent.SetLayoutVertical();
            }

            _applyingLayout = false;
        }
    }
}