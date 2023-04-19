using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NTM
{
    /// <summary>
    /// TeacherTableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TeacherTableWindow : Window
    {
        List<TeacherInfo> teachers = new();
        public TeacherTableWindow()
        {
            InitializeComponent();
            string sql = string.Format("select * from teacher_table ;");//查询教师的信息
            MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
            MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
            if (mySqlDataReader.HasRows)
            {
                while (mySqlDataReader.Read())
                {
                    teachers.Add(new TeacherInfo(mySqlDataReader[7].ToString()!)
                    {
                        No = mySqlDataReader[1].ToString()!,
                        Name = mySqlDataReader[2].ToString()!,
                        Gender = mySqlDataReader[3].ToString()! == "男",
                        Degree = mySqlDataReader[4].ToString()!,
                        Title = mySqlDataReader[5].ToString()!,
                        Department = mySqlDataReader[6].ToString()!,
                    }
                    );
                }

            }
            mySqlDataReader.Close();

            foreach (TeacherInfo teacher in teachers)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                TextBox teacherNo = new TextBox { Text = teacher.No, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(teacherNo);
                Grid.SetColumn(teacherNo, 0);

                TextBox teacherName = new TextBox { Text = teacher.Name, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(teacherName);
                Grid.SetColumn(teacherName, 1);

                TextBox teacherGender = new TextBox { Text = teacher.Gender ? "男" : "女", HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(teacherGender);
                Grid.SetColumn(teacherGender, 2);

                TextBox teacherTitle = new TextBox { Text = teacher.Title, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(teacherTitle);
                Grid.SetColumn(teacherTitle, 3);

                TextBox teacherDegree = new TextBox { Text = teacher.Degree, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(teacherDegree);
                Grid.SetColumn(teacherDegree, 4);

                TextBox teacherDepartment = new TextBox { Text = teacher.Department, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(teacherDepartment);
                Grid.SetColumn(teacherDepartment, 5);

                TextBox teacherTel = new TextBox { Text = teacher.Tel, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(teacherTel);
                Grid.SetColumn(teacherTel, 6);

                Grid gg = new Grid();
                gg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                gg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btn_update = new Button { Content = "改" };
                btn_update.Click += (obj, e) =>
                {
                    try
                    {
                        updateTeacher(teacher, new TeacherInfo(teacherTel.Text)
                        {
                            No = teacherNo.Text,
                            Name = teacherName.Text,
                            Gender = teacherGender.Text == "男",
                            Title = teacherTitle.Text,
                            Degree = teacherDegree.Text,
                            Department = teacherDepartment.Text,
                        });
                        //不论成功与否都关掉重新加载
                        (new TeacherTableWindow()).Show();
                        this.Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("请检查数据");
                    }
                };
                gg.Children.Add(btn_update);
                Grid.SetColumn(btn_update, 0);

                Button btn_drop = new Button { Content = "删" };
                btn_drop.Click += (obj, e) =>
                {
                    dropTeacher(teacher);
                    //不论成功与否都关掉重新加载
                    (new TeacherTableWindow()).Show();
                    this.Close();
                };
                gg.Children.Add(btn_drop);
                Grid.SetColumn(btn_drop, 1);

                grid.Children.Add(gg);
                Grid.SetColumn(gg, 7);

                ListBox_TeacherTable.Items.Add(grid);
            }

        }

        void updateTeacher(TeacherInfo oldTeacher, TeacherInfo newTeacher)
        {
            /*更新教师信息的函数*/
            try
            {
                string gender = newTeacher.Gender ? "男" : "女";
                string sql = $"update teacher_table set name = '{newTeacher.Name}',teacher_no = '{newTeacher.No}',gender = '{gender}'" +
                    $",degree = '{newTeacher.Degree}', department = '{newTeacher.Department}', tel = '{newTeacher.Tel}' where teacher_no='{oldTeacher.No}';";//更改课程信息
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                {
                    MessageBox.Show("修改成功！");
                }
                else
                {
                    MessageBox.Show("修改失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void dropTeacher(TeacherInfo Teacher)
        {
            /*删除教师信息的函数*/
            try
            {
                string sql = $"delete from  teacher_table where teacher_no = '{Teacher.No}';";//更改课程信息
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                {
                    MessageBox.Show("删除成功！");
                }
                else
                {
                    MessageBox.Show("删除失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void insertTeacher(TeacherInfo Teacher)
        {
            /*插入教师信息的函数*/
            try
            {
                string sql = $"insert into teacher_table(teacher_no, name, gender, degree, department, title, tel) " +
                    $"values('{Teacher.No}','{Teacher.Name}','{Teacher.Gender}','{Teacher.Degree}','{Teacher.Department}', '{Teacher.Title}', '{Teacher.Tel}');";//更改课程信息
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                {
                    MessageBox.Show("插入成功！");
                }
                else
                {
                    MessageBox.Show("插入失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Insert_Click(object sender, RoutedEventArgs e)
        {
            insertTeacher(new TeacherInfo(TextBox_Tel.Text)
            {
                No = TextBox_No.Text,
                Name = TextBox_Name.Text,
                Gender = TextBox_Gender.Text == "男",
                Title = TextBox_Gender.Text,
                Degree = TextBox_Degree.Text,
                Department = TextBox_Department.Text,
            });
            (new TeacherTableWindow()).Show();
            this.Close();
        }

    }
}
