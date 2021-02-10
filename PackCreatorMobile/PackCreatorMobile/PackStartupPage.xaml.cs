using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackCreatorMobile.Code.ViewModels;
using TerrLauncherPackCreator.Code.Interfaces;
using TerrLauncherPackCreator.Code.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PackCreatorMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PackStartupPage : ContentPage
    {
        public XamarinPackStartupWindowViewModel ViewModel
        {
            get => BindingContext as XamarinPackStartupWindowViewModel;
            set => BindingContext = value;
        }
        
        public PackStartupPage()
        {
            InitializeComponent();

            ViewModel = new XamarinPackStartupWindowViewModel(
                attachedWindowManipulator: new XamarinAttachedWindowManipulator(this),
                appSettings: null,
                goToMainPage: () => Navigation.PushAsync(new MainPage()),
                // todo: fix
                goToConverterPage: () => {}
            );
        }

        private class XamarinAttachedWindowManipulator : IAttachedWindowManipulator
        {
            private readonly PackStartupPage _packStartupPage;

            public XamarinAttachedWindowManipulator(
                PackStartupPage packStartupPage
            )
            {
                _packStartupPage = packStartupPage;
            }
            
            public void Close()
            {
                _packStartupPage.Navigation.RemovePage(_packStartupPage);
            }
        }
    }
}