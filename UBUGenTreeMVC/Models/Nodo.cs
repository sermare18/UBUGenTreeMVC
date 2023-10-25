using System;
using System.Text;

namespace UBUGenTreeMVC.Models
{
    public class Nodo
    {
        public Persona persona { get; set; }
        public Nodo padre { get; set; }
        public Nodo madre { get; set; }
        public List<Nodo> hijos { get; set; }

        public Nodo(Persona persona, Persona padre, Persona madre)
        {
            this.persona = persona;
            this.hijos = new List<Nodo>();

            if (padre != null)
            {
                this.padre = new Nodo(padre, null, null);
            }

            if (madre != null)
            {
                this.madre = new Nodo(madre, null, null);
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"Persona: {persona.ToString()}");

            if (padre != null)
            {
                result.AppendLine($"Padre: {padre.persona.ToString()}");
            }

            if (madre != null)
            {
                result.AppendLine($"Madre: {madre.persona.ToString()}");
            }

            if (hijos.Count > 0)
            {
                result.AppendLine("Hijos:");
                foreach (var hijo in hijos)
                {
                    result.AppendLine(hijo.persona.ToString());
                }
            }

            return result.ToString();
        }

    }
}

