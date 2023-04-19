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
using static NTM.TeacherInfo.TeachInfo;

namespace NTM {
    /// <summary>
    /// TeacherWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TeacherWindow : Window {
        TeacherInfo teacherInfo;
        public TeacherWindow(string teacherNo) {
            InitializeComponent();

            teacherInfo = TeacherInfo.Query(teacherNo);

            TextBox_Name.Text = teacherInfo.Name;
            TextBox_No.Text = teacherInfo.No;
            TextBox_Gender.Text = teacherInfo.Gender ? "男" : "女";
            TextBox_Dept.Text = teacherInfo.Department;
            TextBox_Title.Text = teacherInfo.Title;
            TextBox_Degree.Text = teacherInfo.Degree;
            TextBox_Tel.Text = teacherInfo.Tel;

            foreach (var teach in teacherInfo.Teaches) {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock courseNo = new TextBlock { Text = teach.Course.No, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseNo);
                Grid.SetColumn(courseNo, 0);

                TextBlock courseName = new TextBlock { Text = teach.Course.Name, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseName);
                Grid.SetColumn(courseName, 1);

                TextBlock courseCredit = new TextBlock { Text = teach.Course.Credit.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseCredit);
                Grid.SetColumn(courseCredit, 2);

                TextBlock courseSemester = new TextBlock { Text = teach.Course.Semester.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseSemester);
                Grid.SetColumn(courseSemester, 3);

                TextBlock courseHours = new TextBlock { Text = teach.Course.Hours.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseHours);
                Grid.SetColumn(courseHours, 4);

                Button btn = new Button { Content = "查看" };
                btn.Click += (obj, e) => {
                    showStus(teach.StuList);
                };
                grid.Children.Add(btn);
                Grid.SetColumn(btn, 5);

                ListBox_Teaches.Items.Add(grid);
            }
        }
        void showStus(List<Stu> stuList) {
            ListBox_Stus.Items.Clear();
            foreach (var stu in stuList) {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock studentNo = new TextBlock { Text = stu.Student.No, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(studentNo);
                Grid.SetColumn(studentNo, 0);

                TextBlock studentName = new TextBlock { Text = stu.Student.Name, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(studentName);
                Grid.SetColumn(studentName, 1);

                TextBlock studentGender = new TextBlock { Text = stu.Student.Gender ? "男" : "女", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(studentGender);
                Grid.SetColumn(studentGender, 2);

                TextBox studentScore = new TextBox { Text = stu.Score.ToString(),MinWidth =50, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(studentScore);
                Grid.SetColumn(studentScore, 3);

                Button btn = new Button { Content = "修改", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                btn.Click += (obj, e) => {
                    stu.Score = int.Parse(studentScore.Text);
                };
                grid.Children.Add(btn);
                Grid.SetColumn(btn, 4);

                ListBox_Stus.Items.Add(grid);
            }
        }

        private void Button_SetTel_Click(object sender, RoutedEventArgs e) {
            teacherInfo.Tel = TextBox_Tel.Text;
        }

        private void Button_SetPassword_Click(object sender, RoutedEventArgs e) {
            teacherInfo.Password = PasswordBox_Password.Password;
        }
    }
}
