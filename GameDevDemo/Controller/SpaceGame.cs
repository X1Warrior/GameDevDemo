using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameDevDemo.Model;
using GameDevDemo.View;
namespace SampleGame.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;
		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState;
		// A movement speed for the player
		private float playerMoveSpeed;
		private Player player;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
			player = new Player();
			// Set a constant player move speed
			playerMoveSpeed = 8.0f;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		// Load the player resources 


		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// TODO: Add your update logic here
			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);


			//Update the player
			UpdatePlayer(gameTime);
			player.Update(gameTime);
			base.Update(gameTime);
	}

	/// <summary>
	/// This is called when the game should draw itself.
	/// </summary>
	/// <param name="gameTime">Provides a snapshot of timing values.</param>
	protected override void Draw(GameTime gameTime)
	{
		// Start drawing 
		spriteBatch.Begin();
		graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

		//TODO: Add your drawing code here

		base.Draw(gameTime);
		// Draw the Player 
		player.Draw(spriteBatch);
		// Stop drawing 
		spriteBatch.End();


	}
		private void UpdatePlayer(GameTime gameTime)
		{

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);
		}

	}
}
