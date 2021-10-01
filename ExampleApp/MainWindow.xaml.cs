using System.ComponentModel;
using System.Windows;

namespace ExampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MyEnum _myEnum;
        public MyEnum MyEnum
        {
            get => _myEnum;
            set
            {
                _myEnum = value;
                PropertyChanged?.Invoke(this, new(nameof(MyEnum)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}