using Kusoge.Interfaces;
using Soar.Variables;

namespace Contents.Gameplay
{
    public class PlayerDirectionVariable : Variable<DirectionEnum>, IPlayerDirectionPresenter
    {
        public void Show(DirectionEnum direction)
        {
            Value = direction;
        }
    }
}