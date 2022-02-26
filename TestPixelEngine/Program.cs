using PixelEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EvoGenome
{
    class Program
    {
        static void Main(string[] args)
        {
            Evo1Sim game = new Evo1Sim();
           
            game.Construct(200, 200, 5, 5, 60);

            game.Start();
        }
    }
}
