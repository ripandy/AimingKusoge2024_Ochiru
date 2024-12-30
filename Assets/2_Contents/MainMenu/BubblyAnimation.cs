using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace Feature.MainMenu
{
    public class BubblyAnimation : MonoBehaviour
    {
        [SerializeField] private float scaleValueX = 1.04f;
        [SerializeField] private float scaleValueY = 1.16f;
        [SerializeField] private float animDuration = 0.5f;
        
        private void Start()
        {
            AnimateTransform().Forget();
        }

        private async UniTask AnimateTransform()
        {
            var labelTransform = transform;
            var localScale = labelTransform.localScale;
            var startScale = new Vector3(localScale.x, scaleValueY, localScale.z);
            var endScale = new Vector3(scaleValueX, localScale.y, localScale.z);

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                await LMotion.Create(startScale, endScale, animDuration)
                    .WithLoops(2, LoopType.Yoyo)
                    .Bind(scale => labelTransform.localScale = scale)
                    .ToUniTask(destroyCancellationToken);
            }
        }
    }
}