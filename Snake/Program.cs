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

        const char Boarder = '░'; 
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
            Block apple = new Block(100, 100); // initialization of apple. Given coordinates are irrelevant
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
                Game(snek, rand, apple);
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
 
        static void Game(List<Block> snake, Random rand, Block apple)
        {
            Stopwatch watch = new Stopwatch();
            Directions CurrentDirection = Directions.None;
            String row = Boarder + "";
            for (int i = 0; i < BoardX - 2; i++)
            {
                row += " ";
            }
            row += Boarder;
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

            apple = GenerateApple(apple, rand, snake);
            while (true)
            {        
                if (DetectWallCollision(snake) || DetectSnakeCollision(snake))
                {
                    break;
                }
                // initial print of the game
                PrintGame(snake, apple, row);
                // ReadKey(true) allows the snake to move every frame under the same direction, without the user having to input every time
                ConsoleKeyInfo key = Console.ReadKey(true);
                watch.Start();
                while (!Console.KeyAvailable)
                {
                    if (DetectWallCollision(snake) || DetectSnakeCollision(snake))
                    {
                        break;
                    }

                    if (watch.ElapsedMilliseconds == speed)
                    {
                        Console.Clear(); // clears the console
                        PrintGame(snake, apple, row); // draws the walls, apple, and snake
                        CurrentDirection = MoveSnake(snake, key, CurrentDirection); // moves the snake, and returns the current direction
                                                                                    // if the snake hits an apple, eats it, and spawns a new one
                        if (DetectAppleCollision(apple, snake))
                        {
                            snake = EatApple(snake, apple);
                            apple = GenerateApple(apple, rand, snake);
                        }
                        watch.Restart();
                    }


                    // time to wait before next frame

                    
                }
                watch.Stop();
                Console.Clear();
            }
            
        }


        /// <summary>
        /// detects if the snake hits the boundry
        /// </summary>
        /// <param name="snake"></param>
        /// <returns></returns>
        static bool DetectWallCollision(List<Block> snake)
        {
 
            if (snake[0].GetY() == 0 || snake[0].GetY() >= BoardY || snake[0].GetX() == 0 || snake[0].GetX() >= BoardX)
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
        /// this is the thick method that will print the game.
        /// </summary>
        /// <param name="snake">the player snake</param>
        /// <param name="apple">randomly genereated apple</param>
        static void PrintGame(List<Block> snake, Block apple, string row)
        {
            // finding the max and min y values of the snake, as a way to tell where the snake is
            int maxY = 0;
            int minY = int.MaxValue;
            foreach(Block b in snake)
            {
                if (b.GetY() > maxY)
                    maxY = b.GetY();
                if (b.GetY() < minY)
                    minY = b.GetY();
            }


            Block playerHead = snake[0]; // this block is initialized so that we can draw the head seperately

            // we use this flag as a check to see if something was drawn. If it is true, then we will not draw a space on this iteration
            bool flag = true;
            // use this boolean to make drawing the head slightly more efficient - don't have to check the head if already drawn
            bool headFlag = true;
            // draws the top wall
            for (int i = 0; i < BoardX; i++)
            {
                Console.Write(Boarder);
            }
            // iterate through the rest of the space minus the bottom wall
            for (int i = 1; i < BoardY - 1; i++)
            {
                // if this row has neither a snake nor an apple, can quickly fill in the row and ignore the rest of the code
                if ((i < minY || i > maxY) && apple.GetY() != i)
                {
                    Console.Write("\n"+row);
                }
                else
                {
                    // draws left wall for this row
                    Console.Write("\n" + Boarder);
                    for (int j = 1; j < BoardX - 1; j++)
                    {
                        flag = false;
                        // if this spot on the board matches the coordinate for the apple, draw apple
                        if (i * 100 + j == apple.GetCode())
                        {
                            Console.Write(Apple);
                            flag = true;
                        }
                        // go through the coordinates for each block and check if there should be a block printed 
                        foreach (Block b in snake)
                        {
                            // check if this coordinate is the player head
                            if (i * 100 + j == playerHead.GetCode() && b.GetCode() == i * 100 + j && headFlag)
                            {
                                Console.Write(PlayerHead);
                                flag = true;
                                // wont check head again until next time the board is drawn
                                headFlag = false;
                            }
                            // check if this coordinate is a body part
                            else if (b.GetCode() == i * 100 + j)
                            {
                                Console.Write(Player);
                                flag = true;
                            }
                        }
                        // if nothing was drawn, draw a body part
                        if (!flag)
                        {
                            Console.Write(" ");
                        }


                    }
                    // draws right wall for this row
                    Console.Write(Boarder);
                }
                
                
            }
            // draw the bottom wall
            Console.Write("\n" + Boarder);
            for (int i = 0; i < BoardX - 1; i++)
            {
                Console.Write(Boarder);
            }
            Console.WriteLine("\nScore: " + snake.Count);
        } 
    }
}
