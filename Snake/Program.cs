using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

namespace Snake
{
    class Program
    {
        
        const int BoardX = 50; // defines the width of the board
        const int BoardY = 30; // defines the heighth of the board

        // following four constants are used for drawing the game

        const char Border = '░'; 
        const char Player = '█';
        const char PlayerHead = '☻';
        const char Apple = '*';

        // represents number of milliseconds between each frame
        // lower number = higher speed;
        static int speed;
        enum Directions {None = 0,  Left = -1, Right = 1, Up = -10, Down = 10}

       
        static void Main(string[] args)
        {

            Random rand = new Random();
            Block apple = new Block(); // initialization of apple. Given coordinates are irrelevant
            string continuePlaying = "y";

            while (continuePlaying.ToLower() == "y") // if user responds yes, game will restart
            {
                Console.Clear();
                Console.WriteLine("What speed would you like? (Slow, Medium, Fast, Very Fast) ");
                string response = Console.ReadLine().ToLower();
                char cResponse = response[0];
                switch (cResponse)
                {
                    case 's':
                        speed = 100;
                        break;
                    case 'm':
                        speed = 50;
                        break;
                    case 'f':
                        speed = 25;
                        break;
                    case 'v':
                        speed = 10;
                        break;
                }
                Console.Clear();
                List<Block> snek = SpawnSnake(rand); // initializes and gives snake a random position
                ExecuteGame(snek, rand, apple);
                Console.WriteLine("Continue playing? (Y/N) ");
                continuePlaying = Console.ReadLine();
            }

        }

        /// <summary>
        /// spawns a snake in a random position facing a random direction
        /// </summary>
        /// <param name="rand">random number generator</param>
        /// <returns>snake</returns>
        static List<Block> SpawnSnake(Random rand)
        {
            // spawn in a random block in the middle half of the board
            Block start = new Block(rand.Next(BoardX / 4, BoardX / 2), rand.Next(BoardY / 4, BoardY / 2));
            List<Block> snake = new List<Block>();
            snake.Add(start);
            // generates a random int. uses this to determine which direction snake faces
            int direction = rand.Next(1, 5);  
            switch(direction)
            {
                // facing up
                case 1:     
                    snake.Add(new Block(start.GetX(), start.GetY() + 1));
                    snake.Add(new Block(start.GetX(), start.GetY() + 2));
                    break;
                // facing down
                case 2:
                    snake.Add(new Block(start.GetX(), start.GetY() - 1));
                    snake.Add(new Block(start.GetX(), start.GetY() - 2));
                    break;
                // facing left
                case 3:
                    snake.Add(new Block(start.GetX() + 1, start.GetY()));
                    snake.Add(new Block(start.GetX() + 2, start.GetY()));
                    break;
                // facing right
                case 4:
                    snake.Add(new Block(start.GetX() - 1, start.GetY()));
                    snake.Add(new Block(start.GetX() - 2, start.GetY()));
                    break;
            }
            
            return snake;
        }
 
        static void ExecuteGame(List<Block> snake, Random rand, Block apple)
        {
            Stopwatch watch = new Stopwatch();
            Directions CurrentDirection = Directions.None;
            // the code below is so that on the opening frame, the user can't run into itself
            if (snake[0].GetX() == snake[1].GetX())
            {
                if (snake[0].GetY() < snake[1].GetY())
                    CurrentDirection = Directions.Up;
                else
                    CurrentDirection = Directions.Down;
            }
            else
            {
                if (snake[0].GetX() < snake[1].GetX())
                    CurrentDirection = Directions.Left;
                else
                    CurrentDirection = Directions.Right;
            }

            DrawWalls();
            apple = GenerateApple(apple, rand, snake);
            while (true)
            {        
                // initial print of the game
                PrintGame(snake, apple);
                // ReadKey(true) allows the snake to move every frame under the same direction, without the user having to input every time
                ConsoleKeyInfo key = Console.ReadKey(true);
                while (!Console.KeyAvailable)
                {
                    
                    DrawOrDeleteSnake(' ', ' ', snake); // clears the console
                    CurrentDirection = MoveSnake(snake, key, CurrentDirection); // moves the snake, and returns the current direction
                                                                                // if the snake hits an apple, eats it, and spawns a new one
                    PrintGame(snake, apple); // draws the walls, apple, and snake

                    if (DetectWallCollision(snake) || DetectSnakeCollision(snake))
                    {
                        Console.Clear();
                        return;
                    }

                    if (DetectAppleCollision(apple, snake))
                    {
                        snake = EatApple(snake, apple);
                        apple = GenerateApple(apple, rand, snake);
                    }
                    System.Threading.Thread.Sleep(speed);


                    // time to wait before next frame
             
                }
                DrawOrDeleteSnake(' ', ' ', snake);
            }
            
        }


        /// <summary>
        /// detects if the snake hits the boundry
        /// </summary>
        /// <param name="snake"></param>
        /// <returns></returns>
        static bool DetectWallCollision(List<Block> snake)
        {
 
            if (snake[0].GetY() == 0 || snake[0].GetY() >= BoardY - 1 || snake[0].GetX() == 0 || snake[0].GetX() >= BoardX - 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// detects if the snake runs into itself
        /// </summary>
        /// <param name="snake"></param>
        /// <returns></returns>
        static bool DetectSnakeCollision(List<Block> snake)
        {
            bool b = false;
            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[i].GetCode() == snake[0].GetCode())
                {
                    b = true;
                }
            }
            return b;
        }

        /// <summary>
        /// this method checks if the snake has come in contact with the apple
        /// </summary>
        /// <param name="apple"></param>
        /// <param name="snake"></param>
        /// <returns></returns>
        static bool DetectAppleCollision(Block apple, List<Block> snake)
        {
            if (snake[0].GetCode() == apple.GetCode())
                return true;
            return false;
        }

        /// <summary>
        /// this method is called when the snake collides with the apple.
        /// this method will add an extra block onto the snake
        /// </summary>
        /// <param name="snake">the player</param>
        /// <param name="apple">randomly generated apple</param>
        /// <returns></returns>
        static List<Block> EatApple(List<Block> snake, Block apple)
        {
            // biggest issue: we want to add the extra block "in line" with the end of the snake
            // solution: check if the last two blocks have the same x or y
            bool horizontal = snake[snake.Count - 1].GetY() == snake[snake.Count - 2].GetY();
            bool vertical = snake[snake.Count - 1].GetX() == snake[snake.Count - 2].GetX();
            // add block horizontally
            if (horizontal || (vertical && horizontal == false))
            {
                snake.Add(new Block(snake[snake.Count - 1].GetX() + 1, snake[snake.Count - 1].GetY()));
            }
            // add block vertically
            else
            {
                snake.Add(new Block(snake[snake.Count - 1].GetX(), snake[snake.Count - 1].GetY() + 1));
            }

            
            return snake;
        }

        // spawns an apple and gives it a random coordinate on the grid
        static Block GenerateApple(Block apple, Random rand, List<Block> snake)
        {
            bool appleCheck = true;
            while (appleCheck)
            {
                appleCheck = false;
                // gives the apple a random coordinate
                apple = new Block(rand.Next(1, BoardX - 1), rand.Next(1, BoardY - 1));
                foreach(Block b in snake)
                {
                    // if the apple has the same coordinate as the snake, respawn the apple
                    if (b.GetCode() == apple.GetCode())
                    {
                        appleCheck = true;
                        break;
                    }
                }
            }
            return apple;
            
        }

        /// <summary>
        /// given a key input, this method will move the snake one unit in one of the four directions
        /// also will check to make sure snake isn't moving in opposite direction
        /// 
        /// </summary>
        /// <param name="snake">the player</param>
        /// <param name="key">an input by the user</param>
        /// <param name="CurrentDirection">the direction the snake was already moving in</param>
        /// <returns> new direction of the snake</returns>
        static Directions MoveSnake(List<Block> snake, ConsoleKeyInfo key, Directions CurrentDirection)
        {
            CurrentDirection = ChangeDirection(key, CurrentDirection);
            // first index: x direction. second index: y direction
            int[] MoveDirection = { 0, 0 };
            if (CurrentDirection == Directions.Left || CurrentDirection == Directions.Right)
                MoveDirection[0] = (int)CurrentDirection;
            else
                MoveDirection[1] = (int)CurrentDirection / 10;

            // if even length, use method for even number, otherwise odd
            if (snake.Count % 2 == 0)
                MoveEven(snake, MoveDirection[0], MoveDirection[1]);
            else
                MoveOdd(snake, MoveDirection[0], MoveDirection[1]);


            return CurrentDirection;
        }

        /// <summary>
        /// this method will check if the user presses a key associated with
        /// a direction that is not the current direction
        /// if so, changes direction and returns
        /// </summary>
        /// <param name="key">the directional key the user inputs</param>
        /// <param name="CurrentDirection">the direction the snake was previously goingw</param>
        /// <returns></returns>
        static Directions ChangeDirection(ConsoleKeyInfo key, Directions CurrentDirection)
        {
            // don't want snake to be able to go in opposite direction
            // ex: if going up, can't go down; will continue going up
            switch (key.Key)
            {
                case ConsoleKey.S:
                    if (CurrentDirection != Directions.Up)
                        CurrentDirection = Directions.Down;
                    break;
                case ConsoleKey.W:
                    if (CurrentDirection != Directions.Down)
                        CurrentDirection = Directions.Up;
                    break;
                case ConsoleKey.A:
                    if (CurrentDirection != Directions.Right)
                        CurrentDirection = Directions.Left;
                    break;
                case ConsoleKey.D:
                    if (CurrentDirection != Directions.Left)
                        CurrentDirection = Directions.Right;
                    break;
            }
            return CurrentDirection;
        }
        /// <summary>
        /// moves the snake when there is an odd number of blocks
        /// </summary>
        /// <param name="snake">the player</param>
        /// <param name="dx">amount / direction to move horizontally</param>
        /// <param name="dy">amount / direction to move vertically</param>
        static void MoveOdd(List<Block> snake, int dx, int dy)
        {
            Block temp = snake[0];
            Block temp2;
            Block temp3 = snake[snake.Count - 2];
            // the head is the block that moves to a new coordinate. the rest of the blocks just shift one,
            // in order to move like a snake
            snake[0] = new Block(snake[0].GetX() + dx, snake[0].GetY() + dy);
            for (int i = 1; i <= (snake.Count / 2) - 1; i++)
            {
                temp2 = snake[2*i - 1];
                snake[2*i - 1] = temp;
                temp = snake[2 * i];
                snake[2 * i] = temp2;
            }
            // since it's odd, there is a bit more to deal with out of the loop          
            snake[snake.Count - 2] = temp;
            snake[snake.Count - 1] = temp3;

        }

        /// <summary>
        /// moves the snake when there is an even number of blocks
        /// </summary>
        /// <param name="snake">the player</param>
        /// <param name="dx">amount / direction to move horizontally</param>
        /// <param name="dy">amount / direction to move vertically</param>
        static void MoveEven(List<Block> snake, int dx, int dy)
        {
            Block temp = snake[0];            
            Block temp2;
            Block temp3 = snake[snake.Count - 2];
            // the head is the block that moves to a new coordinate. the rest of the blocks just shift one,
            // in order to move like a snake
            snake[0] = new Block(snake[0].GetX() + dx, snake[0].GetY() + dy);
            for (int i = 1; i <= (snake.Count / 2) - 1; i++)
            {
                temp2 = snake[2 * i - 1];
                snake[2 * i - 1] = temp;
                temp = snake[2 * i];
                snake[2 * i] = temp2;
            }            
            snake[snake.Count - 1] = temp3;
        }

        /// <summary>
        /// this method, called everytime the snake moves, will draw the 
        /// snake, apple, and score
        /// </summary>
        /// <param name="snake">the player snake</param>
        /// <param name="apple">randomly genereated apple</param>
        static void PrintGame(List<Block> snake, Block apple)
        {
            DrawOrDeleteSnake(Player, PlayerHead, snake);
            Console.SetCursorPosition(apple.GetX(), apple.GetY());
            Console.Write(Apple);
            Console.SetCursorPosition(0, BoardY);
            Console.WriteLine("Score: " + (snake.Count - 3));
        } 

        /// <summary>
        /// this method, depending on the character parameters, will either draw the snake
        /// or replace it with empty space
        /// </summary>
        /// <param name="c">the character representing the segments of the snake</param>
        /// <param name="head">the character representing the head of the snake</param>
        /// <param name="snake">a list of blocks comprising the snake</param>
        static void DrawOrDeleteSnake(char c, char head, List<Block> snake)
        {
            // draws the body
            for (int i = 1; i < snake.Count; i++)
            {
                Console.SetCursorPosition(snake[i].GetX(), snake[i].GetY());
                Console.Write(c);
            }
            // draws the head
            Console.SetCursorPosition(snake[0].GetX(), snake[0].GetY());
            Console.Write(head);
        }

        /// <summary>
        /// this method will draw the walls of the game
        /// </summary>
        static void DrawWalls()
        {
            // draws the top wall
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < BoardX; i++)
            {
                Console.Write(Border);
            }
            // draws the bottom wall
            Console.SetCursorPosition(0, BoardY - 1);
            for (int i = 0; i < BoardX; i++)
            {
                Console.Write(Border);
            }

            // draws the left and right walls
            for (int i = 1; i < BoardY - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(Border);
                Console.SetCursorPosition(BoardX - 1, i);
                Console.Write(Border);
            }
        }
    }
}
