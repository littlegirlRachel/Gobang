using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace k子棋
{
    public partial class Form2 : Form
    {
        
        public const int M = 1; // 单局落子个数
        public const int K = 5; // 连子胜利数个数

        private int[,] chess = new int[15, 15];//虚拟棋盘
        private PictureBox[,] chessPictureBox = new PictureBox[15, 15];// 棋子落子棋盘（真实棋盘）
        private const int black = 2, white = 1, background = 0;  // 对每个棋子的编码
        private int PLAYER;       // 当前玩家
        private int count = 0;    // count落子计数器
        private int LinkCount = 0;  // 没落子一颗时检查是否有连5的计数器，因用的是递归深度搜索，所以声明为全局变量。
        public bool gamestart = false;  // 游戏是否开始开关
        public int nowx, nowy; // 最近下的棋子坐标
        
        // 定义8个方向
        public const int North = 1;
        public const int South = 2;
        public const int East = 3;
        public const int West = 4;
        public const int NorthEast = 5;
        public const int NorthWest = 6;
        public const int SouthEast = 7;
        public const int SouthWest = 8;

        // 定义停表
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
        }

        private void InitializeChessBoard()//初始化棋盘
        {
            chessBorad.Paint += new PaintEventHandler(chessBorad_Paint);
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
            {
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
        }
        private void button1_Click(object sender, EventArgs e) // 开始按钮
        {
            if (gamestart) // 从开始状态 -》 开始状态
            {
                Array.Clear(chess,0,chess.Length); 
            }
            else  // 从未开始状态 -》 开始状态
            {
                button1.Text = "重新开始";
                gamestart = true;
            }
            PLAYER = 0;  // 0代表黑方
            count = 0;
            time_Init();
            sw1.Start();
            timer1.Start();  // 黑方先开始计时
            chessBorad.Refresh();
        }
        private void button2_Click(object sender, EventArgs e) // 悔棋按钮
        {
            if (!gamestart) return;  // 游戏未开始使按钮无效
            if (0 < count) { chess[nowx, nowy] = 0; count--; }  // 将最近所下的棋子清0，玩家着子数减1     
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
            count = 0;  // 使下个选手落子计数器重置
            if (PLAYER == 0)  // 计时器交换计时
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
            checkResult();  // 检查胜利
            
            PLAYER ^= 1; // 切换玩家 
        }

        private void time_Init()   // 关于计时器的初始化
        {
            sw1 = new System.Diagnostics.Stopwatch();
            sw2 = new System.Diagnostics.Stopwatch();  // 定义两个计时器
            // 刷新两个时间label的显示
            label1.Text = String.Format("{0:00}:{1:00}", sw1.Elapsed.Minutes, sw1.Elapsed.Seconds);
            label3.Text = String.Format("{0:00}:{1:00}", sw2.Elapsed.Minutes, sw2.Elapsed.Seconds);
        }
        private void timer1_Tick(object sender, EventArgs e)  // 每秒始终自动触发事件
        {
            TimeSpan ts1 = sw1.Elapsed;
            label1.Text = String.Format("{0:00}:{1:00}", ts1.Minutes, ts1.Seconds); // 设置时钟格式 00：00格式,这样的设计对于五子棋一般足够
        }
        private void Timer2_Tick(object sender, EventArgs e)
        {
            TimeSpan ts2 = sw2.Elapsed;
            label3.Text = String.Format("{0:00}:{1:00}", ts2.Minutes, ts2.Seconds);
        }

        private void button6_Click(object sender, EventArgs e)    // 结束按钮
        {
            gamestart = false;
            button1.Text = "开始";
            sw1.Stop();
            timer1.Stop();
            sw2.Stop();
            timer2.Stop();
            time_Init(); // 停表初始化
            Array.Clear(chess, 0, chess.Length); // 清空棋盘
            chessBorad.Refresh(); // 刷新界面
        }

        private void Form2_Load(object sender, EventArgs e)  // 在加载窗口时将时钟设置
        {
            timer1.Enabled = true;
            timer1.Interval = 100;  // 1s触发一次事件
            timer1.Stop();
            timer2.Enabled = true;
            timer2.Interval = 100;
            timer2.Stop();
        }

        private void checkResult()  // 判断是否胜利
        {
            // 胜利需判断四条线；
            // 使用深度优先搜索，代码量比for循环少，更易读
            bool win = deepsearch(nowx, nowy, 0, PLAYER); ;
            if(win) // 如果胜利
            {
                sw1.Stop();
                timer1.Stop();
                sw2.Stop();
                timer2.Stop();  // 停止计时
                string winner="";
                if (PLAYER == 0) winner = "黑方";
                else winner = "白方";
                MessageBox.Show(winner + "获胜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                Array.Clear(chess,0,chess.Length); // 清空棋盘
                gamestart = false; // 关闭游戏对弈，需要点重新开始后才可继续对弈
            }
        }
        private bool deepsearch(int x,int y,int Direction,int PLAYer) // 使用深度优先搜索,判断连子数
        {
            if (Direction == 0)  // 第一次进入deepsearch，即deepsearch主函数
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
            if (chess[x, y] == black && PLAYer == 1) return false;  // 对手子返回
            LinkCount++; // 连子数+1
            // 一旦进入搜索,就持续向一个方向搜索
            if (Direction == North) { deepsearch(x, y - 1, Direction, PLAYer); } 
            else if (Direction == South) { deepsearch(x, y + 1, Direction, PLAYer); }
            else if (Direction == East) { deepsearch(x + 1, y, Direction, PLAYer); }
            else if (Direction == West) { deepsearch(x - 1, y, Direction, PLAYer); }
            else if (Direction == NorthEast) { deepsearch(x + 1, y - 1, Direction, PLAYer); }
            else if (Direction == NorthWest) { deepsearch(x - 1, y - 1, Direction, PLAYer); }
            else if (Direction == SouthEast) { deepsearch(x + 1, y + 1, Direction, PLAYer); }
            else if (Direction == SouthWest) { deepsearch(x - 1, y + 1, Direction, PLAYer); }
            return false; // 该false在执行完上面递归函数后触发，及在“归”的过程触发
        }

        private void button4_Click(object sender, EventArgs e)  // 导入棋谱
        {
            if(timer1.Enabled)
            {
                sw1.Stop();
                timer1.Stop();
            }
            else if(timer2.Enabled)
            {
                sw2.Stop();
                timer2.Stop();
            }
            else time_Init();
            int blk, wht;  // 导入棋谱时对落子计数，判断当前局面轮到谁下。
            RichTextBox OpenBoard = new RichTextBox();
            OpenFileDialog ofd = new OpenFileDialog(); //新建打开文件对话框
            string path = System.Environment.CurrentDirectory;  // 获取当前路径
            // 需要调整path路径到棋谱路径下
            path = path.Remove(path.LastIndexOf("k子棋\\bin\\Debug")); 
            path += "棋谱";
            ofd.InitialDirectory = path; //设置棋谱文件目录
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*"; //设置打开文件类型
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = ofd.FileName; //FileName就是要打开的文件路径
                OpenBoard.LoadFile(FileName, RichTextBoxStreamType.PlainText); // 读取文本在OpenBoard.Text
                Board_Decode(OpenBoard, out blk, out wht);
                gamestart = true;
                button1.Text = "重新开始";
                if (blk > wht)
                {
                    PLAYER = 1;
                    count = 0;
                    //time_Init();
                    sw2.Start();
                    timer2.Start();  // 白方先开始计时
                }
                else
                {
                    PLAYER = 0;
                    count = 0;
                    //time_Init();
                    sw1.Start();
                    timer1.Start();  // 白方先开始计时
                }
                chessBorad.Paint += new PaintEventHandler(PutAstone);
                chessBorad.Refresh(); // 刷新界面
            }
            else MessageBox.Show("打开失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);          
        }

        void Board_Decode(RichTextBox RTB, out int blk, out int wht)   // 解码函数
        {
            int t;
            blk = 0;
            wht = 0;
            label2.Text = "";
            label4.Text = "";
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    t = RTB.Text[i * 15 + j] - '0';
                    if (t == 1) wht++;
                    else if (t == 2) blk++;
                    chess[i, j] = RTB.Text[i * 15 + j] - '0';  // 变换为整型
                }
            }
            bool f = false;
            int k;
            for (k = 15 * 15 + 1; k < RTB.Text.Length; k++) 
            {
                if (RTB.Text[k] == '*') { if (f) break; f = true;continue; }
                if (!f) label2.Text += RTB.Text[k];
                else label4.Text += RTB.Text[k];
            }
            string temp="";
            for (k = k+1; k < RTB.Text.Length; k++)
            {
                if (RTB.Text[k] == '*')
                {
                    if (f) break;
                    f = true;
                    label1.Text = temp;
                    temp = "";
                    continue;
                }
                if (!f) temp += RTB.Text[k];
                else temp += RTB.Text[k];
            }
            label3.Text = temp;
        }
        private void button5_Click(object sender, EventArgs e)  // 保存棋谱
        {
            string SaveBoard;
            SaveBoard = Board_Encode();
            int fileCount = 1;  // 棋谱文件夹下计数器，默认为1因为将新写入一个棋谱
            string path = @"..\..\..\棋谱";
            var files = Directory.GetFiles(path, "*.txt"); 
            foreach (var file in files)
                fileCount++;
            path += "\\棋谱" + fileCount.ToString() + " " + label2.Text + " vs " + label4.Text + ".txt";  // 加入要写入的棋谱名称
            // 写入棋谱
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);  // 需保存为UTF-8格式 否则读入时将会变为乱码
            sw.WriteLine(SaveBoard);
            sw.Close();
            MessageBox.Show("保存成功");
        }

        string Board_Encode()   // 编码函数
        { 
            string SaveBoard = "";
            for (int i = 0; i < 15; i++) 
            {
                for (int j = 0; j < 15; j++)
                {
                    SaveBoard += chess[i,j].ToString();  // 简单编码：将15*15个格子里的数值输入即可
                }
            }
            SaveBoard += "*" + label2.Text + "*" + label4.Text + "*" + label1.Text + "*" + label3.Text + "*";
            return SaveBoard;
        }
        
        private void chessBorad_Click(object sender, EventArgs e)    // 在棋盘上的点击事件
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
            if (Lx < 0 || Ly < 0 || Lx >= 450 || Ly >= 450)  // 点击位置在棋盘外
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

        private void Form2_FormColsing(object sender, FormClosingEventArgs e)  // 关闭子窗体时也将父窗体关闭
        {
            DialogResult result = MessageBox.Show("是否退出程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
