using System;
namespace UBUGenTreeMVC.Models
{
	public class IndexViewModel
	{
		public Persona persona { get; set; }
		public Persona padre { get; set; }
		public Persona madre { get; set; }

        public IndexViewModel(Persona persona, Persona padre, Persona madre)
        {
			this.persona = persona;
			this.padre = padre;
			this.madre = madre;
        }
    }

}

