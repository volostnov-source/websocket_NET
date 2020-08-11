using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.WebSockets;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace websocket_system
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Server_message get_answer = new Server_message();

        static async Task websocket()
        {
            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        await socket.ConnectAsync(new Uri(""), CancellationToken.None);
                        MessageBox.Show("Соединение установлено " + socket.State);
                        //await Send(socket, "data");
                        await Receive(socket);

                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"ERROR - {ex.Message}");
                        MessageBox.Show("Error " + ex.ToString());
                    }
            } while (true);
        }

        static async Task Receive(ClientWebSocket socket)
        {
            Server_message _result = null;
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        Console.WriteLine(await reader.ReadToEndAsync());
                        var ReadResult_fromServer = await reader.ReadToEndAsync();

                        _result = JsonConvert.DeserializeObject<Server_message>(ReadResult_fromServer); //десериализация сообщения

                        //MessageBox.Show("Сообщение с сервера: " + ReadResult_fromServer);
                        MessageBox.Show("Сообщение с сервера: " + _result.id);
                    }
                }
            } while (true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            websocket();
        }
    }
    public class Server_message
    {
        public string id { get; set; }
        public string message { get; set; } //сообщение для вывода на экран
    }
}
