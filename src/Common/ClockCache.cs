// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Common
{
    /// <summary>
    /// An implementation of the Clock Page Replacement algorithm
    /// </summary>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <remarks>
    /// <para>
    /// Clock is a more efficient version of FIFO than Second-chance because pages
    /// don't have to be constantly pushed to the back of the list, but it performs
    /// the same general function as Second-Chance. The clock algorithm keeps a
    /// circular list of pages in memory, with the "hand" (iterator) pointing to the
    /// last examined page frame in the list. When a page fault occurs and no empty
    /// frames exist, then the R (referenced) bit is inspected at the hand's location.
    /// If R is 0, the new page is put in place of the page the "hand" points to,
    /// otherwise the R bit is cleared. Then, the clock hand is incremented and the
    /// process is repeated until a page is replaced.
    /// </para>
    /// <para>
    /// Andrew S. Tanenbaum. Modern Operating Systems (Second Edition). pp. 218 (4.4.5). 2001.
    /// </para>
    /// </remarks>
    public class ClockCache<TKey, TValue> : IDisposable
    {
        private readonly Dictionary<TKey, MemoryCacheEntry<TKey, TValue>> m_dictionary;
        private readonly int m_maxSize;
        private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();
        private MemoryCacheEntry<TKey, TValue> m_beforeClockHand;
        private int m_currentSize;
        private bool m_disposed;

        public ClockCache(int maxSize)
            : this(null, maxSize)
        {
        }

        public ClockCache(IEqualityComparer<TKey> comparer, int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException("maxSize");
            }

            m_maxSize = maxSize;
            m_currentSize = 0;
            m_dictionary = new Dictionary<TKey, MemoryCacheEntry<TKey, TValue>>(comparer);
        }

        public int Size
        {
            get
            {
                while (!m_readerWriterLock.TryEnterReadLock(0))
                {
                }

                try
                {
                    return m_currentSize;
                }
                finally
                {
                    m_readerWriterLock.ExitReadLock();
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                if (!TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException();
                }

                return value;
            }

            set
            {
                AddHelper(key, value, true);
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="value">Value to associate with key</param>
        public void Add(TKey key, TValue value)
        {
            if (!AddHelper(key, value, false))
            {
                // Key was already in the cache.
                throw new ArgumentException("Specified key already present.", "key");
            }
        }

        /// <summary>
        /// Clears the ClockCache of all entries.
        /// </summary>
        public void Clear()
        {
            m_readerWriterLock.EnterWriteLock();
            try
            {
                m_currentSize = 0;
                m_beforeClockHand = null;
                m_dictionary.Clear();
            }
            finally
            {
                m_readerWriterLock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool KeyExists(TKey key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes a specific key from the cache.
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True if the key was removed from the cache</returns>
        public bool Remove(TKey key)
        {
            while (!m_readerWriterLock.TryEnterReadLock(0))
            {
            }

            try
            {
                MemoryCacheEntry<TKey, TValue> entry;

                if (!m_dictionary.TryGetValue(key, out entry))
                {
                    return false;
                }

                // Unlink the entry.
                entry.Previous.Next = entry.Next;
                entry.Next.Previous = entry.Previous;
                m_dictionary.Remove(entry.Key);
                m_currentSize--;

                if (ReferenceEquals(entry, m_beforeClockHand))
                {
                    // We removed what beforeClockHand pointed to,
                    // so advance it to an extant entry.
                    m_beforeClockHand = m_beforeClockHand.Next;

                    if (m_currentSize == 0)
                    {
                        // Cache is now empty.
                        m_beforeClockHand = null;
                    }
                }

                return true;
            }
            finally
            {
                m_readerWriterLock.ExitReadLock();
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return AddHelper(key, value, false);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Key to look up</param>
        /// <param name="value">[out] Value associated with key, if found</param>
        /// <returns>True if a value was found associated with key</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            while (!m_readerWriterLock.TryEnterReadLock(0))
            {
            }

            try
            {
                MemoryCacheEntry<TKey, TValue> entry;
                if (!m_dictionary.TryGetValue(key, out entry))
                {
                    value = default(TValue);
                    return false;
                }

                entry.Referenced = true;
                value = entry.Value;
                return true;
            }
            finally
            {
                m_readerWriterLock.ExitReadLock();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_readerWriterLock.Dispose();

                    foreach (var value in m_dictionary.Values)
                    {
                        if (value != null && value.Value != null && value.Value is IDisposable)
                        {
                            ((IDisposable)value.Value).Dispose();
                        }
                    }
                }
            }

            m_disposed = true;
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">Key to add</param>
        /// <param name="value">Value to associate with key</param>
        /// <param name="overwrite">If true, will overwrite an existing key</param>
        /// <returns>True if the key was successfully added; false if the key was already in the dictionary</returns>
        private bool AddHelper(TKey key, TValue value, bool overwrite)
        {
            m_readerWriterLock.EnterWriteLock();
            try
            {
                MemoryCacheEntry<TKey, TValue> extantEntry;

                if (m_dictionary.TryGetValue(key, out extantEntry))
                {
                    if (!overwrite)
                    {
                        return false;
                    }

                    // We already have an entry for this key. Update the entry
                    // with the new value and exit.
                    extantEntry.Value = value;
                }
                else if (m_currentSize < m_maxSize)
                {
                    // The cache is still growing -- we do not need an eviction to add this entry.
                    MemoryCacheEntry<TKey, TValue> newEntry = new MemoryCacheEntry<TKey, TValue>(key, value);
                    m_dictionary[key] = newEntry;

                    if (null == m_beforeClockHand)
                    {
                        // currentSize transitioning from 0 to 1.
                        newEntry.Next = newEntry;
                        newEntry.Previous = newEntry;
                    }
                    else
                    {
                        // currentSize transitioning from >= 1 to maxSize.
                        newEntry.Next = m_beforeClockHand.Next;
                        newEntry.Previous = m_beforeClockHand;
                        m_beforeClockHand.Next.Previous = newEntry;
                        m_beforeClockHand.Next = newEntry;
                    }

                    m_beforeClockHand = newEntry;
                    m_currentSize++;

                    Debug.Assert(m_currentSize == m_dictionary.Count);
                }
                else
                {
                    // The cache is full and we need to evict in order to add this entry.
                    // beforeClockHand.Next is the clock hand in the clock replacement algorithm.
                    MemoryCacheEntry<TKey, TValue> clockHand = m_beforeClockHand.Next;

                    while (clockHand.Referenced)
                    {
                        clockHand.Referenced = false;
                        clockHand = clockHand.Next;
                    }

                    // clockHand now points to the entry we will evict. We will re-use
                    // the existing cache entry.
                    m_dictionary.Remove(clockHand.Key);
                    clockHand.Key = key;
                    clockHand.Value = value;
                    clockHand.Referenced = false;
                    m_dictionary[key] = clockHand;

                    // Save the new clock hand position.
                    m_beforeClockHand = clockHand;
                }
            }
            finally
            {
                m_readerWriterLock.ExitWriteLock();
            }

            return true;
        }

        private class MemoryCacheEntry<UKey, UValue>
        {
            public MemoryCacheEntry(UKey key, UValue value)
            {
                Key = key;
                Value = value;
                Previous = null;
                Next = null;
                Referenced = false;
            }

            public UKey Key { get; set; }

            public MemoryCacheEntry<UKey, UValue> Next { get; set; }

            public MemoryCacheEntry<UKey, UValue> Previous { get; set; }

            public bool Referenced { get; set; }

            public UValue Value { get; set; }
        }
    }
}