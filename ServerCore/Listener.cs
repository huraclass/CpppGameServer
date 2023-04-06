using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore {
    internal class Listener {
        Socket listenSocket;
        public void init(IPEndPoint endPoint) {
            listenSocket = new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);//리스너 소캣을 생성 여기서 매개변수로 ip버전 정보, 프로토콜 정보를 담음
            listenSocket.Bind(endPoint);//리스너 소캣에 정보를 등록
            listenSocket.Listen(10);//리스너 소캣을 리슨상태로 변환
        }

        public Socket Accept() {
            return listenSocket.Accept();
        }
        
    }
}
