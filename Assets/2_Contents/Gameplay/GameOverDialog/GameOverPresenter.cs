using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kusoge.DataTransferObjects;
using Kusoge.Interfaces;
using LitMotion;
using UnityEngine;

namespace Contents.Gameplay
{
    public class GameOverPresenter : MonoBehaviour, IGameOverPresenter
    {
        [SerializeField] private Transform[] animationObjects;
        
        public async ValueTask<bool> Show(GameStatsDto stats, CancellationToken cancellationToken = default)
        {
            const float duration = 0.5f;
            const int delayFactor = 100;

            var tasks = animationObjects.Select((obj, i) =>
            {
                var startScale = obj.localScale;
                return LMotion.Create(Vector3.zero, startScale, duration)
                    .WithDelay(delayFactor * i)
                    .WithEase(Ease.InBounce)
                    .Bind(newScale => obj.localScale = newScale)
                    .ToUniTask(destroyCancellationToken);
            });
                
            tasks = tasks.Append(UniTask.Delay(1000, cancellationToken: cancellationToken)); // Debugging purpose;

            await UniTask.WhenAll(tasks);
            
            // TODO: confirm replay or exit.
            // return true for replay
            return true;
        }
    }
}