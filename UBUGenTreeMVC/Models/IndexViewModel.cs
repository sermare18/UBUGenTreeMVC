using System;
namespace UBUGenTreeMVC.Models
{

	public class IndexViewModel
	{
        public List<Persona> Personas { get; set; }

		public IndexViewModel (List<Persona> personasAux)
		{
			Personas = new List<Persona>();

			foreach (Persona p in personasAux)
			{
				Personas.Add(p);
			}
		}
    }

}

