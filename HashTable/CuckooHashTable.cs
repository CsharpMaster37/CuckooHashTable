﻿using System;
using System.Collections;
using System.Collections.Generic;


namespace HashTablesLib
{
    public class CuckooHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IEqualityComparer<TKey> _comparer;
        private Pair<TKey, TValue>[] _items1, _items2;
        private int _capacity;
        private long p1,p2;
        private GetPrimeNumber primeNumber = new GetPrimeNumber();
        private double FillFactor = 0.5;

        private int Hash1(TKey key)
        {
            long hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            return (int)((2* hashCode+3)%p1)%_capacity;
        }

        private int Hash2(TKey key)
        {
            long hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            return (int)((hashCode + 1) % p2) % _capacity;
        }
        public CuckooHashTable()
        {
            Init();
        }
        private void Init()
        {
            _comparer = EqualityComparer<TKey>.Default;
            _capacity = primeNumber.GetMin();
            (p1, p2) = primeNumber.GetPrimes(_capacity);
            _items1 = new Pair<TKey, TValue>[_capacity];
            _items2 = new Pair<TKey, TValue>[_capacity];
            Count = 0;
        }

        public TValue this[TKey key] 
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException();
                int hash1 = Hash1(key);
                int hash2 = Hash2(key);
                if (_items1[hash1] != null && _comparer.Equals(_items1[hash1].Key, key) && !_items1[hash1].IsDeleted())
                    return _items1[hash1].Value;
                if (_items2[hash2] != null && _comparer.Equals(_items2[hash2].Key, key) && !_items2[hash2].IsDeleted())
                    return _items2[hash2].Value;
                throw new KeyNotFoundException($"The key '{key}' was not found in the hash table.");
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException();
                int hash1 = Hash1(key);
                int hash2 = Hash2(key);
                if (_items1[hash1] != null && _comparer.Equals(_items1[hash1].Key, key) && !_items1[hash1].IsDeleted())
                {
                    _items1[hash1] = new Pair<TKey, TValue>(key, value);
                    return;
                }
                if (_items2[hash2] != null && _comparer.Equals(_items2[hash2].Key, key) && !_items2[hash2].IsDeleted())
                {
                    _items2[hash2] = new Pair<TKey, TValue>(key, value);
                    return;
                }
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => throw new NotImplementedException();
        public ICollection<TValue> Values => throw new NotImplementedException();

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException();
            int i = Hash1(key);
            int j = Hash2(key);
            if ((_items1[i] != null && _comparer.Equals(_items1[i].Key, key) && !_items1[i].IsDeleted()) ||
                (_items2[j] != null && _comparer.Equals(_items2[j].Key, key) && !_items2[j].IsDeleted()))
            {
                throw new ArgumentException("Such key already exists");
            }
            TKey currentKey = key;
            TValue currentValue = value;
            int tryNumber = 1;
            bool NoCycle = false;
            while (!_comparer.Equals(currentKey, key) || tryNumber == 1)
            {
                int hash1 = Hash1(currentKey);
                int hash2 = Hash2(currentKey);
                if (_items1[hash1] == null || _items1[hash1].IsDeleted())
                {
                    _items1[hash1] = new Pair<TKey, TValue>(currentKey, currentValue);
                    Count++;
                    NoCycle = true;
                    break;
                }
                else
                {
                    var temp = _items1[hash1];
                    _items1[hash1] = new Pair<TKey, TValue>(currentKey, currentValue);
                    currentKey = temp.Key;
                    currentValue = temp.Value;
                }
                if (_items2[hash2] == null || _items2[hash2].IsDeleted())
                {
                    _items2[hash2] = new Pair<TKey, TValue>(currentKey, currentValue);
                    Count++;
                    NoCycle = true;
                    break;
                }
                else
                {
                    var temp = _items2[hash2];
                    _items2[hash2] = new Pair<TKey, TValue>(currentKey, currentValue);
                    currentKey = temp.Key;
                    currentValue = temp.Value;
                }
                tryNumber++;
            }
            if ((double)Count / (2.0*_capacity) >= FillFactor || !NoCycle)
            {
                IncreaseTable();
            }
            if(!NoCycle)
                Add (key, value);
        }

        private void IncreaseTable()
        {
            _capacity = primeNumber.Next();
            (p1, p2) = primeNumber.GetPrimes(_capacity);
            var temp1 = _items1;
            var temp2 = _items2;
            _items1 = new Pair<TKey, TValue>[_capacity];
            _items2 = new Pair<TKey, TValue>[_capacity];
            Count = 0;
            for (int  i = 0; i < temp1.Length; i++)
            {
                if (temp1[i] == null || !temp1[i].IsDeleted())
                    continue;
                Add(temp1[i].Key, temp1[i].Value);
            }
            for (int i = 0; i < temp2.Length; i++)
            {
                if (temp2[i] == null || !temp2[i].IsDeleted())
                    continue;
                Add(temp2[i].Key, temp2[i].Value);
            }

        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value); 
        }

        public void Clear()
        {
            Init();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();
            int i = Hash1(key);
            int j = Hash2(key);
            if ((_items1[i] != null && _comparer.Equals(_items1[i].Key, key) && !_items1[i].IsDeleted()) ||
                (_items2[j] != null && _comparer.Equals(_items2[j].Key, key) && !_items2[j].IsDeleted()))
            {
                return true;
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            int hash1 = Hash1(key);
            int hash2 = Hash2(key);
            if (_comparer.Equals(_items1[hash1].Key, key) && !_items1[hash1].IsDeleted())
            {
                Count--;
                return _items1[hash1].DeletePair();
            }
            else if (_comparer.Equals(_items2[hash2].Key, key) && !_items2[hash2].IsDeleted())
            {
                Count--;
                return _items1[hash2].DeletePair();
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
