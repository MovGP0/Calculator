﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Serilog;

namespace Calculator.Controls.Variables
{
    public partial class Unit
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Unit), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty BaselineOffsetProperty = DependencyProperty.Register(
            nameof(BaselineOffset), typeof(double), typeof(Unit), new PropertyMetadata(default(double)));

        public double BaselineOffset
        {
            get { return (double) GetValue(BaselineOffsetProperty); }
            set { SetValue(BaselineOffsetProperty, value); }
        }

        static Unit()
        {
            FontSizeProperty.OverrideMetadata(typeof(Unit), 
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits, OnMeasureInvalidated));
        }

        private static void OnMeasureInvalidated(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            InvalidateParents(sender);
        }

        private static void InvalidateParents(DependencyObject sender)
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(sender) as UIElement;
                if (parent == null) return;
                parent.InvalidateMeasure();
                sender = parent;
            }
        }

        public Unit()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw;
            }
        }

        private double _width;
        private double _height;

        protected override Size MeasureOverride(Size constraint)
        {
            var tb = GetTextblock();

            tb.Measure(constraint);
            BaselineOffset = tb.BaselineOffset;

            _width = tb.DesiredSize.Width;
            _height = tb.DesiredSize.Height;
            
            return new Size(_width, _height);
        }

        private TextBlock GetTextblock()
        {
            return new TextBlock
            {
                Text = Text,
                Margin = Margin, 
                FontSize = FontSize,
                Padding = Padding,
                FontFamily = FontFamily, 
                FontStretch = FontStretch,
                FontStyle = FontStyles.Italic, 
                FontWeight = FontWeights.Normal
            };
        }
        
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var tb = GetTextblock();

            tb.Measure(arrangeBounds);
            BaselineOffset = tb.BaselineOffset;

            base.ArrangeOverride(arrangeBounds);
            
            return new Size(_width, _height);
        }
    }
}
