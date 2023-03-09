using SignalDebug.ViewModels;

namespace SignalDebug.Views;

public partial class ShareFilePage : ContentPage
{
    ShareFilePageModel shareFilePageModel = null;
    public ShareFilePage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        shareFilePageModel = this.BindingContext as ShareFilePageModel;
        MessagingCenter.Subscribe<ShareFilePageModel>(this, "OpenFile", async (sender) =>
        {
            if (shareFilePageModel == null || shareFilePageModel.CurrentFileInfo == null)
            {
                await DisplayAlert("提示", "请选择文件!", "确定");
            }
            else
            {
                if(File.Exists(shareFilePageModel.CurrentFileInfo.FullPath))
                {
                    var datas = File.ReadAllLines(shareFilePageModel.CurrentFileInfo.FullPath);
                    await Navigation.PushAsync(new DataViewPage { Data = datas });
                }
            }
        });
        MessagingCenter.Subscribe<ShareFilePageModel>(this, "ShareFile", async (sender) =>
        {
            if (shareFilePageModel == null || shareFilePageModel.CurrentFileInfo == null)
            {
                await DisplayAlert("提示", "请选择文件!", "确定");
            }
            else
            {
                if (File.Exists(shareFilePageModel.CurrentFileInfo.FullPath))
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = "分享文件",
                        File = new ShareFile(shareFilePageModel.CurrentFileInfo.FullPath)
                    });
                }
            }
        });
        MessagingCenter.Subscribe<ShareFilePageModel>(this, "DeleteFile", async (sender) =>
        {
            if (shareFilePageModel == null || shareFilePageModel.CurrentFileInfo == null)
            {
                await DisplayAlert("提示", "请选择文件!", "确定");
            }
            else
            {
                bool rel = await DisplayAlert("提示", $"确认删除文件:{shareFilePageModel.CurrentFileInfo.FileName}吗?", "确认", "取消");
                if (rel)
                {
                    if (File.Exists(shareFilePageModel.CurrentFileInfo.FullPath))
                        File.Delete(shareFilePageModel.CurrentFileInfo.FullPath);
                    shareFilePageModel.FileInfos.Remove(shareFilePageModel.CurrentFileInfo);
                    List<SignalDebug.Models.FileInfo> temps = new List<Models.FileInfo>();
                    shareFilePageModel.FileInfos.ForEach(f =>
                    {
                        temps.Add(f);
                    });
                    shareFilePageModel.FileInfos = temps;
                    shareFilePageModel.CurrentFileInfo = null;
                }
            }
        });
        base.OnAppearing();
    }

    private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        shareFilePageModel.CurrentFileInfo = e.CurrentSelection[0] as Models.FileInfo;
    }
    protected override void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<ShareFilePageModel>(this, "OpenFile");
        MessagingCenter.Unsubscribe<ShareFilePageModel>(this, "ShareFile");
        MessagingCenter.Unsubscribe<ShareFilePageModel>(this, "DeleteFile");
        base.OnDisappearing();
    }
}