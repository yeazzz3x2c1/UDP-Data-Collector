namespace Data_Collector_Server.Packets
{
    internal class Packets_Base
    {
        //Server作為Slave
        //Request Bytes Array: [Data_Index, Cmd, Data(Nullable), CRC(2Bytes)]
        //若通訊Error，則Server不回覆訊息

        //Ack: [Data_Index, CMD, CRC(2Bytes)]
        //CRC: Sum(0, Index(CRC) - 1)
        protected byte Index;
        protected byte CMD;
        protected byte[] Data = null;
        protected byte[] Package_Data = null;
        byte CRC;
        public Packets_Base()
        {

        }
        public Packets_Base(byte[] Package_Data)
        {
            int CRC_Index = Package_Data.Length - 1;
            byte CRC = 0;
            for (int i = 0; i < CRC_Index; i++)
                CRC += Package_Data[i];
            if ((Package_Data[CRC_Index] ^ CRC) != 0)
                throw new Exception("CRC ERROR");
            Index = Package_Data[0];
            CMD = Package_Data[1];
            byte[] Data = new byte[Package_Data.Length - 3]; //-CRC-INDEX-CMD

            if (Data.Length > 0)
                for (int i = 0; i < Package_Data.Length - 3; i++)
                    Data[i] = Package_Data[i + 2];
            this.Data = Data;
            this.Package_Data = Package_Data;
        }

        public Packets_Base(byte Index, byte CMD, byte[] Data)
        {
            Encode_Data(Index, CMD, Data);
        }
        public void Encode_Data(byte Index, byte CMD, byte[] Data)
        {
            this.Index = Index;
            this.CMD = CMD;
            this.Data = Data;
            Package_Data = new byte[3 + Data.Length];
            Package_Data[0] = Index;
            Package_Data[1] = CMD;
            CRC = (byte)(Index + CMD);
            int i = 0;
            foreach (byte b in Data)
            {
                CRC += Data[i];
                Package_Data[i + 2] = Data[i++];
            }
            Package_Data[i + 2] = CRC;
        }
        public byte GetIndex()
        {
            return Index;
        }
        public byte GetCMD()
        {
            return CMD;
        }
        public byte[] GetData()
        {
            return Data;
        }
        public byte[] GetPackageData()
        {
            return Package_Data;
        }
        public virtual Packets_Base Handle(Server host) { return null; }
    }
}
