using System.Threading.Tasks;
using Kusoge.GameStates;
using UnityEngine;

namespace Kusoge.Tests
{
    public class DummyIntroPresenter : IIntroPresenter
    {
        public async ValueTask Show()
        {
            var count = 3;
            while (count > 0)
            {
                Debug.Log($"Game is starting in: {count--}");
                await Task.Delay(1000);
            }
            Debug.Log("Start Game!");
            await Task.Delay(300);
            Debug.Log("========================================");
        }
    }
}