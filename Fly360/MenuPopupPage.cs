using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Fly360
{
    public class MenuPopupPage : PopupPage
    {
        public MenuPopupPage()
        {
            //<Button Text="Close Popup" TextColor="Red" Clicked="OnClose" VerticalOptions="CenterAndExpand" HorizontalOptions="CenterAndExpand"></Button>
            HasSystemPadding = true;

            var label = new Label
            {
                Text = "Suggested Destination Types",
                TextColor = Color.White,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var closeBtn = new Button
            {
                Text = "CLOSE",
                TextColor = Color.Red,
                BackgroundColor = Color.White,
                CornerRadius = 5
            };
            closeBtn.Clicked += async(sender, e) => {
                await PopupNavigation.PopAsync();
            };

            var lables = new string[] {
                "City Break",
                "Ski",
                "Beach",
                "Luxury",
                "Family",
                "Safari"
            };
            var buttons = new List<Button>();
            for (int i = 0; i < 6; i++)
            {
                var btn = new Button
                {
                    Text = lables[i],
                    TextColor = i == 2 ? Color.Black : Color.White,
                    BackgroundColor = i == 2 ? Color.White : Color.DarkGray,
                    Opacity = i == 2 ? 1 : 0.75,
                    BorderColor = i == 2 ? Color.Blue : Color.White,
                    BorderWidth = 2,
                    CornerRadius = 10
                };
                Grid.SetRow(btn, i / 2);
                Grid.SetColumn(btn, i % 2);
                buttons.Add(btn);

                if(i == 2)
                {
                    btn.Clicked += async (sender, e) => {
                        await PopupNavigation.PopAsync();
                        await Navigation.PushAsync(new BeachOptionsPage());
                    };
                }
            }

            var grid = new Grid
            {
                RowDefinitions = {
                    new RowDefinition(),
                    new RowDefinition(),
                    new RowDefinition()
                },
                ColumnDefinitions = {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                },
                ColumnSpacing = 35,
                RowSpacing = 35,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 200
            };

            foreach (var b in buttons)
                grid.Children.Add(b);

            Content = new Grid
            {
                Children = {
                    new Image { Source = "blur.jpg", Opacity = 0.5, Aspect = Aspect.Fill },
                    new StackLayout {
                        Margin = new Thickness(40, 80, 40, 40),
                        Spacing = 30,
                        Children = {
                            label,
                            grid,
                            closeBtn
                        },
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                    }
                },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
        }
    }
}

