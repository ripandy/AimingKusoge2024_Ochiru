using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitMotion;
using Soar.Transactions;
using UnityEngine;

namespace Contents.LevelIntro
{
    public abstract class FadingScreenOverlayHandler : MonoBehaviour
    {
        [SerializeField] private Transaction<int> showScreenRequestResponse;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        protected CancellationTokenSource cts;
        
        private IDisposable subscription;

        private void Start() => Initialize().Forget();

        protected virtual async UniTaskVoid Initialize()
        {
            showScreenRequestResponse.RegisterResponse(Response);
            await UniTask.Yield();
        }

        private async ValueTask<int> Response(int request, CancellationToken cancellationToken)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            var token = cts.Token;
            
            canvasGroup.gameObject.SetActive(true);

            int result;
            try
            {
                await OnFadeIn(request, token);
                result = await OnFullView(request, token);
                await OnFadeOut(request, token);
            }
            catch (OperationCanceledException)
            {
                result = 0;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
            
            return result;
        }

        protected virtual UniTask OnFadeIn(int requestValue, CancellationToken cancellationToken = default)
        {
            return LMotion.Create(0, 1, fadeDuration).Bind(alpha => canvasGroup.alpha = alpha).ToUniTask(cancellationToken: cancellationToken);
        }
        protected abstract UniTask<int> OnFullView(int requestValue = default, CancellationToken cancellationToken = default);
        
        protected virtual UniTask OnFadeOut(int requestValue = default, CancellationToken cancellationToken = default)
        {
            return LMotion.Create(1, 0, fadeDuration).Bind(alpha => canvasGroup.alpha = alpha).ToUniTask(cancellationToken: cancellationToken);
        }

        protected void CancelToken()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }
}