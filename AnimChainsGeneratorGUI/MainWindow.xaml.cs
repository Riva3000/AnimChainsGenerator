using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
//using System.Windows.Shapes;
using System.IO;

namespace AnimChainsGenerator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region    --- INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            // else
            field = value;

            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion --- INotifyPropertyChanged implementation END


        private Size _SheetCellSize = new Size();
        public Size SheetCellSize
        {
            get { return _SheetCellSize; }
            set { SetField(ref _SheetCellSize, value, "SheetCellSize"); }
        }

        private string _SheetFilePath;
        public string SheetFilePath
        {
            get { return _SheetFilePath; }
            set { SetField(ref _SheetFilePath, value, "SheetFileName"); }
        }

        private string _OutputAchxFilePath;
        public string OutputAchxFilePath
        {
            get { return _OutputAchxFilePath; }
            set { SetField(ref _OutputAchxFilePath, value, "OutputAchxFilePath"); }
        }

        private ushort _Rotations = 1;
        public ushort Rotations
        {
            get { return _Rotations; }
            set { SetField(ref _Rotations, value, "Rotations"); }
        }

        private ObservableCollection<AnimDef> _AnimDefinitons = new ObservableCollection<AnimDef>();
        public ObservableCollection<AnimDef> AnimDefinitons { get { return _AnimDefinitons; } }





        public MainWindow()
        {
            //ContentRendered += This_ContentRendered;

            DataContext = this;

            InitializeComponent();
        }

        private void This_ContentRendered(object sender, EventArgs e)
        {
            
        }




        private bool ContainsDuplicates<T, TValue>(IEnumerable<T> source, Func<T, TValue> selector)
        {
            var hashSet = new HashSet<TValue>();
            foreach(var t in source)
            {
                if(! hashSet.Add( selector(t) ) )
                {
                    return true;
                }
            }
            return false;
        }

        private bool DuplicateAnimDefs()
        {
            var hashSet = new HashSet<string>();
            foreach(var animDef in _AnimDefinitons)
            {
                if(! hashSet.Add( animDef.AnimName ) )
                {
                    return true;
                }
            }
            return false;
        }


        private void ButAddAnimDef_Click(object sender, RoutedEventArgs e)
        {
            _AnimDefinitons.Add(new AnimDef());
        }

        private void ButRemoveAnimDef_Click(object sender, RoutedEventArgs e)
        {
            _AnimDefinitons.Remove(
                (AnimDef)(sender as Button).Tag
            );
        }

        private void ButGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (_AnimDefinitons.Count == 0)
            {
                MessageBox.Show("Not Anims defined.");
                return;
            }

            if (_SheetFilePath == null)
            {
                MessageBox.Show("Enter SpriteSheet image file.");
                return;
            }

            if (!File.Exists(_SheetFilePath))
            {
                MessageBox.Show("SpriteSheet image file not found.");
                return;
            }

            if (_OutputAchxFilePath == null)
            {
                MessageBox.Show("Enter Achx output directory.");
                return;
            }

            string achxOutputDir = Path.GetDirectoryName(_OutputAchxFilePath);

            if (!Directory.Exists(achxOutputDir))
            {
                MessageBox.Show("Achx output directory doesn't exist.");
                return;
            }


            if (DuplicateAnimDefs())
            {
                MessageBox.Show("Two animations can't have a same name.");
                return;
            }



            var animChainList = Generator.Generate(
                    _SheetCellSize,
                    Path.GetDirectoryName(_SheetFilePath),
                    _Rotations,
                    _AnimDefinitons
                );

            Generator.SaveAchx(
                animChainList,
                achxOutputDir, Path.GetFileNameWithoutExtension(_OutputAchxFilePath),
                Path.GetDirectoryName(_SheetFilePath)
            );

            MessageBox.Show("Achx generation successful");

            if (CheckBoxOpenAchx.IsChecked.Value)
            {
                System.Diagnostics.Process.Start("_OutputAchxFilePath");
            }
        }
    }
}
