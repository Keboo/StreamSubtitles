using System.Windows;

namespace StreamSubtitles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(new SpeechRecognizerFactory());
            InitializeComponent();
        }
    }
}
