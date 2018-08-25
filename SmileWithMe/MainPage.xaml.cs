using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.AI.MachineLearning.Preview;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Media;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmileWithMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnLoadPicture_ClickAsync(object sender, RoutedEventArgs e)
        {
            lstEmotions.Items.Clear();
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");

            var file = await filePicker.PickSingleFileAsync();
            if (file == null)
            {
                //operation cancelled 
                return;
            }

            BitmapImage imgBitmap = new BitmapImage(new Uri(file.Path));
            imgPicture.Source = imgBitmap;

            StorageFile modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Emotion.onnx"));
            CNTKGraphModel model = await CNTKGraphModel.CreateCNTKGraphModel(modelFile);

            SoftwareBitmap softwareBitmap;
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                VideoFrame vf = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
                CNTKGraphModelInput input = new CNTKGraphModelInput();
                input.Input338 = vf;

                CNTKGraphModelOutput output = await model.EvaluateAsync(input);
                foreach(var item in output.Plus692_Output_0)
                {
                    lstEmotions.Items.Add(item.ToString());
                }
            }


            
        }

    }
}
