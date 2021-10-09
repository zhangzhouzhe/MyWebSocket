using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;

namespace MyWebSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic_Sockets = new Dictionary<string, IWebSocketConnection>();

            var server = new WebSocketServer("ws://127.0.0.1:8888");
            server.RestartAfterListenError = true;

            server.Start(socket =>
            {
                socket.OnOpen =
                    () =>
                    {

                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        dic_Sockets.Add(clientUrl, socket);

                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 建立WebSock连接！");
                    };

                socket.OnClose =
                    () =>
                    {

                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (dic_Sockets.ContainsKey(clientUrl))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            if (dic_Sockets[clientUrl] != null)
                            {
                                dic_Sockets[clientUrl].Close();
                            }
                            dic_Sockets.Remove(clientUrl);
                        }
                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 断开WebSock连接！");
                    };

                socket.OnMessage =
                    (message) =>
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);

                    };

            });


            Console.ReadKey();

            foreach (var item in dic_Sockets.Values)
            {
                if (item.IsAvailable == true)
                {
                    item.Send("服务器消息：" + DateTime.Now.ToString());
                }
            }
            Console.ReadKey();

            foreach (var item in dic_Sockets.Values)
            {
                if (item != null)
                {
                    item.Close();
                }
            }

            Console.ReadKey();

        }
    }
}
