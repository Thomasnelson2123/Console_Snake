using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    // Each individual unit of the snake as well as the apple use this class.
    // class simply keeps track of the coordinates of snake / apple
    class Block
    {

        private int x;
        private int y;

        // returns x value
        public int GetX()
        {
            return this.x;
        }

        // returns y value
        public int GetY()
        {
            return this.y;
        }

        /// <summary>
        /// custom constructor which sets the Block's x and y values. (0, 0) is upper left corner
        /// </summary>
        /// <param name="x">which column the block is in</param>
        /// <param name="y">which row the column is in</param>
        public Block(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Block() { }

        /// <summary>
        /// I used a special system to keep track of each point
        /// essentially just a weighted sum of x and y
        /// </summary>
        /// <returns></returns>
        public int GetCode()
        {
            return this.y * 100 + this.x;
        }


    }
}
