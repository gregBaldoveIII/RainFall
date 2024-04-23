﻿namespace RainFall.Application.Interface;

public interface IQueryHandler<in TQuery, TResult> where TQuery : class
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}