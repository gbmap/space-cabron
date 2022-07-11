using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.CosmicMusicUtensil
{
    public interface IBarGenerator 
    {
        IBar Generate(BarGeneratorParams p);
    }

    public abstract class BaseBarGenerator<T> : IBarGenerator
    {
        public T Config;

        public BaseBarGenerator(T config)
        {
            Config = config;
        }

        public abstract IBar Generate(BarGeneratorParams p);
    }

    /// <summary>
    /// Describes parameters related to the characteristics of
    /// the generated bar. Not related to each specific generator's
    /// functioning.
    /// </summary>
    [System.Serializable]
    public class BarGeneratorParams
    {
        public int MinNotes = 1;
        public int MaxNotes = 12;

        public int MinNoteType = 1;
        public int MaxNoteType = 8;

        public IScale Scale;
        public bool FilterNotes;
    }

    [System.Serializable]
    public class RandomBarGeneratorConfig
    {
    }

    /// <summary>
    /// Generates notes randomly.
    /// </summary>
    public class RandomBarGenerator : BaseBarGenerator<RandomBarGeneratorConfig>
    {
        public RandomBarGenerator(RandomBarGeneratorConfig config) 
            : base(config)
        {

        }

        public override IBar Generate(BarGeneratorParams p)
        {
            throw new System.NotImplementedException();
        }
    }

    public class FunctionBarGeneratorConfig
    {
    }

    // [CreateAssetMenu(menuName="Gmap/Cosmic Music Utensil/Bar Generator/Function")]
    public class FunctionBarGenerator : BaseBarGenerator<FunctionBarGeneratorConfig>
    {
        public FunctionBarGenerator(FunctionBarGeneratorConfig config) 
            : base(config) {}

        public override IBar Generate(BarGeneratorParams p)
        {
            throw new System.NotImplementedException();
        }
    }
}