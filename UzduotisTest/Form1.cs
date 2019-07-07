using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace UzduotisTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ComboBoxPopulation();
            listView1.View = View.Details;
            listView1.Columns.Add("ThreadID", 200);
            listView1.Columns.Add("Data", 200);
            listView1.Width = 400;
            listView1.GridLines = true;
            ClearTable();
        }

        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                    AttachDbFilename=C:\Users\grina\Desktop\UzduotisTest\UzduotisTest\UzduotisTest\Database1.mdf;
                                    Integrated Security=True";
        private bool stopWasPressed = false;

        private void Start_Click(object sender, EventArgs e)
        {
            int number = 0;
            try
            {
                number = int.Parse(comboBox1.SelectedItem.ToString());
            }
            catch
            {
                MessageBox.Show("Nepasirinktas threadu skaicius!");
            }

            //nesukuriant kito threado "luzta" UI.
            var backThread = new Thread(() =>
            {
                BackgroundTask(number);
                
            });
            backThread.Start();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            stopWasPressed = true;

        }

        //Generuojant random info naudojam static random objekta
        //kad Next() butu kvieciamas tam paciam Random, o ne skirtingiems Random tuo paciu metu
        public static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public string GenerateString()
        {
            int index = 0;
            string valid = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            lock (syncLock)
            {
                index = random.Next(5, 10);
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < index; i++)
            {
                sb.Append(valid[random.Next(valid.Length)]);
            }
            Thread.Sleep(random.Next(500, 2000));
            return sb.ToString();
        }

        public void AddingInfoToListView(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adap = new SqlDataAdapter("select * from Duomenys", conn);
                DataTable dt = new DataTable();
                adap.Fill(dt);
                //ziurim ar yra 20 sugeneruotu eiluciu
                //jei >20 - parasom viska
                //jei <20 - tik paskutines 20 eiluciu
                if(dt.Rows.Count - 20 < 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        string[] row = { dr["ThreadID"].ToString(), dr["Data"].ToString() };
                        ListViewItem listViewItem = new ListViewItem(row);
                        if(listView1.InvokeRequired)
                        {
                            listView1.Invoke(new MethodInvoker(delegate
                            {
                                listView1.Items.Add(listViewItem);
                            }));
                        }
                        else
                        {
                            listView1.Items.Add(listViewItem);
                        }
                        

                    }
                }
                else
                {
                    for (int i = dt.Rows.Count - 20; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        string[] row = { dr["ThreadID"].ToString(), dr["Data"].ToString() };
                        ListViewItem listViewItem = new ListViewItem(row);
                        if (listView1.InvokeRequired)
                        {
                            listView1.Invoke(new MethodInvoker(delegate
                            {
                                listView1.Items.Add(listViewItem);
                            }));
                        }
                        else
                        {
                            listView1.Items.Add(listViewItem);
                        }
                    }
                }
            }
        }

        public void AddingInfoToTable()
        {
            while(!stopWasPressed)
            {
                string data = GenerateString();
                int threadId = int.Parse(Thread.CurrentThread.Name);
                DateTime time = DateTime.Now;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        using (SqlCommand comm = conn.CreateCommand())
                        {
                            comm.Transaction = tran;
                            comm.CommandType = CommandType.Text;
                            comm.CommandText = "INSERT INTO Duomenys (ThreadID, Time, Data) values (@threadId, @time, @data)";
                            comm.Parameters.Add(new SqlParameter("@threadID", threadId));
                            comm.Parameters.Add(new SqlParameter("@Time", time));
                            comm.Parameters.Add(new SqlParameter("@Data", data));

                            try
                            {
                                if (comm.ExecuteNonQuery() != 1)
                                {
                                    throw new InvalidProgramException();
                                }
                                tran.Commit();
                            }
                            catch (Exception)
                            {
                                tran.Rollback();
                                conn.Close();
                                throw;
                            }
                        }
                    }
                }
            }
        }

        public void ComboBoxPopulation()
        {
            for (int i = 2; i <= 15; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
        }

        //isvalom db paleidziant
        public void ClearTable()
        {
            string sqlTrunc = "TRUNCATE TABLE Duomenys";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlTrunc, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private void BackgroundTask(int num)
        {
            
            List<Thread> threads = new List<Thread>();
            while (!stopWasPressed)
            {
                threads.Clear();
                for (int i = 1; i <= num; i++)
                {
                    Thread thread = new Thread(new ThreadStart(AddingInfoToTable));
                    thread.Name = i.ToString();
                    thread.Start();
                    threads.Add(thread);
                }

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Reikia antra karta paspausti Start buttona, jog rodytu data 
            Start.Click +=  new EventHandler(AddingInfoToListView);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
    }
}
