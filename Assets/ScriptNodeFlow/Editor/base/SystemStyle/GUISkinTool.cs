using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GUISkinTool
{
    [MenuItem("Assets/GUISkin/GetIcons", true)]
    static bool CanGetIcons()
    {
        return Selection.activeObject != null && Selection.activeObject is GUISkin;
    }

    [MenuItem("Assets/GUISkin/GetIcons",false, priority = 48)]
    static void GetIcons()
    {
        GUISkin skin = Selection.activeObject as GUISkin;

        List<Texture2D> textureList = GetAllTexture(skin);

        string fold = EditorUtility.OpenFolderPanel("save icons", "Assets", "");
        if (string.IsNullOrEmpty(fold))
            return;
        
        foreach (var item in textureList)
        {
            saveIcon2PNG(item, fold);
        }

        AssetDatabase.Refresh();
    }

    public static void saveIcon2PNG(Texture2D texture, string fold)
    {
        if (texture == null)
            return;

        byte[] bytes = texture.GetRawTextureData();

        Texture2D png = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
        png.LoadRawTextureData(bytes);
        png.Apply();

        string path = string.Format("{0}/{1}.png", fold, texture.name);
        FileHelper.CreatFile(path, png.EncodeToPNG());
    }

    public static List<Texture2D> GetAllTexture(GUISkin skin)
    {
        List<Texture2D> textureList = new List<Texture2D>();

        ParseStyle(skin.box, textureList);
        ParseStyle(skin.button, textureList);
        ParseStyle(skin.horizontalScrollbar, textureList);
        ParseStyle(skin.horizontalScrollbarLeftButton, textureList);
        ParseStyle(skin.horizontalScrollbarRightButton, textureList);
        ParseStyle(skin.horizontalScrollbarThumb, textureList);
        ParseStyle(skin.horizontalSlider, textureList);
        ParseStyle(skin.horizontalSliderThumb, textureList);
        ParseStyle(skin.label, textureList);
        ParseStyle(skin.scrollView, textureList);
        ParseStyle(skin.textArea, textureList);
        ParseStyle(skin.textField, textureList);
        ParseStyle(skin.toggle, textureList);
        ParseStyle(skin.verticalScrollbar, textureList);
        ParseStyle(skin.verticalScrollbarDownButton, textureList);
        ParseStyle(skin.verticalScrollbarThumb, textureList);
        ParseStyle(skin.verticalScrollbarUpButton, textureList);
        ParseStyle(skin.verticalSlider, textureList);
        ParseStyle(skin.verticalSliderThumb, textureList);
        ParseStyle(skin.window, textureList);

        foreach (var item in skin.customStyles)
        {
            ParseStyle(item, textureList);
        }

        return textureList;
    }

    public static void ParseStyle(GUIStyle style , List<Texture2D> textureList)
    {
        ParseStyleState(style.normal, textureList);

        ParseStyleState(style.active, textureList);

        ParseStyleState(style.hover, textureList);

        ParseStyleState(style.focused, textureList);

        ParseStyleState(style.onNormal, textureList);

        ParseStyleState(style.onActive, textureList);

        ParseStyleState(style.onHover, textureList);

        ParseStyleState(style.onFocused, textureList);
    }

    static void ParseStyleState(GUIStyleState state,List<Texture2D> textureList)
    {
        if (state.background != null &&
            !textureList.Contains(state.background))
        {
            textureList.Add(state.background);
        }

        foreach (var item in state.scaledBackgrounds)
        {
            if(item!=null 
               && !textureList.Contains(item))
            {
                textureList.Add(item);
            }
        }
    }
}
