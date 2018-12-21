//using System;
//using System.Collections.Generic;
//#if !SILVERLIGHT
//#endif
//#if SILVERLIGHT
//using System.Core; // for System.Core.SR
//#endif

//namespace WpfApp5
//{
//    public class MyDictionary<T> : Dictionary<int,T>
//    {

//        public MyDictionary()
//        {
            
//        }

//        public MyDictionary(List<T> list)
//        {
//            var count = 0;
//            foreach (var item in list)
//            {
//                base.Add(count++,item);
//            }
//        }

//        public int AddOrUpdate(T item)
//        {
//            var hashCode = item.GetHashCode();
//            var exist = this.ContainsKey(hashCode);

//            var result = exist ? Array.IndexOf() : default(T);

//            this[item.GetHashCode()] = item;

//            return result;
//        }

//    }


//    public class MyHashset<T> : HashSet<T>
//    {
//        public MyHashset()
//        {

//        }

//        public MyHashset(List<T> list)
//        {
//            foreach (var item in list)
//            {
//                base.Add(item);
//            }
//        }

//        public new void Add(T item)
//        {
//                this.Remove(item);
//                base.Add(item);
//        }

//    }
//}
