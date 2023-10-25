using System;
using System.ComponentModel.DataAnnotations;

namespace UBUGenTreeMVC.Models
{
    public class AnadirAncestroViewModel
    {
        [Display(Name = "Usuario")]
        public Persona usuario { get; set; }

        [Display(Name = "Padre")]
        public Persona padre { get; set; }

        [Display(Name = "Madre")]
        public Persona madre { get; set; }

        public AnadirAncestroViewModel()
        {
            this.usuario = new Persona();
            this.padre = new Persona();
            this.madre = new Persona();
        }

        public AnadirAncestroViewModel(Persona usuario, Persona padre, Persona madre)
        : this()
        {
            this.usuario = usuario;
            this.padre = padre;
            this.madre = madre;
        }
    }
}

