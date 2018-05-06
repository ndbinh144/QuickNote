using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Dynamic;
using System.Diagnostics;


namespace _1512034_Final
{
    /// <summary>
    /// Interaction logic for TakeNote.xaml
    /// </summary>


    public partial class TakeNote : Window
    {

        private LowLevelKeyboardListener _listener;

        // đọc dữ liệu từ file lên
        public class MyTags
        {
            public string TAG { set; get; }                     //ten tag
            public int NUMBER { set; get; }                     //so luong tag
            public List<string> listContent { set; get; }       //noi dung cua cac tag
        }

        // danh sach cua ghi chu gom tag, so luong, noi dung
        List<MyTags> listData = new List<MyTags>();
        List<string> listTag = new List<string>();

        int ac = 0;//kiem tra ac dang show hay hide

        public TakeNote()
        {
            InitializeComponent();

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Text = "Quick Note";
            ni.Icon = new System.Drawing.Icon("Hamzasaleem-Stock-Notes.ico");
            ni.Visible = true;

            // Thêm menu sau khi nhấn chuột phải
            System.Windows.Forms.ContextMenu notifyIconMenu = new System.Windows.Forms.ContextMenu();
            notifyIconMenu.MenuItems.Add("Quick Note (Ctrl + H)", new EventHandler(Hide_Click));
            notifyIconMenu.MenuItems.Add("View Note", new EventHandler(ViewNote_Click));
            notifyIconMenu.MenuItems.Add("View Statistics", new EventHandler(ViewStatistics_Click));
            notifyIconMenu.MenuItems.Add("Exit", new EventHandler(Exit_Click));

            ni.ContextMenu = notifyIconMenu;

            //load file content
            StreamReader sr = new StreamReader("Tags.txt", Encoding.UTF8);
            string tag = sr.ReadLine();

            int[] numberTag = new int[1000];
            int d = 0;

            while (tag != null)
            {
                string srNumber = sr.ReadLine();
                int number = int.Parse(srNumber);

                numberTag[d] = number;
                d++;

                List<string> listTemp = new List<string>();
                for (int i = 0; i < number; i++)
                {
                    string contentTag = sr.ReadLine();
                    listTemp.Add(contentTag);
                }

                //them vao list tag
                listData.Add(new MyTags()
                {
                    TAG = tag,
                    NUMBER = number,
                    listContent = listTemp
                });

                listTag.Add(tag);
                tag = sr.ReadLine();
            }

            

            //sap xep so luong note
            for (int i = 0; i < d; i++)
            {
                for (int j = i + 1; j < d; j++)
                {
                    if (numberTag[j] > numberTag[i])
                    {
                        //cach trao doi gia tri
                        int tmp = numberTag[i];
                        numberTag[i] = numberTag[j];
                        numberTag[j] = tmp;
                    }
                }
            }

            List<string> suggest = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                foreach (MyTags n in listData)
                {
                    if (numberTag[i] == n.NUMBER)
                    {
                        int f = 0;
                        foreach(string t in suggest)
                        {
                            if(n.TAG.CompareTo(t)==0)
                            {
                                f = 1;
                                break;
                            }
                        }
                        if (f == 1) break;
                        suggest.Add(n.TAG);
                    }
                }
            }
            //5 tag pho bien nhat
            cbbSuggest.ItemsSource = suggest;

            sr.Close();
            ac = 0;
            Show();
            Hide();
        }

        protected void ViewStatistics_Click(Object sender, System.EventArgs e)
        {
            Chart view = new Chart();
            view.Show();
        }

        protected void ViewNote_Click(Object sender, System.EventArgs e)
        {
            MainWindow view = new MainWindow();
            view.Show();
        }

        protected void Hide_Click(Object sender, System.EventArgs e)
        {
            if (ac == 0)
            {
                ac = 1;
                this.Show();
            }
            else
            {
                ac = 0;
                this.Hide();
            }
        }

        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                App.Current.Windows[intCounter].Close();

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _listener = new LowLevelKeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.HookKeyboard();
        }
        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.LeftCtrl && ac == 1)
                _listener.OnKeyPressed += _listener_OnKeyPressed1;
            if (e.KeyPressed == Key.LeftCtrl && ac == 0)
                _listener.OnKeyPressed += _listener_OnKeyPressed2;
        }

        void _listener_OnKeyPressed1(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.H)
            {
                Hide();
                ac = 0;
                return;
            }
        }

        void _listener_OnKeyPressed2(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.H)
            {
                Show();
                ac = 1;
                return;
            }
        }

        //void _listener_OnKeyPressed1(object sender, KeyPressedArgs e)
        //{
        //    if (e.KeyPressed == Key.K)
        //    {
        //        Chart view = new Chart();
        //        Show();
        //        _listener.OnKeyPressed -= _listener_OnKeyPressed;
        //    }

        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.HookKeyboard();
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (txbNote.Text == "" || txbTags.Text == "")
            {
                System.Windows.MessageBox.Show("Please input tags and note!");
                return;
            }
            string tags = txbTags.Text;
            string notes = txbNote.Text;
            string[] tag = tags.Split(',');

            foreach (string n in tag)
            {
                int flag = 0;
                foreach (MyTags p in listData)
                {
                    string t = n.Trim();
                    if (t.CompareTo(p.TAG) == 0)
                    {
                        p.NUMBER++;
                        p.listContent.Add(notes);
                        flag = 1;
                        break;
                    }
                }
                if (flag != 1)
                {
                    MyTags temp = new MyTags();
                    temp.TAG = n.Trim();
                    temp.NUMBER = 1;
                    temp.listContent = new List<string>();
                    temp.listContent.Add(notes);
                    listData.Add(temp);
                }
            }

            // ghi du lieu xuong file
            StreamWriter sw = new StreamWriter("Tags.txt", false, Encoding.UTF8);
            foreach (MyTags n in listData)
            {
                string line = n.TAG;
                sw.WriteLine(line);
                line = n.NUMBER.ToString();
                sw.WriteLine(line);
                foreach (string p in n.listContent)
                {
                    sw.WriteLine(p);
                }
            }
            sw.Flush();
            sw.Close();
            System.Windows.MessageBox.Show("Successful!");
        }


        private void txbTags_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cbbSuggest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = txbTags.Text;
            txbTags.Text = s + cbbSuggest.SelectedItem.ToString();
        }

    }
}
