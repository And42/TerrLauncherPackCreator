using System;
using JetBrains.Annotations;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        public bool Working
        {
            get => _working;
            set => SetProperty(ref _working, value);
        }

        private bool _working;

        protected IDisposable LaunchWork() => new WorkingDisposable(this);
        
        private class WorkingDisposable : IDisposable
        {
            [CanBeNull]
            private ViewModelBase _viewModelBase;
            
            public WorkingDisposable([NotNull] ViewModelBase viewModel)
            {
                _viewModelBase = viewModel;
                viewModel.Working = true;
            }
            
            public void Dispose()
            {
                if (_viewModelBase != null)
                {
                    _viewModelBase.Working = false;
                    _viewModelBase = null;
                }
            }
        }
    }
}