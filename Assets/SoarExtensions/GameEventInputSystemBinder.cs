using System;
using Soar.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Feature.Extension
{
    public class GameEventInputSystemBinder : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReference;
        [SerializeField] private GameEventBindInfo[] gameEventData;

        private void OnEnable()
        {
            inputActionReference.action.Enable();
            foreach (var eventData in gameEventData)
            {
                if (eventData.eventType.HasFlag(EventTypeFlag.Started))
                    inputActionReference.action.started += eventData.BindEvent;
                if (eventData.eventType.HasFlag(EventTypeFlag.Performed))
                    inputActionReference.action.performed += eventData.BindEvent;
                if (eventData.eventType.HasFlag(EventTypeFlag.Canceled))
                    inputActionReference.action.canceled += eventData.BindEvent;
            }
        }

        private void OnDisable()
        {
            inputActionReference.action.Disable();
            foreach (var eventData in gameEventData)
            {
                if (eventData.eventType.HasFlag(EventTypeFlag.Started))
                    inputActionReference.action.started -= eventData.BindEvent;
                if (eventData.eventType.HasFlag(EventTypeFlag.Performed))
                    inputActionReference.action.performed -= eventData.BindEvent;
                if (eventData.eventType.HasFlag(EventTypeFlag.Canceled))
                    inputActionReference.action.canceled -= eventData.BindEvent;
            }
        }
    }

    [Flags]
    internal enum EventTypeFlag
    {
        Started = 1,
        Performed = 1 << 1,
        Canceled = 1 << 2
    }
    
    [Serializable]
    internal class GameEventBindInfo
    {
        public GameEvent eventToFire;
        public EventTypeFlag eventType = EventTypeFlag.Performed;

        public void BindEvent(InputAction.CallbackContext context)
        {
            if (eventToFire is GameEvent<bool> boolEvent)
            {
                var value = context.ReadValue<bool>();
                boolEvent.Raise(value);
            }
            else if (eventToFire is GameEvent<float> floatEvent)
            {
                var value = context.ReadValue<float>();
                floatEvent.Raise(value);
            }
            else if (eventToFire is GameEvent<int> intEvent)
            {
                var value = context.ReadValue<int>();
                intEvent.Raise(value);
            }
            else if (eventToFire is GameEvent<Vector2> vector2Event)
            {
                var value = context.ReadValue<Vector2>();
                vector2Event.Raise(value);
            }
            else
            {
                eventToFire.Raise();
            }
        }
    }
}