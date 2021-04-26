using System.Collections.Generic;

namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// Enumerates all quadruples in a tour.
    /// </summary>
    internal class QuadEnumerable : IEnumerable<Quad>
    {
        private readonly IReadOnlyTour _tour;
        private readonly bool _wrapAround;
        private readonly bool _includePartials;

        internal QuadEnumerable(IReadOnlyTour tour, bool wrapAround, bool includePartials)
        {
            _tour = tour;
            _wrapAround = wrapAround;
            _includePartials = includePartials;
        }

        private class QuadEnumerator : IEnumerator<Quad>
        {
            private readonly bool _wrapAround;
            private readonly bool _includePartials;

            public QuadEnumerator(IEnumerator<int> enumerator, bool wrapAround, bool includePartials)
            {
                _current1 = Tour.NOT_SET;
                _current2 = Tour.NOT_SET;
                _current3 = Tour.NOT_SET;
                _current4 = Tour.NOT_SET;
                
                _enumerator = enumerator;
                _wrapAround = wrapAround;
                _includePartials = includePartials;

                if (_wrapAround) _includePartials = false;
            }
            
            private int _current1;
            private int _current2;
            private int _current3;
            private int _current4;
            private int _first1;
            private int _first2;
            private int _first3;
            private IEnumerator<int> _enumerator;

            public Quad Current => new (_current1, _current2, _current3, _current4);

            public void Dispose()
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            object System.Collections.IEnumerator.Current => this.Current;

            public bool MoveNext()
            {
                if (_includePartials)
                {
                    if (!_enumerator.MoveNext())
                    {
                        _current1 = _current2;
                        _current2 = _current3;
                        _current3 = _current4;
                        _current4 = Tour.NOT_SET;
                        
                        if (_current1 == Tour.NOT_SET) return false;
                        return true;
                    }
                    
                    _current1 = _current2;
                    _current2 = _current3;
                    _current3 = _current4;
                    _current4 = _enumerator.Current;
                    
                    return true;
                }
                
                // the case that we have to seed the first.
                if (_current1 == Tour.NOT_SET && 
                    _current2 == Tour.NOT_SET && 
                    _current3 == Tour.NOT_SET && 
                    _current4 == Tour.NOT_SET)
                {
                    if (!_enumerator.MoveNext()) return false;
                    _current1 = _enumerator.Current;
                    _first1 = _current1;

                    if (!_enumerator.MoveNext()) return false;
                    _current2 = _enumerator.Current;
                    _first2 = _current2;

                    if (!_enumerator.MoveNext()) return false;
                    _current3 = _enumerator.Current;
                    _first3 = _current3;

                    if (!_enumerator.MoveNext()) return false;
                    _current4 = _enumerator.Current;

                    return true;
                }
                
                // move visits and add next.
                if (_enumerator.MoveNext())
                {
                    _current1 = _current2;
                    _current2 = _current3;
                    _current3 = _current4;
                    _current4 = _enumerator.Current;

                    return true;
                }

                // do wrap around if set.
                if (!_wrapAround) return false;

                // first1 is not returned.
                if (_first1 != Tour.NOT_SET)
                {
                    _current1 = _current2;
                    _current2 = _current3;
                    _current3 = _current4;
                    _current4 = _first1;

                    _first1 = Tour.NOT_SET;
                    return true;
                }
                // first2 is not returned.
                if (_first2 != Tour.NOT_SET)
                {
                    _current1 = _current2;
                    _current2 = _current3;
                    _current3 = _current4;
                    _current4 = _first2;

                    _first2 = Tour.NOT_SET;
                    return true;
                }
                // first3 is not returned.
                if (_first3 != Tour.NOT_SET)
                {
                    _current1 = _current2;
                    _current2 = _current3;
                    _current3 = _current4;
                    _current4 = _first3;

                    _first3 = Tour.NOT_SET;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _enumerator.Reset();
                
                _current1 = Tour.NOT_SET;
                _current2 = Tour.NOT_SET;
                _current3 = Tour.NOT_SET;
                _current4 = Tour.NOT_SET;
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Quad> GetEnumerator()
        {
            if (_tour.IsClosed())
            {
                return new QuadEnumerator(_tour.GetEnumerator(), _wrapAround, _includePartials);
            }
            return new QuadEnumerator(_tour.GetEnumerator(), false, _includePartials);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}