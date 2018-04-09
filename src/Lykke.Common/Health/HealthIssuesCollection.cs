using System.Collections;
using System.Collections.Generic;

namespace Lykke.Common.Health
{
    public class HealthIssuesCollection : IReadOnlyCollection<HealthIssue>
    {
        private readonly List<HealthIssue> _list;
        

        public HealthIssuesCollection()
        {
            _list = new List<HealthIssue>();
        }


        public int Count 
            => _list.Count;

        
        public void Add(string type, string value)
        {
            _list.Add(HealthIssue.Create(type, value));
        }

        public IEnumerator<HealthIssue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
