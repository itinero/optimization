namespace Itinero.Optimization.Solvers.Tours
{
    /// <summary>
    /// A quadruplet of visits.
    /// </summary>
    public readonly struct Quad
    {        
        /// <summary>
        /// Creates a new quadruplet.
        ///
        /// before->from->to->after
        /// </summary>
        /// <param name="visit1">The first visit.</param>
        /// <param name="visit2">The second visit.</param>
        /// <param name="visit3">The third visit.</param>
        /// <param name="visit4"></param>
        public Quad(int visit1, int visit2, int visit3, int visit4)
            : this()
        {
            this.Visit1 = visit1;
            this.Visit2 = visit2;
            this.Visit3 = visit3;
            this.Visit4 = visit4;
        }

        /// <summary>
        /// The first visit.
        /// </summary>
        public int Visit1 { get; }

        /// <summary>
        /// The second visit.
        /// </summary>
        public int Visit2 { get; }

        /// <summary>
        /// The third visit.
        /// </summary>
        public int Visit3 { get; }

        /// <summary>
        /// The fourth visit.
        /// </summary>
        public int Visit4 { get; }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + this.Visit1.GetHashCode();
                hash = hash * 31 + this.Visit2.GetHashCode();
                hash = hash * 31 + this.Visit3.GetHashCode();
                hash = hash * 31 + this.Visit4.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Quad quad)
            {
                return quad.Visit1 == this.Visit1 &&
                       quad.Visit2 == this.Visit2 &&
                       quad.Visit3 == this.Visit3 &&
                       quad.Visit4 == this.Visit4;
            }
            return false;
        }

        /// <summary>
        /// Returns a description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Visit1} -> {this.Visit2} -> {this.Visit3} -> {this.Visit4}";
        }
    }
}