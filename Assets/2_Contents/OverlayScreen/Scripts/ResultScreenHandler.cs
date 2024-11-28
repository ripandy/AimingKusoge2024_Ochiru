using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Contents.LevelIntro
{
    public class ResultScreenHandler : FadingScreenOverlayHandler
    {
        [SerializeField] private Button[] buttons;
        
        protected override UniTask<int> OnFullView(int requestValue = default, CancellationToken cancellationToken = default)
        {
            var tasks = buttons.Select(button => button.OnClickAsync(cancellationToken));
            return UniTask.WhenAny(tasks);
        }
    }
}