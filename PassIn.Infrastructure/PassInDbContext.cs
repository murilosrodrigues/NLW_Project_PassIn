﻿using Microsoft.EntityFrameworkCore;
using PassIn.Infrastructure.Entities;

namespace PassIn.Infrastructure;

public class PassInDbContext : DbContext 
{
    public DbSet<Event> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source= C:\\Users\\muril\\OneDrive\\Documentos\\projetos\\Rocketseat\\C#\\PassIn\\PassInDb.db");
    }
}