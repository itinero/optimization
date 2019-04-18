using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.Solvers.Tours.Sequences
{
    internal class ReadOnlyTourWithoutSequence : IReadOnlyTourWithoutSequence
    {
        private readonly IReadOnlyTour _tour;
        private readonly Sequence _remove;
        private readonly int _firstIndex;

        public ReadOnlyTourWithoutSequence(IReadOnlyTour tour, Sequence remove)
        {
            _tour = tour;
            _remove = remove;
            _firstIndex = _remove.IndexOf(tour.First);
        }

        public bool Wraps => _first != -1;

        private int? _first;

        public int First
        {
            get
            {
                if (_firstIndex == -1)
                {
                    return _tour.First;
                }

                if (_first.HasValue)
                {
                    return _first.Value;
                }

                using (var enumerator = this.GetEnumerator())
                {
                    enumerator.MoveNext();
                    _first = enumerator.Current;
                    return enumerator.Current;
                }
            }
        }

        private int? _last;

        public int? Last
        {
            get
            {
                if (!_tour.Last.HasValue)
                {
                    return null;
                }

                if (_tour.IsClosed())
                {
                    return this.First;
                }

                if (_last.HasValue)
                {
                    return _last.Value;
                }

                var last = -1;
                using (var enumerator = this.GetEnumerator())
                    while (enumerator.MoveNext())
                    {
                        last = enumerator.Current;
                    }

                _last = last;
                return last;
            }
        }

        public int Count => _tour.Count - _remove.Length;

        /// <summary>
        /// Returns a description of this tour.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var previous = -1;
            var result = new StringBuilder();
            var c = 0;
            foreach (var visit in this)
            {
                c++;
                if (previous < 0)
                {
                    result.Append('[');
                    result.Append(visit);
                    result.Append(']');
                }
                else if (visit != this.Last)
                {
                    result.Append("->");
                    result.Append(visit);
                }

                previous = visit;
            }

            if (!this.Last.HasValue || this.Count <= 1) return result.ToString();
            result.Append("->[");
            result.Append(this.Last.Value);
            result.Append("]");
            return result.ToString();
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<int>
        {
            private readonly ReadOnlyTourWithoutSequence _parent;
            private readonly IEnumerator<int> _enumerator;
            private readonly int _offset;

            public Enumerator(ReadOnlyTourWithoutSequence parent)
            {
                _parent = parent;
                _enumerator = _parent._tour.GetEnumerator();

                _offset = 0;
                if (_parent._firstIndex >= 0)
                {
                    _offset = _parent._remove.Length - _parent._firstIndex;
                }
            }

            private bool _first = true;

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext())
                {
                    return false;
                }

                if (_first && _offset > 0)
                {
                    for (var i = 0; i < _offset - 1; i++)
                    {
                        if (!_enumerator.MoveNext())
                        {
                            throw new Exception(
                                "Tour contains less visit than expected, most likely the remove sequence is not fully part of the tour.");
                        }
                    }

                    _first = false;

                    if (!_enumerator.MoveNext()) return false;
                    _parent._first = _enumerator.Current;

                    return true;
                }

                if (_parent._remove.Length > 0 &&
                    _parent._remove[0] == _enumerator.Current)
                {
                    // this is the start of the remove sequence, skip it.
                    for (var i = 0; i < _parent._remove.Length; i++)
                    {
                        if (!_enumerator.MoveNext()) return false;
                    }
                }

                return true;
            }

            public void Reset()
            {
                _first = true;
                _enumerator.Reset();
            }

            public int Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }
        }
    }
}