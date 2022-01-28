using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
//tcp client
class Program
{
    static void Main(string[] args)
    {



        var ip = IPAddress.Loopback;
        var port = 11111;

        Console.WriteLine("Client");
        var client = new TcpClient();
        client.Connect(ip, port);
        BinaryReader br = new BinaryReader(client.GetStream());
        BinaryWriter bw = new BinaryWriter(client.GetStream());


        while (true)
        {
            var str = Console.ReadLine().ToUpper();
            if (str == "HELP")
            {
                Console.WriteLine(Commands.Proclist);
                Console.WriteLine(Commands.Kill, " <Process_Name>");
                Console.WriteLine(Commands.Run, " <Process_Name>");
                Console.WriteLine("HELP");
                continue;
            }

            Commands command = new Commands();

            var TextAndParam = str.Split(' ');
            string responce = null;
            switch (TextAndParam[0])
            {
                case Commands.Proclist:
                    command.Text = TextAndParam[0];

                    bw.Write(JsonSerializer.Serialize(command));

                    responce = br.ReadString();
                    var Process = JsonSerializer.Deserialize<string[]>(responce);

                    foreach (var item in Process)
                    {
                        Console.WriteLine(item);
                    }

                    break;
                case Commands.Run:

                    command.Text = TextAndParam[0];
                    command.Param = TextAndParam[1];

                    bw.Write(JsonSerializer.Serialize(command));
                    if (br.ReadBoolean())
                        Console.WriteLine($"Runned --> {command.Param}");
                    break;
                case Commands.Kill:

                    command.Text = TextAndParam[0];
                    command.Param = TextAndParam[1];
                    bw.Write(JsonSerializer.Serialize(command));
                    if (br.ReadBoolean())
                        Console.WriteLine($"Killed --> {command.Param}");
                    
                    break;
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