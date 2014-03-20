using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.LiveTv;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProgrammeViewModel : ViewModelBase
    {
        public ProgramInfoDto SelectedProgramme { get; set; }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ProgrammeItemChangedMsg))
                {
                    SelectedProgramme = (ProgramInfoDto) m.Sender;
                }
            });
        }
    }
}