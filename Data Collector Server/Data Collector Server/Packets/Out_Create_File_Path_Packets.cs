using Data_Collector_Server.File_Helper;
using System.Text;

namespace Data_Collector_Server.Packets
{
    internal class Out_Create_File_Path_Packets : Packets_Base
    {
        private ulong Timestamp = 0;
        private uint Data_Length = 0;
        private ushort Path_Length = 0;
        private string Path = "";
        //CMDs:
        //1. 建立檔案路徑: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Path(Data_Length Bytes), CRC]
        //Response: [Index, CMD, Timestamp, CRC]
        public Out_Create_File_Path_Packets(byte Index, ulong Timestamp)
        {
            byte[] Data = new byte[8];
            int i = 0;
            for (int j = 0; j < 8; j++)
            {
                Data[i++] = (byte)(Timestamp & 0xff);
                Timestamp >>= 8;
            }
            Encode_Data(Index, (byte)Packets_Index.Create_File_Path, Data);
        }

        public Out_Create_File_Path_Packets(byte[] Package_Data) : base(Package_Data)
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
            i += 2;
            for (int j = 0; j < 2; j++)
            {
                Path_Length <<= 8;
                Path_Length |= Data[i - j - 1];
            }

            byte[] path = new byte[Path_Length];
            for (int j = 0; j < Path_Length; j++)
             path[j] = Data[i++];
            Path = Encoding.UTF8.GetString(path);
        }
        public override Packets_Base Handle(Server host)
        {
            Path_Register.Register_Path(Timestamp, Data_Length, Path);
            Console.WriteLine("Create File Path: " + Path + ", Data Size: " + Data_Length);
            return new Out_Create_File_Path_Packets(Index, Timestamp);
        }
    }
}
