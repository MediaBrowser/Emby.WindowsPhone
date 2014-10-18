using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using MediaBrowser.Model.Dto;
using Telerik.Windows.Controls;

namespace MediaBrowser.WindowsPhone.Behaviours
{
    public class AddToCheckedItemBehaviorBase<T> : Behavior<RadDataBoundListBox> where T : class
    {
        public bool TurnSelectionOffOnEmpty { get; set; }
        public bool AddTappedItemOnOpen { get; set; }
        public virtual ObservableCollection<T> SelectedItems { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ItemCheckedStateChanged += AssociatedObject_ItemCheckedStateChanged;
            AssociatedObject.IsCheckModeActiveChanged += AssociatedObjectOnIsCheckModeActiveChanged;
        }

        private void AssociatedObjectOnIsCheckModeActiveChanged(object sender, IsCheckModeActiveChangedEventArgs e)
        {
            if (!e.CheckBoxesVisible || e.TappedItem == null || !AddTappedItemOnOpen)
            {
                return;
            }

            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<T>();
            }

            var item = e.TappedItem as T;
            if (item == null)
            {
                return;
            }

            if (!SelectedItems.Contains(item))
            {
                SelectedItems.Add(item);
            }

            AssociatedObject.CheckedItems.Add(item);
        }

        private void AssociatedObject_ItemCheckedStateChanged(object sender, ItemCheckedStateChangedEventArgs e)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<T>();
            }

            var item = e.Item as T ?? ((Telerik.Windows.Data.IDataSourceItem)e.Item).Value as T;

            if (item == null)
            {
                return;
            }

            if (e.IsChecked)
            {
                if (SelectedItems.Contains(item))
                {
                    return;
                }

                SelectedItems.Add(item);
            }
            else
            {
                SelectedItems.Remove(item);

                if (SelectedItems != null && !SelectedItems.Any() && TurnSelectionOffOnEmpty)
                {
                    AssociatedObject.IsCheckModeActive = false;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ItemCheckedStateChanged -= AssociatedObject_ItemCheckedStateChanged;
            AssociatedObject.IsCheckModeActiveChanged -= AssociatedObjectOnIsCheckModeActiveChanged;
        }
    }

    public class AddItemsToCheckedItems : AddToCheckedItemBehaviorBase<BaseItemDto>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<BaseItemDto>), typeof(AddItemsToCheckedItems), new PropertyMetadata(default(ObservableCollection<BaseItemDto>)));

        public override ObservableCollection<BaseItemDto> SelectedItems
        {
            get { return (ObservableCollection<BaseItemDto>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public AddItemsToCheckedItems()
        {
            AddTappedItemOnOpen = true;
            TurnSelectionOffOnEmpty = true;
        }
    }
}
