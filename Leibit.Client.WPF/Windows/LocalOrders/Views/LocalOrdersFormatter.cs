using Leibit.Core.Common;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.LocalOrders.Views
{
    public class LocalOrdersFormatter
    {

        #region [Command]
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(LocalOrdersFormatter), new PropertyMetadata(null, __OnCommandChanged));

        private static void __OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            __Refresh(d as TextBlock);
        }
        #endregion

        #region [Content]
        public static string GetContent(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject obj, string value)
        {
            obj.SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached("Content", typeof(string), typeof(LocalOrdersFormatter), new PropertyMetadata(null, __OnContentChanged));

        private static void __OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            __Refresh(d as TextBlock);
        }
        #endregion

        #region [__Refresh]
        private static void __Refresh(TextBlock textBlock)
        {
            if (textBlock == null)
                return;

            var content = GetContent(textBlock);

            if (content.IsNullOrWhiteSpace())
                return;

            textBlock.Inlines.Clear();

            var matches = Regex.Matches(content, "[0-9]{4,}");
            int currentIndex = 0;

            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;

                var run = new Run(content[currentIndex..match.Index]);
                textBlock.Inlines.Add(run);

                var link = new Hyperlink(new Run(content.Substring(match.Index, match.Length)));
                var command = GetCommand(textBlock);

                if (command != null)
                {
                    link.Command = command;
                    link.CommandParameter = int.Parse(match.Value);
                }

                textBlock.Inlines.Add(link);

                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < content.Length)
            {
                var run = new Run(content[currentIndex..]);
                textBlock.Inlines.Add(run);
            }
        }
        #endregion

    }
}
