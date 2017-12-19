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
    
    public partial class Form1 : Form
    {
        public bool flag_off = false;  //  父窗体是否打开子窗体
        public static Form1 FatherForm = new Form1();
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;  // 程序决定是否关闭时事件
            this.FormClosed += Form1_FormClosed;    // 程序关闭后事件
            this.StartPosition = FormStartPosition.CenterScreen;   // 窗体居中显示
            this.MaximizeBox = false;
            this.MinimizeBox = false;  // 去掉窗体最大最小化按钮
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;  // 设置窗体不可拉伸
    
            textBox1.Focus(); // textbox可获得焦点
            textBox1.Text = "请输入姓名";
            textBox1.ForeColor = Color.LightGray;   // 默认显示水印文字
            textBox1.GotFocus += TextBox1_GotFocus;   // 捕捉焦点事件    

            textBox2.Focus(); // textbox可获得焦点
            textBox2.Text = "请输入姓名";
            textBox2.ForeColor = Color.LightGray;   // 默认显示水印文字
            textBox2.GotFocus += TextBox2_GotFocus;   // 捕捉焦点事件    
        }

        public void TextBox1_GotFocus(object sender, EventArgs e)  // 捕捉到焦点,即鼠标点击textbox
        {
            textBox1.Text = "";  //  点击后使textbox为空
            textBox1.ForeColor = Color.Black; // 使字体回归黑色
        }
        public void TextBox2_GotFocus(object sender, EventArgs e)  // 捕捉到焦点,即鼠标点击textbox
        {
            textBox2.Text = "";  //  点击后使textbox为空
            textBox2.ForeColor = Color.Black; // 使字体回归黑色
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!flag_off)  //  从子窗口关闭时，则不显示父窗口提示框
            {
                DialogResult result = MessageBox.Show("是否退出程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.ForeColor == Color.LightGray || textBox2.Text == "" || textBox2.ForeColor == Color.LightGray) 
            {
                MessageBox.Show("请输入姓名!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            this.Hide();   //  隐藏父窗体
            Form2 nowForm = new Form2();
            nowForm.Controls["label2"].Text = this.textBox1.Text;
            nowForm.Controls["label4"].Text = this.textBox2.Text;
            nowForm.ShowDialog();  // 显示子窗体
            flag_off = true;  // 设置已打开，父窗体不再弹出提示框
            this.Close();  // 关闭父窗体，触发Closing事件       
        }

        public void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
