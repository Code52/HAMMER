using System;
using Windows.UI.Xaml;

namespace HAMMER.Nails
{
    public sealed partial class FeedbackView
    {
        public Action<FeedbackResult> Results { get; set; }

        public FeedbackView()
        {
            InitializeComponent();
        }

        private void SubmitClicked(object sender, RoutedEventArgs e)
        {
            var r = new FeedbackResult();
            Results(r);
        }
    }
}
