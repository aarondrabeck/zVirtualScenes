using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace zVirtualScenes_WPF
{
    public class VisibilityAnimation : DependencyObject
    {
        private const int DURATION_MS = 200;

        private static readonly Hashtable _hookedElements = new Hashtable();

        public static readonly DependencyProperty IsActiveProperty =
          DependencyProperty.RegisterAttached("IsActive",
          typeof(bool),
          typeof(VisibilityAnimation),
          new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActivePropertyChanged)));

        public static bool GetIsActive(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)element.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsActiveProperty, value);
        }

        static VisibilityAnimation()
        {
            UIElement.VisibilityProperty.AddOwner(typeof(FrameworkElement),
                                                  new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(VisibilityChanged), CoerceVisibility));
        }

        private static void VisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // So what? Ignore.
        }

        private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;
            if (fe == null)
            {
                return;
            }
            if (GetIsActive(fe))
            {
                HookVisibilityChanges(fe);
            }
            else
            {
                UnHookVisibilityChanges(fe);
            }
        }

        private static void UnHookVisibilityChanges(FrameworkElement fe)
        {
            if (_hookedElements.Contains(fe))
            {
                _hookedElements.Remove(fe);
            }
        }

        private static void HookVisibilityChanges(FrameworkElement fe)
        {
            _hookedElements.Add(fe, false);
        }

        private static object CoerceVisibility(DependencyObject d, object baseValue)
        {
            var fe = d as FrameworkElement;
            if (fe == null)
            {
                return baseValue;
            }

            if (CheckAndUpdateAnimationStartedFlag(fe))
            {
                return baseValue;
            }
            // If we get here, it means we have to start fade in or fade out
            // animation. In any case return value of this method will be
            // Visibility.Visible. 

            var visibility = (Visibility)baseValue;

            var da = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(DURATION_MS))
            };

            da.Completed += (o, e) =>
            {
                // This will trigger value coercion again
                // but CheckAndUpdateAnimationStartedFlag() function will reture true
                // this time, and animation will not be triggered.
                fe.Visibility = visibility;
                // NB: Small problem here. This may and probably will brake 
                // binding to visibility property.
            };

            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                da.From = fe.Opacity;
                da.To = 0.0;
            }
            else
            {
                da.From = fe.Opacity;
                da.To = 1.0;
            }

            fe.BeginAnimation(UIElement.OpacityProperty, da);
            return Visibility.Visible;
        }

        private static bool CheckAndUpdateAnimationStartedFlag(FrameworkElement fe)
        {
            var hookedElement = _hookedElements.Contains(fe);
            if (!hookedElement)
            {
                return true; // don't need to animate unhooked elements.
            }

            var animationStarted = (bool)_hookedElements[fe];
            _hookedElements[fe] = !animationStarted;

            return animationStarted;
        }
    }
}
