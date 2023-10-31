using System;
using System.ComponentModel.DataAnnotations;

namespace UBUGenTreeMVC.Models
{
    public class AnadirAncestrosViewModel
    {
        public Persona persona { get; set; }

        [Display(Name = "Padre")]
        public Persona padre { get; set; }

        [Display(Name = "Madre")]
        public Persona madre { get; set; }

        public AnadirAncestrosViewModel()
        {
            this.padre = null;
            this.madre = null;
        }

        public AnadirAncestrosViewModel(Persona persona, Persona padre, Persona madre)
        {
            this.persona = persona;
            this.padre = padre ?? new Persona();
            this.madre = madre ?? new Persona();
        }

    }
}

