namespace Data_Collector_Server.File_Helper
{
    internal class File_Writter
    {
        public static int Write_File(string File_Path, byte[] Data)
        {
            int Data_Length;
            FileMode mode = FileMode.Append;
            if (!File.Exists(File_Path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(File_Path));
                mode = FileMode.Create;
            }
            using (FileStream f = new FileStream(File_Path, mode))
            {
                f.Write(Data);
                Data_Length = (int)f.Length;
                f.Close();
            }
            return Data_Length;
        }
        public static int Rollback_File(string File_Path, int Length)
        {
            try
            {
                if (!File.Exists(File_Path))
                    return 0;
                int Data_Length;
                byte[] original_data = null;
                using (FileStream f = new FileStream(File_Path, FileMode.Open, FileAccess.Read))
                {
                    Data_Length = (int)f.Length - Length;
                    if (Data_Length < 0)
                        Data_Length = 0;
                    if (Data_Length > 0)
                    {

                        original_data = new byte[Data_Length];
                        f.Read(original_data, 0, Data_Length);
                        f.Close();
                    }
                }
                File.Delete(File_Path);
                if (Data_Length > 0)
                {
                    using (FileStream f = new FileStream(File_Path, FileMode.Create, FileAccess.Write))
                    {
                        f.Write(original_data, 0, Data_Length);
                        f.Close();
                    }
                }
                return Data_Length;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
