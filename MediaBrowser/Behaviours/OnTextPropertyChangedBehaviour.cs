using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ScottIsAFool.WindowsPhone.Behaviours
{
    public class UpdateTextBindingOnPropertyChanged : Behavior<TextBox>
    {
        public static readonly DependencyProperty EnterHitCommandProperty =
            DependencyProperty.Register("EnterHitCommand", typeof (ICommand), typeof (UpdateTextBindingOnPropertyChanged), new PropertyMetadata(default(ICommand)));

        public ICommand EnterHitCommand
        {
            get { return (ICommand) GetValue(EnterHitCommandProperty); }
            set { SetValue(EnterHitCommandProperty, value); }
        }

        // Fields
        private BindingExpression expression;

        // Methods
        protected override void OnAttached()
        {
            base.OnAttached();
            this.expression = base.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            base.AssociatedObject.TextChanged += OnTextChanged;
            base.AssociatedObject.KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Enter) return;
            if (EnterHitCommand != null && EnterHitCommand.CanExecute(null))
            {
                EnterHitCommand.Execute(null);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.TextChanged -= OnTextChanged;
            this.expression = null;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs args)
        {
            this.expression.UpdateSource();
        }
    }
}
