using System;

public interface IReturnable
{
    event Action OnReturnStarted;
    event Action OnReturnCompleted;
}