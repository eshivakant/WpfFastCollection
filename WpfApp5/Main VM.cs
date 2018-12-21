using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public class Main_VM: ViewModelBase
    {
        public ObservableKeyedCollection<string, GlobalQuote> Quotes { get; set; } = new ObservableKeyedCollection<string, GlobalQuote>(q => q.The01Symbol, TimeSpan.FromMilliseconds(500));
        //public ObservableKeyedCollection<string, GlobalQuote> Quotes { get; set; } = new ObservableKeyedCollection<string, GlobalQuote>(q => q.The01Symbol);

        public Main_VM()
        {

             var producer = new Producer();
            producer.StartProducing("MSFT", 100);
            producer.StartProducing("GOOGL", 150);
            producer.StartProducing("REEB", 1001);
            producer.StartProducing("AAPL", 1070);
            producer.StartProducing("MSFT1", 100);
            producer.StartProducing("GOOGL1", 150);
            producer.StartProducing("REEB1", 1001);
            producer.StartProducing("AAPL1", 1070);
            producer.StartProducing("MSFT2", 100);
            producer.StartProducing("GOOGL2", 150);
            producer.StartProducing("REEB2", 1001);
            producer.StartProducing("AAPL2", 1070);
            producer.StartProducing("MSFT3", 100);
            producer.StartProducing("GOOGL3", 150);
            producer.StartProducing("REEB3", 1001);
            producer.StartProducing("AAPL3", 1070);
            producer.StartProducing("MSFT4", 100);
            producer.StartProducing("GOOGL4", 150);
            producer.StartProducing("REEB4", 1001);
            producer.StartProducing("AAPL4", 1070);
            producer.StartProducing("MSFT5", 100);
            producer.StartProducing("GOOGL5", 150);
            producer.StartProducing("REEB5", 1001);
            producer.StartProducing("AAPL5", 1070);
            producer.PriceStream.SubscribeOn(NewThreadScheduler.Default)
                .ObserveOn(DispatcherScheduler.Current).Subscribe(q=>
            {
                    Quotes.AddOrUpdate(q.GlobalQuote);
            });
        }

        private void Update(GlobalQuote globalQuote, GlobalQuote found)
        {
            found.The05Price = globalQuote.The05Price;
            found.The02Open = globalQuote.The02Open;
            found.The08PreviousClose = globalQuote.The08PreviousClose;
            found.The03High = globalQuote.The03High;
            found.The04Low = globalQuote.The04Low;

        }
    }

    //public class StateService
    //{
    //    public void Start()
    //    {
    //        var producer = new Producer();
    //        producer.StartProducing("MSFT", 100);
    //        producer.StartProducing("GOOGL", 150);
    //        producer.StartProducing("REEB", 1001);
    //        producer.StartProducing("AAPL", 1070);
    //        producer.PriceStream.ObserveOnDispatcher().Subscribe(q =>
    //        {+6
    //            var found = Quotes.FirstOrDefault(qq => qq.The01Symbol == q.GlobalQuote.The01Symbol);
    //            if (found != null)
    //                Update(q.GlobalQuote, found);
    //            else
    //                Quotes.Add(q.GlobalQuote);
    //        });
    //    }
    //}


}
