using System;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Feature.SplashScreen
{
    public class SplashTextAnimation : MonoBehaviour
    {
        private RectTransform[] charTransforms;

        private static readonly Vector2 MinMaxDelta = new(-5f, 5f);
        private const float RotationMultiplier = 1.6f;

        private IDisposable subscription;

        private void Awake()
        {
            charTransforms = GetComponentsInChildren<TMP_Text>()
                .Select(text => text.GetComponent<RectTransform>())
                .ToArray();
        }

        private void Start()
        {
            subscription = Observable.Interval(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => RandomizePosition());
        }

        private void RandomizePosition()
        {
            foreach (var charTransform in charTransforms)
            {
                var pos = charTransform.anchoredPosition;
                    pos.x = Random.Range(MinMaxDelta.x, MinMaxDelta.y);
                    pos.y = Random.Range(MinMaxDelta.x, MinMaxDelta.y);

                var rot = charTransform.eulerAngles;
                    rot.z = Random.Range(MinMaxDelta.x, MinMaxDelta.y) * RotationMultiplier;

                charTransform.anchoredPosition = pos;
                charTransform.eulerAngles = rot;
            }
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}