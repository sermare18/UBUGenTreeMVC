using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UBUGenTreeMVC.Data;
using UBUGenTreeMVC.Models;

namespace UBUGenTreeMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly MvcUserContext _context;

        public UserController(MvcUserContext context)
        {
            _context = context;
        }

        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("_nombre,_email,_contrasenaHash")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Aquí puedes decidir el rol del usuario basado en tu lógica de negocio
                usuario = Usuario.CrearAdministrador(usuario._nombre, usuario._email, usuario._contrasenaHash);

                // Aquí puedes guardar el usuario en tu base de datos
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return View(usuario);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("_email,_contrasenaHash")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Busca al usuario en la base de datos
                var usuarioDB = await _context.Usuario.FirstOrDefaultAsync(u => u._email == model._email);

                // Si el usuario no existe, añade un error al modelo y devuelve la vista
                if (usuarioDB == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo electrónico no está registrado.");
                    return View(model);
                }

                // Comprueba si la contraseña proporcionada coincide con la almacenada en la base de datos
                if (usuarioDB._contrasenaHash != Utils.Encriptar(model._contrasenaHash))
                {
                    ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                    usuarioDB._intentosFallidos++;

                    // Al administrador no se le puede bloquear la cuenta.
                    if (usuarioDB._intentosFallidos >= 3 && usuarioDB._rol != Roles.Administrador)
                    {
                        usuarioDB._estado = EstadoCuenta.Bloqueada;
                        _context.Update(usuarioDB);
                        await _context.SaveChangesAsync();
                        ModelState.AddModelError(string.Empty, string.Format("Estado de la cuenta: {0}", usuarioDB._estado.ToString()));
                    }

                    _context.Update(usuarioDB);
                    await _context.SaveChangesAsync();

                    return View(model);

                }

                if (usuarioDB._estado == EstadoCuenta.Solicitada || usuarioDB._estado == EstadoCuenta.Bloqueada)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Estado de la cuenta: {0}", usuarioDB._estado.ToString()));
                    return View(model);
                }

                // Si el correo electrónico y la contraseña son correctos, inicia sesión y redirige al usuario
                HttpContext.Session.SetString("UsuarioId", usuarioDB._id.ToString());
                HttpContext.Session.SetString("Rol", usuarioDB._rol.ToString());
                usuarioDB._intentosFallidos = 0;
                _context.Update(usuarioDB);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            // Borra todos los datos de la sesión
            HttpContext.Session.Clear();

            // Redirige al usuario a la página de inicio o de inicio de sesión
            return RedirectToAction("Index", "Home");
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }

            int idUsuarioInt;
            if (Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                // Console.WriteLine(usuarioDB._ancestros);
                // Console.WriteLine(usuarioDB._ancestrosJSON);

                IndexViewModel model;

                if (usuarioDB._ancestros.Count > 0)
                {
                    model = new IndexViewModel(usuarioDB._ancestros);
                }
                else
                {
                    model = null;
                }

                return View(model);
               
            }
            else
            {
                // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                return RedirectToAction("Login", "User");
            }

        }

        // GET: User/TablaUsuarios
        public async Task<IActionResult> GestionarUsuarios ()
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }

            int idUsuarioInt;
            if (Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                if (usuarioDB._rol != Roles.Administrador)
                {
                    return Redirect(nameof(Index));
                }

                var usuariosDB = await _context.Usuario.ToListAsync();

                return View(nameof(GestionarUsuarios), usuariosDB);


            }
            else
            {
                // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                return RedirectToAction("Login", "User");
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "User");
            }

            int idUsuarioInt;
            if(Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                if (usuarioDB._ancestrosJSON != null)
                {
                    var data = JsonConvert.DeserializeObject<List<UBUGenTreeMVC.Models.Persona>>(usuarioDB._ancestrosJSON);

                    Persona personaSeleccionada = null;

                    foreach (var persona in data)
                    {
                        if (persona._id == id)
                        {
                            personaSeleccionada = persona;
                            break;
                        }
                    }

                    return View(nameof(Details), personaSeleccionada);
                }

                return View(nameof(Index));
                
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        // GET: User/AñadirAncestro
        public async Task<IActionResult> AnadirAncestros(int? id)
        {
            Console.WriteLine("desde GET AnadirAncestros");
            Console.WriteLine(id);

            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }

            AnadirAncestrosViewModel model;

            // Comprobamos si esa persona que tiene registrada el usuario ya tiene definido ancestros
            int idUsuarioInt;
            if (Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                Persona persona = null;

                foreach (Persona p in usuarioDB._ancestros)
                {
                    if (p._id == (int)id)
                    {
                        persona = p;
                        break;
                    }
                }

                (int? A1_id, int? A2_id) = usuarioDB.GetIdsAncestros((int)id);

                if (A1_id != null && A2_id != null)
                {
                    Persona padre = null;
                    Persona madre = null;

                    foreach (Persona p in usuarioDB._ancestros)
                    {
                        if (p._id == A1_id)
                        {
                            padre = p;
                        } else if (p._id == A2_id)
                        {
                            madre = p;
                        }
                    }

                    model = new AnadirAncestrosViewModel(persona, padre, madre);
                    return View(nameof(AnadirAncestros), model);

                }
                else
                {
                    model = new AnadirAncestrosViewModel(persona, null, null);
                    return View(nameof(AnadirAncestros), model);
                }

            }
            else
            {
                // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                return RedirectToAction("Login", "User");
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnadirAncestros(AnadirAncestrosViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = HttpContext.Session.GetString("UsuarioId");

                if (idUsuario == null)
                {
                    // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                    return RedirectToAction("Login", "User");
                }

                int idUsuarioInt;
                if (Int32.TryParse(idUsuario, out idUsuarioInt))
                {
                    var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                    if (usuarioDB == null)
                    {
                        return NotFound();
                    }

                    usuarioDB.AnadirAncestros(model.persona, model.padre, model.madre);
                    _context.Update(usuarioDB);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                }
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return View(model);

        }
        

        // GET: User/AñadirPrimeraPersona
        public IActionResult AnadirPrimeraPersona()
        {
            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }
            return View(nameof(AnadirPrimeraPersona));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnadirPrimeraPersona(Persona persona)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = HttpContext.Session.GetString("UsuarioId");

                if (idUsuario == null)
                {
                    // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                    return RedirectToAction("Login", "User");
                }

                int idUsuarioInt;
                if (Int32.TryParse(idUsuario, out idUsuarioInt))
                {
                    var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                    if (usuarioDB == null)
                    {
                        return NotFound();
                    }

                    usuarioDB.SetPrimeraPersona(persona);

                    _context.Update(usuarioDB);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                }
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return View(persona);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivarCuenta(int? id)
        {
           
            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }

            int idUsuarioInt;
            if (Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                // Busca al usuario en la base de datos
                var usuarioAValidar = await _context.Usuario.FirstOrDefaultAsync(u => u._id == id);

                usuarioDB.ValidarCuenta(usuarioAValidar);

                _context.Update(usuarioAValidar);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(GestionarUsuarios));
            }
            else
            {
                // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                return RedirectToAction(nameof(Login));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarCuenta(int? id)
        {

            var idUsuario = HttpContext.Session.GetString("UsuarioId");

            if (idUsuario == null)
            {
                // El usuario no está autenticado, redirígelo a la página de inicio de sesión.
                return RedirectToAction("Login", "User");
            }

            int idUsuarioInt;
            if (Int32.TryParse(idUsuario, out idUsuarioInt))
            {
                var usuarioDB = await _context.Usuario.FindAsync(idUsuarioInt);

                if (usuarioDB == null)
                {
                    return NotFound();
                }

                // Busca al usuario en la base de datos
                var usuarioABloquear = await _context.Usuario.FirstOrDefaultAsync(u => u._id == id);

                usuarioDB.BloquearCuenta(usuarioABloquear);

                _context.Update(usuarioABloquear);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(GestionarUsuarios));
            }
            else
            {
                // No se pudo convertir idUsuario a int. Maneja este error como consideres apropiado.
                return RedirectToAction(nameof(Login));
            }

        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id,_nombre,_email,_contrasenaHash,_rol,_ancestrosJSON,_estado,_ultimoIngreso,_intentosFallidos")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id,_nombre,_email,_contrasenaHash,_rol,_ancestrosJSON,_estado,_ultimoIngreso,_intentosFallidos")] Usuario usuario)
        {
            if (id != usuario._id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario._id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m._id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuario == null)
            {
                return Problem("Entity set 'MvcUserContext.Usuario'  is null.");
            }
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuario?.Any(e => e._id == id)).GetValueOrDefault();
        }
    }
}
