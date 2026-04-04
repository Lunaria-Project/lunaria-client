using System;
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
    }
}