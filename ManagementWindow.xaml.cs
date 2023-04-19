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
    /// ManagementWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManagementWindow : Window {
        public ManagementWindow() {
            InitializeComponent();
        }

        private void Button_Course_Click(object sender, RoutedEventArgs e) {
            (new CourseTableWindow()).Show();
        }

        private void Button_Teacher_Click(object sender, RoutedEventArgs e) {
            (new TeacherTableWindow()).Show();
        }
    }
}
