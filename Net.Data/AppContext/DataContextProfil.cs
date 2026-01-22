using Net.Business.Entities.Profil;
using Microsoft.EntityFrameworkCore;
namespace Net.Data.AppContext
{
    public class DataContextProfil : DbContext
    {
        public DataContextProfil(DbContextOptions<DataContextProfil> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pesaje1Entity>().HasKey(p => new { p.RECORDKEY, p.LineNum });
            modelBuilder.Entity<Pesaje1Entity>().HasOne(p => p.Pesaje).WithMany(c => c.Pesaje1).HasForeignKey(p => p.RECORDKEY);
        }

        public DbSet<PesajeEntity> Pesaje { get; set; }
        public DbSet<Pesaje1Entity> Pesaje1 { get; set; }
    }
}
