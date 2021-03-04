using System;
using System.Windows;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class AttachedWindowManipulator : IAttachedWindowManipulator
    {
        private Window? _window;

        public AttachedWindowManipulator(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
        }

        public void Close()
        {
            CheckWindowNull();

            _window!.Close();
            _window = null;
        }

        private void CheckWindowNull()
        {
            if (_window == null)
                throw new NullReferenceException("manipulator window was closed");
        }
    }
}
