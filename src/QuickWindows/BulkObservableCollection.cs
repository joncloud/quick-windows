using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace QuickWindows
{
    /// <remarks>https://peteohanlon.wordpress.com/2008/10/22/bulk-loading-in-observablecollection/</remarks>
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;
        readonly IDisposable _resetDisposable;
        public BulkObservableCollection()
        {
            _resetDisposable = new ResetDisposable(Reset);
        }
        public BulkObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            _resetDisposable = new ResetDisposable(Reset);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public IDisposable EnableBulkOperations()
        {
            _suppressNotification = true;
            return _resetDisposable;
        }

        void Reset()
        {
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            using var _ = EnableBulkOperations();

            foreach (T item in list)
            {
                Add(item);
            }
        }

        class ResetDisposable : IDisposable
        {
            readonly Action _fn;
            public ResetDisposable(Action fn)
            {
                _fn = fn;
            }

            public void Dispose() => _fn();
        }
    }
}
