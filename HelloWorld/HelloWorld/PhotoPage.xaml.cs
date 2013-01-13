using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace HelloWorld
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PhotoPage : HelloWorld.Common.LayoutAwarePage
    {

        private String mruToken = null;


        public PhotoPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState != null && pageState.ContainsKey("mruToken"))
            {
                object value = null;

                if (pageState.TryGetValue("mruToken", out value))
                {
                    mruToken = value.ToString();
                    StorageFile file = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(mruToken);

                    await DisplayAndBindData(file);

                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (!String.IsNullOrEmpty(mruToken))
            {
                pageState["mruToken"] = mruToken;
            }

        }

        private async void GetPhotoButtonClick(object sender, RoutedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped || Windows.UI.ViewManagement.ApplicationView.TryUnsnap() == true)
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.ViewMode = PickerViewMode.Thumbnail;

                //Filter to include a sample subset of file types
                openPicker.FileTypeFilter.Clear();
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".jpg");

                //Open the file picker
                StorageFile file = await openPicker.PickSingleFileAsync();

                await DisplayAndBindData(file);
            }
        }

        private async System.Threading.Tasks.Task DisplayAndBindData(StorageFile file)
        {
            //File is null if the user cancels the file picker
            if (file != null)
            {
                //Open a stream for the selected file
                IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(fileStream);
                displayImage.Source = bitmapImage;
                this.DataContext = file;

                //Add picked file to Most Recently Used list
                mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
            }
        }
    }
}
