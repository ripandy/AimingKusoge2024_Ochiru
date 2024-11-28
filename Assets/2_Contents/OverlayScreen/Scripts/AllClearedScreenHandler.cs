using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Contents.LevelIntro
{
    public class AllClearedScreenHandler : FadingScreenOverlayHandler
    {
        [SerializeField] private Button button;

        protected override async UniTask<int> OnFullView(int requestValue = default, CancellationToken cancellationToken = default)
        {
            await button.OnClickAsync(cancellationToken);
            return 0;
        }
    }
}