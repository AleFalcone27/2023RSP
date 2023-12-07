using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        public static double CalcularCostoIngrediente(this List<EIngrediente> ingredientes, int costoInicial)
        {

            double costoIncrementado = 0;

            foreach (EIngrediente item in ingredientes)
            {
                costoIncrementado = costoInicial + (costoInicial * ((double)item / 100));
            }
            return costoIncrementado;
        }


        public static List<EIngrediente> IngredientesAleatorios(this Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.QUESO,
                EIngrediente.PANCETA,
                EIngrediente.ADHERESO,
                EIngrediente.HUEVO,
                EIngrediente.JAMON,
            };

            int rand2 = rand.Next(1,ingredientes.Count+1);

            return ingredientes.Take(rand2).ToList();
        }
    }
}
