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
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for ScenePropertiesWindow.xaml
    /// </summary>
    public partial class ScenePropertiesWindow : Window
    {
        private int SceneID = 0; 

        public ScenePropertiesWindow(int SceneID)
        {
            this.SceneID = SceneID;
            InitializeComponent();          
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            SceneControl.SceneID = SceneID;
        }

        private void ResetBtn_Click_1(object sender, RoutedEventArgs e)
        {
            using (zvsLocalDBEntities context = new zvsLocalDBEntities())
            {
                scene s = context.scenes.FirstOrDefault(sc => sc.id == SceneID);
                if (s != null)
                {
                    s.is_running = false;
                    context.SaveChanges();
                }
            }
        }
    }
}
