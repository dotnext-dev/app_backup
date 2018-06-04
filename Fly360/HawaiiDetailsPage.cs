using System;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace Fly360
{
    public class HawaiiDetailsPage : ContentPage
    {
        public enum LinkType { Image360, ImageVR }
        public HawaiiDetailsPage()
        {
            Title = "Hawaii";
            NavigationPage.SetBackButtonTitle(this, string.Empty);
            ToolbarItems.Add(new ToolbarItem("Book", null, async () => 
            {
                await Navigation.PushPopupAsync(new SearchPopupPage());
            }));
                             
            var links = new string[] {
                "https://delta-sharada.github.io/ndc-content/hawaii/",
                "https://delta-sharada.github.io/ndc-content/hawaii/",
                "https://ndc-hackathon-demo.glitch.me/",
                "https://ndc-hackathon-demo.glitch.me/"

            };
            var types = new LinkType[] { 
                LinkType.Image360, 
                LinkType.Image360, 
                LinkType.ImageVR, 
                LinkType.ImageVR 
            };

            var container = new StackLayout();
            for (var i = 1; i <= 4; i++)
            {
                container.Children.Add(GetImage(i, types[i - 1], links[i - 1]));
            }
            Content = new ScrollView
            {
                Content = container
            };
        }

        View GetImage(int index, LinkType type, string link)
        {
            var url = string.Format("Fly360.images.beach{0}.jpg", index);
            var icon = string.Format("Fly360.images.icon{0}.png", type == LinkType.Image360 ? "Eq" : "VR");
            var image = new Image
            {
                Source = ImageSource.FromResource(url),
                Aspect = Aspect.AspectFill
            };
            var indicator = new Image
            {
                BackgroundColor = Color.FromHex("#88ffffff"),
                HeightRequest = 50,
                Source = ImageSource.FromResource(icon),
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var grid = new Grid
            {
                HeightRequest = 175,
                Children = {
                    image, indicator
                }
            };

            string launchUrl = link;
            grid.GestureRecognizers.Add(new TapGestureRecognizer((obj) =>
            {
                Launch(launchUrl);
            }));
            return grid;
        }

        void Launch(string url)
        {
            // We rely on the built-in service lcoator in this example, but you could just
            // as easily locate this service using DI and launch from your ViewModel
            var service = DependencyService.Get<INativeBrowserService>();
            {
                if (service == null) return;

                service.LaunchNativeEmbeddedBrowser(url);
            }
        }
    }
}

