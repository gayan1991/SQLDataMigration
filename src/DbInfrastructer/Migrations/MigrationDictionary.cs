using Db.Infrastructure.Model;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Db.Infrastructure.Migrations
{
    public class MigrationDictionary
    {
        private readonly List<MigrationEntityMetadata> _migrationEntityMetadata = new();

        public MigrationEntityMetadata this[IEntityType key]
        {
            get => _migrationEntityMetadata.First(x => x.EntityType == key);
        }

        public IReadOnlyList<MigrationEntityMetadata> Values => _migrationEntityMetadata.OrderBy(x => x.ReferenceImportanceCount).ToList();

        public ICollection<IEntityType> Keys => _migrationEntityMetadata.Select(x => x.EntityType).ToList();

        public int Count => _migrationEntityMetadata.Count;

        public void Add(IEntityType entity)
        {
            if (Contains(entity))
                return;

            _migrationEntityMetadata.Add(new MigrationEntityMetadata(entity));
        }

        public void Add(MigrationEntityMetadata metadata)
        {
            _migrationEntityMetadata.Add(metadata);
        }

        public IReadOnlyList<MigrationEntityMetadata> GetValues(bool ascendingOrder = true)
        {
            return ascendingOrder ?
                        _migrationEntityMetadata.OrderBy(x => x.ReferenceImportanceCount).ToList() :
                        _migrationEntityMetadata.OrderByDescending(x => x.ReferenceImportanceCount).ToList();
        }

        public void Clear()
        {
            _migrationEntityMetadata.Clear();
        }

        public bool Contains(IEntityType entity)
        {
            return _migrationEntityMetadata.Any(x => x.EntityType == entity);
        }

        public bool Remove(IEntityType entity)
        {
            var obj = _migrationEntityMetadata.FirstOrDefault(x => x.EntityType == entity);
            if (obj is null)
            {
                return false;
            }
            return _migrationEntityMetadata.Remove(obj);
        }
    }
}
