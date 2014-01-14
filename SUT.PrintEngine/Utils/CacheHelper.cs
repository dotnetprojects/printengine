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

using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

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

        private ICacheManager _cacheManager;

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
                var fileSource = new FileConfigurationSource("SUT.PrintEngine.App.config");
                var factory = new CacheManagerFactory(fileSource);
                _cacheManager = factory.CreateDefault();
            }
        }

        #endregion


        #region CacheHelper Members

        public void Add(string key, object data)
        {
            InitializeCacheManager();
            _cacheManager.Add(key, data);
        }

        public bool Contains(string key)
        {
            InitializeCacheManager();
            return _cacheManager.Contains(key);
        }

        public void Flush()
        {
            InitializeCacheManager();
            _cacheManager.Flush();
        }

        public object GetData(string key)
        {
            InitializeCacheManager();
            return _cacheManager.GetData(key);
        }

        public void Remove(string key)
        {
            InitializeCacheManager();
            _cacheManager.Remove(key);
        }

        public int Count
        {
            get 
            {
                InitializeCacheManager();
                return _cacheManager.Count; 
            }
        }

        public object this[string key]
        {
            get 
            {
                InitializeCacheManager();
                return _cacheManager[key]; 
            }
        }

        #endregion
    }
}

// end of namespace
