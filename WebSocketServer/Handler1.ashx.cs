using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace WebSocketServer
{
   
    /// <summary>
    /// Zusammenfassungsbeschreibung für Handler1
    /// </summary>
    public class Handler1 : IHttpHandler 
    {
        private static List<WebSocket> clients = new List<WebSocket>();
        private static String test = "nix";

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
                context.AcceptWebSocketRequest(Answer);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private async Task Answer(AspNetWebSocketContext con)
        {
            WebSocket socket = con.WebSocket;
            clients.Add(socket);
            
            bool connected = true;
            while (socket.State == WebSocketState.Open)       
            //while(connected)     
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                foreach (WebSocket client in clients)
                {
                    if (!client.Equals(socket))
                    {
                        await client.SendAsync(new ArraySegment<byte>(buffer.Array, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            clients.Remove(socket);
        }
    }
}