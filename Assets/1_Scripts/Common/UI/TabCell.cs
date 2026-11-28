using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lunaria
{
    public class TabCell : MonoBehaviour
    {
        [SerializeField] private GameObject[] _selectedObjects;
        [SerializeField] private GameObject[] _unselectedObjects;

        private int _index;
        private Action<int> _onClicked;

        public void Init(int index, Action<int> onClicked)
        {
            _index = index;
            _onClicked = onClicked;
        }

        public void SetSelected(bool isSelected)
        {
            _selectedObjects.SetActiveAll(isSelected);
            _unselectedObjects.SetActiveAll(!isSelected);
        }

        public void OnClick()
        {
            _onClicked?.Invoke(_index);
        }

#if UNITY_EDITOR
        [PropertySpace, OnInspectorGUI]
        private void DrawPreviewButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("미리보기: 선택"))
            {
                SetSelected(true);
            }
            if (GUILayout.Button("미리보기: 비선택"))
            {
                SetSelected(false);
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}