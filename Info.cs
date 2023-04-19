using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static NTM.TeacherInfo.TeachInfo;
using System.Xml.Linq;
using static NTM.StudentInfo;
using Google.Protobuf.WellKnownTypes;

namespace NTM
{
    //课程的信息
    internal class CourseInfo
    {
        public string No { get; init; } = "114514";
        public string Name { get; init; } = "鸡你太美";
        public decimal Credit { get; init; } = 2.5M;
        public int Semester { get; init; } = 0;
        public int Hours { get; init; } = 0;
    }
    //学生的信息
    internal class StudentInfo
    {
        public StudentInfo(string tel="") => this.tel = tel;
        public string No { get; init; } = "114514";
        public string Name { get; init; } = "蔡徐坤";
        public bool Gender { get; init; } = true;//男1女0
        public string Department { get; init; } = "偶像练习生";
        public string Major { get; init; } = "唱跳rap篮球";
        public string Class { get; init; } = "1145";
        string tel = "1919810";
        public string Tel
        {
            get => tel; set
            {
                /*修改数据库中的电话号，失败直接抛错误*/
                try
                {
                    string sql = $"update student_table set tel = '{value}' where student_no='{ No}'";//设置电话号
                    MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                    if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                    {
                        MessageBox.Show("更改成功！");
                    }
                    else
                    {
                        MessageBox.Show("更改失败！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public string Password
        {
            set
            {
                /*修改数据库中的密码，失败直接抛错误*/
                try
                {
                    string sql = string.Format("update user_table set password = '{0}' where username='{1}'", value, No);//设置密码
                    MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                    if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                    {
                        MessageBox.Show("更改成功！");
                    }
                    else
                    {
                        MessageBox.Show("更改失败！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public List<StudyInfo> Studies { get; init; } = new();
        public static StudentInfo Query(string stuNo)
        {
            //(Application.Current as App)!.Conn;
            //查询学生的信息
            string no = "";
            string name = "";
            bool gender = true;
            string department = "";
            string major = "";
            string c = "";
            string tel = "";
            try//查询学生信息
            {
                string sql = string.Format("select * from student_table where student_no = '{0}';", stuNo);
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    mySqlDataReader.Read();
                    no = Convert.ToString(mySqlDataReader["student_no"])!;
                    name = Convert.ToString(mySqlDataReader["student_name"])!;
                    if (Convert.ToString(mySqlDataReader["gender"])! == "女")
                        gender = false;
                    department = Convert.ToString(mySqlDataReader["department"])!;
                    major = Convert.ToString(mySqlDataReader["major"])!;
                    c = Convert.ToString(mySqlDataReader["class_no"])!;
                    tel = Convert.ToString(mySqlDataReader["tel"])!;
                }
                mySqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            StudentInfo studentInfo = new StudentInfo(tel)
            {
                Name = name,
                No = no,
                Gender = gender,
                Department = department,
                Major = major,
                Class = c
            };
            try//查找未选过的课程且没分的课程
            {
                string sql = string.Format("select distinct course_table.course_no,course_table.course_name,course_table.credit,course_table.semester,course_table.hours " +
                    "from course_table where course_table.course_no not in " +
                    "(select course_no from take_table where student_no = '{0}') " +
                    "and course_table.course_no not in " +
                    "(select course_no from score_table where student_no = '{0}');", stuNo);//查询学生未选课程信息
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    while (mySqlDataReader.Read())
                    {
                        studentInfo.Studies.Add(new StudyInfo
                        {
                            Course = new CourseInfo
                            {
                                No = mySqlDataReader[0].ToString()!,
                                Name = mySqlDataReader[1].ToString()!,
                                Credit = (decimal)mySqlDataReader[2],
                                Semester = (int)mySqlDataReader[3],
                                Hours = (int)mySqlDataReader[4]
                            },
                            CourseState = StudyInfo.CourseStateEnum.未选课
                        });
                    }
                }
                mySqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//查找已选课的课
            {
                string sql = string.Format("select distinct course_no,course_name,credit,semester,hours from course_table " +
                    "where course_no in (select course_no from take_table where student_no = '{0}');", stuNo);//查询学生已选过的课
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    while (mySqlDataReader.Read())
                    {
                        studentInfo.Studies.Add(new StudyInfo
                        {
                            Course = new CourseInfo
                            {
                                No = mySqlDataReader[0].ToString()!,
                                Name = mySqlDataReader[1].ToString()!,
                                Credit = (decimal)mySqlDataReader[2],
                                Semester = (int)mySqlDataReader[3],
                                Hours = (int)mySqlDataReader[4]
                            },
                            CourseState = StudyInfo.CourseStateEnum.已选课
                        });
                    }

                }
                mySqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//查找已结课的课
            {
                string sql = string.Format("select distinct course_table.course_no,course_table.course_name,course_table.credit,course_table.semester,course_table.hours ,score_table.score " +
                    "from course_table inner join score_table on course_table.course_no = score_table.course_no where course_table.course_no in " +
                    "(select course_no from score_table where student_no = '{0}') and student_no = '{0}';", stuNo);//查询学生已结课的课
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    while (mySqlDataReader.Read())
                    {
                        studentInfo.Studies.Add(new StudyInfo
                        {
                            Course = new CourseInfo
                            {
                                No = mySqlDataReader[0].ToString()!,
                                Name = mySqlDataReader[1].ToString()!,
                                Credit = (decimal)mySqlDataReader[2],
                                Semester = (int)mySqlDataReader[3],
                                Hours = (int)mySqlDataReader[4]
                            },
                            CourseState = StudyInfo.CourseStateEnum.已结课,
                            Score = (int)mySqlDataReader[5]
                        });
                    }
                }
                mySqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return studentInfo;
        }
        public void Take(StudyInfo study)
        {
            /*学生选study这门课*/
            string sql = string.Format("insert into take_table(student_no, course_no, year, semester) values('{0}','{1}', 2023, 6);", No, study.Course.No);//学生选课
            MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
            if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
            {
                MessageBox.Show("选课成功！");
            }
            else
            {
                MessageBox.Show("选课失败！");
            }
        }
        public void Drop(StudyInfo study)
        {
            /*学生退study这门课*/
            string sql = string.Format("delete from take_table where student_no = '{0}' and course_no = '{1}';", No, study.Course.No);//学生退课
            MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
            if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
            {
                MessageBox.Show("退课成功！");
            }
            else
            {
                MessageBox.Show("退课失败！");
            }
        }

        internal class StudyInfo
        {
            public CourseInfo Course { get; init; } = new();
            public enum CourseStateEnum
            {
                未选课,
                已选课,
                已结课,
            }
            public CourseStateEnum CourseState { get; set; }
            public int Score { get; init; } = 0;
        }
    }
    //教师的信息
    internal class TeacherInfo
    {
        public TeacherInfo(string tel="") => this.tel = tel;
        public string No { get; init; } = "114514";
        public string Name { get; init; } = "蔡徐坤";
        public bool Gender { get; init; } = true;//男1女0
        public string Degree { get; init; } = "鸡王";
        public string Title { get; init; } = "全民偶像";
        public string Department { get; init; } = "偶像练习生";
        string tel = "1919810";
        public string Tel
        {
            get => tel; set
            {
                /*修改数据库中的电话号，失败直接抛错误*/
                try
                {
                    string sql = string.Format("update teacher_table set tel = '{0}' where teacher_no='{1}'", value, No);//设置电话号
                    MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                    if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                    {
                        MessageBox.Show("更改成功！");
                    }
                    else
                    {
                        MessageBox.Show("更改失败！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
       
        public string Password
        {
            set
            {
                /*修改数据库中的密码，失败直接抛错误*/
                try
                {
                    string sql = string.Format("update user_table set password = '{0}' where username='{1}'", value, No);//设置密码
                    MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                    if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                    {
                        MessageBox.Show("更改成功！");
                    }
                    else
                    {
                        MessageBox.Show("更改失败！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public List<TeachInfo> Teaches { get; init; } = new();
        public static TeacherInfo Query(string teaNo)
        {
            /*查询老师的信息*/
            string no = "";
            string name = "";
            bool gender = true;//男1女0
            string degree = "";
            string title = "";
            string department = "";
            string tel = "";
            try//查询老师信息
            {
                string sql = string.Format("select * from teacher_table where teacher_no = '{0}';", teaNo);
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    mySqlDataReader.Read();
                    no = Convert.ToString(mySqlDataReader["teacher_no"])!;
                    name = Convert.ToString(mySqlDataReader["name"])!;
                    if (Convert.ToString(mySqlDataReader["gender"])! == "女")
                        gender = false;
                    department = Convert.ToString(mySqlDataReader["department"])!;
                    degree = Convert.ToString(mySqlDataReader["degree"])!;
                    title = Convert.ToString(mySqlDataReader["title"])!;
                    tel = Convert.ToString(mySqlDataReader["tel"])!;
                }
                mySqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            TeacherInfo teacherInfo = new TeacherInfo(tel)
            {
                Name = name,
                No = no,
                Gender = gender,
                Degree = degree,
                Title = title,
                Department = department
            };
            try
            {
                List<string> course_list = new();
                string sql = string.Format("select course_no from teach_table where teacher_no = '{0}'", no);//查询老师教授的课
                MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                if (mySqlDataReader.HasRows)
                {
                    while (mySqlDataReader.Read())//遍历教师教授的课程
                    {
                        string course_no = Convert.ToString(mySqlDataReader[0])!;
                        course_list.Add(course_no);
                    }
                }
                mySqlDataReader.Close();
                for(int i = 0; i < course_list.Count;i++)
                {
                    List<Stu> stus = new();//储存所有选课学生信息
                    sql = string.Format("select s1.student_no, s1.student_name, s1.gender,s1.department,s1.major,s1.class_no, s1.tel, s2.score " +
                                "from student_table as s1 inner join score_table as s2 on s1.student_no = s2.student_no " +
                                "where s2.course_no = '{0}';", course_list[i]);//查询选择了这门课的所有学生信息
                    MySqlCommand cmd_student = new(sql, (Application.Current as App)!.Conn);
                    MySqlDataReader mySqlDataReader1 = cmd_student.ExecuteReader();
                    //将所有选择这门课的学生信息存入列表
                    if (mySqlDataReader1.HasRows)
                    {
                        while (mySqlDataReader1.Read())//遍历教师教授课程的所有学生
                        {
                            stus.Add(new Stu((int)mySqlDataReader1["score"])
                            { 
                                Student = new StudentInfo
                                {
                                    No = Convert.ToString(mySqlDataReader1[0])!,
                                    Name = Convert.ToString(mySqlDataReader1[1])!,
                                    Gender = Convert.ToString(mySqlDataReader1[2])! == "男",
                                    Department = Convert.ToString(mySqlDataReader1[3])!,
                                    Major = Convert.ToString(mySqlDataReader1[4])!,
                                    Class = Convert.ToString(mySqlDataReader1[5])!,
                                }
                            });
                        }
                    }
                    mySqlDataReader1.Close();

                    sql = string.Format("select * from course_table where course_no = '{0}'", course_list[i]);//查询这门课的所有信息
                    MySqlCommand cmd_course = new(sql, (Application.Current as App)!.Conn);
                    MySqlDataReader mySqlDataReader2 = cmd_course.ExecuteReader();
                    mySqlDataReader2.Read();
                    teacherInfo.Teaches.Add(new TeachInfo
                    {
                        Course = new CourseInfo
                        {
                            No = Convert.ToString(mySqlDataReader2[1])!,
                            Name = Convert.ToString(mySqlDataReader2[2])!,
                            Credit = (decimal)mySqlDataReader2[3],
                            Semester = (int)mySqlDataReader2[4],
                            Hours = (int)mySqlDataReader2[5]
                        },StuList = stus
                    });
                    mySqlDataReader2.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return teacherInfo;
        }
        internal class TeachInfo
        {
            public CourseInfo Course { get; init; } = new();
            public List<Stu/*学生信息+分数*/> StuList { get; init; } = new();
            internal class Stu
            {
                //这里是存的学这门课的学生的基本信息，复用了StudentInfo，不需要设置Studies
                public StudentInfo Student { get; init; } = new();
                public Stu(int score=0) => this.score = score;
                int score;
                public int Score
                {
                    get => score;
                    set
                    {
                        /*修改数据库中的成绩信息*/
                        try
                        {
                            string sql = string.Format("update score_table set score = '{0}' where student_no='{1}'", value, Student.No);//重新设置数据库中成绩
                            MySqlCommand cmd = new(sql, (Application.Current as App)!.Conn);
                            if (Convert.ToInt32(cmd.ExecuteNonQuery()) > 0)
                            {
                                MessageBox.Show("更改成功！");
                            }
                            else
                            {
                                MessageBox.Show("更改失败！");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        score = value;
                    }
                }
            }
        }
    }

}
