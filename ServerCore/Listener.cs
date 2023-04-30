using System.Net;
using System.Net.Sockets;

namespace ServerCore {
    internal class Listener {
        Socket listenSocket;
        Func<Session> sessionFactory;

        public void init(IPEndPoint endPoint,Func<Session> sessionFactory) {
            listenSocket = new Socket(endPoint.AddressFamily,SocketType.Stream,ProtocolType.Tcp);//리스너 소캣을 생성 여기서 매개변수로 ip버전 정보, 프로토콜 정보를 담음
            this.sessionFactory += sessionFactory;
            listenSocket.Bind(endPoint);//리스너 소캣에 정보를 등록
            listenSocket.Listen(10);//리스너 소캣을 리슨상태로 변환
            var args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args);
        }

        private void RegisterAccept(SocketAsyncEventArgs args) {
            args.AcceptSocket = null;
            if (!listenSocket.AcceptAsync(args)) {
                OnAcceptCompleted(null,args);
            }
        }

        private void OnAcceptCompleted(object sender,SocketAsyncEventArgs args) {//최종적으로 실제 접속이 이루저지는 함수
            if (args.SocketError == SocketError.Success) {
                var session = sessionFactory.Invoke();
                session.init(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else {
                Console.WriteLine(args.SocketError.ToString());
            }
            RegisterAccept(args);
        }

        public Socket Accept() {
            return listenSocket.Accept();
        }
        
    }
}
