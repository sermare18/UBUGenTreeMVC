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
        public Nodo? _ancestros
        {
            get
            {
                if (string.IsNullOrEmpty(_ancestrosJSON))
                {
                    return null;
                }
                else
                {
                    // Deserializa _ancestrosJSON a un objeto anónimo
                    var data = JsonConvert.DeserializeObject<dynamic>(_ancestrosJSON);

                    // Crea objetos Persona para la persona, el padre y la madre
                    Persona persona = JsonConvert.DeserializeObject<Persona>(data.persona.ToString());
                    Persona padre = data.padre != null ? JsonConvert.DeserializeObject<Persona>(data.padre.persona.ToString()) : null;
                    Persona madre = data.madre != null ? JsonConvert.DeserializeObject<Persona>(data.madre.persona.ToString()) : null;

                    // Crea el objeto Nodo
                    Nodo ancestros = new Nodo(persona, padre, madre);

                    return ancestros;
                }
            }
            set => _ancestrosJSON = value == null ? null : JsonConvert.SerializeObject(value);
        }

        public Usuario()
        {

        }

        private Usuario(string nombre, string email, string contrasenaHash, Roles rol)
        {
            this._nombre = nombre;
            this._email = email;
            this._contrasenaHash = Utils.Encriptar(contrasenaHash);
            this._rol = rol;
            _ancestros = null;
            _estado = EstadoCuenta.Solicitada; // Los usuarios de nuevo ingreso tienen el estado "Solicitada".
            _ultimoIngreso = DateTime.Now;
            _intentosFallidos = 0;

        }

        public static Usuario CrearAdministrador(string nombre, string email, string contraseñaHash)
        {
            return new Usuario(nombre, email, contraseñaHash, Roles.Administrador);
        }

        public static Usuario CrearGestor(string nombre, string email, string contraseñaHash)
        {
            return new Usuario(nombre, email, contraseñaHash, Roles.Gestor);
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

        public Nodo GetAncestros()
        {
            return this._ancestros;
        }

        public void SetAncestros(Persona usuario, Persona padre, Persona madre)
        {
            this._ancestros = new Nodo(usuario, padre, madre);
            
        }

        public List<Nodo> GetSucesores()
        {
            return this._ancestros.hijos;
        }

     
        public void AddHijo(Persona hijo, Persona padre, Persona madre)
        {
            this._ancestros.hijos.Add(new Nodo(hijo, padre, madre));
        }
      

        public void Ingresar(string contrasena)
        {

            if (this._estado == EstadoCuenta.Solicitada || this._estado == EstadoCuenta.Bloqueada)
            {
                return;
            }

            if (Utils.Encriptar(contrasena) == this._contrasenaHash)
            {
                this._ultimoIngreso = DateTime.Now;
                this._intentosFallidos = 0;

                if ((DateTime.Now - this._ultimoIngreso).TotalDays > 30)
                {
                    this._estado = EstadoCuenta.Inactiva;
                }
                else
                {
                    this._estado = EstadoCuenta.Activa;
                }
            }
            else
            {
                this._intentosFallidos++;

                if (this._intentosFallidos >= 3)
                {
                    this._estado = EstadoCuenta.Bloqueada;

                    // Desbloquear la cuenta después de 10 segundos
                    Task.Delay(10000).ContinueWith(t => this._estado = EstadoCuenta.Activa);
                }
            }
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

