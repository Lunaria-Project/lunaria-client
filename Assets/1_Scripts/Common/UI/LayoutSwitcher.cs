using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lunaria
{
    public class LayoutSwitcher : MonoBehaviour
    {
        [Serializable]
        public class Layout
        {
            public string Key;
            public GameObject[] Objects;
        }

        [SerializeField] private Layout[] _layouts;

        public void SetLayout(string key)
        {
            foreach (var layout in _layouts)
            {
                layout.Objects.SetActiveAll(false);
            }

            foreach (var layout in _layouts)
            {
                if (layout.Key == key)
                {
                    layout.Objects.SetActiveAll(true);
                }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
#endif
        }

#if UNITY_EDITOR
        [PropertySpace, OnInspectorGUI]
        private void DrawPreviewButtons()
        {
            if (_layouts == null) return;

            foreach (var layout in _layouts)
            {
                if (GUILayout.Button($"미리보기: {layout.Key}"))
                {
                    SetLayout(layout.Key);
                }
            }

            if (GUILayout.Button("LayoutName 복사"))
            {
                var sb = new System.Text.StringBuilder();
                foreach (var layout in _layouts)
                {
                    sb.AppendLine($"private const string {layout.Key}LayoutKey = \"{layout.Key}\";");
                }
                GUIUtility.systemCopyBuffer = sb.ToString().TrimEnd();
            }
        }
#endif
    }
}
