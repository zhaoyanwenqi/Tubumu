﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Tubumu.Modules.Core.FastLambda
{
    /// <summary>
    /// HashedListCache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashedListCache<T> : IDisposable, IExpressionCache<T> where T : class
    {
        private Dictionary<int, SortedList<Expression, T>> m_storage = new Dictionary<int, SortedList<Expression, T>>();
        private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public T Get(Expression key, Func<Expression, T> creator)
        {
            SortedList<Expression, T> sortedList;
            T value;

            int hash = new Hasher().Hash(key);
            this.m_rwLock.EnterReadLock();
            try
            {
                if (this.m_storage.TryGetValue(hash, out sortedList) &&
                    sortedList.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            finally
            {
                this.m_rwLock.ExitReadLock();
            }

            this.m_rwLock.EnterWriteLock();
            try
            {
                if (!this.m_storage.TryGetValue(hash, out sortedList))
                {
                    sortedList = new SortedList<Expression, T>(new Comparer());
                    this.m_storage.Add(hash, sortedList);
                }

                if (!sortedList.TryGetValue(key, out value))
                {
                    value = creator(key);
                    sortedList.Add(key, value);
                }

                return value;
            }
            finally
            {
                this.m_rwLock.ExitWriteLock();
            }
        }

        private class Hasher : ExpressionHasher
        {
            protected override Expression VisitConstant(ConstantExpression c)
            {
                return c;
            }
        }

        internal class Comparer : ExpressionComparer
        {
            protected override int CompareConstant(ConstantExpression x, ConstantExpression y)
            {
                return 0;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    m_rwLock.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HashedListCache() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
