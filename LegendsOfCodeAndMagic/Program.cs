using System;

namespace LegendsOfCodeAndMagic
{
    class Player
    {
        static void Main(string[] args)
        {
            TurnProcessor processor = new TurnProcessor();

            for (int i = 0; i < 30; i++)
            {
                processor.ProcessDraftTurn();
            }

            // game loop
            while (true)
            {
                processor.ProcessGameTurn();
            }
        }
    }
}
