using Db.Infrastructure.EntityConfiguration;
using Db.Infrastructure.Migrations;
using Db.Infrastructure.Model;
using Db.Infrastructure.Util;
using Microsoft.EntityFrameworkCore;

namespace Db.Infrastructure
{
    public class BaseContext<T> : DbContext where T : DbContext
    {
        private readonly ConnectionModel _connection;

        public BaseContext()
        {
            _connection = new ConnectionModel("localhost,1433", "sa", "P@ssw0rd", "ConfigDb");
        }

        public BaseContext(ConnectionModel connectionModel)
        {
            _connection = connectionModel;
        }

        public BaseContext(DbContextOptions<T> options, ConnectionModel connectionModel)
            : base(options)
        {
            _connection = connectionModel;
        }

        #region DbSets

        public DbSet<VerificationCount> VerificationCounts { get; set; }
        public DbSet<MigrationDataModel> MigrationData { get; set; }
        public DbSet<DataVerificationModel> DataVerifications { get; set; }
        public DbSet<DataVerficiationQueries> DataVerficiationQueries { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VerificationCount>().HasNoKey();
            modelBuilder.ApplyConfiguration(new MigrationDataEC());
            modelBuilder.ApplyConfiguration(new DataVerificationEC());
            modelBuilder.ApplyConfiguration(new DataVerificationQueriesEC());

            InsertData(modelBuilder, _connection.ToString());
            OnBaseModelCreated(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.DefaultTypeMapping<VerificationCount>();
            base.ConfigureConventions(configurationBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                                    => optionsBuilder.UseSqlServer(_connection.ToString(), sqlServerOptions => sqlServerOptions.CommandTimeout(120));

        public virtual void OnBaseModelCreated(ModelBuilder modelBuilder)
        {

        }

        public virtual void InsertData(ModelBuilder modelBuilder, string connection)
        {

        }

        #region EntitySet

        public DbSet<TEntity> EntitySet<TEntity>(string entityName = null) where TEntity : class
        {
            return typeof(TEntity) == typeof(Dictionary<string, object>) ?
                                                        Set<TEntity>(entityName) :
                                                        Set<TEntity>();
        }

        public void EntitySet<TEntity>(TEntity obj, string entityName = null) where TEntity : class
        {
            if (typeof(TEntity) == typeof(Dictionary<string, object>))
            {
                Set<TEntity>(entityName).Add(obj);
            }
            else
            {
                Set<TEntity>().Add(obj);
            }
        }

        public void EntitySetRange<TEntity>(IEnumerable<TEntity> obj, string entityName = null) where TEntity : class
        {
            if (typeof(TEntity) == typeof(Dictionary<string, object>))
            {
                Set<TEntity>(entityName).AddRange(obj);
            }
            else
            {
                Set<TEntity>().AddRange(obj);
            }
        }

        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProgressIndicator.Show();
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                ProgressIndicator.Hide();
            }
        }
    }
}