#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static partial class LunariaMenu
{
    [MenuItem("GameObject/UI/Text - Lunaria", false, 1)]
    private static void CreateSpText(MenuCommand menuCommand)
    {
        var gameObject = new GameObject("Text");
        Undo.RegisterCreatedObjectUndo(gameObject, "Create Text (Lunaria)");

        var text = gameObject.AddComponent<Lunaria.Text>();
        text.text = "New Text";
        text.font = UIEditor.DefaultFont;
        text.raycastTarget = false;

        GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
        Selection.activeObject = gameObject;
    }

    [MenuItem("GameObject/UI/Image - Lunaria", false, 1)]
    private static void CreateLunariaImage(MenuCommand menuCommand)
    {
        var gameObject = new GameObject("Image");
        Undo.RegisterCreatedObjectUndo(gameObject, "Create Image (Lunaria)");

        var image = gameObject.AddComponent<Lunaria.Image>();
        image.raycastTarget = false;

        GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
        Selection.activeObject = gameObject;
    }

    [MenuItem("GameObject/UI/Button - Lunaria", false, 2)]
    private static void CreateLunariaButton(MenuCommand menuCommand)
    {
        // Button Root
        var buttonObject = new GameObject("Button", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(buttonObject, "Create Button (Lunaria)");

        var buttonRectTransform = buttonObject.GetComponent<RectTransform>();
        buttonRectTransform.sizeDelta = new Vector2(160f, 40f);

        buttonObject.AddComponent<Lunaria.Image>();
        buttonObject.AddComponent<Lunaria.Button>();

        // Text Child
        var textObject = new GameObject("Text", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(textObject, "Create Button Text (Lunaria)");

        GameObjectUtility.SetParentAndAlign(textObject, buttonObject);

        var textRectTransform = textObject.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;

        var text = textObject.AddComponent<Lunaria.Text>();
        text.text = "Button";
        text.font = UIEditor.DefaultFont;
        text.color = Color.black;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.raycastTarget = false;

        GameObjectUtility.SetParentAndAlign(buttonObject, menuCommand.context as GameObject);
        Selection.activeObject = buttonObject;
    }

    [MenuItem("CONTEXT/TextMeshProUGUI/TextMeshProUGUI => Lunaria.Text", false)]
    private static void ChangeTLunariaText(MenuCommand menuCommand)
    {
        var source = menuCommand.context as TMPro.TextMeshProUGUI;
        UIEditor.ChangeScript<TMPro.TextMeshProUGUI, Lunaria.Text>(source);
    }

    [MenuItem("CONTEXT/Image/Image => Lunaria.Image", false)]
    private static void ChangeToLunariaImage(MenuCommand menuCommand)
    {
        var source = menuCommand.context as UnityEngine.UI.Image;
        UIEditor.ChangeScript<UnityEngine.UI.Image, Lunaria.Image>(source);
    }

    [MenuItem("CONTEXT/Button/Button => Lunaria.Button", false)]
    private static void ChangeToLunariaButton(MenuCommand menuCommand)
    {
        var source = menuCommand.context as UnityEngine.UI.Button;
        UIEditor.ChangeScript<UnityEngine.UI.Button, Lunaria.Button>(source);
    }
}
#endif