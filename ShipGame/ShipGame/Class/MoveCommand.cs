﻿using ShipGame.Interface;

namespace ShipGame.Class
{
    public class MoveCommand : ICommand
    {
        private IMovable movable;

        public MoveCommand(IMovable movable)
        {
            this.movable = movable;
        }
        public void Execute()
        {
            movable.Position = Vector.Sum(movable.Position, movable.Velocity);
        }


    }
}
