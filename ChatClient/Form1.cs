using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets; // 통신을 하기 위한 첫번째 단계
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient _client;
        byte[] _buffer = new byte[4096];

        public Form1()
        {
            InitializeComponent();

            _client = new TcpClient();                     
              
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            _client.Connect("127.0.0.1", 54000); // 포트 번호 임의 생성


            _client.GetStream().BeginRead(_buffer,
                                            0,
                                            _buffer.Length,
                                            Server_MessageReceived,                                          null);
        }

        private void Server_MessageReceived(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                // TODO : RECEIVE MESSAGE
                var bytesIn = _client.GetStream().EndRead(ar);

                if (bytesIn > 0)
                {
                    var tmp = new byte[bytesIn];
                    Array.Copy(_buffer, 0, tmp, 0, bytesIn); // 시작위치 0, 바이트가 될 길이가 
                    var str = Encoding.ASCII.GetString(tmp);
                    // TODO : MAKE THIS AN ACTUAL DELEGATE READ DOCS
                    BeginInvoke((Action)(() => // Action F12로 확인해보면 public delegate 공개대리자
                    {
                        listBox1.Items.Add(str);
                        listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    }));
                }
                /*BeginInvoke()*/ // c# 내부 라이브러리. 이작업이 메세지를 받는데 유용하다
                                  // 쓰레드의 위험성에서 조금 벗어나게 해주는 존재?
                Array.Clear(_buffer, 0, _buffer.Length);

            }
        }

        private void sendbtn_Click(object sender, EventArgs e)
        {
            var msg = Encoding.ASCII.GetBytes(textBox1.Text);
            _client.GetStream().Write(msg, 0, msg.Length);

            textBox1.Text = "";
            textBox1.Focus();
        }
    }
}
