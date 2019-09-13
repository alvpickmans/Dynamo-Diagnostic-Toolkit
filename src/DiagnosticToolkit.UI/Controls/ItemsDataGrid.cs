using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace DiagnosticToolkit.UI.Controls
{
    public class ItemsDataGrid : DataGrid
    {
        public ItemsDataGrid()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedElements = this.SelectedItems;
        }

        public IList SelectedElements
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                    DependencyProperty.Register(nameof(SelectedElements), typeof(IList), typeof(ItemsDataGrid), new PropertyMetadata(null));
    }
}