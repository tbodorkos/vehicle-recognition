namespace Core.Utils
{
    public static class FileWriter
    {
        public static void Delete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static void Save(byte[] bytes, string location)
        {
            var fileStream = new FileStream(location, FileMode.Create);
            var binaryWriter = new BinaryWriter(fileStream);
            
            try
            {
                binaryWriter.Write(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
                binaryWriter.Close();
            }
        }
    }
}
