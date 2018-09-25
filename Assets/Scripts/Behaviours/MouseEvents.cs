using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Sevakon.Behaviours
{
    /// <summary>
    /// Wire mouse events (from message or event system) to Unity events visible in the inspector
    /// </summary>
    public class MouseEvents : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Mouse enter event
        /// </summary>
        public UnityEvent MouseEnter;
        
        /// <summary>
        /// Mouse exit event
        /// </summary>
        public UnityEvent MouseExit;
        
        /// <summary>
        /// Mouse click event
        /// </summary>
        public UnityEvent MouseClick;

        private void OnMouseEnter()
        {
            MouseEnter.Invoke();
        }

        private void OnMouseExit()
        {
            MouseExit.Invoke();
        }

        private void OnMouseDown()
        {
            MouseClick.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEnter.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseExit.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MouseClick.Invoke();
        }
    }
}
