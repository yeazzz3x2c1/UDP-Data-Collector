using Data_Collector_Server;


void Save_Config(Server server)
{
    using (StreamWriter w = new StreamWriter("setting.ini", false))
    {
        w.WriteLine("packet_length:" + server.Get_Packet_Length());
        w.WriteLine("timeout:" + server.Get_Timeout());
        w.WriteLine("maximum_image_size:" + server.Get_Maximum_Stacksize());
        w.Close();
    }
}
void Reload_Config(Server server)
{
    string path = "setting.ini";
    if (!File.Exists(path))
        Save_Config(server);
    using (StreamReader r = new StreamReader(path))
    {
        while (r.Peek() > -1)
        {
            string line = r.ReadLine();
            string[] spl = line.Split(':');
            switch (spl[0].ToLower())
            {
                case "packet_length":
                    server.Set_Packet_Length(uint.Parse(spl[1]));
                    break;
                case "timeout":
                    server.Set_Timeout(ushort.Parse(spl[1]));
                    break;
                case "maximum_image_size":
                    server.Set_Maximum_Stacksize(ushort.Parse(spl[1]));
                    break;
            }
        }
        r.Close();
    }
}

void main()
{

    string Version = "1.0";
    string Date = "2024/04/06";

    Server server = new Server();
    Console.WriteLine("Data Collector Server");
    Console.WriteLine("Version: " + Version);
    Console.WriteLine("Date: " + Date);
    Console.WriteLine("------------------------------------------");
    Reload_Config(server);
    Console.WriteLine("Maximum Packet Length: " + server.Get_Packet_Length() + " Bytes");
    Console.WriteLine("Timeout: " + server.Get_Timeout() + " milliseconds");
    // Console.WriteLine("Maximum client stack size: " + server.Get_Maximum_Stacksize() + " "); // Deprecated
    server.Start_Listen();
    while (true)
    {
        string? Input = Console.ReadLine();
        if (Input == null)
        {
            Tutorial.Send_Help();
            continue;
        }
        if (Input[0] != '/')
        {
            Tutorial.Send_Help();
            continue;
        }
        string data = Input.Substring(1);
        string[] spl = data.Split(' ');

        switch (spl[0].ToLower())
        {
            case "timeout":
                server.Set_Timeout(ushort.Parse(spl[1]));
                Console.WriteLine("Timeout: " + server.Get_Timeout() + " milliseconds");
                Save_Config(server);
                break;
            case "length":
                server.Set_Packet_Length(uint.Parse(spl[1]));
                Console.WriteLine("Maximum Packet Length: " + server.Get_Packet_Length() + " Bytes");
                Save_Config(server);
                break;
            //case "stack": // Deprecated
            //    server.Set_Maximum_Stacksize(ushort.Parse(spl[1]));
            //    Console.WriteLine("最大照片堆積設定: " + server.Get_Maximum_Stacksize() + " 張");
            //    Save_Config(server);
            //    break;
            case "debug":
                server.Set_Debug(bool.Parse(spl[1]));
                Console.WriteLine("Debug mode: " + bool.Parse(spl[1]));
                break;
        }
    }
}
main();