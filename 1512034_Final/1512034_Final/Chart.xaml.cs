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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace _1512034_Final
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public class QuickNote : INotifyPropertyChanged
    {
        private string m_Tag = "";
        private int m_Number = 0;
        public string Tag
        {
            get { return m_Tag; }
            set
            {
                m_Tag = value;
                NotifyPropertyChanged("Tag");
            }
        }
        public int Number
        {
            get { return m_Number;}
            set { m_Number = value; NotifyPropertyChanged("Number"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainViewModel
    {
        // Tạo danh sách số dân một số nước bằng ObservableCollection vì có hổ trợ tự động NotifyPropertychanged
        private readonly ObservableCollection<QuickNote> _QuickNotes = new ObservableCollection<QuickNote>();

        // Property sẽ được Binding từ GUI(View)
        public ObservableCollection<QuickNote> QuickNotes
        {
            get { return _QuickNotes; }
        }

        public class MyTags
        {
            public string TAG { set; get; }                     //ten tag
            public int NUMBER { set; get; }                     //so luong tag
            public List<string> listContent { set; get; }       //noi dung cua cac tag
        }
        List<MyTags> listTag = new List<MyTags>();
        // Add data(Model-Item) cho List
        public MainViewModel()
        {
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
            foreach(MyTags n in listTag)
            {
                _QuickNotes.Add(new QuickNote() { Tag = n.TAG, Number = n.NUMBER });
            }
        }
    }

    public partial class Chart : Window
    {
        // danh sach cua ghi chu gom tag, so luong, noi dung
        
        public Chart()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }
    }
}
