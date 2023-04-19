using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NTM
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();


        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            // TODO: 根据用户名和密码验证用户身份

            try
            {
                string sql = string.Format("select password from user_table where username='{0}'", username);//根据账户查询密码
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                int ii = Convert.ToInt32(cmd.ExecuteScalar());
                if (ii == 0)
                {
                    // 登录失败，提示用户重新输入
                    MessageBox.Show("用户名或密码错误，请重新输入。", "登录失败");
                    txtUsername.Text = "";
                    txtPassword.Password = "";
                }
                else
                {
                    string true_password = "";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        true_password = reader[0].ToString()!;
                    }
                    reader.Close();
                    if (username.Length == 4 && true_password == password)
                    {
                        // 登录成功，跳转到管理界面
                        ManagementWindow managementWindow = new ManagementWindow();
                        managementWindow.Show();
                        this.Close();
                    }
                    else if (username.Length == 6 && true_password == password)
                    {
                        // 登录成功，跳转到教师界面
                        TeacherWindow teacherWindow = new TeacherWindow(username );
                        teacherWindow.Show();
                        this.Close();
                    }
                    else if (username.Length == 8 && true_password == password)
                    {
                        // 登录成功，跳转到学生界面
                        StudentWindow studentWindow = new StudentWindow(username);
                        studentWindow.Show();
                        this.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
