using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Soar.Events;
using UnityEngine;

namespace Feature.SplashScreen
{

    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private string nextState = "MainMenu";
        [SerializeField] private float splashDuration;
        
        [Header("Output")]
        [SerializeField] private GameEvent<string> setNextStateEvent;

        private async void Start()
        {
            await ShowSplash(destroyCancellationToken);
            
            setNextStateEvent.Raise(nextState);
            
            await UniTask.NextFrame(destroyCancellationToken);
            Destroy(gameObject);
        }

        private async UniTask ShowSplash(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(splashDuration), cancellationToken: token);
        }
    }
}
