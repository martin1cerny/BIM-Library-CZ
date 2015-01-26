using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BLSpec
{
    public interface IExternalCommand
    {
        void Execute(BLData.BLModel model, UIHelper ui);
        string Name { get; }
        Guid ID { get; }
    }

    public class UIHelper
    {
        private Window _mainWin;

        internal UIHelper(Window window)
        {
            _mainWin = window;
        }
        public void RegisterToMainWindow(Window child) 
        {
            child.Owner = _mainWin;
        }

        public bool? ShowDialog(FileDialog dialog)
        {
            return dialog.ShowDialog(_mainWin);
        }

        public void ShowMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
