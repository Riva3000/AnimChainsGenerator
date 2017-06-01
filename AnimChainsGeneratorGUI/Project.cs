using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimChainsGenerator
{
    public class Project : INotifyPropertyChanged
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

        private Size _SheetCellSize = new Size { Width = 32, Height = 32 };
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

        private Offset<double> _FramesOffset = new Offset<double>();
        public Offset<double> FramesOffset
        {
            get { return _FramesOffset; }
            set { SetField(ref _FramesOffset, value, "FramesOffset"); }
        }

        private ObservableCollection<AnimDef> _AnimDefinitons = new ObservableCollection<AnimDef>();
        public ObservableCollection<AnimDef> AnimDefinitons { get { return _AnimDefinitons; } }
    }
}
