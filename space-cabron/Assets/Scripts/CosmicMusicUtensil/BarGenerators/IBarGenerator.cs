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
        public ENote Root;
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

    public class FunctionBarGenerator : BaseBarGenerator<FunctionBarGeneratorConfig>
    {
        public FunctionBarGenerator(FunctionBarGeneratorConfig config) 
            : base(config) {}

        public override IBar Generate(BarGeneratorParams p)
        {
            throw new System.NotImplementedException();
        }
    }

    public class WaveBarGeneratorConfig
    {
        public int BaseOctave = 3;
        public float Amplitude = 1;
    }

    public class WaveBarGenerator : BaseBarGenerator<WaveBarGeneratorConfig>
    {
        public WaveBarGenerator(WaveBarGeneratorConfig config) 
            : base(config) {}

        public override IBar Generate(BarGeneratorParams p)
        {
            Bar bar = new Bar();
            float step = 1f/p.Scale.GetNumberOfNotes();
            for (float t = 0; t < 1f; t += step)
            {
                float wave = Mathf.Sin(t*2*Mathf.PI+step*0.5f);
                int octave = Config.BaseOctave;
                int nNotes = p.Scale.GetNumberOfNotes();
                int noteIndex = Mathf.FloorToInt(wave * nNotes);
                ENote note = p.Scale.GetNote(p.Root, noteIndex);
                bar.AddNote(new Note(note, 4, octave));
            }

            return bar;
        }

    }
}