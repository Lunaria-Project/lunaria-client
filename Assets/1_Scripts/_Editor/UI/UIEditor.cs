#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UIEditor
{
    public static TAfter ChangeScript<TBefore, TAfter>(TBefore beforeScript) where TBefore : MonoBehaviour where TAfter : MonoBehaviour
    {
        var beforeGameObject = beforeScript.gameObject;
        var beforeScriptSerializedObject = new SerializedObject(beforeScript);
        var scriptSerializedProperty = beforeScriptSerializedObject.FindProperty("m_Script");

        var afterComponent = typeof(TAfter).IsSubclassOf(typeof(UIBehaviour)) ? new GameObject("TempChangeScript", typeof(RectTransform)).AddComponent<TAfter>() : new GameObject("TempChangeScript").AddComponent<TAfter>();
        scriptSerializedProperty.objectReferenceValue = MonoScript.FromMonoBehaviour(afterComponent);
        beforeScriptSerializedObject.ApplyModifiedProperties();
        Object.DestroyImmediate(afterComponent.gameObject);

        return beforeGameObject.GetComponent<TAfter>();
    }

    public static TMPro.TMP_FontAsset DefaultFont => AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(
        "Assets/Fonts/NPSfont_regular SDF.asset"
    );
}
#endif