using Soar;
using Soar.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Debugging
{
    [CreateAssetMenu(fileName = "ResetAppCommand", menuName = MenuHelper.DefaultCommandMenu + "Reset App Command")]
    public class ResetAppCommand : Command
    {
        public override void Execute()
        {
            SceneManager.LoadScene("Boot");
        }
    }
}