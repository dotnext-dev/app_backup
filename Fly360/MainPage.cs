using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Urho.Forms;
using Xamarin.Forms;

namespace Fly360
{
    public class MainPage : ContentPage
    {
        UrhoSurface urhoSurface;
        EarthGlobeView urhoApp;
        MenuPopupPage popupPage;

        public MainPage()
        {
            Title = "Fly 360";
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            BackgroundColor = Color.Black;

            var entry = new Entry
            {
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                Placeholder = "Search",
                PlaceholderColor = Color.LightGray,
                Margin = 25
            };
            var frame = new Frame
            {
                BackgroundColor = Color.LightGray,
                BorderColor = Color.LightGray,
                CornerRadius = 15,
                Opacity = 0.33,
                Margin = 25
            };

            var button = new Button
            {
                BackgroundColor = Color.White,
                TextColor = Color.Blue,
                Text = "   Suggested Destinations   ",
                HorizontalOptions = LayoutOptions.Center,
                Margin = 40
            };
            Grid.SetRow(button, 2);
            button.Clicked += async(s, a) => {
                await Navigation.PushPopupAsync(popupPage);
            };

            urhoSurface = new UrhoSurface();
            urhoSurface.VerticalOptions = LayoutOptions.FillAndExpand;
            Grid.SetRowSpan(urhoSurface, 3);

            popupPage = new MenuPopupPage();
            popupPage.Appearing += (s, a) =>
            {
                frame.IsVisible = false;
                entry.IsVisible = false;
                button.IsVisible = false;
            };
            popupPage.Disappearing += (s, a) =>
            {
                frame.IsVisible = true;
                entry.IsVisible = true;
                button.IsVisible = true;
            };

            Content = new Grid
            {
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                },
                Children = {
                    urhoSurface,
                    frame,
                    entry,
                    button,
                }
            };
        }

        protected override void OnDisappearing()
        {
            UrhoSurface.OnDestroy();
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            await StartUrhoApp();
            urhoApp.CitySelected += UrhoApp_CitySelected;
        }

        async Task StartUrhoApp()
        {
            urhoApp = await urhoSurface.Show<EarthGlobeView>(
                new Urho.ApplicationOptions(assetsFolder: "Data") { 
                    Orientation = Urho.ApplicationOptions.OrientationType.LandscapeAndPortrait 
            });
        }

        void UrhoApp_CitySelected(object sender, EventArgs e)
        {
            urhoApp.CitySelected -= UrhoApp_CitySelected;
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new HawaiiDetailsPage());
            });
        }

    }

}

