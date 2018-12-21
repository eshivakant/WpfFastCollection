using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp5
{

    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        private Func<TItem, TKey> _getKeyForItemDelegate;

        // Constructor now requires a delegate to get the key from the item
        public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate) : base()
        {
            if (getKeyForItemDelegate == null)
                throw new ArgumentNullException("Delegate passed can't be null!");

            _getKeyForItemDelegate = getKeyForItemDelegate;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return _getKeyForItemDelegate(item);
        }

        // Overrides a lot of methods that can cause collection change
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        private bool _deferNotifyCollectionChanged = false;
        public void AddRange(IEnumerable<TItem> items)
        {
            _deferNotifyCollectionChanged = true;
            foreach (var item in items)
                Add(item);
            _deferNotifyCollectionChanged = false;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddOrUpdate(TItem item)
        {
            var key = this.GetKeyForItem(item);
            var index = this.IndexOf(item);
            if (index > -1)
            {

                this.Remove(key);

                this.InsertItem(index, item);

            }
            else
                Add(item);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_deferNotifyCollectionChanged)
                return;

            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }

}
