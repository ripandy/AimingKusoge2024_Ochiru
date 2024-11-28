using System;
using System.Collections.Generic;
using Soar.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.PagedWindow
{
    public class PagedWindowHandler : MonoBehaviour
    {
        [SerializeField] private Collection<Sprite> spriteCollection;
        [SerializeField] private PageCounterVariable pageCounter;
        [SerializeField] private Image pagePrefab;
        [SerializeField] private Transform container;

        private readonly List<GameObject> pageContainers = new();
        
        private IDisposable subscription;

        private void Awake()
        {
            var pageCount = spriteCollection.Count;
            pageContainers.Clear();
            pageCounter.pageCount = pageCount;
            for (var i = 0; i < pageCount; i++)
            {
                var page = Instantiate(pagePrefab, container);
                    page.sprite = spriteCollection[i];
                var go = page.gameObject;
                pageContainers.Add(go);
                go.SetActive(i == pageCounter.Value);
            }
        }
        
        private void Start()
        {
            subscription = pageCounter.Subscribe(SetActivePage);
        }

        private void SetActivePage(int page)
        {
            for (var i = 0; i < pageContainers.Count; i++)
            {
                pageContainers[i].SetActive(i == page);
            }
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}