using Entidades.Enumerados;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using Entidades.MetodosDeExtension;
using System.Text;
using Entidades.DataBase;

namespace Entidades.Modelos
{
    public class Hamburguesa : IComestible
    {

        private static int costoBase;
        private bool esDoble;
        private double costo;
        private bool estado;
        private string imagen;
        List<EIngrediente> ingredientes;
        Random random;
        static Hamburguesa() => Hamburguesa.costoBase = 1500;


        public Hamburguesa() : this(false) { }
        public Hamburguesa(bool esDoble)
        {
            this.esDoble = esDoble;
            this.random = new Random();
        }

        public string Ticket => $"{this}\nTotal a pagar:{this.costo}";

        public bool Estado { get { return this.estado; } }

        public string Imagen{ get { return this.imagen; } }

        private void AgregarIngredientes()
        {
             this.ingredientes = this.random.IngredientesAleatorios();
        }

        private string MostrarDatos()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Hamburguesa {(this.esDoble ? "Doble" : "Simple")}");
            stringBuilder.AppendLine("Ingredientes: ");
            this.ingredientes.ForEach(i => stringBuilder.AppendLine(i.ToString()));
            return stringBuilder.ToString();

        }


        public override string ToString() => this.MostrarDatos();


        public void FinalizarPreparacion(string cocinero)
        {

            this.ingredientes.CalcularCostoIngrediente(Hamburguesa.costoBase);
            this.estado = !this.estado;

        }

        public void IniciarPreparacion()
        {
            if (!this.estado)
            {
                // chekear esto
                this.imagen = DataBaseManager.GetImagenComida($"Hamburguesa_{this.random.Next(1, 9)}");
                AgregarIngredientes();
            }

        }
    }
}