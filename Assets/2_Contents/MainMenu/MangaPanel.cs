using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Soar.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.MainMenu
{
    public class MangaPanel : MonoBehaviour
    {
        [SerializeField] private GameEvent<string> setNextStateEvent;
        [SerializeField] private string nextState = "Gameplay";
        [SerializeField] private Image[] mangaPanels;

        private void Start() => AnimatePanels(destroyCancellationToken).Forget();

        private async UniTaskVoid AnimatePanels(CancellationToken token = default)
        {
            foreach (var panel in mangaPanels)
            {
                var color = panel.color;
                color.a = 0;
                panel.color = color;
            }
            
            const float duration = 0.8f;
            
            await LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[0], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            await LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[1], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            await LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[2], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            var panelMidFadeOut = LMotion.Create(1f, 0f, duration).WithEase(Ease.OutBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[2], alpha)).ToUniTask(cancellationToken: token);
            
            var panel2FadeIn = LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[3], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.WhenAll(panelMidFadeOut, panel2FadeIn);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            await LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[4], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            await LMotion.Create(0f, 1f, duration).WithEase(Ease.InBack)
                .Bind(alpha => UpdateAlpha(mangaPanels[5], alpha)).ToUniTask(cancellationToken: token);
            
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            
            setNextStateEvent.Raise(nextState);
        }
        
        private void UpdateAlpha(Image panel, float alpha)
        {
            var color = panel.color;
            color.a = alpha;
            panel.color = color;
        }
    }
}