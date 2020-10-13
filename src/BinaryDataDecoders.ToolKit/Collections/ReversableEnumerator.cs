﻿using System.Collections;
using System.Collections.Generic;

namespace BinaryDataDecoders.ToolKit.Collections
{
    /// <summary>
    /// this is a enumerator is bidirectional
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReversableEnumerator<T> : IReversibleEnumerator<T>
    {
        private readonly IEnumerator<T> _base;
        private IDoubleLinkedList<T>? _pointer = null;
        private bool _reset = false;
        private bool _end = false;

        /// <summary>
        /// Wrap existing IEnumerable
        /// </summary>
        /// <param name="base"></param>
        public ReversableEnumerator(IEnumerable<T> @base) : this(@base.GetEnumerator()) { }
        /// <summary>
        /// Wrap existing IEnumerator
        /// </summary>
        /// <param name="base"></param>
        public ReversableEnumerator(IEnumerator<T> @base) => _base = @base;

        public T Current => _pointer == null ? _base.Current : _pointer.Current;

#pragma warning disable CS8603 // Possible null reference return.
        object IEnumerator.Current => Current;
#pragma warning restore CS8603 // Possible null reference return.

        /// <summary>
        /// free any underlying resources
        /// </summary>
        public void Dispose() => _base.Dispose();

        /// <summary>
        /// allow playing to end of current state before checking for new values in enumerable set.
        /// </summary>
        /// <returns>true if advanced</returns>
        public bool MoveNext()
        {
            if (_end)
            {
                return false;
            }
            if (_reset && _pointer != null)
            {
                _reset = false;
                return true;
            }

            if (_pointer == null)
            {
                var advanceBase = _base.MoveNext();
                if (advanceBase)
                {
                    _pointer = new DoubleLinkedList<T>(_base.Current);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var next = _pointer.Next;
                if (next != null)
                {
                    _pointer = next;
                    return true;
                }
                else
                {
                    var advanceBase = _base.MoveNext();
                    if (advanceBase)
                    {
                        _pointer = _pointer.InsertAfter(_base.Current);
                        return true;
                    }
                    else
                    {
                        _end = true;
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// if the enumerator has been advanced it may be stepped back here.
        /// </summary>
        /// <returns>true if stepped back</returns>
        public bool MovePrevious()
        {
            var moveTo = _pointer?.Previous;
            if (moveTo == null) return false;
            _pointer = moveTo;
            return true;
        }

        /// <summary>
        /// if the rewind to the beginning.
        /// </summary>
        public void Reset()
        {
            if (_pointer != null)
            {
                _pointer = _pointer?.Rewind();
                _reset = true;
                _end = false;
            }
        }
    }
}