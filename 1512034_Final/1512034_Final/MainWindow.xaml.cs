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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace _1512034_Final
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public class MyTags
        {
            public string TAG { set; get; }                     //ten tag
            public int NUMBER { set; get; }                     //so luong tag
            public List<string> listContent { set; get; }       //noi dung cua cac tag
        }

        // danh sach cua ghi chu gom tag, so luong, noi dung
        List<MyTags> listTag = new List<MyTags>();
        List<string> listContent = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            //load file content
            StreamReader sr = new StreamReader("Tags.txt", Encoding.UTF8);
            string tag = sr.ReadLine();
            while (tag != null)
            {
                string srNumber = sr.ReadLine();
                int number = int.Parse(srNumber);

                List<string> listTemp = new List<string>();
                for (int i = 0; i < number; i++)
                {
                    string contentTag = sr.ReadLine();
                    listTemp.Add(contentTag);
                    listContent.Add(contentTag);
                }

                //them vao list tag
                listTag.Add(new MyTags()
                {
                    TAG = tag,
                    NUMBER = number,
                    listContent = listTemp
                });

                tag = sr.ReadLine();
            }
            sr.Close();
            //xuat thong tin ra man hinh
            lvTags.ItemsSource = listTag;


        }

        private void lvTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tag = (lvTags.SelectedItem as MyTags).TAG;

            List<string> takenote = new List<string>();

            foreach (MyTags n in listTag)
            {
                if (n.TAG.CompareTo(tag) == 0)
                {
                    foreach (string st in n.listContent)
                    {
                        string temp="";
                        if (st.Length > 50)
                        {
                            temp = st.Substring(0, 50);      // cat 50 ky tu dau tien 
                        }
                        else
                        {
                            temp = st;
                        }
                        takenote.Add(temp);
                    }
                    break;
                }
            }

            lvNotes.ItemsSource = takenote;
        }

        private void lvNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txbFullContent.Clear();
            if (lvNotes.SelectedItem == null)
            {
                return;
            }

            foreach(string n in listContent )
            {
                if (n.Contains(lvNotes.SelectedItems[0].ToString())== true)
                {
                    txbFullContent.Text =n;
                }
            }
        }

    }
}
