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

namespace Echiquier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly JeuEtat _jeu;

        public MainWindow()
        {
            InitializeComponent();

            _jeu = new JeuEtat();
            DataContext = _jeu;
        }

        private void Case_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border caseFond = (Border)sender;
            CaseEtat c = (CaseEtat)caseFond.DataContext;

            _jeu.Interaction(c);
        }
    }
}
