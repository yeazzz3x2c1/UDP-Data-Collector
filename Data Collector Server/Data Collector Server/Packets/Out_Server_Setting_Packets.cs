namespace Data_Collector_Server.Packets
{
    internal class Out_Server_Setting_Packets : Packets_Base
    {

        //CMDs:
        //0. 伺服器設定資料請求: [Index, CMD, CRC](無送出資料)
        //Response: [Index, CMD, 每個封包最長長度(4Bytes), 逾時時間(ms;2Bytes), 像機影像堆積最大數量(2Bytes), CRC]

        public Out_Server_Setting_Packets(byte Index, uint Package_Length, ushort Timeout, ushort Maximum_Image_Stacksize)
        {
            byte[] Data = new byte[8];
            int i = 0;
            for (int j = 0; j < 4; j++)
            {
                Data[i++] = (byte)(Package_Length & 0xff);
                Package_Length >>= 8;
            }
            for (int j = 0; j < 2; j++)
            {
                Data[i++] = (byte)(Timeout & 0xff);
                Timeout >>= 8;
            }
            for (int j = 0; j < 2; j++)
            {
                Data[i++] = (byte)(Maximum_Image_Stacksize & 0xff);
                Maximum_Image_Stacksize >>= 8;
            }
            Encode_Data(Index, (byte)Packets_Index.Server_Setting, Data);
        }
        public Out_Server_Setting_Packets(byte[] Package_Data) : base(Package_Data)
        { }
        public override Packets_Base Handle(Server host)
        {
            Console.WriteLine("Response Server Setting.");
            return new Out_Server_Setting_Packets(Index, host.Get_Packet_Length(), host.Get_Timeout(), host.Get_Maximum_Stacksize());
        }
    }
}
