using System.Net;
using System.Net.Sockets;
using System.Text;


string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);//위에서 찾아낸 호스트명을 기반으로 호스트의 ip주소 정보를 받아옴
IPAddress ipAddr = ipHost.AddressList[0];//호스트의 ip주소중 ipv6의 주로를 받아좀 ipv4의 주소는 배열의 1값
IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);//host의 리슨 정보 현재는 ipv6주소와 포트번호를 등록해놓은 상태


while (true) {
    Socket socket = new Socket(endPoint.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

    socket.Connect(endPoint);
    Console.WriteLine(socket.RemoteEndPoint.ToString());

    byte [] sendBuff = Encoding.UTF8.GetBytes("hello C# socket server");
    int sendBytes = socket.Send(sendBuff);

    byte [] recvBuff= new byte [1024];
    int recvBytes = socket.Receive(recvBuff);
    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
    Console.WriteLine(recvData);
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
    Thread.Sleep(500);
}


