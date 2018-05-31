//using System;

//using System.Collections.Generic;

//using System.Linq;

//using Microsoft.Xna.Framework;

//using Microsoft.Xna.Framework.Audio;

//using Microsoft.Xna.Framework.Content;

//using Microsoft.Xna.Framework.Graphics;

//using Microsoft.Xna.Framework.Input;

//using Microsoft.Xna.Framework.Media;

//namespace ScreenManager

//{

//	public class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent

//	{

//		string[] menuItems;

//		int selectedIndex;



//		Color normal = Color.White;

//		Color hilite = Color.Yellow;



//		KeyboardState keyboardState;

//		KeyboardState oldKeyboardState;



//		SpriteBatch spriteBatch;

//		SpriteFont spriteFont;

//		MenuComponent menuComponent;




//		Vector2 position;

//		float width = 0f;

//		float height = 0f;



//		public int SelectedIndex

//		{


//			get { return selectedIndex; }

//			set

//			{

//				selectedIndex = value;


//				if (selectedIndex < 0)


//					selectedIndex = 0;

//				if (selectedIndex >= menuItems.Length)

//					selectedIndex = menuItems.Length - 1;

//			}

//		}
//		protected override void LoadContent()
//		{
//			string[] menuItems = { "Start Game", "High Scores", "End Game" };

//			spriteBatch = new SpriteBatch(GraphicsDevice);

//			menuComponent = new MenuComponent(this,

//				spriteBatch,

//				Content.Load<SpriteFont>("menufont"),

//				menuItems);

//			Components.Add(menuComponent);
//		}



//		public MenuComponent(Game game,


//			SpriteBatch spriteBatch,


//			SpriteFont spriteFont,


//			string[] menuItems)

//			: base(game)

//		{


//			this.spriteBatch = spriteBatch;

//			this.spriteFont = spriteFont;

//			this.menuItems = menuItems;


//			MeasureMenu();

//		}






//		private void MeasureMenu()

//		{

//			height = 0;

//			width = 0;

//			foreach (string item in menuItems)

//			{

//				Vector2 size = spriteFont.MeasureString(item);

//				if (size.X > width)

//					width = size.X;

//				height += spriteFont.LineSpacing + 5;

//			}



//			position = new Vector2(

//				(Game.Window.ClientBounds.Width - width) / 2,

//				(Game.Window.ClientBounds.Height - height) / 2);

//		}



//		public override void Initialize()

//		{

//			base.Initialize();

//		}



//		private bool CheckKey(Keys theKey)

//		{

//			return keyboardState.IsKeyUp(theKey) &&

//				oldKeyboardState.IsKeyDown(theKey);

//		}



//		public override void Update(GameTime gameTime)

//		{

//			keyboardState = Keyboard.GetState();



//			if (CheckKey(Keys.Down))

//			{

//				selectedIndex++;

//				if (selectedIndex == menuItems.Length)

//					selectedIndex = 0;

//			}

//			if (CheckKey(Keys.Up))

//			{

//				selectedIndex--;

//				if (se0lectedIndex < 0)


//					selectedIndex = menuItems.Length - 1;

//			}

//			base.Update(gameTime);



//			oldKeyboardState = keyboardState;
//		}



//		public override void Draw(GameTime gameTime)

//		{

//			base.Draw(gameTime);

//			Vector2 location = position;

//			Color tint;



//			GraphicsDevice.Clear(Color.CornflowerBlue);

//			spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

//			base.Draw(gameTime);

//			spriteBatch.End();

//			for (int i = 0; i < menuItems.Length; i++)

//			{

//				if (i == selectedIndex)
//					tint = hilite;
//				else
//					tint = normal;
//				spriteBatch.DrawString(

//					spriteFont,

//					menuItems[i],

//					location,

//					tint);

//				location.Y += spriteFont.LineSpacing + 5;

//			}

//		}

//	}

//}
