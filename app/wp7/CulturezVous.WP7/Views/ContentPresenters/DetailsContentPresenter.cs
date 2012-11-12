﻿using System.Windows;
using System.Windows.Controls;
using CulturezVous.WP7.ViewModels;

namespace CulturezVous.WP7.Views.ContentPresenters
{
    /// <summary>
    /// DataTemplate selector for elements
    /// </summary>
    /// <remarks>Thanks to http://www.scottlogic.co.uk/blog/colin/2011/07/a-wp7-conversation-view/ </remarks>
    public class ElementsContentPresenter : ContentControl
    {
        /// <summary>
        /// The DataTemplate to use for Words
        /// </summary>
        public DataTemplate WordTemplate { get; set; }

        /// <summary>
        /// The DataTemplate to use for Contrepetries
        /// </summary>
        public DataTemplate ContrepetrieTemplate { get; set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            // apply the required template
            ElementViewModel element = newContent as ElementViewModel;
            if (element is WordViewModel)
            {
                ContentTemplate = WordTemplate;
            }
            else if (element is ContrepeterieViewModel)
            {
                ContentTemplate = ContrepetrieTemplate;
            }
        }
    }
}
