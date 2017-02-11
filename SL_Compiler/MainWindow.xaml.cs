using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SL_Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string file = null;
        public MainWindow()
        {
            InitializeComponent();
            StartCompiling();
        }

        private void StartCompiling()
        {
            if (file == null)
                return;
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            parser.Parse();
            OutputBox.Text = parser.errors.count + " error(s) detected" + "\n";
            foreach (string synError in parser.errors.ErrorList)
                OutputBox.Text += synError + "\n";

            if (parser.errors.ErrorList.Count == 0)
            {
                string dir = System.IO.Path.GetDirectoryName(file);
                string combined = System.IO.Path.Combine(dir, System.IO.Path.GetFileName("obj1"));
                File.WriteAllBytes(combined, parser.ByteCode);
            }
        }

        private bool OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "sl files (*.sl)|*.sl*";

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                file = openFileDialog1.FileName;
                return true;
            }

            file = null;
            return false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OpenFile())
            {
                try
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        String line = await sr.ReadToEndAsync();
                        SourceBox.Text = line;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not read the file");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartCompiling();
        }

        private void Reset()
        {
            OutputBox.Clear();
        }
    }
}
