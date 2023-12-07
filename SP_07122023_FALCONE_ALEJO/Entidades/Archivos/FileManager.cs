using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Entidades.Files
{

    public static class FileManager
    {
        private static string path; 

        static FileManager()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string parcialFolderName = "2023RSP"; 

            FileManager.path = Path.Combine(desktopPath, parcialFolderName); 

            FileManager.ValidaExistenciaDeDirectorio();
        }


        private static void ValidaExistenciaDeDirectorio()
        {
            try
            {
                if (!Directory.Exists(FileManager.path))
                {
                    Directory.CreateDirectory(FileManager.path);
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt");
                throw new FileManagerException("Error al crear el directorio", ex);
            }
        }


        public static void Guardar(string data,string nombreArchivo, bool append=true)
        {
            try
            {
                string filePath = Path.Combine(nombreArchivo, data);
                using (StreamWriter writer = new StreamWriter(filePath, append))
                {
                    writer.Write(data);
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt");
                throw new FileManagerException("Error al guardar el archivo", ex);
            }
        }


        public static bool Serializar<T>(T elemento, string nombreArchivo)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (StreamWriter stream = new StreamWriter(nombreArchivo))
                {
                    xmlSerializer.Serialize(stream, elemento);
                }

                return true;
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt");
                throw new FileManagerException("Error al serializar el objeto", ex);
            }
        }
    }
}
