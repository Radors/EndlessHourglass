using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Static
{
    public static class TextureStore
    {
        public static Texture2D Map { get; set; }
        public static Texture2D Player { get; set; }
        public static Texture2D Rotator { get; set; }
        public static Texture2D Staff { get; set; }
        public static Texture2D RotatorProjectile { get; set; }
        public static Texture2D StaffProjectile { get; set; }
        public static Texture2D RedRanged { get; set; }
        public static Texture2D HealthBar { get; set; }
        public static Texture2D Numbers { get; set; }
        public static BitmapFont Font { get; set; }
        public static List<Rectangle> RotatorProjectileRectangles { get; set; }
        public static List<Rectangle> HealthBarRectangles { get; set; }
        public static List<Rectangle> NumbersRectangles { get; set; }

        public static void Setup(ContentManager contentManager)
        {
            Map = contentManager.Load<Texture2D>("ase_prod/map");
            Player = contentManager.Load<Texture2D>("ase_prod/player");
            Rotator = contentManager.Load<Texture2D>("ase_prod/rotator");
            Staff = contentManager.Load<Texture2D>("ase_prod/staff");
            RotatorProjectile = contentManager.Load<Texture2D>("ase_prod/rotatorProjectile");
            StaffProjectile = contentManager.Load<Texture2D>("ase_prod/staffProjectile");
            RedRanged = contentManager.Load<Texture2D>("ase_prod/redRanged");
            HealthBar = contentManager.Load<Texture2D>("ase_prod/healthbar");
            Font = contentManager.Load<BitmapFont>("font/current");
            Numbers = contentManager.Load<Texture2D>("ase_prod/numbers");

            RotatorProjectileRectangles = SetAnimationRectangles(RotatorProjectile, 16, 16);
            HealthBarRectangles = SetAnimationRectangles(HealthBar, 16, 4);
            NumbersRectangles = SetAnimationRectangles(Numbers, 42, 13);
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
