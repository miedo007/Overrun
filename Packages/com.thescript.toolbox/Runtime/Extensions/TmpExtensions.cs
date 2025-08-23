#if TMP_AVAILABLE

using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class TmpExtensions
    {
        /// <summary>
        /// Set the text on each TMP_Text entry
        /// </summary>
        public static void SetText(this IEnumerable<TMP_Text> texts, object text) => SetText(texts, text.ToString());

        /// <summary>
        /// Set the text on each TMP_Text entry
        /// </summary>
        public static void SetText(this IEnumerable<TMP_Text> texts, string text)
        {
            if (texts == null)
            {
                return;
            }

            foreach (var t in texts)
            {
                t.SetText(text);
            }
        }
    }
}

#endif