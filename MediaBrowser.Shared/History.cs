using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.DTO;

namespace MediaBrowser.Shared
{
    public class History
    {
        private static Dictionary<Type, List<object>> _historyItems;
        private static History _current;
        public static History Current
        {
            get { return _current ?? (_current = new History()); }
        }

        private History()
        {
            _historyItems = new Dictionary<Type, List<object>>();
        }

        public virtual void AddHistoryItem(Type viewModelType, object item)
        {
            if (_historyItems.ContainsKey(viewModelType))
            {
                _historyItems[viewModelType].Add(item);
            }
            else
            {
                _historyItems.Add(viewModelType, new List<object>
                                                     {
                                                         item
                                                     });
            }
        }

        public virtual T GetLastItem<T>(Type viewModelType, bool lastIsCurrent = false) where T : class
        {
            if (!_historyItems.ContainsKey(viewModelType))
            {
                return null;
            }

            var item = _historyItems[viewModelType].LastOrDefault();
            _historyItems[viewModelType].Remove(item);
            if (lastIsCurrent)
            {
                item = _historyItems[viewModelType].LastOrDefault();
            }
            return (T)item;
        }

        public virtual void RemoveItem(Type viewModelType, object item)
        {
            if (_historyItems.ContainsKey(viewModelType))
            {
                _historyItems[viewModelType].Remove(item);
            }
        }

        public virtual void ClearAllForType(Type viewModelType)
        {
            if(_historyItems.ContainsKey(viewModelType))
            {
                _historyItems[viewModelType].Clear();
            }
        }

        public virtual void ClearAll()
        {
            foreach(var type in _historyItems.Keys)
            {
                _historyItems[type].Clear();
            }
        }
    }
}
