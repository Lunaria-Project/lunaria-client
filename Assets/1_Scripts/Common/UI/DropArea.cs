using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lunaria
{
    public class DropArea : MonoBehaviour, IDropHandler
    {
        private Action _onDropAction;

        public void SetDropAction(Action onDropAction)
        {
            _onDropAction = onDropAction;
        }

        public void OnDrop(PointerEventData eventData)
        {
            _onDropAction?.Invoke();
        }
    }
}
