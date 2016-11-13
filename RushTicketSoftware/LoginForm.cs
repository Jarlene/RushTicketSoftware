﻿using RushTicketSoftware.Common;
using RushTicketSoftware.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RushTicketSoftware
{
    public partial class LoginForm : Form
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<PicPoint> _pointList = new List<PicPoint>();
        public static CookieContainer cookieContainer = new CookieContainer();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            //this.btValiPic_Click(sender, e);
            RequestHelper.GetCookie(cookieContainer);
            this.btValiPic_Click(sender, e);
            this.tbName.Text = "chen_peng7";
            this.tbPassword.Text = "cp2290812";
            log.Info("进入程序");
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            string name = this.tbName.Text;
            string password = this.tbPassword.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("用户名不能为空");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            //MessageBox.Show(string.Format("账号：{0} 密码：{1}", name, password));
            var strPoints = CommonHelper.GetPointsStr(_pointList);
            //校验验证码与验证账号密码
            if (RequestHelper.CheckValidatePic(strPoints, cookieContainer, Encoding.UTF8) && RequestHelper.DoLogin(strPoints, this.tbName.Text, this.tbPassword.Text, cookieContainer, Encoding.UTF8))
            {
                var webResponse = RequestHelper.GetWebResponse("https://kyfw.12306.cn/otn/index/initMy12306", cookieContainer);
                Stream respStream = webResponse.GetResponseStream();
                StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
                string result = respStreamReader.ReadToEnd();
                log.Info("登录成功");
                MessageBox.Show("登录成功");
                this.Hide();
                var userTickedPanel = new UserTickedPanel();
                userTickedPanel.Show();
            }
            else
            {
                this.btValiPic_Click(sender, e);
                log.Info("登录失败");
                MessageBox.Show("登录失败");
            }

        }

        private void btQuit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btValiPic_Click(object sender, EventArgs e)
        {
            try
            {
                var webResponse = RequestHelper.GetWebResponse("https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand", cookieContainer);
                Stream picStream = webResponse.GetResponseStream();
                Bitmap sourcebm = new Bitmap(picStream);//初始化Bitmap图片
                this.pictureBox1.Image = sourcebm;
                _pointList = new List<PicPoint>();
            }
            catch (Exception ex)
            {
                log.Error("btValiPic_Click", ex);
            }
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            _pointList.Add(new PicPoint() { X = e.X, Y = e.Y });
        }

        private void btValidate_Click(object sender, EventArgs e)
        {
            var points = CommonHelper.GetPointsStr(_pointList);
            if (RequestHelper.CheckValidatePic(points, cookieContainer, Encoding.UTF8))
            {
                log.Info("验证成功");
                MessageBox.Show("验证成功");
            }
            else
            {
                log.Info("验证失败");
                MessageBox.Show("验证失败");
                this.btValiPic_Click(sender, e);
            }
            //this.btValiPic_Click(sender, e);
        }

        
    }
}
