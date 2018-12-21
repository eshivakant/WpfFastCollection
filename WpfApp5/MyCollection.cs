using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WpfApp5
{

    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        private Func<TItem, TKey> _getKeyForItemDelegate;

        private Subject<TItem> _inStream = new Subject<TItem>();
        private ConcurrentDictionary<TKey, TItem> _staging = new ConcurrentDictionary<TKey, TItem>();
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(10);


        public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate, TimeSpan updateInterval) : base()
        {
            this._updateInterval = updateInterval;
            Initialize(getKeyForItemDelegate);
        }

        public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate) : base()
        {
            Initialize(getKeyForItemDelegate);
        }

        public void Initialize(Func<TItem, TKey> getKeyForItemDelegate) 
        {
            if (getKeyForItemDelegate == null)
                throw new ArgumentNullException("Delegate passed can't be null!");

            _getKeyForItemDelegate = getKeyForItemDelegate;

            var managedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            var count = 0;
            _inStream.Select(i =>
            {
                count++;
                _staging[this.GetKeyForItem(i)] = i;
                return true;
            })
            .Buffer(_updateInterval)
            .SubscribeOn(NewThreadScheduler.Default)
            .ObserveOnDispatcher()
            .Subscribe(async i =>
            {
                Console.WriteLine(count);

                var copy = _staging.ToArray();
                _staging.Clear();

                foreach (var item in copy)
                {
                    var key = item.Key;
                    var index = this.IndexOf(item.Value);
                    if (index > -1)
                    {
                        await Task.Delay(100);

                        this.Remove(key);
                        this.InsertItem(index, item.Value);
                    }
                    else
                        Add(item.Value);

                }             

               
            });
           

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
            _inStream.OnNext(item);           
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
