using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace k子棋
{
    public partial class Form2 : Form
    {
        private int[,] chess = new int[15, 15];//虚拟棋盘
        private PictureBox[,] chessPictureBox = new PictureBox[15, 15];// 棋子落子棋盘（真实棋盘）
        private const int black = 2, white = 1, background = 0;
        private int PLAYER;
        private int totalCount = 0, count, LinkCount = 0;
        public bool gamestart = false;
        public int nowx, nowy; // 最近下的棋子
        public const int M = 1; // 单局落子个数
        public const int K = 5; // 连子胜利数个数
        public const int North = 1;
        public const int South = 2;
        public const int East = 3;
        public const int West = 4;
        public const int NorthEast = 5;
        public const int NorthWest = 6;
        public const int SouthEast = 7;
        public const int SouthWest = 8;
        System.Diagnostics.Stopwatch sw1, sw2;
        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;  // 窗体居中显示
            this.FormClosing += Form2_FormColsing;  // 程序决定是否关闭时事件
            this.MaximizeBox = false;
            this.MinimizeBox = false;  // 去掉窗体最大最小化按钮
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;  // 窗体不可拉伸

            chessBorad.Paint += new PaintEventHandler(chessBorad_Paint);
            InitializeChessBoard();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer2.Tick += new EventHandler(Timer2_Tick);   // 计时事件每秒更新
            //chessBorad.MouseMove += new MouseEventHandler(chessBorad_MouseMove);
            // this.MouseMove += new MouseEventHandler(Form2_MouseMove);
            //chessBorad.MouseClick += new MouseEventHandler(chessBorad_Click);
        }

        private void InitializeChessBoard()//初始化棋盘
        {
            //chessBorad.Paint += new PaintEventHandler(chessBorad_Paint);
            int x, y;
            for (x = 0; x < 15; x++)
                for (y = 0; y < 15; y++)
                {
                    chess[x, y] = 0;
                    chessPictureBox[x, y] = new PictureBox();
                    chessPictureBox[x, y].Location = new Point(20 + x * 30, 20 + y * 30); // 定义为棋盘上的点
                    chessPictureBox[x, y].Size = new Size(30, 30);
                    chessPictureBox[x, y].BackColor = Color.Transparent;
                    chessPictureBox[x, y].SizeMode = PictureBoxSizeMode.CenterImage;
                    chessPictureBox[x, y].Visible = false;  // 因没有图片所以无所谓  
                    chessBorad.Controls.Add(chessPictureBox[x, y]);
                }
        }
        //画出棋盘上的格子：
        private void chessBorad_Paint(object sender, PaintEventArgs e) // 画棋盘线
        {

            int i;
            Graphics gr = e.Graphics;
            Pen myPen = new Pen(Color.Black, 1);   // 定义画笔
            SolidBrush brush = new SolidBrush(Color.Black); // 定义画刷
            for (i = 0; i < 15; i++)
            {
                gr.DrawLine(myPen, 20 + i * 30, 20, 20 + i * 30, 440);  // 画竖线
                gr.DrawLine(myPen, 20, 20 + i * 30, 440, 20 + i * 30);  // 画横线
            }
            gr.FillEllipse(brush, 226, 226, 8, 8);  // 天元圆点
            gr.FillEllipse(brush, 107, 107, 6, 6);
            gr.FillEllipse(brush, 347, 107, 6, 6);
            gr.FillEllipse(brush, 107, 347, 6, 6);
            gr.FillEllipse(brush, 347, 347, 6, 6);  // 四个星位的圆点

        }

        private void PutAstone(object sender, PaintEventArgs e) // 画棋盘线
        {
            Graphics gr = e.Graphics;
            SolidBrush brush;
            if (PLAYER == 0) brush = new SolidBrush(Color.Black);   // 定义画笔
            else brush = new SolidBrush(Color.White);
            int x, y;
            for (x = 0; x < 15; x++)
                for (y = 0; y < 15; y++)
                {
                    int Movex, Movey;
                    Movex = x * 30 + 20;  // 离边框20，格子宽30
                    Movey = y * 30 + 20;
                    if (chess[x, y] == black) 
                    {
                        brush = new SolidBrush(Color.Black);
                        gr.FillEllipse(brush, Movex - 15, Movey - 15, 30, 30);  // 着子
                    }
                    else if (chess[x, y] == white)
                    {
                        brush = new SolidBrush(Color.White);
                        gr.FillEllipse(brush, Movex - 15, Movey - 15, 30, 30);  // 着子
                    }
                    
                }
        }

        private void button1_Click(object sender, EventArgs e) // 开始按钮
        {
            if (gamestart)
            {
                Array.Clear(chess,0,chess.Length); 
            }
            else
            {
                button1.Text = "重新开始";
                gamestart = true;
            }
            count = 0;
            PLAYER = 0;  // 0代表黑方
            sw1 = new System.Diagnostics.Stopwatch();
            sw2 = new System.Diagnostics.Stopwatch();  // 定义两个计时器
            sw1.Start();
            timer1.Start();  // 黑方先开始计时
            chessBorad.Refresh();
        }

        private void button2_Click(object sender, EventArgs e) // 悔棋按钮
        {
            if (!gamestart) return;  // 游戏未开始使按钮无效
            
            
            if (0 < count) { chess[nowx, nowy] = 0; count--; }       
            chessBorad.Refresh();
        }
        private void button3_Click(object sender, EventArgs e) // 确认按钮
        {
            if (!gamestart) return;
            if (count!=M)
            {
                MessageBox.Show("未落子", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            
            count = 0;
            checkResult();
            if (PLAYER == 0)
            {
                sw1.Stop();
                timer1.Stop();
                sw2.Start();
                timer2.Start();
            }
            else
            {
                sw2.Stop();
                timer2.Stop();
                sw1.Start();
                timer1.Start();
            }
                PLAYER ^= 1; // 切换玩家 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan ts1 = sw1.Elapsed;
            label1.Text = String.Format("{0:00}:{1:00}", ts1.Minutes, ts1.Seconds);
        }
        private void Timer2_Tick(object sender, EventArgs e)
        {
            TimeSpan ts2 = sw2.Elapsed;
            label3.Text = String.Format("{0:00}:{1:00}", ts2.Minutes, ts2.Seconds);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 100;
            timer1.Stop();
            timer2.Enabled = true;
            timer2.Interval = 100;
            timer2.Stop();
        }

        private void Form2_FormColsing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("是否退出程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void checkResult()  // 判断是否胜利
        {
            // 胜利需判断四条线；
            // 使用深度优先搜索，代码量应该比for循环少
            bool win = deepsearch(nowx, nowy, 0, PLAYER); ;
            if(win) // 如果胜利
            {
                sw1.Stop();
                timer1.Stop();
                sw2.Stop();
                timer2.Stop();
                string ss="";
                if (PLAYER == 0) ss = "黑方";
                else ss = "白方";
                MessageBox.Show(ss + "获胜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                Array.Clear(chess,0,chess.Length); // 清空棋盘
                gamestart = false; // 关闭游戏对弈，需要点重新开始后才可继续对弈
            }
        }
        private bool deepsearch(int x,int y,int direct,int PLAYer)
        {
            if (direct == 0)
            {
                // 先进行竖线搜索
                LinkCount = 1; // 当前落子点有一颗子所以初始化为1
                deepsearch(x, y - 1, North, PLAYer);
                deepsearch(x, y + 1, South, PLAYer);
                if (LinkCount >= K) return true;  // 大于设定的k个子则返回胜利
                // 否则进行横线搜索
                LinkCount = 1;
                deepsearch(x + 1, y, East, PLAYer);
                deepsearch(x - 1, y, West, PLAYer);
                if (LinkCount >= K) return true;
                // 以下进行正斜反斜搜索
                LinkCount = 1;
                deepsearch(x + 1, y - 1, NorthEast, PLAYer);
                deepsearch(x - 1, y + 1, SouthWest, PLAYer);
                if (LinkCount >= K) return true;
                LinkCount = 1;
                deepsearch(x - 1, y - 1, NorthWest, PLAYer);
                deepsearch(x + 1, y + 1, SouthEast, PLAYer);
                if (LinkCount >= K) return true;
            }
            // 回溯条件
            if (x >= 15 || y >= 15 || x < 0 || y < 0) return false;  // 越界返回
            if (chess[x, y] == 0) return false; // 空子返回
            if (chess[x, y] == white && PLAYer == 0) return false;  // 对手子返回
            if (chess[x, y] == black && PLAYer == 1) return false;
            LinkCount++; // 连子数+1
            // 一旦进入搜索,就持续向一个方向搜索
            if (direct == North) { deepsearch(x, y - 1, direct, PLAYer); } 
            else if (direct == South) { deepsearch(x, y + 1, direct, PLAYer); }
            else if (direct == East) { deepsearch(x + 1, y, direct, PLAYer); }
            else if (direct == West) { deepsearch(x - 1, y, direct, PLAYer); }
            else if (direct == NorthEast) { deepsearch(x + 1, y - 1, direct, PLAYer); }
            else if (direct == NorthWest) { deepsearch(x - 1, y - 1, direct, PLAYer); }
            else if (direct == SouthEast) { deepsearch(x + 1, y + 1, direct, PLAYer); }
            else if (direct == SouthWest) { deepsearch(x - 1, y + 1, direct, PLAYer); }
            return false; // 该false在执行完上面递归函数后触发，及在“归”的过程触发
        }


        private void chessBorad_Click(object sender, EventArgs e)
        {
            if (!gamestart) return;  // 锁
            if(count>=M)  // 如果落子已经达到每轮落子则提示后返回
            {
                MessageBox.Show("已达到落子数", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            
            int Lx = chessBorad.PointToClient(MousePosition).X + 15;  // 15为偏移值处理
            int Ly = chessBorad.PointToClient(MousePosition).Y + 15;  // 获取实际棋盘上的坐标
           
            Lx = Lx - 20;  // 选取的与上、左边框有20的初值差
            Ly = Ly - 20;
            if (Lx < 0 || Ly < 0 || Lx >= 450 || Ly >= 450) 
            {
                MessageBox.Show("点击位置错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            int Vx = Lx / 30;  // 计算在虚拟棋盘中的位置
            int Vy = Ly / 30;
            if (chess[Vx, Vy] != 0)  // 该点已经落子
            {
                MessageBox.Show("该点有子，请尝试其他点", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (PLAYER == 0)   // 判断落子颜色
            {
                chess[Vx, Vy] = black;
            }
            else if (PLAYER == 1)
            {
                chess[Vx, Vy] = white;
            }             
            else
            {
                MessageBox.Show("玩家置换错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            count++;
            nowx = Vx;
            nowy = Vy;  // 记录落子位置 方便悔棋及扩展传递
            chessBorad.Paint += new PaintEventHandler(PutAstone);
            chessBorad.Refresh();     // 刷新棋盘
        }
    }
}
