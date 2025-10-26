using System;
using MVVM_Tools.Code.Classes;

namespace TerrLauncherPackCreator.Code.ViewModels;

public abstract class ViewModelBase : BindableBase
{
    public bool Working
    {
        get;
        set => SetProperty(ref field, value);
    }

    protected IDisposable LaunchWork() => new WorkingDisposable(this);
        
    private class WorkingDisposable : IDisposable
    {
        private ViewModelBase? _viewModelBase;
            
        public WorkingDisposable(ViewModelBase viewModel)
        {
            _viewModelBase = viewModel;
            viewModel.Working = true;
        }
            
        public void Dispose()
        {
            if (_viewModelBase == null)
                return;

            _viewModelBase.Working = false;
            _viewModelBase = null;
        }
    }
}