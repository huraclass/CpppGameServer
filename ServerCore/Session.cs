using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

class GameSession : Session {
    
    public override void OnConnected(EndPoint endPoint) {
        Console.WriteLine($"OnConnected : {endPoint}");
        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG SERVER");
        Send(sendBuff);
        Thread.Sleep(1000);
        Disconnect();
    }

    public override void OnRecv(ArraySegment<byte> buffer) {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($"from client{recvData}");
    }

    public override void OnSend(int numOfBytes) {
        Console.WriteLine($"Transferred bytes : {numOfBytes}");
    }

    public override void OnDisconnected(EndPoint endPoint) {
        Console.WriteLine($"OnDisconnected : {endPoint}");
    }
}

public abstract class Session {
    private Socket _socket;
    private int _disconnected = 0;
    private Queue<byte[]> _sendQueue = new Queue<byte[]>();
    SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
    SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

    public abstract void OnConnected(EndPoint endPoint);

    public abstract void OnRecv(ArraySegment<byte> buffer);
    
    public abstract void OnSend(int numOfBytes);

    public abstract void OnDisconnected(EndPoint endPoint);

    object _lock = new object();
    public void init(Socket socket) {
        this._socket = socket;
        
        _recvArgs.Completed += OnRecvCompleted;
        _recvArgs.SetBuffer(new byte[1024],0,1024);
        
        _sendArgs.Completed += OnSendCompleted;
        
        RegisterRecv();
    }
    
    void OnSendCompleted(object sender, SocketAsyncEventArgs args) {
        lock (_lock) {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
                try {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();
                    OnSend(_sendArgs.BytesTransferred);
                    
                    if (_sendQueue.Count > 0) {
                        RegisterSend();
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            else {
                Disconnect();
            }
        }
    }

    void OnRecvCompleted(object sender,SocketAsyncEventArgs args) {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
            try {
                OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                RegisterRecv();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        else {
            Disconnect();
        }
    }

    public void Send(byte[] sendBuffer) {
        lock (_lock) {
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0)
                RegisterSend();    
        }
    }

    void RegisterSend() {
        _pendingList.Clear();
        while (_sendQueue.Count > 0) {
            var buff = _sendQueue.Dequeue();
            _pendingList.Add(new ArraySegment<byte>(buff,0,buff.Length));
        }

        _sendArgs.BufferList = _pendingList;
        bool pending = _socket.SendAsync(_sendArgs);
        if (pending == false) {
            OnSendCompleted(null, _sendArgs);
        }
    }

    

    private void RegisterRecv() {
        var pending = _socket.ReceiveAsync(_recvArgs);
        if (pending == false){
            OnRecvCompleted(null, _recvArgs);
        }
    }

    public void Disconnect() {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1) {
            return;
        }
        OnDisconnected(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    
}