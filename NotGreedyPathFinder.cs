using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
                       
            var maxChestFind = int.MinValue;
            var result = new List<Tuple<int, List<Point>>>();
            var chestList = new List<Point>();

            foreach (var item in new DijkstraPathFinder().GetPathsByDijkstra(state, state.Position, state.Chests))
            {
                chestList.Add(item.End);
            }

            while (chestList.Count > 0)
            {
                var countChests = 0;
                var list = PathFind(state, ref countChests, chestList);
                result.Add(Tuple.Create(countChests, list));

                if (maxChestFind < countChests) maxChestFind = countChests;
                chestList.Remove(chestList.First());
            }
            
            return result.OrderByDescending(price => price.Item1).First().Item2;
        }

        public static List<Point> PathFind(State state, ref int countChests, List<Point> chestList)
        {            
            var result = new List<Point>();
            var energy = state.Energy;
            var stepPoint = state.Position;
            var stepChestsList = chestList.ToList();

            while (energy > 0)
            {
                if (stepChestsList.Count == 0)
                {
                    stepChestsList = state.Chests.Where(p => p != result.Last()).ToList();
                }

                var step = new DijkstraPathFinder().GetPathsByDijkstra(state, stepPoint, stepChestsList);
                if (step.FirstOrDefault() == null) return new List<Point>();
                energy -= step.First().Cost;
                if (energy < 0) return new List<Point>();
                var way = step.First().Path.Skip(1);
                if (way.Count() == 0) return new List<Point>();

                foreach (var item in way) 
                {
                    if (stepChestsList.Contains(item))
                    {
                        countChests++;
                        stepChestsList.Remove(item);
                    }
                    result.Add(item);
                }
                stepPoint = result.Last();
            }

            return result;
        }
    }
}