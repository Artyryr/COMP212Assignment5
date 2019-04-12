// Fig. 23.4: FickrViewerForm.cs
// Invoking a web service asynchronously with class HttpClient
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FlickrViewer
{
    public partial class FickrViewerForm : Form
    {
        // Use your Flickr API key here--you can get one at:
        // https://www.flickr.com/services/apps/create/apply
        private const string KEY = "a2b003481632a966c5f43b9671f70a9b";

        // object used to invoke Flickr web service      
        private static HttpClient flickrClient = new HttpClient();

        Task<string> flickrTask = null; // Task<string> that queries Flickr

        List<FlickrResult> flickrPhotos = new List<FlickrResult>() { };

        public FickrViewerForm()
        {
            InitializeComponent();
        }

        // initiate asynchronous Flickr search query; 
        // display results when query completes
        private async void searchButton_Click(object sender, EventArgs e)
        {
            // if flickrTask already running, prompt user 
            if (flickrTask?.Status != TaskStatus.RanToCompletion)
            {
                var result = MessageBox.Show(
                   "Cancel the current Flickr search?",
                   "Are you sure?", MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question);

                // determine whether user wants to cancel prior search
                if (result == DialogResult.No)
                {
                    return;
                }
                else
                {
                    flickrClient.CancelPendingRequests(); // cancel search
                }
            }

            // Flickr's web service URL for searches                         
            var flickrURL = "https://api.flickr.com/services/rest/?method=" +
               $"flickr.photos.search&api_key={KEY}&" +
               $"tags={inputTextBox.Text.Replace(" ", ",")}" +
               "&tag_mode=all&per_page=500&privacy_filter=1";

            imagesListBox.DataSource = null; // remove prior data source
            imagesListBox.Items.Clear(); // clear imagesListBox
            pictureBox.Image = null; // clear pictureBox
            imagesListBox.Items.Add("Loading..."); // display Loading...

            // invoke Flickr web service to search Flick with user's tags
            flickrTask = flickrClient.GetStringAsync(flickrURL);

            // await flickrTask then parse results with XDocument and LINQ
            XDocument flickrXML = XDocument.Parse(await flickrTask);

            // gather information on all photos
            //List<FlickrResult> flickrPhotos = new List<FlickrResult>() { };
            flickrPhotos =
            (from photo in flickrXML.Descendants("photo")
             let id = photo.Attribute("id").Value
             let title = photo.Attribute("title").Value
             let secret = photo.Attribute("secret").Value
             let server = photo.Attribute("server").Value
             let farm = photo.Attribute("farm").Value
             select new FlickrResult
             {
                 Title = title,
                 URL = $"https://farm{farm}.staticflickr.com/" +
                   $"{server}/{id}_{secret}.jpg"
             }).ToList();
            imagesListBox.Items.Clear(); // clear imagesListBox

            // set ListBox properties only if results were found
            if (flickrPhotos.Any())
            {
                imagesListBox.DataSource = flickrPhotos.ToList();
                imagesListBox.DisplayMember = "Title";
            }
            else // no matches were found
            {
                imagesListBox.Items.Add("No matches");
            }
        }

        // display selected image
        private async void imagesListBox_SelectedIndexChanged(
           object sender, EventArgs e)
        {
            if (imagesListBox.SelectedItem != null)
            {
                string selectedURL = ((FlickrResult)imagesListBox.SelectedItem).URL;

                // use HttpClient to get selected image's bytes asynchronously
                byte[] imageBytes = await flickrClient.GetByteArrayAsync(selectedURL);

                // display downloaded image in pictureBox                  
                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    pictureBox.Image = Image.FromStream(memoryStream);
                }
            }
        }

        // ------------------Added Code------------------

        // All images are saved in FlikrViewer/bin/Debug/Images
        private static async void ResizeImage(FlickrResult photo, int width)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(photo.URL);

                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = Image.FromStream(stream);

                        var height = (width * image.Height) / image.Width;
                        var thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                        if (photo.Title != "" && photo.Title != null && photo.Title.Count() <= 250)
                        {
                            thumbnail.Save(@"Images\" + photo.Title + ".jpeg");
                        }
                    }
                }
                catch { }
            });
        }
        private async void btnSaveAll_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(txtNewSize.Text);
                if (width >= 1 && width <= 2000)
                {
                    await Task.Factory.StartNew(() => {
                        ParallelLoopResult loopResult = Parallel.ForEach<FlickrResult>(flickrPhotos, p =>
                        {
                            ResizeImage(p, width);
                        });
                    });

                }
                else
                {
                    MessageBox.Show("Size of an image should be in range 1 to 2000");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnSaveCurrent_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(txtNewSize.Text);
                if (width >= 1 && width <= 2000)
                {
                    if (imagesListBox.SelectedItem != null)
                    {
                        FlickrResult selectedImage = (FlickrResult)imagesListBox.SelectedItem;

                        ResizeImage(selectedImage, width);
                        MessageBox.Show("The image is saved");
                    }
                }
                else
                {
                    MessageBox.Show("Size of an image should be in range 1 to 2000");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

/**************************************************************************
 * (C) Copyright 1992-2017 by Deitel & Associates, Inc. and               *
 * Pearson Education, Inc. All Rights Reserved.                           *
 *                                                                        *
 * DISCLAIMER: The authors and publisher of this book have used their     *
 * best efforts in preparing the book. These efforts include the          *
 * development, research, and testing of the theories and programs        *
 * to determine their effectiveness. The authors and publisher make       *
 * no warranty of any kind, expressed or implied, with regard to these    *
 * programs or to the documentation contained in these books. The authors *
 * and publisher shall not be liable in any event for incidental or       *
 * consequential damages in connection with, or arising out of, the       *
 * furnishing, performance, or use of these programs.                     *
 **************************************************************************/
