using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameDevDemo.View;
namespace GameDevDemo.Model

{
	public class Player2
	{
		public Texture2D texture, healthTexture;
		public Rectangle boundingBox, healthRectangle;
		public Vector2 position, healthBarPosition;

		public Player2()
		{
		}

		// Initialize the player
		public void Initialize(Animation animation, Vector2 position, Texture2D healthTexture)
		{
			player2Animation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			healthBarPosition = new Vector2(500, 50);

			this.healthTexture = healthTexture;

			// Set the player health
			Health = 200;
		}


		// Update the player animation
		public void Update(GameTime gameTime)
		{
			player2Animation.Position = Position;
			player2Animation.Update(gameTime);
			healthRectangle = new Rectangle((int)healthBarPosition.X, (int)healthBarPosition.Y, health, 25);
		}


		// Draw the player
		public void Draw(SpriteBatch spriteBatch)
		{
			player2Animation.Draw(spriteBatch);
			spriteBatch.Draw(healthTexture, healthRectangle, Color.White);
		}

		// Animation representing the player
		private Animation player2Animation;
		public Animation Player2Animation
		{
			get { return player2Animation; }
			set { player2Animation = value; }
		}


		// Position of the Player relative to the upper left side of the screen
		// As a struct it cannot be used as a property 😢 sad panda
		public Vector2 Position;

		// State of the player
		private bool active;
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		// Amount of hit points that player has
		private int health;
		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		// Get the width of the player ship
		// Get the width of the player ship
		public int Width
		{
			get { return player2Animation.FrameWidth; }
		}

		// Get the height of the player ship
		public int Height
		{
			get { return player2Animation.FrameHeight; }
		}
	}
}
