namespace Db.Infrastructure.Migrations
{
    public class VerificationQueryModel
    {
        private readonly Dictionary<int, int> _results = new();
        private Dictionary<int, string> _queries { get; set; } = new();

        public List<int> Keys => _queries.Select(x => x.Key).ToList();

        public int this[int key]
        {
            get => _results[key];
            set
            {
                if (_results.ContainsKey(key))
                {
                    _results[key] = value;
                }
                else
                {
                    _results.Add(key, value);
                }
            }
        }

        public string GetQueury(int key) => _queries[key];

        public VerificationQueryModel AddQueryDictionary(Dictionary<int, string> queries)
        {
            _queries = queries;
            return this;
        }
    }
}
