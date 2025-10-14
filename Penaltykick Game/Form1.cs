using System;
using System.Linq;
using System.Windows.Forms;

namespace Penaltykick_Game
{
    public partial class Form1 : Form
    {
        NetClient net = new NetClient();
        string myRole = "-";
        string kickerName = "-";
        int p1 = 0, p2 = 0, k1 = 0, k2 = 0;

        public Form1()
        {
            InitializeComponent();
            net.OnLine += OnLine;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SetTargetsEnabled(false);
        }

        void SetTargetsEnabled(bool enabled)
        {
            foreach (var pb in Controls.OfType<PictureBox>())
            {
                if (pb.Tag != null)
                    pb.Enabled = enabled;
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (await net.Connect(txtHost.Text.Trim(), int.Parse(txtPort.Text.Trim())))
                lblStatus.Text = "����: ���� ���� �Ϸ�";
            else
                lblStatus.Text = "����: ���� ���� ����";
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            await net.Send($"REGISTER {txtUser.Text.Trim()} {txtPass.Text}");
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await net.Send($"LOGIN {txtUser.Text.Trim()} {txtPass.Text}");
        }

        private async void btnReady_Click(object sender, EventArgs e)
        {
            await net.Send("READY");
        }

        private async void Target_Click(object sender, EventArgs e)
        {
            var target = ((PictureBox)sender).Tag.ToString();
            if (myRole == "KICKER")
            {
                football.Top -= 200; // ������ �̵� ����
                await net.Send($"SHOOT:{target}");
            }
            else if (myRole == "GOALKEEPER")
            {
                await net.Send($"SAVE:{target}");
            }
        }

        void OnLine(string line)
        {
            BeginInvoke(new Action(() => HandleServerMessage(line)));
        }

        void HandleServerMessage(string line)
        {
            if (line == "REGISTER_OK") lblStatus.Text = "ȸ������ ����";
            else if (line == "REGISTER_FAIL") lblStatus.Text = "ȸ������ ����";
            else if (line == "LOGIN_OK") lblStatus.Text = "�α��� ����";
            else if (line == "LOGIN_FAIL") lblStatus.Text = "�α��� ����";
            else if (line == "QUEUED") lblStatus.Text = "��Ī �����...";
            else if (line.StartsWith("MATCH_START"))
            {
                lblStatus.Text = line;
                SetTargetsEnabled(true);
            }
            else if (line.StartsWith("ROLE:"))
            {
                myRole = line.Substring(5);
                lblRole.Text = "����: " + (myRole == "KICKER" ? "ŰĿ" : "��Ű��");
            }
            else if (line.StartsWith("TURN:"))
            {
                kickerName = line.Split('=')[1];
                lblRound.Text = "Kicker: " + kickerName;
                football.Top = 500;
                goalkeeper.Left = 400;
                goalkeeper.Top = 300;
            }
            else if (line.StartsWith("RESULT:"))
            {
                var parts = line.Split('|');
                var res = parts[0].Split(':')[1];
                var sc = parts[1].Split('=')[1].Split(':');
                var kc = parts[2].Split('=')[1].Split(':');
                p1 = int.Parse(sc[0]); p2 = int.Parse(sc[1]);
                k1 = int.Parse(kc[0]); k2 = int.Parse(kc[1]);
                lblScore.Text = $"Score {p1}:{p2} ({res})";
            }
            else if (line == "SUDDEN_DEATH")
            {
                lblStatus.Text = "���絥�� ����!";
            }
            else if (line.StartsWith("GAME_OVER"))
            {
                var winner = line.Split('|').First(x => x.StartsWith("winner=")).Split('=')[1];
                var final = line.Split('|').First(x => x.StartsWith("final=")).Split('=')[1];
                MessageBox.Show($"Winner: {winner}\nFinal: {final}", "Game Over");
                lblStatus.Text = "���� ����";
                SetTargetsEnabled(false);
            }
        }
    }
}
