﻿using ICommand = ShipGame.Move.ICommand;
namespace ShipGame.Server
{
    public interface IReceiver
    {
        public ICommand Receive();
        public bool IsEmpty();
    }
}
