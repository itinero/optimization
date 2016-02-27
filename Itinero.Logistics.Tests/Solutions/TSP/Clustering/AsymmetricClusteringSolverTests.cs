// Itinero - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using System.Linq;
using Itinero.Logistics.Solutions.TSP;
using Itinero.Logistics.Solutions.TSP.Clustering;
using Itinero.Logistics.Solutions.TSP.GA.EAX;
using Itinero.Logistics.Solvers.GA;
using System.Collections.Generic;
using Itinero.Logistics.Algorithms;

namespace Itinero.Logistics.Tests.Solutions.TSP.Clustering
{
    /// <summary>
    /// Contains tests for asymmetric clustering solver.
    /// </summary>
    [TestFixture]
    public class AsymmetricClusteringSolverTests
    {
        /// <summary>
        /// Tests clustering and solving of a sequence in a matrix of 10.
        /// </summary>
        [Test]
        public void TestSequence10()
        {
            // build a default matrix.
            var defaultWeight = 60;
            var size = 10;
            var weights = new float[size][];
            for (var x = 0; x < weights.Length; x++)
            {
                weights[x] = new float[weights.Length];
                for (var y = 0; y < weights[x].Length; y++)
                {
                    weights[x][y] = defaultWeight;
                }
                weights[x][x] = 0;
            }

            weights[0][1] = 10;

            // set 2 and 3 and 1 and 2 as close.
            weights[1][2] = 1;
            weights[2][1] = 1;
            weights[2][3] = 1;
            weights[3][2] = 1;
            weights[4][3] = 1;
            weights[3][4] = 1;
            weights[4][5] = 1;
            weights[5][4] = 1;

            weights[1][3] = 2;
            weights[3][1] = 2;
            weights[2][4] = 2;
            weights[4][2] = 2;
            weights[5][3] = 2;
            weights[3][5] = 2;

            weights[1][4] = 3;
            weights[4][1] = 3;
            weights[2][5] = 3;
            weights[5][2] = 3;

            weights[1][5] = 4;
            weights[5][1] = 4;

            weights[5][6] = 10;
            weights[6][7] = 10;
            weights[7][8] = 10;
            weights[8][9] = 10;

            // execute the clustering and merge 2 and 3.
            var clustering = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));
            double fitness;
            var route = clustering.Solve(TSPHelper.CreateTSP(0, weights), new MinimumWeightObjective(), out fitness);

            // verify route.
            Assert.AreEqual(54, fitness);
            Assert.AreEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, route.ToArray());
        }

        /// <summary>
        /// Tests the solver on an 'open' solution but with a fixed end.
        /// </summary>
        [Test]
        public void TestSolutions5NotClosedNotFixed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 60);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(4, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver on an 'open' solution but with a fixed end.
        /// </summary>
        [Test]
        public void TestSolutions5NotClosedFixed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(4, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions5ClosedNotFixed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(5, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutions5ClosedFixed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 4, 5, 10);
            problem.Weights[0][1] = 1;
            problem.Weights[1][2] = 1;
            problem.Weights[2][3] = 1;
            problem.Weights[3][4] = 1;
            problem.Weights[4][0] = 1;

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }));

            for (int i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(4, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutionbr17Fixed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            var weights = new float[][] {
                new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
                new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
                new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
                new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
                new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}};

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 16, weights);

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 5,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 200
            }), (p) => new AsymmetricClustering(p.Weights, 1)); // use a lower threshold, this problem is not based on time.

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(0, solution.First);
                Assert.AreEqual(16, solution.Last);
                Assert.IsTrue(fitness <= 34);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.IsTrue(solutionList.Remove(5));
                Assert.IsTrue(solutionList.Remove(6));
                Assert.IsTrue(solutionList.Remove(7));
                Assert.IsTrue(solutionList.Remove(8));
                Assert.IsTrue(solutionList.Remove(9));
                Assert.IsTrue(solutionList.Remove(10));
                Assert.IsTrue(solutionList.Remove(11));
                Assert.IsTrue(solutionList.Remove(12));
                Assert.IsTrue(solutionList.Remove(13));
                Assert.IsTrue(solutionList.Remove(14));
                Assert.IsTrue(solutionList.Remove(15));
                Assert.IsTrue(solutionList.Remove(16));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutionbr17Closed()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            var weights = new float[][] {
                new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
                new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
                new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
                new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
                new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}};

            // create problem.
            var problem = TSPHelper.CreateTSP(0, 0, weights);

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }), (p) => new AsymmetricClustering(p.Weights, 1)); // use a lower threshold, this problem is not based on time.

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.AreEqual(39, fitness);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.IsTrue(solutionList.Remove(5));
                Assert.IsTrue(solutionList.Remove(6));
                Assert.IsTrue(solutionList.Remove(7));
                Assert.IsTrue(solutionList.Remove(8));
                Assert.IsTrue(solutionList.Remove(9));
                Assert.IsTrue(solutionList.Remove(10));
                Assert.IsTrue(solutionList.Remove(11));
                Assert.IsTrue(solutionList.Remove(12));
                Assert.IsTrue(solutionList.Remove(13));
                Assert.IsTrue(solutionList.Remove(14));
                Assert.IsTrue(solutionList.Remove(15));
                Assert.IsTrue(solutionList.Remove(16));
                Assert.AreEqual(0, solutionList.Count);
            }
        }

        /// <summary>
        /// Tests the solver.
        /// </summary>
        [Test]
        public void TestSolutionbr17Open()
        {
            Itinero.Logistics.Algorithms.RandomGeneratorExtensions.GetGetNewRandom = () =>
            {
                return new RandomGenerator(4541247);
            };

            var weights = new float[][] {
                new float[] {9999,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,   0,   3,   5,   8,   8,   5},
                new float[] {   3,9999,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   5,   3,9999,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,   0,  48,  48,  24},
                new float[] {  48,  48,  74,9999,   0,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {  48,  48,  74,   0,9999,   6,   6,  12,  12,  48,  48,  48,  48,  74,   6,   6,  12},
                new float[] {   8,   8,  50,   6,   6,9999,   0,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,9999,   8,   8,   8,   8,   8,   8,  50,   0,   0,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,9999,   0,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,9999,   5,   5,   5,   5,  26,   8,   8,   0},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,9999,   0,   3,   0,   3,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,9999,   3,   0,   3,   8,   8,   5},
                new float[] {   0,   3,   5,  48,  48,   8,   8,   5,   5,   3,   3,9999,   3,   5,   8,   8,   5},
                new float[] {   3,   0,   3,  48,  48,   8,   8,   5,   5,   0,   0,   3,9999,   3,   8,   8,   5},
                new float[] {   5,   3,   0,  72,  72,  48,  48,  24,  24,   3,   3,   5,   3,9999,  48,  48,  24},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,9999,   0,   8},
                new float[] {   8,   8,  50,   6,   6,   0,   0,   8,   8,   8,   8,   8,   8,  50,   0,9999,   8},
                new float[] {   5,   5,  26,  12,  12,   8,   8,   0,   0,   5,   5,   5,   5,  26,   8,   8,9999}};

            // create problem.
            var problem = TSPHelper.CreateTSP(0, weights);

            // create the solver.
            var solver = new AsymmetricClusteringSolver(new EAXSolver(new GASettings()
            {
                CrossOverPercentage = 10,
                ElitismPercentage = 1,
                PopulationSize = 100,
                MaxGenerations = 100000,
                MutationPercentage = 0,
                StagnationCount = 100
            }), (p) => new AsymmetricClustering(p.Weights, 1)); // use a lower threshold, this problem is not based on time.

            for (var i = 0; i < 10; i++)
            {
                // generate solution.
                double fitness;
                var solution = solver.Solve(problem, new MinimumWeightObjective(), out fitness);

                // test contents.
                Assert.IsTrue(fitness <= 39);
                var solutionList = new List<int>(solution);
                Assert.AreEqual(0, solutionList[0]);
                Assert.IsTrue(solutionList.Remove(0));
                Assert.IsTrue(solutionList.Remove(1));
                Assert.IsTrue(solutionList.Remove(2));
                Assert.IsTrue(solutionList.Remove(3));
                Assert.IsTrue(solutionList.Remove(4));
                Assert.IsTrue(solutionList.Remove(5));
                Assert.IsTrue(solutionList.Remove(6));
                Assert.IsTrue(solutionList.Remove(7));
                Assert.IsTrue(solutionList.Remove(8));
                Assert.IsTrue(solutionList.Remove(9));
                Assert.IsTrue(solutionList.Remove(10));
                Assert.IsTrue(solutionList.Remove(11));
                Assert.IsTrue(solutionList.Remove(12));
                Assert.IsTrue(solutionList.Remove(13));
                Assert.IsTrue(solutionList.Remove(14));
                Assert.IsTrue(solutionList.Remove(15));
                Assert.IsTrue(solutionList.Remove(16));
                Assert.AreEqual(0, solutionList.Count);
            }
        }
    }
}