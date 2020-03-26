using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;
using UnityEngine;
////TODO: custom icon for OnScreenButton component

namespace UnityEngine.InputSystem.OnScreen {
    /// <summary>
    /// A button that is visually represented on-screen and triggered by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu ("Input/ButtonInput")]
    public class ButtonInput : OnScreenControl, IPointerDownHandler, IPointerUpHandler {
        public Button _零件;
        public void OnPointerUp (PointerEventData eventData) {
            if (_零件.interactable)
            {
                 SendValueToControl (0.0f);
            }
           
        }

        public void OnPointerDown (PointerEventData eventData) {
              if (_零件.interactable)
            {
                 SendValueToControl (1.0f);
            }
        }
        ////TODO: pressure support
        /*
        /// <summary>
        /// If true, the button's value is driven from the pressure value of touch or pen input.
        /// </summary>
        /// <remarks>
        /// This essentially allows having trigger-like buttons as on-screen controls.
        /// </remarks>
        [SerializeField] private bool m_UsePressure;
        */

        [InputControl (layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}