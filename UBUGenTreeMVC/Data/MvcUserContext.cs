using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UBUGenTreeMVC.Models;

namespace UBUGenTreeMVC.Data
{
    public class MvcUserContext : DbContext
    {
        public MvcUserContext (DbContextOptions<MvcUserContext> options)
            : base(options)
        {
        }

        public DbSet<UBUGenTreeMVC.Models.Usuario> Usuario { get; set; } = default!;
    }
}
