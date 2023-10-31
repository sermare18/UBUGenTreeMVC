using System;
using Microsoft.EntityFrameworkCore;
using UBUGenTreeMVC.Data;

namespace UBUGenTreeMVC.Models
{
	public static class SeedData
	{
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcUserContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcUserContext>>()))
            {
                // Look for any users.
                if (context.Usuario.Any())
                {
                    return;   // DB has been seeded
                }
                context.Usuario.AddRange(
                    Usuario.CrearAdministrador("Admin", "admin@gmail.com", "test1234"),
                    Usuario.CrearGestor("Gestor", "gestor@gmail.com", "test1234"),
                    Usuario.CrearUsuario("Usuario", "usuario@gmail.com", "test1234")
                );
                context.SaveChanges();
            }
        }
    }
}

