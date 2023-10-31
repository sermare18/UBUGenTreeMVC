using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


/*
 * Crear vista Administrador que pueda activar cuentas de usuarios previamente registrados y las demás funciones de los otros roles.
 * Crear vista Gestor puede crear personas y borrar y visualizar
 * El usuario solo visualiza  las personas que crea el gestor o el administrador
 */

namespace UBUGenTreeMVC.Models
{
    public enum Roles
    {
        Administrador,
        Gestor,
        Usuario
    }

    public enum EstadoCuenta
    {
        Solicitada,
        Activa,
        Inactiva,
        Bloqueada
    }

    public class Usuario
    {
        [Key]
        public int _id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string _nombre { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string _email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string _contrasenaHash { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public Roles _rol { get; set; }

        public string? _ancestrosJSON { get; set; }

        [Required]
        [Display(Name = "Estado cuenta")]
        public EstadoCuenta _estado { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Último ingreso")]
        public DateTime _ultimoIngreso { get; set; }

        [Required]
        public int _intentosFallidos { get; set; }

        [NotMapped]
        public List<Persona>? _ancestros
        {
            get
            {
                if (string.IsNullOrEmpty(_ancestrosJSON))
                {
                    return null;
                }
                else
                {
                    // Deserializa _ancestrosJSON a una lista de objetos Persona
                    List<Persona> ancestros = JsonConvert.DeserializeObject<List<Persona>>(_ancestrosJSON);
                    return ancestros;
                }
            }
            set => _ancestrosJSON = value == null ? null : JsonConvert.SerializeObject(value);
        }

        public Usuario()
        {
            _ancestros = new List<Persona>();
            _ancestrosJSON = JsonConvert.SerializeObject(_ancestros);
        }

        private Usuario(string nombre, string email, string contrasenaHash, Roles rol, EstadoCuenta estado = EstadoCuenta.Solicitada)
        {
            this._nombre = nombre;
            this._email = email;
            this._contrasenaHash = Utils.Encriptar(contrasenaHash);
            this._rol = rol;
            _ancestros = new List<Persona>();
            _estado = estado; // Los usuarios de nuevo ingreso tienen el estado "Solicitada".
            _ultimoIngreso = DateTime.Now;
            _intentosFallidos = 0;
            _ancestrosJSON = JsonConvert.SerializeObject(_ancestros);

        }

        public static Usuario CrearAdministrador(string nombre, string email, string contraseñaHash)
        {
            return new Usuario(nombre, email, contraseñaHash, Roles.Administrador, EstadoCuenta.Activa);
        }

        public static Usuario CrearGestor(string nombre, string email, string contraseñaHash)
        {
            return new Usuario(nombre, email, contraseñaHash, Roles.Gestor, EstadoCuenta.Activa);
        }

        public static Usuario CrearUsuario(string nombre, string email, string contraseñaHash)
        {
            return new Usuario(nombre, email, contraseñaHash, Roles.Usuario);
        }

        public override string ToString()
        {
            return $"ID: {_id}, Nombre: {_nombre}, Email: {_email}, Rol: {_rol}, Estado de la cuenta: {_estado}, Último ingreso: {_ultimoIngreso}, Intentos fallidos: {_intentosFallidos}";
        }

        public Usuario GetUsuario()
        {
            return this;
        }

        public List<Persona> GetAncestros()
        {
            return this._ancestros;
        }

        public void SetAncestros(Persona usuario, Persona padre, Persona madre)
        {
            // Asegúrate de que _ancestros está inicializado
            /*
            if (_ancestros == null)
            {
                _ancestros = new List<Persona>();
            }

            Console.WriteLine(usuario is Persona);
            Console.WriteLine(usuario is AnadirAncestrosViewModel);

            this._ancestros.Add((Persona)usuario);
            this._ancestros.Add((Persona)padre);
            this._ancestros.Add((Persona)madre);
            */

            List<Persona> debugList = new List<Persona>();
            debugList.Add(usuario);
            debugList.Add(padre);
            debugList.Add(madre);

            // Serializa la lista completa de ancestros a JSON
            this._ancestrosJSON = JsonConvert.SerializeObject(debugList);

        }

        public void AnadirAncestros(Persona target, Persona padre, Persona madre)
        {
            // Deserializa _ancestrosJSON a una lista de objetos Persona
            var ancestrosCopia = JsonConvert.DeserializeObject<List<Persona>>(_ancestrosJSON);

            ancestrosCopia.Add(padre);
            ancestrosCopia.Add(madre);

            foreach (Persona p in ancestrosCopia)
            {
                if (p.Nombre == target.Nombre &&
                    p.Apellido1 == target.Apellido1 &&
                    p.Apellido2 == target.Apellido2 &&
                    p.Localidad == target.Localidad &&
                    p.FechaNac == target.FechaNac)
                {
                    p.A1_id = padre._id;
                    p.A2_id = madre._id;
                    break;
                }
            }

            this._ancestrosJSON = JsonConvert.SerializeObject(ancestrosCopia);

        }

        public void SetPrimeraPersona(Persona persona)
        {
            // Deserializa _ancestrosJSON a una lista de objetos Persona
            _ancestros = JsonConvert.DeserializeObject<List<Persona>>(_ancestrosJSON);

            // Asegúrate de que _ancestros está inicializado
            if (_ancestros == null)
            {
                _ancestros = new List<Persona>();
            }

            // Añade los objetos Persona a la lista
            List<Persona> ancestros = new List<Persona>();
            ancestros.Add(persona);

            // Serializa la lista completa de ancestros a JSON
            this._ancestrosJSON = JsonConvert.SerializeObject(ancestros);
        }

        public (int?, int?) GetIdsAncestros(int persona_id)
        {
            foreach (Persona persona in _ancestros)
            {
                if (persona._id == persona_id)
                {
                    return (persona.A1_id, persona.A2_id);
                }
            }
            return (null, null);
        }

        public void ValidarCuenta(Usuario usuario)
        {
            if (this._rol == Roles.Administrador)
            {
                usuario._estado = EstadoCuenta.Activa;
            }
        }

        public void BloquearCuenta(Usuario usuario)
        {
            if (this._rol == Roles.Administrador)
            {
                usuario._estado = EstadoCuenta.Bloqueada;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Usuario usuario &&
                   this._id == usuario._id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id);
        }

    }
}

