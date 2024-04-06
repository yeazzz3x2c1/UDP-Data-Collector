namespace Data_Collector_Server.Packets
{
    public enum Packets_Index
    {
        //CMDs:
        /// <summary>
        /// 設定資料請求
        /// <para>樹梅派傳來: [Index, CMD, CRC]</para> 
        /// <para>Response: [Index, CMD, 每個封包最長長度(4Bytes), 逾時時間(ms;2Bytes), CRC]</para>
        /// </summary>
        Server_Setting = 0,

        /// <summary>
        /// 建立檔案路徑
        /// <para>樹梅派傳來: [Index, CMD, Timestamp(File_Index;8Bytes), Path(2Bytes), Data, CRC]</para> 
        /// <para>Response: [Index, CMD, Timestamp, CRC]</para>
        /// </summary>
        Create_File_Path = 1,

        /// <summary>
        /// 上傳檔案片段
        /// <para>樹梅派傳來: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Data, CRC]</para> 
        /// <para>Response: [Index, CMD, Timestamp, 目前總資料長度(4Bytes), CRC]</para> 
        /// </summary>
        Write_File_Slice = 2,

        /// <summary>
        /// 刪除檔案片段
        /// <para>樹梅派傳來: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Data, CRC]</para> 
        /// <para>Response: [Index, CMD, Timestamp, 目前總資料長度(4Bytes), CRC]</para> 
        /// </summary>
        Remove_File_Slice = 3
    }
}
