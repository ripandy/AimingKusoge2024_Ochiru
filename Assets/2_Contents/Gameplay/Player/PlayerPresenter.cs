using System;
using Kusoge.Interfaces;
using Soar.Events;
using UnityEngine;

namespace Contents.Gameplay
{
    public class PlayerPresenter : MonoBehaviour
    {
        [SerializeField] private GameEvent<DirectionEnum> playerPlayerDirection;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Transform mouthTransform;
        [SerializeField] private float lookDistance = 1.8f;

        private IDisposable subscription;
        
        private Vector3 defaultPosition;

        private void Start()
        {
            defaultPosition = mouthTransform.localPosition;
            subscription = playerPlayerDirection.Subscribe(OnPlayerEvent);
        }

        private void OnPlayerEvent(DirectionEnum direction)
        {
            var position = defaultPosition;
            position.x += direction switch
            {
                DirectionEnum.Forward => 0,
                DirectionEnum.Left => -lookDistance,
                DirectionEnum.Right => lookDistance,
                _ => 0
            };
            mouthTransform.localPosition = position;
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}