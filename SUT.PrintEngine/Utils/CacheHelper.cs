/**
 * File name: CacheHelper.cs 
 * Author: Mosfiqur.Rahman
 * Date: 9/6/2009 4:53:09 PM format: MM/dd/yyyy
 * 
 * 
 * Modification history:
 * Name				Date					Desc
 * 
 *  
 * Version: 1.0
 * Copyright (c) 2008: Orbitax LLC.
 * */

#region Using Directives

using System;
using System.Collections.Generic;
using System.Runtime.Caching;

#endregion

namespace SUT.PrintEngine.Utils
{
    /// <summary>
    /// This is a wrapper of Microsoft Patterns & Practices CachingManager class.
    /// Currently it has been configured only to cache in memory.
    /// </summary>
    public class CacheHelper
    {
        #region Member Variables

        private MemoryCache _cacheManager;

        #endregion

        #region Constructors

        #endregion

        #region Properties
        #endregion

        #region Methods

        private void InitializeCacheManager()
        {
            if (null == _cacheManager)
            {
                _cacheManager = new MemoryCache("SUT.PrintEngine");
            }
        }

        #endregion


        #region CacheHelper Members

        public void Add(string key, object data)
        {
            InitializeCacheManager();
            _cacheManager.Add(key, data, DateTimeOffset.MaxValue);
        }

        public bool Contains(string key)
        {
            InitializeCacheManager();
            return _cacheManager.Contains(key);
        }

        public void Flush()
        {
            InitializeCacheManager();
            _cacheManager.Dispose();
        }

        public object GetData(string key)
        {
            InitializeCacheManager();
            return _cacheManager.Get(key);
        }

        public void Remove(string key)
        {
            InitializeCacheManager();
            _cacheManager.Remove(key);
        }

        public long Count
        {
            get 
            {
                InitializeCacheManager();
                return _cacheManager.GetCount(); 
            }
        }

        public object this[string key]
        {
            get 
            {
                InitializeCacheManager();
                return _cacheManager[key]; 
            }
            set
            {
                InitializeCacheManager();
                _cacheManager[key] = value;
            }
        }

        #endregion
    }
}

// end of namespace
