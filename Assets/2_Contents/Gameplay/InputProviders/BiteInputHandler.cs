using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using Soar.Collections;
using Soar.Events;
using UnityEngine;

namespace Contents.Gameplay
{
    public class BiteInputHandler : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private GameEvent<bool> mouthOpenEvent;
        
        [Header("Output")]
        [SerializeField] private GameEvent<int> bittenBeanEvent;
        
        [Header("Dependencies")]
        [SerializeField] private Collection<int, GameObject> beans;
        [SerializeField] private GameObject mouthObject;
        
        private const float MouthColliderEnableDuration = 0.5f;
        private Collider2D mouthCollider;
        private SpriteRenderer mouthImage;

        private IDisposable subscription;

        private void Start()
        {
            mouthCollider = mouthObject.GetComponent<Collider2D>();
            mouthCollider.enabled = false;
            
            mouthImage = mouthObject.GetComponent<SpriteRenderer>();
            
            subscription = mouthOpenEvent.AsObservable().SubscribeAwait(CheckForBite, AwaitOperation.ThrottleFirstLast);
        }
        
        private async ValueTask CheckForBite(bool opened, CancellationToken cancellationToken)
        {
            if (opened)
            {
                mouthImage.color = Color.green;
            }
            
            if (opened || mouthCollider.enabled) return;
            
            mouthCollider.enabled = true;
            mouthImage.color = Color.red;
            
            await UniTask.Delay(TimeSpan.FromSeconds(MouthColliderEnableDuration), cancellationToken: cancellationToken);
            
            mouthCollider.enabled = false;
            mouthImage.color = Color.white;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Bean")) return;
            
            var pair = (beans as IDictionary<int, GameObject>).FirstOrDefault(pair => pair.Value == other.gameObject);
            if (pair.Value == null) return;
            
            bittenBeanEvent.Raise(pair.Key);
            mouthCollider.enabled = false;
        }
        
        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}