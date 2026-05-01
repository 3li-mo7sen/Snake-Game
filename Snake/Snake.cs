using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Snake : Form
    {
        const int Cell = 20;

        int cols, rows;
        int score = 0;
        int speed = 150;

        bool paused = false;
        bool isRunning = false;

        int dx = Cell, dy = 0;
        int nextDx = Cell, nextDy = 0;

        LinkedList<Piece> snake = new LinkedList<Piece>();
        bool[,] visit;

        List<PictureBox> obstacles = new List<PictureBox>();

        Random rand = new Random();
        Timer timer = new Timer();

        Start st;

        public Snake(Start st)
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.Resize += Snake_Resize;

            rows = this.ClientSize.Height / Cell;
            cols = this.ClientSize.Width / Cell;

            InitGame();
            LaunchTimer();

            this.st = st;
        }

        // ================= TIMER =================
        private void LaunchTimer()
        {
            timer.Interval = speed;
            timer.Tick += Move;
            timer.Start();
        }

        // ================= RESIZE =================
        private void Snake_Resize(object sender, EventArgs e)
        {
            rows = this.ClientSize.Height / Cell;
            cols = this.ClientSize.Width / Cell;

            timer.Stop();
            InitGame();
            timer.Start();
        }

        // ================= INIT GAME =================
        private void InitGame()
        {
            foreach (var p in snake)
                Controls.Remove(p);

            snake.Clear();

            foreach (var o in obstacles)
                Controls.Remove(o);

            obstacles.Clear();

            visit = new bool[rows, cols];
            score = 0;
            paused = false;
            isRunning = false;

            speed = 150;
            timer.Interval = speed;

            int startRow = rows / 2;
            int startCol = cols / 2;

            Piece head = new Piece(startCol * Cell, startRow * Cell);
            snake.AddFirst(head);

            visit[startRow, startCol] = true;
            Controls.Add(head);

            dx = Cell; dy = 0;
            nextDx = Cell; nextDy = 0;

            CreateObstacles();
            RandomFood();

            lblScore.Text = "Score: 0  |  SPACE = Pause  |  R = Restart";
            isRunning = true;
        }

        // ================= OBSTACLES =================
        private void CreateObstacles()
        {
            int count = 15;

            for (int i = 0; i < count; i++)
            {
                PictureBox block = new PictureBox();
                block.Size = new Size(3 * Cell, 3 * Cell);
                block.SizeMode = PictureBoxSizeMode.StretchImage;
                block.BackColor = Color.Transparent;

                block.ImageLocation = "C:\\Users\\UG\\Documents\\GitHub\\Snake-Game\\Snake\\wall.png";

                int r, c;

                do
                {
                    r = rand.Next(rows);
                    c = rand.Next(cols);
                }
                while (visit[r, c]);

                block.Location = new Point(c * Cell, r * Cell);

                obstacles.Add(block);
                Controls.Add(block);
            }
        }

        private bool IsObstacle(int x, int y)
        {
            Rectangle snakeHead = new Rectangle(x, y, Cell, Cell);

            foreach (var obs in obstacles)
            {
                Rectangle obstacleRect = new Rectangle(
                    obs.Location.X,
                    obs.Location.Y,
                    obs.Width,
                    obs.Height
                );

                if (snakeHead.IntersectsWith(obstacleRect))
                    return true;
            }

            return false;
        }

        // ================= MOVE =================
        private void Move(object sender, EventArgs e)
        {
            if (!isRunning || paused) return;

            dx = nextDx;
            dy = nextDy;


            var head = snake.First.Value;

            int newX = head.Location.X + dx;
            int newY = head.Location.Y + dy;

            int maxX = cols * Cell;
            int maxY = rows * Cell;

            // wrap
            if (newX < 0) newX = maxX - Cell;
            if (newX >= maxX) newX = 0;
            if (newY < 0) newY = maxY - Cell;
            if (newY >= maxY) newY = 0;

            int newRow = newY / Cell;
            int newCol = newX / Cell;

            // hit wall
            if (IsObstacle(newX, newY))
            {
                EndGame("Hit a wall!");
                return;
            }

            bool ateFood = IsFood(newX, newY);

            // hit self
            if (visit[newRow, newCol])
            {
                EndGame("Hit your own body!");
                return;
            }

            if (ateFood)
            {
                score++;
                lblScore.Text = $"Score: {score}";

                Piece newHead = new Piece(newX, newY);
                snake.AddFirst(newHead);

                visit[newRow, newCol] = true;
                Controls.Add(newHead);

                RandomFood();

                // 🔥 SPEED INCREASE (FIXED)
                speed = Math.Max(40, speed - 5);

                timer.Stop();
                timer.Interval = speed;
                timer.Start();
            }
            else
            {
                var tail = snake.Last.Value;

                int tailRow = tail.Location.Y / Cell;
                int tailCol = tail.Location.X / Cell;

                visit[tailRow, tailCol] = false;

                snake.RemoveLast();

                tail.Location = new Point(newX, newY);
                snake.AddFirst(tail);

                visit[newRow, newCol] = true;
            }
        }

        // ================= INPUT =================
        private void Snake_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (nextDx != -Cell) { nextDx = Cell; nextDy = 0; }
                    break;

                case Keys.Left:
                    if (nextDx != Cell) { nextDx = -Cell; nextDy = 0; }
                    break;

                case Keys.Up:
                    if (nextDy != Cell) { nextDx = 0; nextDy = -Cell; }
                    break;

                case Keys.Down:
                    if (nextDy != -Cell) { nextDx = 0; nextDy = Cell; }
                    break;

                case Keys.Space:
                case Keys.P:
                    paused = !paused;
                    break;

                case Keys.R:
                    timer.Stop();
                    InitGame();
                    timer.Start();
                    break;
            }
        }

        // ================= FOOD =================
        private void RandomFood()
        {
            List<int> free = new List<int>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int x = j * Cell;
                    int y = i * Cell;

                    if (!visit[i, j] && !IsObstacle(x, y))
                        free.Add(i * cols + j);
                }
            }

            if (free.Count == 0)
            {
                EndGame("YOU WIN!");
                return;
            }

            int cell = free[rand.Next(free.Count)];
            lblFood.Location = new Point((cell % cols) * Cell, (cell / cols) * Cell);
        }

        private bool IsFood(int x, int y)
        {
            return x == lblFood.Location.X && y == lblFood.Location.Y;
        }

        // ================= GAME OVER =================
        private void EndGame(string msg)
        {
            isRunning = false;
            timer.Stop();

            var res = MessageBox.Show(
                msg + "\n\nReturn to Start?",
                "Game Over",
                MessageBoxButtons.YesNo
            );

            if (res == DialogResult.Yes)
            {
                st.Show();
                this.Close();
            }
            else
            {
                Application.Exit();
            }
        }
    }
}