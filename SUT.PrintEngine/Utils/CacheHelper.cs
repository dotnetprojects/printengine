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
using System.Collections.Specialized;
using System.Runtime.Caching;

#endregion

namespace SUT.PrintEngine.Utils
{
    /// <summary>
    /// This is a wrapper of Microsoft Patterns & Practices CachingManager class.
    /// Currently it has been configured only to cache in memory.
    /// </summary>
    public sealed class CacheHelper
    {
        #region Member Variables

        private MemoryCache _cacheManager;

        #endregion

        #region Constructor/ Singleton implementation

        private static volatile CacheHelper instance;
        private static object syncRoot = new Object();

        private CacheHelper()
        {
            _cacheManager = new MemoryCache("SUT.PrintEngine");                
        }

        public static CacheHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CacheHelper();
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Properties
        #endregion

        #region CacheHelper Members

        public void Add(string key, object data)
        {            
            _cacheManager.Add(key, data, DateTimeOffset.MaxValue);
        }

        public bool Contains(string key)
        {
            return _cacheManager.Contains(key);
        }

        public void Flush()
        {
            _cacheManager.Dispose();
        }

        public object GetData(string key)
        {
            return _cacheManager.Get(key);
        }

        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        public long Count
        {
            get 
            {                
                return _cacheManager.GetCount(); 
            }
        }

        public object this[string key]
        {
            get 
            {               
                return _cacheManager[key]; 
            }
            set
            {
                _cacheManager[key] = value;
            }
        }

        #endregion
    }
}

// end of namespace
