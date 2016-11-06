using System.Collections.Generic;
using System.Windows;
using Calculator.GestureRecognizer;

namespace Calculator.Pages
{
    public sealed class PathSample : DependencyObject
    {
        public static readonly DependencyProperty CharacterProperty = DependencyProperty.Register(nameof(Character), typeof(string), typeof(PathSample), new PropertyMetadata(default(string)));

        public string Character
        {
            get { return (string) GetValue(CharacterProperty); }
            set { SetValue(CharacterProperty, value); }
        }

        public static readonly DependencyProperty Sample1Property = DependencyProperty.Register(nameof(Sample1), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer.GestureRecognizer), new PropertyMetadata(default(IEnumerable<Stroke>)));

        public IEnumerable<Stroke> Sample1
        {
            get { return (IEnumerable<Stroke>) GetValue(Sample1Property); }
            set { SetValue(Sample1Property, value); }
        }

        public static readonly DependencyProperty Sample2Property = DependencyProperty.Register(nameof(Sample2), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer.GestureRecognizer), new PropertyMetadata(default(IEnumerable<Stroke>)));

        public IEnumerable<Stroke> Sample2
        {
            get { return (IEnumerable<Stroke>) GetValue(Sample2Property); }
            set { SetValue(Sample2Property, value); }
        }
        
        public static readonly DependencyProperty Sample3Property = DependencyProperty.Register(nameof(Sample3), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer.GestureRecognizer), new PropertyMetadata(default(IEnumerable<Stroke>)));

        public IEnumerable<Stroke> Sample3
        {
            get { return (IEnumerable<Stroke>) GetValue(Sample3Property); }
            set { SetValue(Sample3Property, value); }
        }
        
        public static readonly DependencyProperty Sample4Property = DependencyProperty.Register(nameof(Sample4), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer.GestureRecognizer), new PropertyMetadata(default(IEnumerable<Stroke>)));

        public IEnumerable<Stroke> Sample4
        {
            get { return (IEnumerable<Stroke>) GetValue(Sample4Property); }
            set { SetValue(Sample4Property, value); }
        }
        
        public static readonly DependencyProperty Sample5Property = DependencyProperty.Register(nameof(Sample5), typeof(IEnumerable<Stroke>), typeof(GestureRecognizer.GestureRecognizer), new PropertyMetadata(default(IEnumerable<Stroke>)));
        
        public IEnumerable<Stroke> Sample5
        {
            get { return (IEnumerable<Stroke>) GetValue(Sample5Property); }
            set { SetValue(Sample5Property, value); }
        }
        
    }
}
