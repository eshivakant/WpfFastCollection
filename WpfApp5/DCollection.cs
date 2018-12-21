using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public class CollectionItem<T>
    {
        public T Item { get; set; }
        public bool IsDirty { get; set; }
    }

    public class CList<T>:HashSet<CollectionItem<T>>
    {
        

        public IEnumerable<T> GetDirtyItems()
        {
            return this.Where(i => i.IsDirty).Select(i=>i.Item);
        }
    }



}



