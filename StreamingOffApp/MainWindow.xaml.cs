using StreamingOffApp.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StreamingOffApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SortColumn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader header && DataContext is MainViewModel viewModel)
            {
                if (header.Column?.HeaderTemplate?.LoadContent() is StackPanel stackPanel)
                {
                    var textBlock = stackPanel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Tag != null);
                    if (textBlock?.Tag is string columnName)
                    {
                        viewModel.SortCommand.Execute(columnName);
                    }
                }
            }
        }

        private void OffersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}