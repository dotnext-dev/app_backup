using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LaunchContentDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _host.Children.Clear();
            var urls = new Dictionary<string,string> {
                { "TN - Economy", "https://delta-sharada.github.io/ndc-content/tn-economy/index.html" },
                { "TN - Business", "https://delta-sharada.github.io/ndc-content/tn-business/index.html" },
                { "SQ - Economy", "https://delta-sharada.github.io/ndc-content/sq-economy/index.html" },
                { "SQ - Premium", "https://delta-sharada.github.io/ndc-content/sq-premium-economy/index.html" },
                { "SQ - Business", "https://delta-sharada.github.io/ndc-content/sq-business/index.html" },
                { "VR - mode", "https://ndc-hackathon-demo.glitch.me/" },
            };

            foreach (var key in urls.Keys)
            {
                var btn = new Button
                {
                    Text = key,
                    BackgroundColor = Color.Silver
                };

                var url = urls[key];
                btn.Clicked += (s, args) => Launch(url);
                _host.Children.Add(btn);
            }
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
