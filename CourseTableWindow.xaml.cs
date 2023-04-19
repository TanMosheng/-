using Google.Protobuf.WellKnownTypes;
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
using static NTM.StudentInfo;
using static NTM.TeacherInfo.TeachInfo;

namespace NTM {
    /// <summary>
    /// CourseTableWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CourseTableWindow : Window {
        /*这里改成获取全部课程的逻辑*/
        List<CourseInfo> courses = new() {
            
        };
        public CourseTableWindow() {
            InitializeComponent();
            string sql = string.Format("select distinct course_no,course_name,credit,semester,hours from course_table ;");//查询学生已选过的课
            MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
            MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
            if (mySqlDataReader.HasRows)
            {
                while (mySqlDataReader.Read())
                {
                    courses.Add(new CourseInfo
                        {
                            No = mySqlDataReader[0].ToString()!,
                            Name = mySqlDataReader[1].ToString()!,
                            Credit = (decimal)mySqlDataReader[2],
                            Semester = (int)mySqlDataReader[3],
                            Hours = (int)mySqlDataReader[4]
                        }
                    );
                }

            }
            mySqlDataReader.Close();

            foreach (CourseInfo course in courses) {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                TextBox courseNo = new TextBox { Text = course.No, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseNo);
                Grid.SetColumn(courseNo, 0);

                TextBox courseName = new TextBox { Text = course.Name, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseName);
                Grid.SetColumn(courseName, 1);

                TextBox courseCredit = new TextBox { Text = course.Credit.ToString(), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseCredit);
                Grid.SetColumn(courseCredit, 2);

                TextBox courseSemester = new TextBox { Text = course.Semester.ToString(), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(courseSemester);
                Grid.SetColumn(courseSemester, 3);

                TextBox courseHours = new TextBox { Text = course.Hours.ToString(), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center };
                grid.Children.Add(courseHours);
                Grid.SetColumn(courseHours, 4);

                Grid gg = new Grid();
                gg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                gg.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btn_update = new Button { Content = "改" };
                btn_update.Click += (obj, e) => {
                    try {
                        updateCourse(course, new CourseInfo {
                            No = courseNo.Text,
                            Name = courseName.Text,
                            Credit = decimal.Parse(courseCredit.Text),
                            Semester = int.Parse(courseSemester.Text),
                            Hours = int.Parse(courseHours.Text),
                        });
                        //不论成功与否都关掉重新加载
                        (new CourseTableWindow()).Show();
                        this.Close();
                    } catch (Exception) {
                        MessageBox.Show("请检查数据");
                    }
                };
                gg.Children.Add(btn_update);
                Grid.SetColumn(btn_update, 0);

                Button btn_drop = new Button { Content = "删" };
                btn_drop.Click += (obj, e) => {
                    dropCourse(course);
                    //不论成功与否都关掉重新加载
                    (new CourseTableWindow()).Show();
                    this.Close();
                };
                gg.Children.Add(btn_drop);
                Grid.SetColumn(btn_drop, 1);

                grid.Children.Add(gg);
                Grid.SetColumn(gg, 5);

                ListBox_CourseTable.Items.Add(grid);
            }
        }
        void updateCourse(CourseInfo oldCourse, CourseInfo newCourse) {
            /*更新课程信息的函数*/
            try
            {
                string sql = $"update course_table set course_name = '{newCourse.Name}',course_no = '{newCourse.No}',credit = '{newCourse.Credit}'," +
                    $"semester = '{newCourse.Semester}', hours = '{newCourse.Hours}' where course_no='{oldCourse.No}';";//更改课程信息
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                {
                    MessageBox.Show("更新成功！");
                }
                else
                {
                    MessageBox.Show("更新失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void dropCourse(CourseInfo course) {
            /*删除课程信息的函数*/
            try
            {
                string sql = $"delete from  course_table where course_no = '{course.No}';";//删除相关课程
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

        void insertCourse(CourseInfo course) {
            /*添加信息的函数*/
            /*插入课程信息的函数*/
            try
            {
                string sql = $"insert into course_table(course_no, course_name, credit, semester, hours) values('{course.No}', '{course.Name}','{course.Credit}','{course.Semester}','{course.Hours}');";//插入相关课程
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                {
                    MessageBox.Show("添加成功！");
                }
                else
                {
                    MessageBox.Show("添加失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Insert_Click(object sender, RoutedEventArgs e) {
            try {
                insertCourse(new CourseInfo {
                    No = TextBox_No.Text,
                    Name = TextBox_Name.Text,
                    Credit = decimal.Parse(TextBox_Credit.Text),
                    Semester = int.Parse(TextBox_Semester.Text),
                    Hours = int.Parse(TextBox_Hour.Text),
                });
                //不论成功与否都关掉重新加载
                (new CourseTableWindow()).Show();
                this.Close();
            } catch (Exception) {
                MessageBox.Show("请检查数据");
            }
        }
    }
}
