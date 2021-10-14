﻿using Microsoft.EntityFrameworkCore;
using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL
{
    public class ManagmentContext : DbContext
    {
        private IConfig _config;

        public ManagmentContext(IConfig config) : base()
        {
            _config = config;
        }

        public DbSet<PenaltyInfo> Penalties { get; set; }
        public DbSet<OwnedRole> OwnedRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_config.Get().ConnectionString,
                new MySqlServerVersion(new System.Version(5, 7)),
                mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend));
        }
    }
}