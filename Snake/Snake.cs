using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Snake
{
    public partial class Snake : Form
    {
        private void Snake_Resize(object sender, EventArgs e)
        {
            
            rows = this.ClientSize.Height / Cell;
            cols = this.ClientSize.Width / Cell;
            if (isRunning) return;

            timer.Stop();
            InitGame();
            timer.Start();
        }
        const int Cell = 20;
        int cols, rows;

        int score = 0;
        bool paused = false;
        bool isRunning = false;

        int dx = Cell, dy = 0;
        int nextDx = Cell, nextDy = 0;

        LinkedList<Piece> snake = new LinkedList<Piece>();
        bool[,] visit;

        Random rand = new Random();
        Timer timer = new Timer();

        public Snake()
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.Resize += Snake_Resize;

            rows = this.ClientSize.Height / Cell;
            cols = this.ClientSize.Width / Cell;

            InitGame();
            LaunchTimer();
        }

        private void LaunchTimer()
        {
            timer.Interval = 150;
            timer.Tick += Move;
            timer.Start();
        }

        private void InitGame()
        {
            foreach (var p in snake)
                Controls.Remove(p);

            snake.Clear();

            visit = new bool[rows, cols];
            score = 0;
            paused = false;
            isRunning = false;

            int startRow = rows / 2;
            int startCol = cols / 2;

            Piece head = new Piece(startCol * Cell, startRow * Cell);
            snake.AddFirst(head);

            visit[startRow, startCol] = true;
            Controls.Add(head);

            dx = Cell; dy = 0;
            nextDx = Cell; nextDy = 0;

            RandomFood();

            lblScore.Text = "Score: 0  |  SPACE = Pause  |  R = Restart";
            isRunning = true;
        }

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

            // Wrap around
            if (newX < 0) newX = maxX - Cell;
            if (newX >= maxX) newX = 0;
            if (newY < 0) newY = maxY - Cell;
            if (newY >= maxY) newY = 0;

            int newRow = newY / Cell;
            int newCol = newX / Cell;

            bool ateFood = IsFood(newX, newY);

            if (visit[newRow, newCol])
            {
                EndGame("Hit your own body!");
                return;
            }

            if (ateFood)
            {
                score++;
                lblScore.Text = $"Score: {score}  |  SPACE = Pause  |  R = Restart";

                Piece newHead = new Piece(newX, newY);
                snake.AddFirst(newHead);

                visit[newRow, newCol] = true;
                Controls.Add(newHead);

                RandomFood();

                timer.Interval = Math.Max(40, timer.Interval - 5);
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
                    timer.Interval = 150;
                    timer.Start();
                    break;
            }
        }

        private void RandomFood()
        {
            List<int> free = new List<int>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (!visit[i, j])
                        free.Add(i * cols + j);

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
                new Start().Show();
                this.Close();
            }
            else
            {
                Application.Exit();
            }
        }
    }
}