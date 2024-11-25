using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Feature.Extension
{
    public class UnityEventInputSystemBinder : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReference;
        [SerializeField] private UnityEvent onActionStarted;
        [SerializeField] private UnityEvent onActionPerformed;
        [SerializeField] private UnityEvent onActionCanceled;

        private void OnEnable()
        {
            inputActionReference.action.Enable();
            inputActionReference.action.started += OnInputActionStarted;
            inputActionReference.action.performed += OnInputActionPerformed;
            inputActionReference.action.canceled += OnInputActionCanceled;
        }

        private void OnDisable()
        {
            inputActionReference.action.Disable();
            inputActionReference.action.started -= OnInputActionStarted;
            inputActionReference.action.performed -= OnInputActionPerformed;
            inputActionReference.action.canceled -= OnInputActionCanceled;
        }
        
        private void OnInputActionStarted(InputAction.CallbackContext context)
        {
            onActionStarted.Invoke();
        }
        
        private void OnInputActionPerformed(InputAction.CallbackContext context)
        {
            onActionPerformed.Invoke();
        }
        
        private void OnInputActionCanceled(InputAction.CallbackContext context)
        {
            onActionCanceled.Invoke();
        }
    }
}