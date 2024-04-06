namespace Data_Collector_Server.File_Helper
{
    internal class Path_Register
    {
        private static Dictionary<ulong, string> path_table = new Dictionary<ulong, string>();
        private static Dictionary<ulong, uint> file_size_table = new Dictionary<ulong, uint>();
        private static List<ulong> timestamp_list = new List<ulong>();
        public static void Register_Path(ulong Timestamp, uint File_Size, string Path)
        {
            lock (file_size_table)
                lock (path_table)
                {
                    if (path_table.ContainsKey(Timestamp))
                    {
                        path_table.Remove(Timestamp);
                        file_size_table.Remove(Timestamp);
                        timestamp_list.Remove(Timestamp);
                    }
                    path_table.Add(Timestamp, Path);
                    file_size_table.Add(Timestamp, File_Size);
                    timestamp_list.Add(Timestamp);
                    if (timestamp_list.Count > 100)
                    {
                        path_table.Remove(timestamp_list[0]);
                        file_size_table.Remove(timestamp_list[0]);
                        timestamp_list.RemoveAt(0);
                    }
                }
        }
        public static string? Get_Path(ulong Timestamp)
        {
            lock (path_table)
            {
                if (!path_table.ContainsKey(Timestamp))
                    return null;
                return path_table[Timestamp];
            }
        }
        public static uint? Get_File_Size(ulong Timestamp)
        {
            lock (file_size_table)
            {
                if (!file_size_table.ContainsKey(Timestamp))
                    return null;
                return file_size_table[Timestamp];
            }
        }
    }
}
