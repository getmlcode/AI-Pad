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

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorMenu.xaml
    /// </summary>
    public partial class TextEditorMenu : UserControl
    {
        public TextEditorMenu()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A Text Editor By Siddharth Shakya");
        }
    }
}
