using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kusoge.Interfaces;
using R3;
using Soar.Events;
using Soar.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Contents.Gameplay
{
    public class PlayerSpritePresenter : MonoBehaviour
    {
        [SerializeField] private GameEvent<DirectionEnum> playerDirection;
        [SerializeField] private GameEvent<bool> mouthOpenEvent;
        [SerializeField] private Variable<float> mouthColliderEnableDuration;
        [SerializeField] private PlayerSpriteCollection playerSpriteCollection;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private readonly ReactiveProperty<PlayerSpriteEnum> currentSprite = new();
        private IDisposable subscription;

        private void Start()
        {
            var s1 = playerDirection.Subscribe(OnPlayerDirection);
            var s2 = mouthOpenEvent.AsObservable().SubscribeAwait(OnMouthOpen, AwaitOperation.Drop);
            
            // Setup Auto Blink
            var s3 = Observable.Interval(TimeSpan.FromSeconds(3))
                .SubscribeAwait(async (_, token) => await TryUpdateBlink(token));

            var s4 = currentSprite.Subscribe(UpdateSprite);
            
            subscription = new CompositeDisposable(s1, s2, s3, s4);
            
            currentSprite.Value = PlayerSpriteEnum.IdleMouthClosed;
        }

        private void OnPlayerDirection(DirectionEnum direction)
        {
            var newSprite = currentSprite.Value & ~(PlayerSpriteEnum.Left | PlayerSpriteEnum.Right);
            newSprite |= direction switch
            {
                DirectionEnum.Left => PlayerSpriteEnum.Left,
                DirectionEnum.Right => PlayerSpriteEnum.Right,
                _ => PlayerSpriteEnum.None
            };
            currentSprite.Value = newSprite;
        }
        
        private async ValueTask OnMouthOpen(bool opened, CancellationToken cancellationToken)
        {
            if (opened)
            {
                currentSprite.Value = (currentSprite.Value & ~PlayerSpriteEnum.MouthClose) | PlayerSpriteEnum.MouthOpen;
                await UniTask.NextFrame(cancellationToken);
            }
            
            if (opened || currentSprite.Value.HasFlag(PlayerSpriteEnum.Bite)) return;

            currentSprite.Value = (currentSprite.Value &
                                  ~(PlayerSpriteEnum.MouthOpen | PlayerSpriteEnum.Blink | PlayerSpriteEnum.Idle))
                                  | PlayerSpriteEnum.Bite;
            
            await UniTask.Delay(TimeSpan.FromSeconds(mouthColliderEnableDuration), cancellationToken: cancellationToken);
            
            currentSprite.Value = (currentSprite.Value & ~PlayerSpriteEnum.Bite) | PlayerSpriteEnum.IdleMouthClosed;
        }

        private async ValueTask TryUpdateBlink(CancellationToken token = default)
        {
            const float blinkChance = 0.4f;
            if (Random.value > blinkChance || currentSprite.Value.HasFlag(PlayerSpriteEnum.Bite)) return;
            
            currentSprite.Value = (currentSprite.Value & ~PlayerSpriteEnum.Idle) | PlayerSpriteEnum.Blink;
            
            const float blinkDuration = 0.2f;
            await UniTask.Delay(TimeSpan.FromSeconds(blinkDuration), cancellationToken: token);
            
            currentSprite.Value = (currentSprite.Value & ~PlayerSpriteEnum.Blink) | PlayerSpriteEnum.Idle;
        }

        private void UpdateSprite(PlayerSpriteEnum newSpriteFlag)
        {
            Debug.Log($"[{GetType().Name}] UpdateSprite: {newSpriteFlag}");
            if (!playerSpriteCollection.TryGetValue(newSpriteFlag, out var sprite)) return;
            spriteRenderer.sprite = sprite;
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}