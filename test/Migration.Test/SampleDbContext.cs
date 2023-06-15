using Db.Infrastructure;
using Db.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Migration.Test.Models;

namespace Migration.Test
{
    public partial class SampleDbContext : BaseContext<SampleDbContext>
    {
        public SampleDbContext() : base()
        {

        }

        public SampleDbContext(ConnectionModel connectionModel) : base(connectionModel)
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options, ConnectionModel connectionModel)
            : base(options, connectionModel)
        {
        }

        public virtual DbSet<EntityOne> EntityOnes { get; set; }

        public virtual DbSet<EntityTwo> EntityTwos { get; set; }

        public override void OnBaseModelCreated(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityOne>(entity =>
            {
                entity.ToTable("EntityOne");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EntityTwo>(entity =>
            {
                entity.ToTable("EntityTwo");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        public override void InsertData(ModelBuilder modelBuilder, string connection)
        {
            base.InsertData(modelBuilder, connection);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
