using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Fly360
{
    public class SearchPopupPage : PopupPage
    {
        public SearchPopupPage()
        {
            HasSystemPadding = true;
            var imgSource = ImageSource.FromResource("Fly360.images.search.png");
            var image = new Image
            {
                Source = imgSource,
                Aspect = Aspect.Fill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            image.GestureRecognizers.Add(new TapGestureRecognizer(async (obj) =>
            {
                await PopupNavigation.PopAsync();
                await Navigation.PushPopupAsync(new ResultsPopupPage());
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
    }
}

