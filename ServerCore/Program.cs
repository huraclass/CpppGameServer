using System.Net;
using System.Net.Sockets;
using System.Text;using ServerCore;

Listener listener = new Listener();

string host = Dns.GetHostName();
IPHostEntry ipHost = Dns.GetHostEntry(host);//위에서 찾아낸 호스트명을 기반으로 호스트의 ip주소 정보를 받아옴
IPAddress ipAddr = ipHost.AddressList[0];//호스트의 ip주소중 ipv6의 주로를 받아좀 ipv4의 주소는 배열의 1값
IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);//host의 리슨 정보 현재는 ipv6주소와 포트번호를 등록해놓은 상태

Socket listenSocket = new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);//리스너 소캣을 생성 여기서 매개변수로 ip버전 정보, 프로토콜 정보를 담음



listener.init(endPoint);
while (true) {
    Console.WriteLine("listenSocket listening");

    Socket clientSocket = listener.Accept();

    byte [] buffer = new byte [1024];
    int recvBytes = clientSocket.Receive(buffer);
    string recvData = Encoding.UTF8.GetString(buffer,0,recvBytes );
    Console.WriteLine (recvData);

    byte [] data = Encoding.UTF8.GetBytes("welcome to MMORPG server");
    clientSocket.Send(data);

    clientSocket.Shutdown(SocketShutdown.Both);
    clientSocket.Close();
}
