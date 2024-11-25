using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Kusoge.Interfaces;
using R3;
using Soar.Variables;
using UnityEngine;

namespace Contents.Gameplay
{
    [CreateAssetMenu(fileName = "FaceDirectionConverterVectorVariable", menuName = "Kusoge/FaceDirectionConverterVectorVariable")]
    public class FaceDirectionConverterVectorVariable : Variable<Vector2>, IPlayerDirectionInputProvider
    {
        [SerializeField] private float faceLookThreshold = 0.3f;
        
        private DirectionEnum currentDirection;
        
        public async ValueTask<DirectionEnum> WaitForDirectionInput(CancellationToken cancellationToken = default)
        {
            currentDirection = await AsObservable().Select(ConvertToDirectionEnum)
                .FirstOrDefaultAsync(direction => direction != currentDirection, cancellationToken: cancellationToken);
            await UniTask.Yield();
            return currentDirection;
        }
        
        private DirectionEnum ConvertToDirectionEnum(Vector2 directionVector)
        {
            return directionVector.x >= faceLookThreshold ? DirectionEnum.Right :
                directionVector.x <= -faceLookThreshold ? DirectionEnum.Left :
                DirectionEnum.Forward;
        }
    }
}