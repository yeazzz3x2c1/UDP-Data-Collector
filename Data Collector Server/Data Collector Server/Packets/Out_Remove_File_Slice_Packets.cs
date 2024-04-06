using Data_Collector_Server.File_Helper;

namespace Data_Collector_Server.Packets
{
    internal class Out_Remove_File_Slice_Packets : Packets_Base
    {
        private ulong Timestamp = 0;
        private uint Data_Length = 0;
        //CMDs:
        //3. 刪除檔案片段: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(4Bytes), Data, CRC]
        //Response: [Index, CMD, Timestamp, 目前總資料長度(4Bytes), CRC]

        public Out_Remove_File_Slice_Packets(byte Index, ulong Timestamp, uint Total_Length)
        {
            byte[] Data = new byte[12];
            int i = 0;
            for (int j = 0; j < 8; j++)
            {
                Data[i++] = (byte)(Timestamp & 0xff);
                Timestamp >>= 8;
            }
            for (int j = 0; j < 4; j++)
            {
                Data[i++] = (byte)(Total_Length & 0xff);
                Total_Length >>= 8;
            }
            Encode_Data(Index, (byte)Packets_Index.Remove_File_Slice, Data);
        }

        public Out_Remove_File_Slice_Packets(byte[] Package_Data) : base(Package_Data)
        {
            int i = 0;
            i += 8;
            for (int j = 0; j < 8; j++)
            {
                Timestamp <<= 8;
                Timestamp |= Data[i - j - 1];
            }
            i += 4;
            for (int j = 0; j < 4; j++)
            {
                Data_Length <<= 8;
                Data_Length |= Data[i - j - 1];
            }
        }
        public override Packets_Base Handle(Server host)
        {
            string? Path = Path_Register.Get_Path(Timestamp);
            if(Path == null)
                return new Packets_Base(Index, 0xff, new byte[0]); //Reset Packet
            Console.WriteLine("Rollback image...: " + Data_Length);
            uint total_length = (uint)File_Writter.Rollback_File(Path, (int)Data_Length);
            return new Out_Remove_File_Slice_Packets(Index, Timestamp, total_length);
        }
    }
}
