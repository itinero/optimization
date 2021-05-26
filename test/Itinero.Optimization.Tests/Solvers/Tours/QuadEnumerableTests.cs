using System.Linq;
using Itinero.Optimization.Solvers.Tours;
using Xunit;

namespace Itinero.Optimization.Tests.Solvers.Tours
{
    public class QuadEnumerableTests
    {
        [Fact]
        public void Tour_Quadruplets_4TourOpen_OneQuadruplet()
        {
            var tour = new Tour(new [] {0, 1, 2, 3}, null);

            var s = tour.Quadruplets().ToList();
            Assert.NotNull(s);
            Assert.Single(s);
            Assert.Equal(new Quad(0,1,2,3), s[0]);
        }
        
        [Fact]
        public void Tour_Quadruplets_4TourClosed_NoWrap_OneQuadruplet()
        {
            var tour = new Tour(new [] {0, 1, 2, 3}, 0);

            var s = tour.Quadruplets(false).ToList();
            Assert.NotNull(s);
            Assert.Single(s);
            Assert.Equal(new Quad(0,1,2,3), s[0]);
        }
        
        [Fact]
        public void Tour_Quadruplets_4TourClosed_Wrap_4Quadruplet()
        {
            var tour = new Tour(new [] {0, 1, 2, 3}, 0);

            var s = tour.Quadruplets().ToList();
            Assert.NotNull(s);
            Assert.Equal(4,s.Count);
            Assert.Equal(new Quad(0,1,2,3), s[0]);
            Assert.Equal(new Quad(1,2,3,0), s[1]);
            Assert.Equal(new Quad(2,3,0,1), s[2]);
            Assert.Equal(new Quad(3,0,1,2), s[3]);
        }
        
        [Fact]
        public void Tour_Quadruplets_4TourOpen_IncludePartials_7Quadruplet()
        {
            var tour = new Tour(new [] {0, 1, 2, 3}, null);

            var s = tour.Quadruplets(includePartials: true).ToList();
            Assert.NotNull(s);
            Assert.Equal(7,s.Count);
            Assert.Equal(new Quad(Tour.NOT_SET,Tour.NOT_SET,Tour.NOT_SET,0), s[0]);
            Assert.Equal(new Quad(Tour.NOT_SET,Tour.NOT_SET,0,1), s[1]);
            Assert.Equal(new Quad(Tour.NOT_SET,0,1,2), s[2]);
            Assert.Equal(new Quad(0,1,2,3), s[3]);
            Assert.Equal(new Quad(1,2,3,Tour.NOT_SET), s[4]);
            Assert.Equal(new Quad(2,3,Tour.NOT_SET,Tour.NOT_SET), s[5]);
            Assert.Equal(new Quad(3,Tour.NOT_SET,Tour.NOT_SET,Tour.NOT_SET), s[6]);
        }
        
        [Fact]
        public void Tour_Quadruplets_6TourOpen_TreeQuadruplets()
        {
            var tour = new Tour(new [] {0, 1, 2, 3, 4, 5}, null);

            var s = tour.Quadruplets().ToList();
            Assert.NotNull(s);
            Assert.Equal(3, s.Count);
            Assert.Equal(new Quad(0,1,2,3), s[0]);
            Assert.Equal(new Quad(1,2,3,4), s[1]);
            Assert.Equal(new Quad(2,3,4,5), s[2]);
        }
    }
}