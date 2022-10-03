using System.Linq;
using Gmap.Utils;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    /// <summary>
    /// This class is responsible for generating color health patterns.
    /// </summary>
    public abstract class HealthGenerator
    {
        public abstract EColor[] Generate(int amount);
        public enum EHealthGeneratorType {
            Constant = 1,
            Alternate = 1 << 1,
            Random = 1 << 2,
            All
        }

        private static HealthGenerator[] HealthGenerators
            = new HealthGenerator[] {
                new HealthGeneratorConstant(EColor.Blue),
                new HealthGeneratorConstant(EColor.Green),
                new HealthGeneratorConstant(EColor.Pink),
                new HealthGeneratorConstant(EColor.Yellow),
                new HealthGeneratorAlternate(EColor.Blue, EColor.Green),
                new HealthGeneratorAlternate(EColor.Blue, EColor.Pink),
                new HealthGeneratorAlternate(EColor.Blue, EColor.Yellow),
                new HealthGeneratorAlternate(EColor.Yellow, EColor.Green),
                new HealthGeneratorAlternate(EColor.Yellow, EColor.Pink),
                new HealthGeneratorAlternate(EColor.Pink, EColor.Green),
                HealthGeneratorShuffleBag.CreateRandom(),
                HealthGeneratorShuffleBag.CreateRandom(),
                HealthGeneratorShuffleBag.CreateRandom(),
                HealthGeneratorShuffleBag.CreateRandom(),
                HealthGeneratorShuffleBag.CreateRandom(),
                HealthGeneratorShuffleBag.CreateRandom(),
        };

        public static HealthGenerator GetRandom()
        {
            return HealthGenerators[Random.Range(0, HealthGenerators.Length)];
        }
    }

    public class HealthGeneratorConstant : HealthGenerator
    {
        EColor color;
        public HealthGeneratorConstant(EColor color)
        {
            this.color = color;
        }
        public override EColor[] Generate(int amount)
        {
            return Enumerable.Range(0, amount).Select(i => color).ToArray();
        }
    }

    public class HealthGeneratorAlternate : HealthGenerator
    {
        EColor colorA;
        EColor colorB;
        public HealthGeneratorAlternate(EColor colorA, EColor colorB)
        {
            this.colorA = colorA;
            this.colorB = colorB;
        }

        public override EColor[] Generate(int amount)
        {
            return Enumerable.Range(0, amount)
                             .Select(i => i % 2 == 0 ? colorA : colorB)
                             .ToArray();
        }
    }

    public class HealthGeneratorShuffleBag : HealthGenerator
    {
        ShuffleBag<EColor> bag;
        public HealthGeneratorShuffleBag(ShuffleBag<EColor> bag)
        {
            this.bag = bag;
        }

        public override EColor[] Generate(int amount)
        {
            return Enumerable.Range(0, amount)
                             .Select(i => bag.Next())
                             .ToArray();
        }

        public static HealthGeneratorShuffleBag CreateRandom()
        {
            ShuffleBag<EColor> bag = new ShuffleBag<EColor>();
            for (int i = 0; i < 5; i++) {
                bag.Add((EColor)Random.Range(0, 4));
            }
            return new HealthGeneratorShuffleBag(bag);
        }
    }
}