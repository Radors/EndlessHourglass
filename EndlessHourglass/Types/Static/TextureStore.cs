using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace EndlessHourglass.Types.Static
{
    public static class TextureStore
    {
        public static Texture2D Map { get; private set; }
        public static Texture2D MapEnd { get; private set; }
        public static Texture2D Player { get; private set; }
        public static Texture2D Rotator { get; private set; }
        public static Texture2D Staff { get; private set; }
        public static Texture2D RotatorProjectile { get; private set; }
        public static Texture2D StaffProjectile { get; private set; }
        public static Texture2D RedMelee { get; private set; }
        public static Texture2D RedMeleeEffect { get; private set; }
        public static Texture2D RedRanged { get; private set; }
        public static Texture2D RedRangedProjectile { get; private set; }
        public static Texture2D HealthBarRed { get; private set; }
        public static Texture2D HealthBarGreen { get; private set; }
        public static Texture2D Numbers { get; private set; }
        public static BitmapFont Font { get; private set; }
        public static List<Rectangle> RotatorProjectileRectangles { get; private set; }
        public static List<Rectangle> StaffProjectileRectangles { get; private set; }
        public static List<Rectangle> HealthBarRedRectangles { get; private set; }
        public static List<Rectangle> HealthBarGreenRectangles { get; private set; }
        public static List<Rectangle> NumbersRectangles { get; private set; }
        public static List<Rectangle> RedMeleeEffectRectangles { get; private set; }
        public static List<Rectangle> RedRangedProjectileRectangles { get; private set; }

        public static void Setup(ContentManager contentManager)
        {
            Map = contentManager.Load<Texture2D>("ase_prod/map");
            MapEnd = contentManager.Load<Texture2D>("ase_prod/mapEnd");
            Player = contentManager.Load<Texture2D>("ase_prod/player");
            Rotator = contentManager.Load<Texture2D>("ase_prod/rotator");
            Staff = contentManager.Load<Texture2D>("ase_prod/staff");
            RotatorProjectile = contentManager.Load<Texture2D>("ase_prod/rotatorProjectile");
            StaffProjectile = contentManager.Load<Texture2D>("ase_prod/staffProjectile");
            RedMelee = contentManager.Load<Texture2D>("ase_prod/redMelee");
            RedMeleeEffect = contentManager.Load<Texture2D>("ase_prod/redMeleeEffect");
            RedRanged = contentManager.Load<Texture2D>("ase_prod/redRanged");
            RedRangedProjectile = contentManager.Load<Texture2D>("ase_prod/redRangedProjectile");
            HealthBarRed = contentManager.Load<Texture2D>("ase_prod/healthBarRed");
            HealthBarGreen = contentManager.Load<Texture2D>("ase_prod/healthBarGreen");
            Font = contentManager.Load<BitmapFont>("font/current");
            Numbers = contentManager.Load<Texture2D>("ase_prod/numbers");
            
            RotatorProjectileRectangles = SetAnimationRectangles(RotatorProjectile, 16, 16);
            StaffProjectileRectangles = SetAnimationRectangles(StaffProjectile, 12, 12);
            HealthBarRedRectangles = SetAnimationRectangles(HealthBarRed, 16, 4);
            HealthBarGreenRectangles = SetAnimationRectangles(HealthBarGreen, 16, 4);
            NumbersRectangles = SetAnimationRectangles(Numbers, 42, 13);
            RedMeleeEffectRectangles = SetAnimationRectangles(RedMeleeEffect, 16, 16);
            RedRangedProjectileRectangles = SetAnimationRectangles(RedRangedProjectile, 8, 8);
        }

        public static List<Rectangle> SetAnimationRectangles(Texture2D texture, int width, int height)
        {
            var rectangles = new List<Rectangle>();
            var frameCount = texture.Width / width;
            for (int i = 0; i < frameCount; i++)
            {
                rectangles.Add(
                    new Rectangle(new Point(i * width, 0), new Point(width, height))
                );
            }
            return rectangles;
        }
    }
}
