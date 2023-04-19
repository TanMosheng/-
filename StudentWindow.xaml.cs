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

namespace NTM {
    /// <summary>
    /// StudentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StudentWindow : Window {
        StudentInfo studentInfo;
        public StudentWindow(string studentNo = "") {
            InitializeComponent();

            studentInfo = StudentInfo.Query(studentNo);

            TextBox_Name.Text = studentInfo.Name;
            TextBox_No.Text = studentInfo.No;
            TextBox_Gender.Text = studentInfo.Gender ? "男" : "女";
            TextBox_Dept.Text = studentInfo.Department;
            TextBox_Major.Text = studentInfo.Major;
            TextBox_Class.Text = studentInfo.Class;
            TextBox_Tel.Text = studentInfo.Tel;

            foreach (var study in studentInfo.Studies) {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock courseNo = new TextBlock { Text = study.Course.No,HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center, };
                grid.Children.Add(courseNo);
                Grid.SetColumn(courseNo, 0);

                TextBlock courseName = new TextBlock { Text = study.Course.Name,HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseName);
                Grid.SetColumn(courseName, 1);

                TextBlock courseCredit = new TextBlock { Text = study.Course.Credit.ToString() , HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseCredit);
                Grid.SetColumn(courseCredit, 2);

                TextBlock courseSemester = new TextBlock { Text = study.Course.Semester.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseSemester);
                Grid.SetColumn(courseSemester, 3);

                TextBlock courseHours = new TextBlock { Text = study.Course.Hours.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                grid.Children.Add(courseHours);
                Grid.SetColumn(courseHours, 4);

                Button btn = new Button { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, };
                RoutedEventHandler take = (obj, e) => { };
                RoutedEventHandler drop = (obj, e) => { };
                take = (obj, e) => {
                    studentInfo.Take(study);
                    btn.Content = "退课";
                    btn.Click -= take;
                    btn.Click += drop;
                };
                drop = (obj, e) => {
                    studentInfo.Drop(study);
                    btn.Content = "选课";
                    btn.Click -= drop;
                    btn.Click += take;
                };

                switch (study.CourseState) {
                    case StudentInfo.StudyInfo.CourseStateEnum.未选课:
                        btn.Content = "选课";
                        btn.Click += take;
                        goto default;
                    case StudentInfo.StudyInfo.CourseStateEnum.已选课:
                        btn.Content = "退课";
                        btn.Click += drop;
                        goto default;
                    case StudentInfo.StudyInfo.CourseStateEnum.已结课:
                        TextBlock txt = new TextBlock {Text = study.Score.ToString(), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        grid.Children.Add(txt);
                        Grid.SetColumn(txt, 5);
                        break;
                    default:
                        grid.Children.Add(btn);
                        Grid.SetColumn(btn, 5);
                        break;
                }
                ListBox_Studies.Items.Add(grid);
            }
        }

        private void Button_SetTel_Click(object sender, RoutedEventArgs e) {
            studentInfo.Tel = TextBox_Tel.Text; 
        }

        private void Button_SetPassword_Click(object sender, RoutedEventArgs e) {
            studentInfo.Password = PasswordBox_Password.Password; 
        }
    }
}
