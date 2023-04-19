global using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace NTM
{
    public partial class App : Application
    {
        const string conn = @"
server=localhost;
database=db_test;
user=root;
";
        public MySqlConnection Conn { get; } = new(conn);
        public string No { get; set; } = "";
        public App()
        {
            Conn.Open();
        }
    }
}
