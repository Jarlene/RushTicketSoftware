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
        private static List<PicPoint> _pointList = new List<PicPoint>();
        private static CookieContainer cookieContainer = new CookieContainer();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            //this.btValiPic_Click(sender, e);
            RequestHelper.GetCookie(cookieContainer);
            this.btValiPic_Click(sender, e);
        }

        private void btLogin_Click(object sender, EventArgs e)
        {
            //string name = this.tbName.Text;
            //string password = this.tbPassword.Text;
            //MessageBox.Show(string.Format("账号：{0} 密码：{1}", name, password));
            var strPoints = CommonHelper.GetPointsStr(_pointList);
            RequestHelper.CheckValidatePic(strPoints, cookieContainer, Encoding.UTF8);
            var webResponse = RequestHelper.DoLogin(strPoints, this.tbName.Text, this.tbPassword.Text, cookieContainer, Encoding.UTF8);

        }

        private void btQuit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btValiPic_Click(object sender, EventArgs e)
        {

            var webResponse = RequestHelper.GetWebResponse("https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand", cookieContainer);
            Stream picStream = webResponse.GetResponseStream();
            Bitmap sourcebm = new Bitmap(picStream);//初始化Bitmap图片
            this.pictureBox1.Image = sourcebm;
            _pointList = new List<PicPoint>();
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            _pointList.Add(new PicPoint() { X = e.X, Y = e.Y });
        }

        private void btValidate_Click(object sender, EventArgs e)
        {
            var points = CommonHelper.GetPointsStr(_pointList);
            RequestHelper.CheckValidatePic(points, cookieContainer, Encoding.UTF8);
            //this.btValiPic_Click(sender, e);
        }

        
    }
}
