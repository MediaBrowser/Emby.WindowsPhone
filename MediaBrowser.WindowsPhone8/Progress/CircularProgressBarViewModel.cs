using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MediaBrowser.WindowsPhone.Behaviours;

namespace MediaBrowser.WindowsPhone.Progress
{
    /// <summary>
    /// An attached view model that adapts a ProgressBar control to provide properties
    /// that assist in the creation of a circular template
    /// </summary>
    public class CircularProgressBarViewModel : FrameworkElement, INotifyPropertyChanged
    {
        #region Attach attached property

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(object), typeof(CircularProgressBarViewModel), new PropertyMetadata(null, new PropertyChangedCallback(OnAttachChanged)));

        public static CircularProgressBarViewModel GetAttach(DependencyObject d)
        {
            return (CircularProgressBarViewModel)d.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject d, CircularProgressBarViewModel value)
        {
            d.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Change handler for the Attach property
        /// </summary>
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // set the view model as the DataContext for the rest of the template
            FrameworkElement targetElement = d as FrameworkElement;
            CircularProgressBarViewModel viewModel = e.NewValue as CircularProgressBarViewModel;
            targetElement.DataContext = viewModel;

            // handle the loaded event
            targetElement.Loaded += new RoutedEventHandler(Element_Loaded);
        }

        /// <summary>
        /// Handle the Loaded event of the element to which this view model is attached
        /// in order to enable the attached
        /// view model to bind to properties of the parent element
        /// </summary>
        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement targetElement = sender as FrameworkElement;
            CircularProgressBarViewModel attachedModel = GetAttach(targetElement);

            // find the ProgressBar and associated it with the view model
            var progressBar = targetElement.Ancestors<ProgressBar>().Single() as ProgressBar;
            attachedModel.SetProgressBar(progressBar);
        }



        #endregion


        #region fields

        private double _angle;

        private double _centreX;

        private double _centreY;

        private double _radius;

        private double _innerRadius;

        private double _diameter;

        private double _percent;

        private double _holeSizeFactor = 0.0;

        protected ProgressBar _progressBar;

        #endregion

        #region properties

        public double Percent
        {
            get
            {
                return this._percent;
            }
            set
            {
                this._percent = value;
                this.OnPropertyChanged("Percent");
            }
        }

        public double Diameter
        {
            get
            {
                return this._diameter;
            }
            set
            {
                this._diameter = value;
                this.OnPropertyChanged("Diameter");
            }
        }

        public double Radius
        {
            get
            {
                return this._radius;
            }
            set
            {
                this._radius = value;
                this.OnPropertyChanged("Radius");
            }
        }

        public double InnerRadius
        {
            get
            {
                return this._innerRadius;
            }
            set
            {
                this._innerRadius = value;
                this.OnPropertyChanged("InnerRadius");
            }
        }

        public double CentreX
        {
            get
            {
                return this._centreX;
            }
            set
            {
                this._centreX = value;
                this.OnPropertyChanged("CentreX");
            }
        }

        public double CentreY
        {
            get
            {
                return this._centreY;
            }
            set
            {
                this._centreY = value;
                this.OnPropertyChanged("CentreY");
            }
        }

        public double Angle
        {
            get
            {
                return this._angle;
            }
            set
            {
                this._angle = value;
                this.OnPropertyChanged("Angle");
            }
        }

        public double HoleSizeFactor
        {
            get
            {
                return this._holeSizeFactor;
            }
            set
            {
                this._holeSizeFactor = value;
                this.ComputeViewModelProperties();
            }
        }

        #endregion


        /// <summary>
        /// Re-computes the various properties that the elements in the template bind to.
        /// </summary>
        protected virtual void ComputeViewModelProperties()
        {
            if (this._progressBar == null) return;

            this.Angle = (this._progressBar.Value - this._progressBar.Minimum) * 360 / (this._progressBar.Maximum - this._progressBar.Minimum);
            this.CentreX = this._progressBar.ActualWidth / 2;
            this.CentreY = this._progressBar.ActualHeight / 2;
            this.Radius = Math.Min(this.CentreX, this.CentreY);
            this.Diameter = this.Radius * 2;
            this.InnerRadius = this.Radius * this.HoleSizeFactor;
            this.Percent = this.Angle / 360;
        }

        /// <summary>
        /// Add handlers for the updates on various properties of the ProgressBar
        /// </summary>
        private void SetProgressBar(ProgressBar progressBar)
        {
            this._progressBar = progressBar;
            this._progressBar.SizeChanged += (s, e) => this.ComputeViewModelProperties();
            this.RegisterForNotification("Value", progressBar, (d, e) => this.ComputeViewModelProperties());
            this.RegisterForNotification("Maximum", progressBar, (d, e) => this.ComputeViewModelProperties());
            this.RegisterForNotification("Minimum", progressBar, (d, e) => this.ComputeViewModelProperties());

            this.ComputeViewModelProperties();
        }


        /// Add a handler for a DP change
        /// see: http://amazedsaint.blogspot.com/2009/12/silverlight-listening-to-dependency.html
        private void RegisterForNotification(string propertyName, FrameworkElement element, PropertyChangedCallback callback)
        {

            //Bind to a dependency property  
            Binding b = new Binding(propertyName) { Source = element };
            var prop = System.Windows.DependencyProperty.RegisterAttached("ListenAttached" + propertyName, typeof(object), typeof(UserControl), new PropertyMetadata(callback));

            element.SetBinding(prop, b);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}