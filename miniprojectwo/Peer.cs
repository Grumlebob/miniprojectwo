using System.Net;
using System.Net.Sockets;

namespace miniprojectwo;

using System;
using System.IO; // Add this for File handling
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;


//https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide

class Peer
{
    private string name;
    private int port;
    private int ownPrivateValue = 0;
    private List<int> receivedIntegers = new List<int>();
    private int individualSums = 0;

    public Peer(string name, int port, int ownPrivateValue)
    {
        this.name = name;
        this.port = port;
        this.ownPrivateValue = ownPrivateValue;
    }

    public void Start()
    {
        Thread listenerThread = new Thread(() => StartListener());
        listenerThread.Start();

        // Allow time for the listener to start
        Thread.Sleep(100);

        // Simulate peer behavior
        var sharesResult = calculateShares();

        // Allow time for the messages to be processed
        Thread.Sleep(1000);

        // Display individual sums (Note: Each peer can only see its own sum)
        DistributeShares(sharesResult);
    }

    public int[] calculateShares()
    {
        Random random = new Random();
        int target = ownPrivateValue;
        int remaining = target;
        int[] randomNumbers = new int[3];

        for (int i = 0; i < randomNumbers.Length - 1; i++)
        {
            int randomNumber = random.Next(-remaining, remaining + 1);
            randomNumbers[i] = randomNumber;
            remaining -= randomNumber;
        }

        randomNumbers[randomNumbers.Length - 1] = remaining;

        // Shuffle the array to make the numbers random
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            int temp = randomNumbers[i];
            int randomIndex = random.Next(i, randomNumbers.Length);
            randomNumbers[i] = randomNumbers[randomIndex];
            randomNumbers[randomIndex] = temp;
        }

        return randomNumbers;
    }
    
    public void SendIntegerToPeer(int data, Peer targetPeer)
    {
        SendMessage(targetPeer.name, data);
    }

    private void StartListener()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Console.WriteLine($"{name} is listening for connections on port {port}...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();

            // Wrap the stream with an SslStream
            SslStream sslStream = new SslStream(client.GetStream(), false, ValidateCertificate);
            sslStream.AuthenticateAsServer(GetServerCertificate(name), false, SslProtocols.Tls, false);

            Thread clientThread = new Thread(() => HandleClient(sslStream));
            clientThread.Start();
        }
    }


    private bool ValidateCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslpolicyerrors)
    {
        return true;
    }


    private void HandleClient(SslStream sslStream)
    {
        using (sslStream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
            int receivedData = BitConverter.ToInt32(buffer, 0);

            Console.WriteLine($"{name} received: {receivedData} from {sslStream.RemoteCertificate.Subject}");

            StoreReceivedInteger(receivedData);

            sslStream.Close();
        }
    }

    private void StoreReceivedInteger(int receivedData)
    {
        receivedIntegers.Add(receivedData); 
        individualSums += receivedData;
    }

    private void SendMessage(string recipient, int data)
    {
        TcpClient client = new TcpClient("127.0.0.1", GetPortForRecipient(recipient));

        // Wrap the stream with an SslStream
        SslStream sslStream = new SslStream(client.GetStream(), false, ValidateCertificate);
        sslStream.AuthenticateAsClient(recipient, null, SslProtocols.Tls, false);

        byte[] dataBytes = BitConverter.GetBytes(data);
        sslStream.Write(dataBytes, 0, dataBytes.Length);

        Console.WriteLine($"Sent: {data} to {recipient}");

        sslStream.Close();
        client.Close();
    }

    private int GetPortForRecipient(string recipient)
    {
        switch (recipient)
        {
            case "Alice":
                return 12345;
            case "Bob":
                return 12346;
            case "Charlie":
                return 12347;
            case "Hospital":
                return 12348;
            default:
                throw new ArgumentException($"Invalid recipient: {recipient}");
        }
    }

    private void DistributeShares(int[] shares)
    {
        if (name == "Alice")
        {
            StoreReceivedInteger(shares[0]);
            SendIntegerToPeer(shares[1], GetPeerByName("Bob"));
            SendIntegerToPeer(shares[2], GetPeerByName("Charlie"));
        }
        else if (name == "Bob")
        {
            SendIntegerToPeer(shares[0], GetPeerByName("Alice"));
            StoreReceivedInteger(shares[1]);
            SendIntegerToPeer(shares[2], GetPeerByName("Charlie"));
        }
        else if (name == "Charlie")
        {
            SendIntegerToPeer(shares[0], GetPeerByName("Alice"));
            SendIntegerToPeer(shares[1], GetPeerByName("Bob"));
            StoreReceivedInteger(shares[2]);
        }
    }
    
    
    private X509Certificate GetServerCertificate(string name)
    {
        var certificateFileName = $"{name}.pfx";

        if (File.Exists(certificateFileName))
        {
            return new X509Certificate2(certificateFileName);
        }
        
        var ecdsa = ECDsa.Create();
        var request = new CertificateRequest("CN=name", ecdsa, HashAlgorithmName.SHA256);
        var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

        var certificateBytes = certificate.Export(X509ContentType.Pfx);
        File.WriteAllBytes(certificateFileName, certificateBytes);
        return certificate;
    }
    
    private Peer GetPeerByName(string peerName)
    {
        // This method returns a reference to the desired peer based on its name
        switch (peerName)
        {
            case "Alice":
                return Peers.Alice;
            case "Bob":
                return Peers.Bob;
            case "Charlie":
                return Peers.Charlie;
            case "Hospital":
                return Peers.Hospital;
            default:
                throw new ArgumentException($"Invalid peer name: {peerName}");
        }
    }

    public void PrintAllFields()
    {
        //print all fields
        Console.WriteLine("Name: " + name);
        Console.WriteLine("Port: " + port);
        Console.WriteLine("OwnPrivateValue: " + ownPrivateValue);
        foreach (var share in receivedIntegers)
        {
            Console.WriteLine("ReceivedIntegers: " + share);
        }
        Console.WriteLine("IndividualSums: " + individualSums);
        
    }
}

class Peers
{
    public static Peer Alice;
    public static Peer Bob;
    public static Peer Charlie;
    public static Peer Hospital;
}
