using Soar.Variables;
using UnityEngine;

namespace Feature.PagedWindow
{
    [CreateAssetMenu(fileName = "PageCounterVariable", menuName = "Kusoge/PageCounterVariable")]
    public class PageCounterVariable : Variable<int>
    {
        public int pageCount;
        [Tooltip("Whether to repeat (loop) or clamp page over the first/last page.")]
        [SerializeField] private bool repeat;

        public override int Value
        {
            get => base.Value;
            set
            {
                var v = value;
                if (repeat)
                    v = (v %= pageCount) < 0 ? v + pageCount : v;
                else
                    v = Mathf.Clamp(value, 0, pageCount - 1);
                base.Value = v;
            }
        }

        public void NextPage() => Value++;
        public void PrevPage() => Value--;
    }
}