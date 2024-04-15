using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soda.Utilities
{
    internal class GUIHelperSoda
    {
        #region Layout
        static Texture Banner;
        static int logoCounter = 0;
        internal static void ShowHeaderLogo()
        {
            if (Banner == null && logoCounter < 5)
            {
                logoCounter++;
                Banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/SodaCreative/SodaCreativeHeader.png", typeof(Texture));
            }

            if (Banner == null) return;

            var rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = Banner.width;
            rect.height = Banner.height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, Banner);

            var e = Event.current;
            if (e.type != EventType.MouseUp)
            {
                return;
            }
            if (!rect.Contains(e.mousePosition))
            {
                return;
            }
        }
        #endregion

        #region Styling
        internal static GUIStyle guiMessageStyle
        {
            get
            {
                var messageStyle = new GUIStyle(GUI.skin.label);
                messageStyle.wordWrap = true;

                return messageStyle;
            }
        }
        #endregion
    }
}
