using System.Net;
using System.Net.Sockets;
using System.Text;using ServerCore;

var listener = new Listener();

listener.init(getEndPoint(), () => { return new GameSession(); });

while (true) {}

IPEndPoint getEndPoint() {
    var host = Dns.GetHostName();
    var ipHost = Dns.GetHostEntry(host);//위에서 찾아낸 호스트명을 기반으로 호스트의 ip주소 정보를 받아옴
    IPAddress ipAddr = ipHost.AddressList[0];//호스트의 ip주소중 ipv6의 주로를 받아좀 ipv4의 주소는 배열의 1값
    IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);//host의 리슨 정보 현재는 ipv6주소와 포트번호를 등록해놓은 상태
    return endPoint;
}

void OnAcceptHandler(Socket clientSocket) {
    try {
        var data = Encoding.UTF8.GetBytes("welcome to MMORPG server");
        var session = new GameSession();
        session.init(clientSocket);
        session.Send(data);
        Thread.Sleep(1000);
        session.Disconnect();
    }
    catch (Exception e) {
        Console.WriteLine(e.ToString());
    }
}
