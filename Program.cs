// reciever specifies IP
// sender gives files

using System.Net;
using System.Net.Sockets;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// scary stuff
System.Console.WriteLine("Dont be scared...");
string os = Environment.OSVersion.Platform.ToString();
System.Console.WriteLine("OS: " + os);
string username = Environment.UserName.ToString();
IPAddress lip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[3];
System.Console.WriteLine($"local IP is {lip}"); // local ip

// get the downloads folder
string downloads;
if (os == "Unix")
    downloads = $"/Users/{username}/Downloads";
else // if cant find, install here
    downloads = ".";
System.Console.WriteLine("Files will be downloaded to: " + downloads);

if (Environment.GetCommandLineArgs().Length < 2 || Environment.GetCommandLineArgs().Length > 2) {
    System.Console.WriteLine("ERROR: Improper amount of arguments");
    System.Console.WriteLine("For sending:");
    System.Console.WriteLine("\t./Celery sen file.txt");
    System.Console.WriteLine("For receiving:");
    System.Console.WriteLine("\t./Celery rec 127.0.0.7");
    System.Console.WriteLine("\nConnect to the Local IP of the sender, this is given when the program starts");

    return;
}

string mode = Environment.GetCommandLineArgs()[1]; // sending or recieving

int PORT = 3111;
if (mode == "sen") {
    string filename = Environment.GetCommandLineArgs()[2];
    dataSend(filename);
} else if (mode == "rec") {
    string senderIP = Environment.GetCommandLineArgs()[2];
    dataReceive(senderIP);
} else
    return;
System.Console.WriteLine($"{mode} {downloads}");

void dataSend(string filename) {
    IPAddress ipAddr = IPAddress.Parse("127.0.0.1");//lip;
    IPEndPoint localEndPoint = new IPEndPoint(ipAddr, PORT);

    Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    listener.Bind(localEndPoint);
    listener.Listen();

    Socket receiver = listener.Accept();

    receiver.SendFile(filename, Encoding.UTF8.GetBytes(filename), null, TransmitFileOptions.UseDefaultWorkerThread); // same what i did, kinda, but its built and better, but could do it myself

    // Release the socket.
    receiver.Shutdown(SocketShutdown.Both);
    receiver.Close();
}

void dataReceive(string sip) {
    IPAddress ipAddr = IPAddress.Parse(sip);
    IPEndPoint localEndPoint = new IPEndPoint(ipAddr, PORT);
    Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    sender.Connect(localEndPoint);

    byte[] bufferName = new byte[1024];
    byte[] bufferData = new byte[1024]; // make bigger
    byte[] bufferPost = new byte[1024]; // make bigger

    sender.Receive(bufferName);
    System.Console.WriteLine(Encoding.UTF8.GetString(bufferName));
    sender.Receive(bufferData);
    System.Console.WriteLine(Encoding.UTF8.GetString(bufferData));
    sender.Receive(bufferPost);
    System.Console.WriteLine(BitConverter.ToString(bufferPost));
    if (bufferPost[0] == 0) // should work
        System.Console.WriteLine("done");
    
    File.WriteAllBytes(Encoding.UTF8.GetString(bufferName), bufferData);

    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
}