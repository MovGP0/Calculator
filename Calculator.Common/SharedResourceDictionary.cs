using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Calculator.Common
{
    public sealed class SharedResourceDictionary : ResourceDictionary
    {
        private static readonly Dictionary<Uri, ResourceDictionary> SharedDictionaries = new Dictionary<Uri, ResourceDictionary>();
        private static bool IsInDesignMode => (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(DependencyObject)).Metadata.DefaultValue;
        private Uri SourceUri { get; set; }

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) to load resources from.
        /// </summary>
        public new Uri Source
        {
            get
            {
                return IsInDesignMode ? base.Source : SourceUri;
            }
            set
            {
                if (IsInDesignMode)
                {
                    UpdateSourceInDesignMode(value);
                    return;
                }

                UpdateSource(value);
            }
        }

        private void UpdateSource(Uri value)
        {
            SourceUri = new Uri(value.OriginalString);

            lock (((ICollection) SharedDictionaries).SyncRoot)
            {
                if (!SharedDictionaries.ContainsKey(value))
                {
                    base.Source = value;
                    SharedDictionaries.Add(value, this);
                }
                else
                {
                    MergedDictionaries.Add(SharedDictionaries[value]);
                }
            }
        }

        private void UpdateSourceInDesignMode(Uri value)
        {
            try
            {
                base.Source = value;
            }
            catch
            {
                // ignored
            }
        }
    }
}