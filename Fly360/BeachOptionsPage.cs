using System;

using Xamarin.Forms;

namespace Fly360
{
    public class BeachOptionsPage : ContentPage
    {
        public BeachOptionsPage()
        {
            Title = "Beach";
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            var beaches = new string[] {
                "Hawaii", "Miramar Beach", "Nissi", "Goa", "Brighton", "Caibbean"
            };
            var container = new StackLayout();
            for (var i = 1; i <= 5; i++)
            {
                container.Children.Add(GetImage(i, beaches[i - 1]));    
            }
            Content = new ScrollView
            {
                Content = container
            };
        }

        View GetImage(int index, string destination)
        {
            var url = string.Format("Fly360.images.beach{0}.jpg", index);
            var image = new Image
            {
                Source = ImageSource.FromResource(url),
                Aspect = Aspect.AspectFill
            };
            var label = new Label
            {
                BackgroundColor = Color.FromHex("#88000000"),
                TextColor = Color.White,
                Text = destination,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.End
            };
            var grid = new Grid
            {
                HeightRequest = 175,
                Children = {
                    image, label
                }
            };

            grid.GestureRecognizers.Add(new TapGestureRecognizer(async(obj) =>
            {
                await Navigation.PushAsync(new HawaiiDetailsPage());
            }));    
            return grid;
        }
    }
}

