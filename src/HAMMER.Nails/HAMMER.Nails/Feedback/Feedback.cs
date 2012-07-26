using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace HAMMER.Nails
{
    public static class Feedback
    {
        public static async Task<FeedbackResult> ShowFeedback()
        {
            var result = new FeedbackResult();
            var tcs = new TaskCompletionSource<int>();
            var p = new Popup
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };

            var f = new FeedbackView
                        {
                            VerticalAlignment = VerticalAlignment.Bottom,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Results = (r) =>
                                          {

                                              result = r;
                                              tcs.TrySetResult(1);
                                              p.IsOpen = false;
                                          }
                        };
            p.Child = f;
            p.IsOpen = true;
            await tcs.Task;
            return result;
        }

        private static List<FeedbackOption> DefaultLikeOptions
        {
            get
            {
                return new List<FeedbackOption>();
            }
        }

        private static List<FeedbackOption> DefaultDislikeOptions
        {
            get
            {
                return new List<FeedbackOption>();
            }
        }
    }
}
