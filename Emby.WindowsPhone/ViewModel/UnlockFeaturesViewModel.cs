using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Services;
using GalaSoft.MvvmLight.Command;

namespace Emby.WindowsPhone.ViewModel
{
    public class UnlockFeaturesViewModel : ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public UnlockFeaturesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public RelayCommand UnlockCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (await TrialHelper.Current.Buy())
                    {
                        Log.Info("In-App purchase bought");
                        App.ShowMessage(AppResources.MessageUpgradeSuccessful);
                        _navigationService.GoBack();
                    }
                });
            }
        }
    }
}
