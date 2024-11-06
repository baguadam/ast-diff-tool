using ASTDiffTool.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTDiffTool.Services
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Node> Nodes {  get; set; }
        public DbSet<Edge> Edges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the primary key for the Node entity
            _ = modelBuilder.Entity<Node>()
                .HasKey(n => n.Id);

            // Configure the composite primary key for the Edge entity
            _= modelBuilder.Entity<Edge>()
                .HasKey(e => new { e.ChildId, e.ParentId });

            // Configure the foreign key relationships for the Edge entity
            _ = modelBuilder.Entity<Edge>()
                .HasOne(e => e.ParentNode)
                .WithMany(n => n.ParentEdges)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            _ = modelBuilder.Entity<Edge>()
                .HasOne(e => e.ChildNode)
                .WithMany(n => n.ChildEdges)
                .HasForeignKey(e => e.ChildId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}
