using System;
using Verse;

namespace AutomataRace
{
    public class PseudoRandom : IExposable
    {
        public int seed;
        public long counter;
        private Random _random;

        public int Next
        {
            get
            {
                counter++;
                return _random.Next();
            }
        }

        public PseudoRandom()
        {
            seed = Find.TickManager.TicksGame;
            counter = 0;

            _random = new Random(seed);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref seed, "seed");
            Scribe_Values.Look(ref counter, "counter");

            _random = new Random(seed);
            for (int i = 0; i < counter; ++i)
            {
                _random.Next();
            }
        }
    }
}
