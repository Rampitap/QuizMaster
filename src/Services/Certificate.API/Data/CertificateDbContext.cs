using Certificate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Certificate.API.Data;

public class CertificateDbContext : DbContext
{
    public CertificateDbContext(DbContextOptions<CertificateDbContext> options) : base(options)
    {
    }

    public DbSet<CertificateMetaData> Certificates => Set<CertificateMetaData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CertificateMetaData>().Property(x => x.UserId).IsRequired();
    }
}
