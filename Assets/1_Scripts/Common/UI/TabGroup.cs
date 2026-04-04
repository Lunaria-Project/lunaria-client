using System;
using UnityEngine;

namespace Lunaria
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private TabCell[] _tabCells;

        public int SelectedIndex => _selectedIndex;

        private int _selectedIndex = -1;
        private Action<int> _onTabChanged;

        public void SetTabChangedAction(Action<int> onTabChanged)
        {
            _onTabChanged = onTabChanged;
        }

        public void Init(int defaultIndex = 0)
        {
            for (int i = 0; i < _tabCells.Length; i++)
            {
                _tabCells[i].Init(i, SelectTab);
            }

            SelectTab(defaultIndex);
        }

        public void SelectTab(int index)
        {
            if (_selectedIndex == index) return;

            _selectedIndex = index;

            for (int i = 0; i < _tabCells.Length; i++)
            {
                _tabCells[i].SetSelected(i == index);
            }

            _onTabChanged?.Invoke(index);
        }
    }
}
