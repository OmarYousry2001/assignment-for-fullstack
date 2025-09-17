using Domains.Entities.Identity;
using Domains.Identity;
using EcommerceAPI.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.ApplicationContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser , Role, string>
    {
        private readonly IEncryptionProvider _encryptionProvider;
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public DbSet<Product> Products { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _encryptionProvider = new GenerateEncryptionProvider("6326131c78014aa894a4daeee1a32276");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.UseEncryption(_encryptionProvider);

            #region Order Confgiration

            //modelBuilder.Entity<Orders>().OwnsOne(x => x.shippingAddress, sa =>
            //{
            //    sa.Property(a => a.FirstName).HasColumnName("ShippingFirstName");
            //    sa.Property(a => a.LastName).HasColumnName("ShippingLastName");
            //    sa.Property(a => a.City).HasColumnName("ShippingCity");
            //    sa.Property(a => a.ZipCode).HasColumnName("ShippingZipCode");
            //    sa.Property(a => a.Street).HasColumnName("ShippingStreet");
            //    sa.Property(a => a.State).HasColumnName("ShippingState");
            //});

            //modelBuilder.Entity<Orders>()
            //    .HasMany(x => x.orderItems)
            //    .WithOne()
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Orders>()
            //    .Property(x => x.status)
            //    .HasConversion(
            //        o => o.ToString(),
            //        o => (Status)Enum.Parse(typeof(Status), o)
            //    ); 
            #endregion


            //modelBuilder.Entity<VwBook>(entity =>
            //{
            //    entity.HasNoKey();
            //    entity.ToView("VwBook");
            //});

            //modelBuilder.Entity<TbRefreshToken>(entity =>
            //{
            //    entity.HasKey(e => e.Id);

            //    entity.Property(e => e.Token)
            //          .IsRequired();

            //    entity.Property(e => e.ExpiresAt)
            //          .IsRequired();

            //    entity.Property(e => e.CurrentState)
            //          .HasDefaultValue(1);


            //});

        }
    }

}
