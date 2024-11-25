using Soar.Variables;
using UnityEngine;

namespace Feature.Debugging
{
    public class DebugLogToVariable : MonoBehaviour
    {
        [SerializeField] private StringVariable debugTextVariable;

        private void OnEnable()
        {
            Application.logMessageReceived += OnApplicationOnLogMessageReceived;
        }
        
        private void OnDisable()
        {
            Application.logMessageReceived -= OnApplicationOnLogMessageReceived;
        }

        private void OnApplicationOnLogMessageReceived(string condition, string trace, LogType type)
        {
            debugTextVariable.Value += condition + "\n";
        }
    }
}