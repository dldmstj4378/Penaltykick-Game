using System;
using System.Linq;
using System.Windows.Forms;

namespace Penaltykick_Game
{
    public partial class Form1 : Form
    {
        NetClient net = new NetClient();
        private List<PictureBox> goalTarget;
        string myRole = "-";
        string kickerName = "-";
        int p1 = 0, p2 = 0;
        private System.Windows.Forms.Timer? shootTimer;
        private System.Windows.Forms.Timer? keeperTimer;


        public Form1()
        {
            InitializeComponent();   // 폼 디자이너 초기화 (가장 먼저!)

            // 🟡 1. 타겟 PictureBox 생성
            topLeft = new PictureBox();
            top = new PictureBox();
            topRight = new PictureBox();
            left = new PictureBox();
            right = new PictureBox();

            // 🟡 2. 각 PictureBox 속성 설정 (위치, 크기, 색 등)
            topLeft.Size = new Size(50, 50);
            topLeft.Location = new Point(180, 100);
            topLeft.BackColor = Color.Yellow;
            topLeft.SizeMode = PictureBoxSizeMode.StretchImage;

            top.Size = new Size(50, 50);
            top.Location = new Point(370, 100);
            top.BackColor = Color.Yellow;
            top.SizeMode = PictureBoxSizeMode.StretchImage;

            topRight.Size = new Size(50, 50);
            topRight.Location = new Point(560, 100);
            topRight.BackColor = Color.Yellow;
            topRight.SizeMode = PictureBoxSizeMode.StretchImage;

            left.Size = new Size(50, 50);
            left.Location = new Point(200, 270);
            left.BackColor = Color.Yellow;
            left.SizeMode = PictureBoxSizeMode.StretchImage;

            right.Size = new Size(50, 50);
            right.Location = new Point(540, 270);
            right.BackColor = Color.Yellow;
            right.SizeMode = PictureBoxSizeMode.StretchImage;

            // 🟡 3. 폼에 추가 (실제로 보여지게 하기)
            this.Controls.Add(topLeft);
            this.Controls.Add(top);
            this.Controls.Add(topRight);
            this.Controls.Add(left);
            this.Controls.Add(right);

            // 🟡 4. goalTarget 리스트에 담기 (공통 제어용)
            goalTarget = new List<PictureBox> { topLeft, top, topRight, left, right };
        }




        private void ChangeGoalKeeperImage(string direction)
        {
            switch (direction)
            {
                case "left":
                    goalKeeper.Image = Properties.Resources.left_save_small;
                    break;
                case "right":
                    goalKeeper.Image = Properties.Resources.right_save_small;
                    break;
                case "top":
                    goalKeeper.Image = Properties.Resources.top_save_small;
                    break;
                case "topLeft":
                    goalKeeper.Image = Properties.Resources.top_left_save_small;
                    break;
                case "topRight":
                    goalKeeper.Image = Properties.Resources.top_right_save_small;
                    break;
                default:
                    goalKeeper.Image = Properties.Resources.stand_small;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializePositions();
            SetTargetsEnabled(false);

            // Z-Order 보정
            topLeft.BringToFront();
            top.BringToFront();
            topRight.BringToFront();
            left.BringToFront();
            right.BringToFront();
            football.BringToFront();
            goalKeeper.BringToFront();
        }

        private void InitializePositions()
        {
            // 공 위치 초기화
            football.Location = new Point(430, 500);

            // 골키퍼 위치 초기화
            goalKeeper.Location = new Point(418, 169);
            goalKeeper.Image = Properties.Resources.stand_small;

            // 타겟 색상 초기화
            foreach (PictureBox target in goalTarget)
            {
                target.BackColor = Color.Yellow;
            }
        }


        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (await net.Connect(txtHost.Text.Trim(), int.Parse(txtPort.Text.Trim())))
                lblStatus.Text = "상태: 서버 연결 완료";
            else
                lblStatus.Text = "상태: 서버 연결 실패";
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
            string target = ((PictureBox)sender).Tag.ToString()!;
            if (myRole == "KICKER")
                await net.Send($"SHOOT:{target}");
            else if (myRole == "GOALKEEPER")
                await net.Send($"SAVE:{target}");

            // 내 선택 뒤에는 비활성화 (상대/타이머 대기)
            SetTargetsEnabled(false);
        }

        private void OnLine(string line) => BeginInvoke(new Action(() => HandleServerMessage(line)));

        private void HandleServerMessage(string line)
        {
            if (line == "REGISTER_OK") lblStatus.Text = "회원가입 성공";
            else if (line == "REGISTER_FAIL") lblStatus.Text = "회원가입 실패";
            else if (line == "LOGIN_OK") lblStatus.Text = "로그인 성공";
            else if (line == "LOGIN_FAIL") lblStatus.Text = "로그인 실패";
            else if (line == "QUEUED") lblStatus.Text = "매칭 대기중...";
            else if (line.StartsWith("MATCH_START"))
            {
                lblStatus.Text = line;
                InitializePositions();
            }
            else if (line.StartsWith("ROLE:"))
            {
                myRole = line.Substring(5);
                lblRole.Text = "역할: " + (myRole == "KICKER" ? "키커" : "골키퍼");
                InitializePositions();
            }
            else if (line.StartsWith("TURN:"))
            {
                kickerName = line.Split('=')[1];
                lblRound.Text = "Kicker: " + kickerName;
                InitializePositions();
            }

            else if (line.StartsWith("ROUND_START"))
            {
                var parts = line.Split('|');
                int roundNum = int.Parse(parts[1].Split('=')[1]);
                lblStatus.Text = $"{roundNum} 라운드 시작! (10초 안에 선택)";
                SetTargetsEnabled(true);
                InitializePositions();
            }


            else if (line.StartsWith("RESULT:"))
            {
                var parts = line.Split('|');
                string result = parts[0].Split(':')[1];
                string[] sc = parts[1].Split('=')[1].Split(':');
                string shooterDir = parts[2].Split('=')[1];
                string keeperDir = parts[3].Split('=')[1];

                p1 = int.Parse(sc[0]);
                p2 = int.Parse(sc[1]);
                lblScore.Text = $"Score {p1}:{p2}";

                AnimateShoot(shooterDir);
                ChangeGoalKeeperImage(keeperDir);

                lblStatus.Text = result == "goal" ? "⚽ 골!" : "🧤 세이브!";

                var delayTimer = new System.Windows.Forms.Timer { Interval = 2000 };
                delayTimer.Tick += (s, e) =>
                {
                    delayTimer.Stop();
                    InitializePositions();
                    SetTargetsEnabled(false);
                };
                delayTimer.Start();
            }



            else if (line.StartsWith("GAME_OVER"))
            {
                shootTimer?.Stop();
                keeperTimer?.Stop();

                InitializePositions(); // 공 & 골키퍼 원위치
                SetTargetsEnabled(false);

                var winner = line.Split('|').First(x => x.StartsWith("winner=")).Split('=')[1];
                var final = line.Split('|').First(x => x.StartsWith("final=")).Split('=')[1];
                MessageBox.Show($"Winner: {winner}\nFinal: {final}", "Game Over");
                lblStatus.Text = "게임 종료";
            }
        }



        private void SetTargetsEnabled(bool enabled)
        {
            foreach (var pb in new[] { topLeft, top, topRight, left, right })
                pb.Enabled = enabled;
        }

        private void AnimateShoot(string direction)
        {
            int targetX = football.Left;
            int targetY = goalBackground.Top + 180;

            switch (direction)
            {
                case "left": targetX = goalBackground.Left + 200; break;
                case "right": targetX = goalBackground.Left + 500; break;
                case "top": targetX = goalBackground.Left + 350; targetY = goalBackground.Top + 100; break;
                case "topLeft": targetX = goalBackground.Left + 200; targetY = goalBackground.Top + 100; break;
                case "topRight": targetX = goalBackground.Left + 500; targetY = goalBackground.Top + 100; break;
            }

            shootTimer?.Stop();
            shootTimer = new System.Windows.Forms.Timer { Interval = 10 };
            shootTimer.Tick += (s, e) =>
            {
                football.Left += (targetX - football.Left) / 10;
                football.Top += (targetY - football.Top) / 10;
                if (Math.Abs(football.Left - targetX) < 5 && Math.Abs(football.Top - targetY) < 5)
                {
                    shootTimer.Stop();
                }
            };
            shootTimer.Start();
        }


        private void AnimateGoalkeeper(string direction)
        {
            int targetX = goalKeeper.Left;
            int targetY = goalKeeper.Top;

            switch (direction)
            {
                case "left": targetX = goalBackground.Left + 250; break;
                case "right": targetX = goalBackground.Left + 450; break;
                case "top": targetY = goalBackground.Top + 200; break;
                case "topLeft": targetX = goalBackground.Left + 250; targetY = goalBackground.Top + 200; break;
                case "topRight": targetX = goalBackground.Left + 450; targetY = goalBackground.Top + 200; break;
            }

            keeperTimer?.Stop();
            keeperTimer = new System.Windows.Forms.Timer { Interval = 10 };
            keeperTimer.Tick += (s, e) =>
            {
                goalKeeper.Left += (targetX - goalKeeper.Left) / 10;
                goalKeeper.Top += (targetY - goalKeeper.Top) / 10;
                if (Math.Abs(goalKeeper.Left - targetX) < 5 && Math.Abs(goalKeeper.Top - targetY) < 5)
                {
                    keeperTimer.Stop();
                }
            };
            keeperTimer.Start();
        }

    }
}
