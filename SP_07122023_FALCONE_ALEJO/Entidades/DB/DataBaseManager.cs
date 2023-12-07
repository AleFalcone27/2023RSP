using System.Data.SqlClient;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static SqlConnection connection = new SqlConnection();
        private static string stringConnection = "Server = ALE; Database = 20230622SP; Trusted_Connection = True;";

        public static string GetImagenComida(string tipo)
        {
            using (SqlConnection connection = new SqlConnection(DataBaseManager.stringConnection))
            {
                try
                {
                    string query = "SELECT Imagen FROM comidas WHERE Tipo = @tipo";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tipo", tipo);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        string imagen = reader.GetString(0);

                        return imagen;
                    }
                    else
                    {
                        FileManager.Guardar("Tipo de comida no existente", "logs.txt");
                        throw new ComidaInvalidaExeption("Tipo de comida no existente");
                    }
                }
                catch (ComidaInvalidaExeption ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    FileManager.Guardar(ex.Message, "logs.txt");
                    throw new DataBaseManagerException("Error al leer la Base de datos", ex);
                }
            }
        }


        // CHEKEAR ESTA
        public static void GuardarTicket<T>(string nombreCliente, T comida) where T : IComestible, new()
        {
            if (comida == null)
            {
                FileManager.Guardar("La comida no puede ser nula", "logs.txt");
                throw new ArgumentNullException(nameof(comida), "La comida no puede ser nula");
            }

            if (string.IsNullOrEmpty(nombreCliente))
            {
                FileManager.Guardar("La comida no puede ser nula", "logs.txt");
                throw new ArgumentException("El nombre del cliente no puede estar vacío", nameof(nombreCliente));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(stringConnection))
                {
                    string query = "INSERT INTO Tickets (NombreEmpleado, TicketComida) VALUES (@NombreEmpleado, @TicketComida)";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@NombreEmpleado", nombreCliente);
                    command.Parameters.AddWithValue("@TicketComida", comida.ToString()); // Suponiendo que el ticket es una representación de cadena de la comida

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt");
                throw new DataBaseManagerException("Error al escribir el ticket en la base de datos", ex);
            }
        }

    }
}
