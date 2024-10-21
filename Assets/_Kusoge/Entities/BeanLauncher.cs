using System;
using System.Collections.Generic;

namespace Kusoge.Entities
{
    [Serializable]
    public class BeanLauncher
    {
        public int launchRate = 2;
        
        private readonly Dictionary<int, Bean> beans = new();
        private readonly Random rnd = new();
        
        public int LaunchedBeanCount => beans.Count;
        public int LaunchDelay => 1000 / launchRate;

        public void Initialize()
        {
            Bean.id = 0;
            beans.Clear();
        }
        
        public Bean LaunchBean()
        {
            var rndVal = rnd.Next(0, 3) - 1;
            var bean = new Bean((Player.DirectionEnum)rndVal);
            beans.Add(bean.Id, bean);
            return bean;
        }
        
        public bool TryGetBean(int id, out Bean bean)
        {
            return beans.TryGetValue(id, out bean);
        }

        public void RemoveBean(int id)
        {
            beans.Remove(id);
        }
    }
}