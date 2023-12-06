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
        private static string stringConnection = "Server = COMPLETAR; Database = 20230622SP;Trusted_Connection = True;";




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

                throw new DataBaseManagerException("Error al guardar el ticket en la base de datos", ex);
                // guardar en lgs

            }
        }









    }



    public static bool GetAndInitializeProducts()
    {
        using (SqlConnection connection = new SqlConnection(GestorSql.ConnectionString))
        {
            try
            {
                string query = "SELECT * FROM Productos";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string nombre = reader.GetString(1);
                        string? ingredientes = reader.GetString(2);
                        double precio = reader.GetDouble(3);
                        string condimentos = reader.GetString(4);
                        bool vegano = reader.GetBoolean(5);

                        Producto producto = new Producto(nombre, ingredientes, precio, condimentos, vegano);

                        Producto.Productos.Add(producto);
                    }
                    return true;
                }
            }

            catch (Exception ex)
            {
                throw new ErrorDeConexionException("Error de conexión a la Base de datos");
            }
        }
        return false;
    }


}
