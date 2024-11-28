using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Soar.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Contents.LevelIntro
{
    public class LevelIntroScreenHandler : FadingScreenOverlayHandler, IPointerClickHandler
    {
        [SerializeField] private Variable<float> initialDelayDuration;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private float holdDuration = 2f;
        
        private IDisposable subscription;

        protected override async UniTaskVoid Initialize()
        {
            if (initialDelayDuration != null)
                await UniTask.Delay(TimeSpan.FromSeconds(initialDelayDuration), cancellationToken: cts.Token);
            
            subscription = Observable.EveryUpdate()
                .Where(_ => cts != null && Input.anyKeyDown)
                .Subscribe(_ => CancelToken());
            
            base.Initialize().Forget();
        }

        protected override UniTask OnFadeIn(int requestValue, CancellationToken cancellationToken = default)
        {
            levelText.text = requestValue.ToString();
            return base.OnFadeIn(requestValue, cancellationToken);
        }

        protected override async UniTask<int> OnFullView(int requestValue = default, CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(holdDuration), cancellationToken: cancellationToken);
            return 0;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            CancelToken();
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}