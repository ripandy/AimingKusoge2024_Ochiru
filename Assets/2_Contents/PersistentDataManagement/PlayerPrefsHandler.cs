using System.Collections.Generic;
using R3;
using Soar.Events;
using Soar.Variables;
using UnityEngine;

namespace Feature.PersistentDataManagement
{
    public class PlayerPrefsHandler : MonoBehaviour
    {
        [SerializeField] private List<GameEvent> variables;

        private void Start()
        {
            Load();
            Bind();
        }

        private void Load()
        {
            foreach (var variable in variables)
            {
                if (!PlayerPrefs.HasKey(variable.name)) continue;
                if (variable is Variable<int> intVar)
                    intVar.Value = PlayerPrefs.GetInt(variable.name);
                else if (variable is Variable<float> floatVar)
                    floatVar.Value = PlayerPrefs.GetFloat(variable.name);
                else if (variable is Variable<string> stringVar) 
                    stringVar.Value = PlayerPrefs.GetString(variable.name);
                else if (variable is IJsonable ivar)
                    ivar.FromJsonString(PlayerPrefs.GetString(variable.name));
            }
        }

        private void Bind()
        {
            foreach (var variable in variables)
            {
                if (variable is Variable<int> intVar)
                    intVar.Subscribe(value => SaveInt(variable.name, value)).AddTo(this);
                else if (variable is Variable<float> floatVar)
                    floatVar.Subscribe(value => SaveFloat(variable.name, value)).AddTo(this);
                else if (variable is Variable<string> stringVar)
                    stringVar.Subscribe(value => SaveString(variable.name, value)).AddTo(this);
                else if (variable is IJsonable ivar)
                    variable.Subscribe(() => SaveString(variable.name, ivar.ToJsonString())).AddTo(this);
            }
        }

        private void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }
        
        private void SaveFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
        
        private void SaveString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }
}