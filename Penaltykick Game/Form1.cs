using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Penaltykick_Game
{
    public partial class Form1 : Form
    {
        private NetClient net = new NetClient();
        private List<PictureBox> goalTarget;
        private string myRole = "-";
        private string kickerName = "-";
        private int p1 = 0, p2 = 0;
        private System.Windows.Forms.Timer? shootTimer;
        private System.Windows.Forms.Timer? keeperTimer;

        // 🟡 초기 위치 저장용
        private Point initialBallPosition;
        private Point initialKeeperPosition;
        private Size initialKeeperSize;

        // 🛑 애니메이션 중복 방지용
        private bool isAnimating = false;

        public Form1()
        {
            InitializeComponent();
            goalTarget = new List<PictureBox> { topLeft, top, topRight, left, right };

            net.OnLine += OnLine;

            // Parent를 goalBackground로 고정
            goalKeeper.Parent = goalBackground;
            football.Parent = goalBackground;
            foreach (var t in goalTarget) t.Parent = goalBackground;

            this.Load += Form1_Load;
            this.Resize += (s, e) => PositionElements();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PositionElements();

            // 📌 초기 위치 저장
            initialBallPosition = football.Location;
            initialKeeperPosition = goalKeeper.Location;
            initialKeeperSize = goalKeeper.Size;
        }

        private void PositionElements()
        {
            // 🟩 초록색 배경
            goalBackground.Width = 915;
            goalBackground.Height = 717;
            goalBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            goalBackground.Left = (this.ClientSize.Width - goalBackground.Width) / 2;
            goalBackground.Top = 110;
            

            // 🧤 골키퍼
            goalKeeper.Width = 80;
            goalKeeper.Height = 130;
            goalKeeper.Location = new Point(
                (goalBackground.Width - goalKeeper.Width) / 2,
                190
            );

            // ⚽ 축구공
            football.Width = 50;
            football.Height = 50;
            football.Location = new Point(
                (goalBackground.Width - football.Width) / 2,
                530
            );

            // 🟡 타겟
            topLeft.Location = new Point(200, 80);
            top.Location = new Point(437, 80);
            topRight.Location = new Point(680, 80);
            left.Location = new Point(200, 245);
            right.Location = new Point(680, 245);

            foreach (var t in goalTarget)
            {
                t.Width = 50;
                t.Height = 50;
                t.BackColor = Color.Yellow;
                t.SizeMode = PictureBoxSizeMode.StretchImage;
                t.BringToFront();
            }

            goalKeeper.BackColor = Color.Transparent;
            football.BackColor = Color.Transparent;

            goalKeeper.BringToFront();
            football.BringToFront();
        }

        private void InitializePositions()
        {
            PositionElements();
            goalKeeper.Image = Properties.Resources.stand_small;
        }

        // 🧭 위치 리셋 함수
        private void ResetPositions()
        {
            goalKeeper.Location = initialKeeperPosition;
            goalKeeper.Size = initialKeeperSize;
            goalKeeper.Image = Properties.Resources.stand_small;

            football.Location = initialBallPosition;

            shootTimer?.Stop();
            keeperTimer?.Stop();
            shootTimer = null;
            keeperTimer = null;

            isAnimating = false;
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (await net.Connect(txtHost.Text.Trim(), int.Parse(txtPort.Text.Trim())))
                lblStatus.Text = "상태: 서버 연결 완료";
            else
                lblStatus.Text = "상태: 서버 연결 실패";
        }

        private async void btnRegister_Click(object sender, EventArgs e) =>
            await net.Send($"REGISTER {txtUser.Text.Trim()} {txtPass.Text}");

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (net == null)
            {
                MessageBox.Show("서버 연결이 없습니다.");
                return;
            }

            try
            {
                await net.Send($"LOGIN {txtUser.Text.Trim()} {txtPass.Text}");
                MessageBox.Show("로그인 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버 연결 오류: {ex.Message}");
            }
        }


        private async void btnReady_Click(object sender, EventArgs e) =>
            await net.Send("READY");

        private async void Target_Click(object sender, EventArgs e)
        {
            string target = ((PictureBox)sender).Tag.ToString()!;
            if (myRole == "KICKER")
                await net.Send($"SHOOT:{target}");
            else if (myRole == "GOALKEEPER")
                await net.Send($"SAVE:{target}");

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
                AnimateGoalkeeper(keeperDir);
                ChangeGoalKeeperImage(keeperDir);

                lblStatus.Text = result == "goal" ? "⚽ 골!" : "🧤 세이브!";

                // ⏳ 애니메이션 종료 후 위치 초기화
                var delayTimer = new System.Windows.Forms.Timer { Interval = 2000 };
                delayTimer.Tick += (s, e) =>
                {
                    delayTimer.Stop();
                    ResetPositions();
                    SetTargetsEnabled(false);
                };
                delayTimer.Start();
            }
            else if (line.StartsWith("GAME_OVER"))
            {
                shootTimer?.Stop();
                keeperTimer?.Stop();
                InitializePositions();
                SetTargetsEnabled(false);

                var winner = line.Split('|').First(x => x.StartsWith("winner=")).Split('=')[1];
                var final = line.Split('|').First(x => x.StartsWith("final=")).Split('=')[1];
                MessageBox.Show($"Winner: {winner}\nFinal: {final}", "Game Over");
                lblStatus.Text = "게임 종료";
            }
        }

        private void SetTargetsEnabled(bool enabled)
        {
            foreach (var pb in goalTarget)
                pb.Enabled = enabled;
        }

        // ⚽ 공 애니메이션
        private void AnimateShoot(string direction)
        {
            if (isAnimating) return;
            isAnimating = true;

            football.Location = initialBallPosition;

            int targetX = football.Left;
            int targetY = goalBackground.Top + 180;

            switch (direction)
            {
                case "left":
                    targetX = left.Left;
                    targetY = left.Top;
                    break;

                case "right":
                    targetX = right.Left;
                    targetY = right.Top;
                    break;

                case "topLeft":
                    targetX = topLeft.Left;
                    targetY = topLeft.Top;
                    break;

                case "top":
                    targetX = top.Left;
                    targetY = top.Top;
                    break;

                case "topRight":
                    targetX = topRight.Left;
                    targetY = topRight.Top;
                    break;
            }

            shootTimer?.Stop();
            shootTimer = new System.Windows.Forms.Timer();
            shootTimer.Interval = 15;
            shootTimer.Tick += (s, e) =>
            {
                football.Left += (targetX - football.Left) / 8;
                football.Top += (targetY - football.Top) / 8;
                if (Math.Abs(football.Left - targetX) < 4 && Math.Abs(football.Top - targetY) < 4)
                {
                    shootTimer.Stop();
                    shootTimer = null;
                }
            };
            shootTimer.Start();
        }

        // 🧤 골키퍼 애니메이션
        private void AnimateGoalkeeper(string direction)
        {
            // 기본 위치 및 크기 초기화
            goalKeeper.Left = initialKeeperPosition.X;
            goalKeeper.Top = initialKeeperPosition.Y;

            int targetX = goalKeeper.Left;
            int targetY = goalKeeper.Top;

            switch (direction)
            {
                case "left":
                    targetX = left.Left + 10;      // ← 살짝 왼쪽으로 더 이동
                    targetY = left.Top - 20;       // ↑ 위로 살짝
                    break;

                case "right":
                    targetX = right.Left - 140;     // → 오른쪽으로 더 이동
                    targetY = right.Top - 25;
                    break;

                case "topLeft":
                    targetX = topLeft.Left + 10;   // ← 살짝 왼쪽
                    targetY = topLeft.Top + 20;    // ↑ 많이 위로
                    break;

                case "topRight":
                    targetX = topRight.Left - 110;  // → 살짝 오른쪽
                    targetY = topRight.Top + 20;
                    break;

                case "top":
                    targetX = top.Left - 60;
                    targetY = top.Top + 20;        // ↑ 정중앙에서 많이 위로
                    break;
            }

            keeperTimer?.Stop();
            keeperTimer = new System.Windows.Forms.Timer();
            keeperTimer.Interval = 15;
            keeperTimer.Tick += (s, e) =>
            {
                goalKeeper.Left += (targetX - goalKeeper.Left) / 8;
                goalKeeper.Top += (targetY - goalKeeper.Top) / 8;
                if (Math.Abs(goalKeeper.Left - targetX) < 4 && Math.Abs(goalKeeper.Top - targetY) < 4)
                {
                    keeperTimer.Stop();
                    keeperTimer = null;
                }
            };
            keeperTimer.Start();
        }


        private void ChangeGoalKeeperImage(string direction)
        {
            int diveWidth = 160;
            int diveHeight = 100;

            switch (direction)
            {
                case "left":
                case "right":
                    goalKeeper.Size = new Size(180, 110);
                    break;

                case "topLeft":
                case "topRight":
                    goalKeeper.Size = new Size(160, 120);
                    break;

                case "top":
                    goalKeeper.Size = new Size(130, 180);
                    break;

                default:
                    goalKeeper.Size = initialKeeperSize;
                    break;
            }

            

            switch (direction)
            {
                case "left": goalKeeper.Image = Properties.Resources.left_save_small; break;
                case "right": goalKeeper.Image = Properties.Resources.right_save_small; break;
                case "top": goalKeeper.Image = Properties.Resources.top_save_small; break;
                case "topLeft": goalKeeper.Image = Properties.Resources.top_left_save_small; break;
                case "topRight": goalKeeper.Image = Properties.Resources.top_right_save_small; break;
                default: goalKeeper.Image = Properties.Resources.stand_small; break;
            }

            goalKeeper.SizeMode = PictureBoxSizeMode.Zoom;
        }


    }
}
