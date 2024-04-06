using Data_Collector_Server.Packets;
using System.Net;
using System.Net.Sockets;

namespace Data_Collector_Server
{
    class Server
    {
        private Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private bool Debug = false;
        private uint Package_Length = 1024; //1KB
        private ushort Timeout = 5000; //5s
        private ushort Maximum_Stacksize = 500; //500 images
        public uint Get_Packet_Length()
        {
            return Package_Length;
        }
        public ushort Get_Timeout()
        {
            return Timeout;
        }
        public ushort Get_Maximum_Stacksize()
        {
            return Maximum_Stacksize;
        }
        public void Set_Packet_Length(uint Length)
        {
            Package_Length = Length;
        }
        public void Set_Timeout(ushort Timeout)
        {
            this.Timeout = Timeout;
        }
        public void Set_Maximum_Stacksize(ushort Maximum_Stacksize)
        {
            this.Maximum_Stacksize = Maximum_Stacksize;
        }
        public void Set_Debug(bool Debug)
        {
            this.Debug = Debug;
        }
        public void Start_Listen()
        {
            //Server作為Slave
            //Request Bytes Array: [Data_Index, Cmd, Data(Nullable), CRC(2Bytes)]
            //若通訊Error，則Server不回覆訊息

            //Ack: [Data_Index, CMD, CRC(2Bytes)]
            //CRC: Sum(0, Index(CRC) - 1)

            //CMDs:
            //0. 伺服器設定資料請求: [Index, CMD, CRC](無送出資料)
            //Response: [Index, CMD, 每個封包最長長度(4Bytes), 逾時時間(ms;2Bytes), CRC]

            //1. 建立檔案路徑: [Index, CMD, Timestamp(File_Index;8Bytes), Path(2Bytes), Data, CRC]
            //Response: [Index, CMD, Timestamp, CRC]

            //2. 上傳檔案片段: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Data, CRC]
            //Response: [Index, CMD, Timestamp, 目前總資料長度(4Bytes), CRC]

            //3. 刪除檔案片段: [Index, CMD, Timestamp(File_Index;8Bytes), Data_Length(2Bytes), Data, CRC]
            //Response: [Index, CMD, Timestamp, 目前總資料長度(4Bytes), CRC]
            int Total_Package_Type_Length = 4;
            Type[] packages = new Type[Total_Package_Type_Length];
            packages[(int)Packets_Index.Server_Setting] = typeof(Out_Server_Setting_Packets);
            packages[(int)Packets_Index.Create_File_Path] = typeof(Out_Create_File_Path_Packets);
            packages[(int)Packets_Index.Write_File_Slice] = typeof(Out_Write_File_Slice_Packets);
            packages[(int)Packets_Index.Remove_File_Slice] = typeof(Out_Remove_File_Slice_Packets);
            new Thread(() =>
            {
                try
                {
                    sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sck.Bind(new IPEndPoint(IPAddress.Any, 35525));
                    byte[] temp = new byte[2048000];
                    int Length = 0;
                    EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                    while (true)
                    {
                        try
                        {
                            ep = new IPEndPoint(IPAddress.Any, 0);
                            Length = sck.ReceiveFrom(temp, temp.Length, SocketFlags.None, ref ep);
                            byte cmd = temp[1];
                            byte[] received = new byte[Length];
                            Array.Copy(temp, received, Length);
                            Packets_Base instance = (Packets_Base)Activator.CreateInstance(packages[cmd], received);
                            byte[] response_data = instance.Handle(this).GetPackageData();
                            sck.SendTo(response_data, ep);
                        }
                        catch (Exception ex)
                        {
                            if (Debug)
                            {
                                Console.WriteLine("Invaild Packet: " + ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }
                catch { }
            }).Start();
        }
    }
}
