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
using Microsoft.Win32;
using System.Xml.Serialization;

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

        private Project _Project = new Project();
        public Project Project
        {
            get { return _Project; }
            set { SetField(ref _Project, value, "Project"); }
        }
        





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
            foreach(var animDef in _Project.AnimDefinitons)
            {
                if(! hashSet.Add( animDef.AnimName ) )
                {
                    return true;
                }
            }
            return false;
        }


        private void ButLoadProjectFile_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog SaveFileDialog
            var dialog = new OpenFileDialog
            {
                DereferenceLinks = true, // default is false
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Open project file",
                Filter = "AnimChainsGenerator project (*.achpx)|*.achpx",
            };
                
            var dialogResult = dialog.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            //&& !String.IsNullOrWhiteSpace(dialog.FileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));

                using (var stream = File.OpenRead(dialog.FileName))
                {
                    Project = serializer.Deserialize(stream) as Project;
                }
            }
        }

        private void ButAddAnimDef_Click(object sender, RoutedEventArgs e)
        {
            _Project.AnimDefinitons.Add( new AnimDef { CellXstartIndex = 1, CellYstartIndex = 1 } );
        }

        private void ButRemoveAnimDef_Click(object sender, RoutedEventArgs e)
        {
            _Project.AnimDefinitons.Remove(
                (AnimDef)(sender as Button).Tag
            );
        }

        private void ButGenerate_Click(object sender, RoutedEventArgs e)
        {
            #region    -- Error checking
            if (_Project.AnimDefinitons.Count == 0)
            {
                MessageBox.Show("Not Anims defined.");
                return;
            }

            if (_Project.SheetFilePath == null)
            {
                MessageBox.Show("Enter SpriteSheet image file.");
                return;
            }

            if (!File.Exists(_Project.SheetFilePath))
            {
                MessageBox.Show("SpriteSheet image file not found.");
                return;
            }

            if (_Project.OutputAchxFilePath == null)
            {
                MessageBox.Show("Enter Achx output directory.");
                return;
            }

            string achxOutputDir = Path.GetDirectoryName(_Project.OutputAchxFilePath);

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
            #endregion -- Error checking END

            string achxOutputFileNameWOExt = Path.GetFileNameWithoutExtension(_Project.OutputAchxFilePath);

            // Save project
            if (CheckBoxSaveProject.IsChecked.Value)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                using (TextWriter writer = new StreamWriter(Path.Combine(achxOutputDir, achxOutputFileNameWOExt + ".achpx")))
                {
                    serializer.Serialize(
                        writer,
                        _Project
                    );
                }
            }

            // Remove whitespace
            // Convert AnimDefs to zero based indexing
            var zeroIndexedAnimDefinitons = new AnimDef[_Project.AnimDefinitons.Count];
            AnimDef animDefClone;
            for (int i = 0; i < zeroIndexedAnimDefinitons.Length; i++)
            {
                // AnimNames error check
                if (String.IsNullOrWhiteSpace(_Project.AnimDefinitons[i].AnimName))
                {
                    MessageBox.Show("All animations must have name defined.\nName can't be all whitespaces.");
                    return;
                }

                // Remove whitespace
                _Project.AnimDefinitons[i].AnimName = _Project.AnimDefinitons[i].AnimName.Trim();

                // Convert AnimDefs to zero based indexing
                animDefClone = AnimDef.Clone(_Project.AnimDefinitons[i]);
                animDefClone.CellXstartIndex -= 1;
                animDefClone.CellYstartIndex -= 1;
                zeroIndexedAnimDefinitons[i] = animDefClone;
            }

            // Create data
            var animChainList = Generator.Generate(
                    // sheet cell size
                    _Project.SheetCellSize,
                    // sheet file name only (wo path)
                    Path.GetFileName(_Project.SheetFilePath),
                    // 
                    _Project.Rotations,
                    //
                    new Offset<float> { X = (float)_Project.FramesOffset.X, Y = (float)_Project.FramesOffset.Y },
                    // 
                    zeroIndexedAnimDefinitons
                );

            // Save achx
            Generator.SaveAchx(
                // AnimChainListSave generated by Generator.Generate()
                animChainList,
                // - achx
                // output path
                achxOutputDir,
                // output file name wo extension 
                achxOutputFileNameWOExt,
                // path to sprite sheet 
                Path.GetDirectoryName(_Project.SheetFilePath)
            );

            MessageBox.Show("Achx generation successful");

            // Run achx
            if (CheckBoxOpenAchx.IsChecked.Value)
            {
                System.Diagnostics.Process.Start(
                    Path.Combine(achxOutputDir, achxOutputFileNameWOExt + ".achx")
                );
            }
        }

    }
}
