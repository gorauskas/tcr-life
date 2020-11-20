using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SparseLife {
    public class Tests {
        [SetUp]
        public void Setup() { }

        [Test]
        public void CountNeighborsOfACell() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (0, 2));
            var result = world.Select(c => world.CountNeighbors(c)).ToList();
            Assert.AreEqual(new List<int> {1,2,1}, result);
        }

        [Test]
        public void CellWithOneNeighborDies() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (0, 2));
            Assert.IsTrue(!world.Step().Contains((0, 0)));
            Assert.IsTrue(!world.Step().Contains((0, 2)));
        }

        [Test]
        public void CellWithTwoNeighborsLives() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (0, 2));
            Assert.IsTrue(world.Step().Contains((0, 1)));
        }
        
        [Test]
        public void CellWithThreeNeighborsLives() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (1, 1), (0, 2));
            Assert.IsTrue(world.Step().Contains((0, 1)));
        }
        
        [Test]
        public void CellWithFourNeighborsDies() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (-1, 1), (1, 1), (0, 2));
            Assert.IsTrue(!world.Step().Contains((0, 1)));
        }

        [Test]
        public void DeadCellWithThreeNeighborsIsborn() {
            var world = SparseLife.Life.WorldOf((0, 0), (0, 1), (0, 2));
            Assert.IsTrue(world.Step().Contains((1, 1)));
        }
    }

    public sealed class SparseLife {
        private HashSet<(int, int)> _world = null;
        private static SparseLife _instance = null;

        private SparseLife() { }
        
        public static SparseLife Life => _instance ??= new SparseLife();

        public HashSet<(int, int)> WorldOf(params (int, int)[] cells) {
            this._world = new HashSet<(int, int)>(cells);
            return this._world;
        }
    }

    public static class Extensions {
        public static HashSet<(int, int)> Step(this HashSet<(int, int)> world) {
            var newWorld = world.Where(cell => world.ShouldBeAlive(isAliveNow: true, world.CountNeighbors(cell))).ToHashSet();
            newWorld.UnionWith(world.SelectMany(cell => world.DeadNeighbors(cell).Where(deadCell => world.ShouldBeAlive(isAliveNow: false, world.CountNeighbors(deadCell)))));
            return newWorld;
        }

        private static bool ShouldBeAlive(this HashSet<(int, int)> world, bool isAliveNow, int neighborCount) {
            return (isAliveNow && (neighborCount >= 2 && neighborCount <= 3)) ||
                   (!isAliveNow && neighborCount == 3);
        }

        public static int CountNeighbors(this HashSet<(int, int)> world, (int, int) cell) {
            return world.LiveNeighbors(cell).Count;
        }

        private static List<(int, int)> LiveNeighbors(this HashSet<(int, int)> world, (int, int) cell) {
            return cell.Neighbors().Where(world.Contains).ToList();
        }

        private static List<(int, int)> DeadNeighbors(this HashSet<(int, int)> world, (int, int) cell) {
            return cell.Neighbors().Where(c => !world.Contains(c)).ToList();
        }

        private static List<(int, int)> Neighbors(this (int, int) cell) {
            var (rw, cl) = cell;

            return new List<(int, int)> {
                (rw - 1, cl - 1),
                (rw - 1, cl),
                (rw - 1, cl + 1),
                (rw, cl - 1),
                // omit the cell itself
                (rw, cl + 1),
                (rw + 1, cl - 1),
                (rw + 1, cl),
                (rw + 1, cl + 1)
            };
        }
    }
}