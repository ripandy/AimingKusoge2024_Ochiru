using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.PagedWindow
{
    [RequireComponent(typeof(ToggleGroup))]
    public class BreadcrumbHandler : MonoBehaviour
    {
        [SerializeField] private PageCounterVariable pageCounter;
        [SerializeField] private GameObject breadcrumbPrefab;

        private readonly List<Toggle> toggles = new();
        private ToggleGroup toggleGroup;
        
        private IDisposable subscription;

        private void Awake()
        {
            TryGetComponent(out toggleGroup);
            GenerateBreadcrumbs();
        }

        private void Start()
        {
            subscription = pageCounter.Subscribe(UpdateBreadcrumb);
        }
        
        private void GenerateBreadcrumbs()
        {
            toggles.Clear();
            for (var i = 0; i < pageCounter.pageCount; i++)
            {
                var breadcrumb = Instantiate(breadcrumbPrefab, transform);
                var toggle = breadcrumb.GetComponent<Toggle>();
                var page = i;
                
                toggle.SetIsOnWithoutNotify(i == pageCounter.Value);
                toggle.OnValueChangedAsObservable()
                    .Where(isOn => isOn)
                    .Subscribe(_ => pageCounter.Value = page)
                    .AddTo(this);

                toggle.group = toggleGroup;
                
                toggles.Add(toggle);
            }
        }
        
        private void UpdateBreadcrumb(int page)
        {
            toggles[page].SetIsOnWithoutNotify(true);
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}