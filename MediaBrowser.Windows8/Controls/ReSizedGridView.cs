using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ScottIsAFool.Windows8.Controls
{
    public class ReSizedGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            try
            {
                dynamic _Item = item;
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, _Item.ColSpan);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, _Item.RowSpan);
            }
            catch
            {
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
            }
            finally
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }
    }
}