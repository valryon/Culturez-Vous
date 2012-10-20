using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using CulturezVous.WP7.Utils;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace CulturezVous.WP7.Controls
{
    /// <summary>
    /// Enhanced listbox with scrolling events
    /// </summary>
    public class ScrollingListControl : ListBox
    {
        /// <summary>
        /// The list is scrolling
        /// </summary>
        public event Action<ListBox, double> ListScrolled;

        /// <summary>
        /// The user has reached the end of the list
        /// </summary>
        public event Action ListEndScroll;

        private ScrollBar m_scrollbar;
        private double m_lastScrollbarValue;

        public ScrollingListControl()
            : base()
        {
            Loaded += new RoutedEventHandler(ElementListControl_Loaded);
        }

        void ElementListControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_scrollbar != null)
                m_scrollbar.ValueChanged -= scrollBarValueChanged;

            m_scrollbar = VisualChildFinder.FindVisualChild<ScrollBar>(this);

            if (m_scrollbar != null)
                m_scrollbar.ValueChanged += scrollBarValueChanged;
        }

        void scrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ListScrolled != null)
                ListScrolled(this, m_scrollbar.Value);

            // Load data when reaching the last 15% of the scroll
            double loadLimit = m_scrollbar.Maximum - (m_scrollbar.Maximum * 15) / 100;

            if (e.NewValue >= loadLimit && e.NewValue != m_lastScrollbarValue)
            {
                if (ListEndScroll != null)
                    ListEndScroll();
            }

            m_lastScrollbarValue = e.NewValue;
        }

    }
}
