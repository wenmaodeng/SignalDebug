using SignalDebug.ViewModels;

namespace SignalDebug.Views;

public partial class ShareDirectoryPage : ContentPage
{
    ShareDirectoryModel shareDirectoryModel = null;

    public ShareDirectoryPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        shareDirectoryModel = this.BindingContext as ShareDirectoryModel;
        MessagingCenter.Subscribe<ShareDirectoryModel>(this, "OpenDirectory", async (sender) =>
        {
            if (shareDirectoryModel==null|| shareDirectoryModel.CurrentDirectoryInfo==null)
            {
                await DisplayAlert("��ʾ", "��ѡ���ļ���!", "ȷ��");
            }
            else
            {
                ShareFilePageModel shareFilePageModel = new ShareFilePageModel();
                var files = Directory.GetFiles(shareDirectoryModel.CurrentDirectoryInfo.FullDirectory);
                files?.ToList().ForEach(f =>
                {
                    SignalDebug.Models.FileInfo fileInfo = new SignalDebug.Models.FileInfo();
                    fileInfo.FileName = System.IO.Path.GetFileName(f);
                    fileInfo.FullPath = f;
                    shareFilePageModel.FileInfos.Add(fileInfo);
                });

                await Navigation.PushAsync(new ShareFilePage { BindingContext = shareFilePageModel });
            }
        });
        MessagingCenter.Subscribe<ShareDirectoryModel>(this, "DeleteDirectory", async (sender) =>
        {
            if (shareDirectoryModel == null || shareDirectoryModel.CurrentDirectoryInfo == null)
            {
                await DisplayAlert("��ʾ", "��ѡ���ļ���!", "ȷ��");
            }
            else
            {
                bool rel = await DisplayAlert("��ʾ", $"ȷ��ɾ���ļ���:{shareDirectoryModel.CurrentDirectoryInfo.Directory}��?", "ȷ��", "ȡ��");
                if (rel)
                {
                    if (Directory.Exists(shareDirectoryModel.CurrentDirectoryInfo.FullDirectory))
                        Directory.Delete(shareDirectoryModel.CurrentDirectoryInfo.FullDirectory);
                    shareDirectoryModel.DirectoryInfos.Remove(shareDirectoryModel.CurrentDirectoryInfo);
                    List<SignalDebug.Models.DirectoryInfo> temps = new List<Models.DirectoryInfo>();
                    shareDirectoryModel.DirectoryInfos.ForEach(d =>
                    {
                        temps.Add(d);
                    });
                    shareDirectoryModel.DirectoryInfos = temps;
                    shareDirectoryModel.CurrentDirectoryInfo = null;
                }
            }
        });
        base.OnAppearing();
    }

    private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        shareDirectoryModel.CurrentDirectoryInfo = e.CurrentSelection[0] as SignalDebug.Models.DirectoryInfo;
    }
    protected override void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<ShareDirectoryModel>(this, "OpenDirectory");
        MessagingCenter.Unsubscribe<ShareDirectoryModel>(this, "DeleteDirectory");
        base.OnDisappearing();
    }
}