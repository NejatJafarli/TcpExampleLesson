
//TCP Client
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var ip = IPAddress.Loopback;
        var port = 11111;

        var Server = new TcpListener(ip, port);
        Server.Start(100);

        while (true)
        {
            var client = Server.AcceptTcpClient();

            var stream = client.GetStream();

            var br = new BinaryReader(stream);
            var bw = new BinaryWriter(stream);

            Console.WriteLine("Server");
            while (true)
            {
                var input = br.ReadString();
                if (input == "Exit")
                    break;
                var Command = JsonSerializer.Deserialize<Commands>(input);
                if (Command == null) continue;

                Console.WriteLine(Command.Text);
                Console.WriteLine(Command.Param);

                switch (Command.Text)
                {
                    case Commands.Proclist:
                        var Proces = Process.GetProcesses();
                        var JsonText = JsonSerializer.Serialize(Proces.Select(p => p.ProcessName));
                        bw.Write(JsonText);
                        break;
                    case Commands.Run:

                        try
                        {
                            var Run = Command.Param;
                            Process.Start(Run);
                            bw.Write("1");
                        }
                        catch (Exception ex)
                        {


                            bw.Write("0");
                            throw;
                        }

                        break;
                    case Commands.Kill:
                        try
                        {
                            var kill = Process.GetProcessesByName(Command.Param).FirstOrDefault();
                            kill?.Kill();
                            bw.Write("1");
                        }
                        catch (Exception ex)
                        {
                            bw.Write("0");
                        }
                        break;
                }

            }


        }


    }
}

class Commands
{
    public const string Proclist = "PROCLIST";
    public const string Kill = "KILL";
    public const string Run = "RUN";
    public string Text { get; set; }
    public string Param { get; set; }
}