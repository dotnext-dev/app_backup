using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Fly360
{
    public class ResultsPopupPage : PopupPage
    {
        public ResultsPopupPage()
        {
            HasSystemPadding = true;
            var imgSource = ImageSource.FromResource("Fly360.images.results.png");
            var image = new Image
            {
                Source = imgSource,
                Aspect = Aspect.Fill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            image.GestureRecognizers.Add(new TapGestureRecognizer(async (obj) =>
            {
                Launch("https://fly360.github.io/content/sq-business/index.html");
            }));
            Content = new Grid
            {
                Children = {
                    image
                },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
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

